using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfirmarySlot : MonoBehaviour
{
    public Image icon;
    public TMP_Text quantity;
    public Image backlight;

    public Color grey;
    public Color red;

    private SquadtipTrigger squadtipTrigger;

    private void Start()
    {
        squadtipTrigger = GetComponent<SquadtipTrigger>();
        backlight.color = grey;
    }

    public void ResetSlot()
    {
        backlight.color = grey;
        icon.enabled = false;
        quantity.enabled = false;
    }

    public void FillTheInfarmarySlot(Unit unit, int count)
    {
        squadtipTrigger.SetUnit(unit);

        backlight.color = red;

        icon.enabled = true;

        icon.sprite = unit.unitIcon;

        quantity.enabled = true;
        quantity.text = count.ToString();        
    }
}
