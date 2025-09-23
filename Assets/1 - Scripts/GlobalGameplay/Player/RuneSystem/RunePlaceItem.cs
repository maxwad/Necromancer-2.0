using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;
using static Enums;

public class RunePlaceItem : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    private RunesSystem runesManager;
    private RunesWindow runesWindow;
    private PlayerStats playerStats;

    public Image lockImage;
    public Image bg;
    public Image border;
    public Image icon;
    public InfotipTrigger infotip;

    private bool isUnlocked = false;
    private bool isNegativeCell = false;
    private bool isConditionCell = false;

    public int indexCell;
    public int indexRow;

    public Color activeColor;
    public Color activeNegativeColor;
    public Color inactiveColor;

    private Color originalColor;
    private Color runeColor;

    [HideInInspector] public RuneUIItem runeUI;
    [HideInInspector] public RuneSO currentRune;
    private int allowedLevel = -1;

    [Inject]
    public void Construct(
        RunesSystem runesManager,
        PlayerPersonalWindow playerMilitaryWindow,
        PlayerStats playerStats)
    {
        this.runesManager = runesManager;
        this.runesWindow = playerMilitaryWindow.GetComponentInChildren<RunesWindow>();
        this.playerStats = playerStats;
    }

    //private void Awake()
    //{
    //    runesManager = GlobalStorage.instance.runesSystem;
    //    runesWindow = GlobalStorage.instance.playerMilitaryWindow.GetComponentInChildren<RunesWindow>();
    //}

    #region INITS

    public void InitCell(bool unlockMode, int row, int cell)
    {
        indexCell = cell;
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

    public void InitNegativeCell(bool unlockMode, int row, int cell)
    {
        indexCell = cell;
        indexRow = row;
        isNegativeCell = true;
        isUnlocked = unlockMode;

        if(isUnlocked == true)
        {
            originalColor = activeNegativeColor;
            bg.color = activeNegativeColor;

            if(playerStats.GetCurrentParameter(PlayersStats.NegativeCell) > 0)
                lockImage.gameObject.SetActive(false);
            else
            {
                lockImage.gameObject.SetActive(true);
                isUnlocked = false;
            }
        }
        else
        {
            bg.color = inactiveColor;
            lockImage.gameObject.SetActive(true);
        }

        border.color = activeNegativeColor;

        if(currentRune != null) FillCell();
    }

    public void InitConductionCell(bool unlockMode, int row, int cell)
    {
        indexCell = cell;
        indexRow = row;
        isConditionCell = true;

        originalColor = (unlockMode == true) ? activeColor : inactiveColor;
        bg.color = originalColor;

        allowedLevel = (runesWindow == null) ? -1 : runesWindow.CheckNegativeCell(indexCell);
        isUnlocked = (allowedLevel == -1) ? false : true;
        if(currentRune != null && allowedLevel < currentRune.level) isUnlocked = false;

        lockImage.gameObject.SetActive(!isUnlocked);

        if(currentRune != null)
        {
            if(isUnlocked == false)
            {
                ClearCell();
            }
            else
            {
                if(allowedLevel < currentRune.level)
                    ClearCell();
                else
                    FillCell();
            }
        }            
    }

    #endregion

    private void FillCell()
    {
        if(isNegativeCell == false) bg.color = runeColor;

        icon.gameObject.SetActive(true);
        icon.sprite = currentRune.activeIcon;
    }

    public void ClearCell()
    {
        if(currentRune == null) return;

        bg.color = originalColor;
        icon.gameObject.SetActive(false);
        infotip.SetRune(null);

        currentRune = null;
        runesManager.ApplyRune(indexRow, indexCell, currentRune);

        runeUI.ResetRune();
        runeUI = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(GlobalStorage.instance.IsGlobalMode() == false)
        {
            InfotipManager.ShowWarning("You cannot make any changes during the battle.");
            return;
        }

        if(eventData.button == PointerEventData.InputButton.Right)
        {
            ClearCell();            
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(GlobalStorage.instance.IsGlobalMode() == false)
        {
            InfotipManager.ShowWarning("You cannot make any changes during the battle.");
            return;
        }

        if(eventData.pointerDrag != null)
        {
            RuneUIItem rune = eventData.pointerDrag.GetComponent<RuneUIItem>();
            if(rune == null) return;

            if(isUnlocked == false) 
            { 
                InfotipManager.ShowWarning("This cell is locked.");
                return;
            }

            if(isConditionCell == true)
            {
                if(rune.rune.level > allowedLevel)
                {
                    InfotipManager.ShowWarning("The level of the rune must be less or equal than the level of the negative rune.");
                    return;
                }
            }

            if(runeUI != null)
            {
                if(runesManager.CanIReplaceThisRune(isNegativeCell, currentRune, rune.rune, indexRow, indexCell) == false)
                {
                    InfotipManager.ShowWarning("Sorry, but you cannot replace this rune, because in this case, the parameter will be lower than allowed.");
                    return;
                }
                runeUI.ResetRune();
                runesManager.ApplyRune(indexRow, indexCell, null);
            }
            else
            {
                if(runesManager.CanIUseThisRune(isNegativeCell, rune.rune) == false) 
                {
                    InfotipManager.ShowWarning("Sorry, but you cannot go beyond the minimum limit (-99%) for this effect.");
                    return;
                }

            }

            InsertRune(eventData.pointerDrag, rune);
            //currentRune = rune.rune;
            //infotip.SetRune(currentRune);      

            //eventData.pointerDrag.transform.SetParent(transform, false);
            //eventData.pointerDrag.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            //eventData.pointerDrag.transform.localPosition = Vector3.zero;
            //tempRune = rune;

            //runesWindow.CutRuneFromList(rune);
            //runesManager.FillCell(currentRune);

            //runesManager.ApplyRune(indexRow, indexCell, currentRune);
        }
    }

    public void InsertRune(GameObject runeGO, bool loadMode = false)
    {
        //if(runesWindow == null)
        //{
        //    runesManager = GlobalStorage.instance.runesSystem;
        //    runesWindow = GlobalStorage.instance.playerMilitaryWindow.GetComponentInChildren<RunesWindow>();
        //}

        RuneUIItem rune = runeGO.GetComponent<RuneUIItem>();
        currentRune = rune.rune;
        infotip.SetRune(currentRune);

        runeGO.transform.SetParent(transform, false);
        runeGO.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        runeGO.transform.localPosition = Vector3.zero;
        runeUI = rune;

        runesWindow.CutRuneFromList(rune);
        runesManager.FillCell(currentRune);
        runesManager.ApplyRune(indexRow, indexCell, currentRune, loadMode);

        //if(loadMode == false)
        //{
        //    runesWindow.CutRuneFromList(rune);
        //    runesManager.FillCell(currentRune);
        //    runesManager.ApplyRune(indexRow, indexCell, currentRune, loadMode);
        //}
        //else
        //{
        //    FillCell();
        //}        
    }

    public void SetParameters(int row, int cell)
    {
        indexRow = row;
        indexCell = cell;
    }
}
