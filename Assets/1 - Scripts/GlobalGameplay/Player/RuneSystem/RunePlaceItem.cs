using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static NameManager;

public class RunePlaceItem : MonoBehaviour
{
    public Image lockImage;
    public Image bg;
    public Image icon;

    private bool isUnlocked = false;
    private bool isNegativeCell = false;
    private bool isCondactionCell = false;

    private float index;
    private float indexRow;

    public Color activeColor;
    public Color activeNegativeColor;
    public Color inactiveColor;

    private RuneSO currentRune;
    private int allowedLevel = -1;

    public void InitCell(bool unlockMode, int row, float cell)
    {
        index = cell;
        indexRow = row;

        bg.color = (unlockMode == true) ? activeColor : inactiveColor;

        if(unlockMode == true)
        {
            lockImage.gameObject.SetActive(!unlockMode);
            isUnlocked = unlockMode;
        }        
    }

    public void InitNegativeCell(bool unlockMode, int row, float cell)
    {
        index = cell;
        indexRow = row;
        isNegativeCell = true;       

        if(unlockMode == true)
        {
            bg.color = activeNegativeColor;
            if(GlobalStorage.instance.playerStats.GetCurrentParameter(PlayersStats.NegativeCell) > 0)
            {
                lockImage.gameObject.SetActive(false);
            }
            else
            {
                lockImage.gameObject.SetActive(true);
            }

        }
        else
        {
            bg.color = inactiveColor;
            lockImage.gameObject.SetActive(true);
        }
    }

    public void InitConductionCell(bool unlockMode, int row, float cell)
    {
        index = cell;
        indexRow = row;
        isCondactionCell = true;

        bg.color = (unlockMode == true) ? activeColor : inactiveColor;
    }


    public void FillCell()
    {
        if(isUnlocked == false) Debug.Log("Cell is locked!");

    }

    public void UnlockCell()
    {
        lockImage.gameObject.SetActive(false);
        isUnlocked = true;
    }
}
