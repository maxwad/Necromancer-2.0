using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class BoostInBattleUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text amount;
    [SerializeField] private TooltipTrigger tip;

    [SerializeField] private Color negativeColor;
    [SerializeField] private Color positiveColor;

    public void Init(RunesType runeType, Sprite pict, string descr, float value)
    {
        icon.sprite = pict;

        string before = "";
        if (value > 0) before = "+";
        string after = "%";
        amount.text = before + value + after;

        Color color;

        if(runeType != RunesType.CoolDown)
            color = (value > 0) ? positiveColor : negativeColor;
        else
            color = (value > 0) ? negativeColor : positiveColor;

        if(value == 0) color = Color.white;

        amount.color = color;

        tip.content = descr.Replace("$", value.ToString());
    }
}
