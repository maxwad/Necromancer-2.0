using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfirmarySlot : MonoBehaviour
{
    [SerializeField] public GameObject content;
    [SerializeField] public Image icon;
    [SerializeField] public Image timerVeil;
    [SerializeField] public TMP_Text quantity;

    public Color grey;
    public Color red;

    [SerializeField] private InfotipTrigger squadtipTrigger;
    [SerializeField] private TooltipTrigger tooltipTrigger;

    public void ResetSlot()
    {
        content.SetActive(false);
        squadtipTrigger.SetUnit(null);
    }

    public void FillTheInfarmarySlot(Unit unit, int count)
    {
        squadtipTrigger.SetUnit(unit);

        icon.enabled = true;

        icon.sprite = unit.unitIcon;

        quantity.enabled = true;
        quantity.text = count.ToString();        
    }
}
