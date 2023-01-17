using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class RuneRWItemUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private Image selectBG;
    [SerializeField] private Image bg;
    [SerializeField] private GameObject countBG;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private InfotipTrigger infotip;

    [SerializeField] private List<Color> levelColors;
    [SerializeField] private Color selectColor;
    [SerializeField] private Color disableColor;

    private Vector3 selectScale = new Vector3(1.25f, 1.25f, 1.25f);


    private bool isStoreRune = false;
    private RuneSO rune;
    private RuneWorkroom workroom;

    public void Init(RuneWorkroom room, RuneSO currentRune, bool storeRuneMode, string count = "")
    {
        if(workroom == null)
        {
            workroom = room;
        }

        isStoreRune = storeRuneMode;

        rune = currentRune;
        icon.sprite = rune.activeIcon;
        bg.color = (storeRuneMode == true) ? levelColors[0] : levelColors[rune.level];

        countBG.SetActive(!storeRuneMode);
        countText.text = count;

        infotip.SetRune(currentRune);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Select();
    }

    public void Select()
    {
        workroom.ResetSelections();
        selectBG.color = selectColor;
        gameObject.transform.localScale = selectScale;

        if(isStoreRune == true)
        {
            workroom.SetRuneToCreate(rune.rune, this);
        }
        else
        {
            workroom.SetRuneToDestroy(rune.rune, this);
        }
    }

    public void ResetSelection()
    {
        selectBG.color = Color.white;
        gameObject.transform.localScale = Vector3.one;
    }
}
