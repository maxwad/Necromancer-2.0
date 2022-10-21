using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunePlaceItem : MonoBehaviour
{
    public Image lockImage;
    public Image bg;
    public Image icon;

    private bool isUnlocked = false;
    private bool isNegativeCell = false;

    private float index;
    private float indexRow;

    public Color activeColor;
    public Color activeNegativeColor;
    public Color inactiveColor;

    private RuneSO currentRune;
    private int allowedLevel = -1;

    public void Init(bool unlockMode, bool negativeMode, int row, float cell)
    {
        bg.color = (unlockMode == true) ? activeColor : inactiveColor;

        if(negativeMode == true) 
        {
            bg.color = (unlockMode == true) ? activeNegativeColor : inactiveColor;
            isNegativeCell = true;
        }

        if(unlockMode == true)
        {
            lockImage.gameObject.SetActive(!unlockMode);
            isUnlocked = unlockMode;
        }

        index = cell;
        indexRow = row;
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
