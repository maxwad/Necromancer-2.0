using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class AltarUnitSlotUI : MonoBehaviour
{
    private AltarUI altarUI;
    private float maxAmount;
    private UnitsTypes currentUnit;
    [SerializeField] private InfotipTrigger infotip;

    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text amount;
    [SerializeField] private Slider slider;


    public void Init(AltarUI altar, Unit unit, float quantity)
    {
        gameObject.SetActive(true);

        altarUI = altar;
        infotip.SetUnit(unit);

        icon.sprite = unit.unitIcon;
        amount.text = quantity.ToString();

        currentUnit = unit.unitType;
        maxAmount = quantity;
        slider.value = 1;
    }

    public void ShowSlider(bool showMode)
    {
        slider.gameObject.SetActive(showMode);
    }

    //Slider
    public void ChangeAmoumt()
    {
        float newAmount = Mathf.Round(maxAmount * slider.value);
        amount.text = newAmount.ToString();

        altarUI.ChangeUnitsAmount(currentUnit, newAmount);
    }
}
