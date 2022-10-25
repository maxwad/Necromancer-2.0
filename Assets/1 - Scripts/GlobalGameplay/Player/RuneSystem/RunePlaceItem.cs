using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static NameManager;

public class RunePlaceItem : MonoBehaviour, IDropHandler, IPointerClickHandler, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    private ObjectsPoolManager poolManager;
    private RunesManager runesManager;

    public Image lockImage;
    public Image bg;
    public Image icon;
    public InfotipTrigger infotip;

    private bool isUnlocked = false;
    private bool isNegativeCell = false;
    private bool isCondactionCell = false;

    private float index;
    private float indexRow;

    public Color activeColor;
    public Color activeNegativeColor;
    public Color inactiveColor;

    private Color originalColor;
    private Color runeColor;

    private GameObject runeGO;
    private RuneSO currentRune;
    private int allowedLevel = -1;

    private GameObject tempRuneGO;
    private RuneUIItem tempRune;
    private CanvasGroup canvasGroup;
    private Canvas dragdrop;
    private RunesWindow runesWindow;

    private void Start()
    {
        poolManager = GlobalStorage.instance.objectsPoolManager;
        Canvas[] group = GlobalStorage.instance.playerMilitaryWindow.GetComponentsInChildren<Canvas>();
        dragdrop = group[group.Length - 1];
        runesManager = GlobalStorage.instance.runesManager;
        runesWindow = GlobalStorage.instance.playerMilitaryWindow.GetComponentInChildren<RunesWindow>();
    }

    #region Inits

    public void InitCell(bool unlockMode, int row, float cell)
    {
        index = cell;
        indexRow = row;

        originalColor = (unlockMode == true) ? activeColor : inactiveColor;
        bg.color = originalColor;

        if(unlockMode == true)
        {
            lockImage.gameObject.SetActive(!unlockMode);
            isUnlocked = unlockMode;
        }

        if(currentRune != null) FillCell();
    }

    public void InitNegativeCell(bool unlockMode, int row, float cell)
    {
        index = cell;
        indexRow = row;
        isNegativeCell = true;       

        if(unlockMode == true)
        {
            originalColor = activeNegativeColor;
            bg.color = activeNegativeColor;

            if(GlobalStorage.instance.playerStats.GetCurrentParameter(PlayersStats.NegativeCell) > 0)
                lockImage.gameObject.SetActive(false);
            else
                lockImage.gameObject.SetActive(true);
        }
        else
        {
            bg.color = inactiveColor;
            lockImage.gameObject.SetActive(true);
        }

        if(currentRune != null) FillCell();
    }

    public void InitConductionCell(bool unlockMode, int row, float cell)
    {
        index = cell;
        indexRow = row;
        isCondactionCell = true;

        originalColor = (unlockMode == true) ? activeColor : inactiveColor;
        bg.color = originalColor;

        if(currentRune != null) FillCell();
    }

    #endregion

    private void FillCell()
    {
        bg.color = runeColor;
        icon.gameObject.SetActive(true);
        icon.sprite = currentRune.activeIcon;
    }

    public void ClearCell()
    {
        currentRune = null;
        bg.color = originalColor;
        icon.gameObject.SetActive(false);
        infotip.SetRune(null);

        //Clear data here
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            ClearCell();
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(eventData.pointerDrag != null)
        {
            RuneUIItem rune = eventData.pointerDrag.GetComponent<RuneUIItem>();
            if(rune == null) return;

            if(isUnlocked == false) 
            { 
                InfotipManager.ShowWarning("This cell is locked.");
                return;
            }

            //check allowed here

            currentRune = rune.rune;
            infotip.SetRune(currentRune);
            runeColor = rune.bg.color;

            eventData.pointerDrag.transform.SetParent(transform, false);
            eventData.pointerDrag.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            eventData.pointerDrag.transform.localPosition = Vector3.zero;


            runesManager.FillCell(currentRune);
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        //canvasGroup = tempRuneGO.GetComponent<CanvasGroup>();
        //canvasGroup.alpha = 0.8f;
        //canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //tempRuneGO.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //canvasGroup.alpha = 1f;
        //canvasGroup.blocksRaycasts = true;


        ////runesWindow.UpdateWindow();
    }
}
