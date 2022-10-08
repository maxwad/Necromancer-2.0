using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class LevelUpCard : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private NewSkillUI newSkillUI;
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
    public bool isCoroutineStarted = false;

    private Coroutine coroutine;
    private float openingDelay;
    private float constDelay = 0.5f;

    public void Init(bool visibilityMode, MacroAbilitySO ability, float index, NewSkillUI levelUI)
    {
        card.SetActive(true);
        if(cardRect == null) cardRect = GetComponent<RectTransform>();

        isTaken = false;
        isVisible = visibilityMode;
        newSkillUI = levelUI;
        currentAbility = ability;
        openingDelay = index - constDelay * index;

        cardRect.rotation = Quaternion.Euler(0, 0, 0);
        border.fillAmount = 1f;

        FillCard();

        if(isReplacement == true)
        {
            cover.SetActive(true);
            content.SetActive(false);

            if(isVisible == true)
                coroutine = StartCoroutine(TurnCoverCard(true, openingDelay));
            else
            {
                canIGetAbility = true;
            }                
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
                            coroutine = StartCoroutine(TurnCoverCard(true, -constDelay));
                        else
                            newSkillUI.Result(currentAbility);
                    }
                }
            }
        }
    }

    private void FillCard()
    {
        caption.text = currentAbility.abilityName;
        icon.sprite = currentAbility.activeIcon;
        description.text = currentAbility.realDescription;
        fakeDescription.text = currentAbility.fakeDescription;
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
        isCoroutineStarted = true;
        yield return new WaitForSecondsRealtime(delay + constDelay);

        float y = cardRect.localEulerAngles.y;
        float step = 2f;

        Quaternion currentEuler = Quaternion.Euler(0, 0, 0);

        while(currentEuler.eulerAngles.y < 90f)
        {
            y += step;
            if(y > 90f) y = 90f;

            currentEuler = Quaternion.Euler(0, y, 0);
            cardRect.rotation = currentEuler;
            yield return new WaitForSecondsRealtime(0.005f);
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
            yield return new WaitForSecondsRealtime(0.005f);
        }

        if(mode == true)
        {
            if(isVisible == false)
            {
                isVisible = true;
                newSkillUI.Result(currentAbility);
            }
            else
            {
                canIGetAbility = !newSkillUI.CheckTakenAbility();
            }

            isReplacement = false;
            isCoroutineStarted = false;
        }
        else
        {
            newSkillUI.ReadyToReplace();
        }
    }

    public bool CanIClickAnyButton()
    {
        return !isCoroutineStarted;
    }

    public void Replace()
    {
        canIGetAbility = false;
        isReplacement = true;

        if(isVisible == true)
            coroutine = StartCoroutine(TurnCoverCard(false, -constDelay));
        else
            newSkillUI.ReadyToReplace();
    }

    private void OnEnable()
    {
        cover.SetActive(!isVisible);
        content.SetActive(isVisible);
        isReplacement = false;

        canIGetAbility = !newSkillUI.CheckTakenAbility();
    }

    private void OnDisable()
    {
        if(coroutine != null) StopCoroutine(coroutine);
        isCoroutineStarted = false;
        cardRect.rotation = Quaternion.Euler(0, 0, 0);
    }
}
