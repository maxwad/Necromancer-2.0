using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class LevelUpCard : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private NewMacroLevelUI newLevelUI;
    private MacroAbilitySO currentAbility;

    [SerializeField] private GameObject card;
    [SerializeField] private RectTransform cardRect;
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject cover;
    [SerializeField] private TMP_Text caption;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text fakeDescription;
    [SerializeField] private Image icon;
    [SerializeField] private Image border;

    private bool isVisible = true;
    private bool isTaken = false;
    private bool isMousePressed = false;
    private bool canIGetAbility = false;
    private bool isReplacement = false;

    private float openingDelay;
    private float constDelay = 1f;

    public void Init(bool visibilityMode, MacroAbilitySO ability, float openDelay, NewMacroLevelUI levelUI)
    {
        card.SetActive(true);
        if(cardRect == null) cardRect = GetComponent<RectTransform>();

        isTaken = false;
        isVisible = visibilityMode;
        newLevelUI = levelUI;
        currentAbility = ability;
        openingDelay = openDelay;

        cardRect.rotation = Quaternion.Euler(0, 0, 0);
        StopAllCoroutines();
        border.fillAmount = 1f;

        caption.text = currentAbility.abilityName;
        icon.sprite = currentAbility.activeIcon;
        description.text = currentAbility.realDescription;
        fakeDescription.text = currentAbility.fakeDescription;

        cover.SetActive(!isVisible);
        content.SetActive(isVisible);

        if(isReplacement == true)
        {
            cover.SetActive(true);
            content.SetActive(false);

            if(isVisible == true)
                StartCoroutine(TurnCoverCard(true, openingDelay));
            else
                canIGetAbility = true;
        }
        else
        {
            cover.SetActive(!isVisible);
            content.SetActive(isVisible);
            canIGetAbility = true;
        }
    }

    private void Update()
    {
        if(canIGetAbility == true)
        {
            if(isTaken == false)
            {
                if(isMousePressed == true)
                {
                    border.fillAmount -= Time.unscaledDeltaTime * 0.5f;
                    if(border.fillAmount <= 0f)
                    {
                        isTaken = true;                        
                        if(isVisible == false) 
                            StartCoroutine(TurnCoverCard(true, -constDelay));
                        else
                            newLevelUI.Result(currentAbility);
                    }
                }
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isMousePressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isMousePressed = false;
        if(isTaken == false) border.fillAmount = 1f;
    }

    public void LockCard()
    {
        canIGetAbility = false;
    }

    private IEnumerator TurnCoverCard(bool mode, float delay)
    {
        yield return new WaitForSecondsRealtime(delay + constDelay);

        float y = cardRect.localEulerAngles.y;
        float step = 2.5f;

        Quaternion currentEuler = Quaternion.Euler(0, 0, 0);

        while(currentEuler.eulerAngles.y < 90f)
        {
            y += step;
            if(y > 90f) y = 90f;

            currentEuler = Quaternion.Euler(0, y, 0);
            cardRect.rotation = currentEuler;
            yield return new WaitForSecondsRealtime(0.01f);
        }

        if(mode == true)
        {
            cover.SetActive(false);
            content.SetActive(true);
        }
        else
        {
            cover.SetActive(true);
            content.SetActive(false);
        }

        while(currentEuler.eulerAngles.y > 0f)
        {
            y -= step;
            if(y < 0f) y = 0f;

            currentEuler = Quaternion.Euler(0, y, 0);
            cardRect.rotation = currentEuler;
            yield return new WaitForSecondsRealtime(0.01f);
        }

        if(mode == true)
        {
            if(isVisible == false)
                newLevelUI.Result(currentAbility);
            else
                canIGetAbility = true;

            isReplacement = false;
        }
        else
        {
            newLevelUI.ReadyToReplace();
        }

    }

    public void Replace()
    {
        canIGetAbility = false;
        isReplacement = true;

        if(isVisible == true) 
            StartCoroutine(TurnCoverCard(false, -constDelay));
        else
            newLevelUI.ReadyToReplace();
    }

    private IEnumerator Replacement()
    {
        yield return new WaitForSecondsRealtime(0.01f);
    }
}
