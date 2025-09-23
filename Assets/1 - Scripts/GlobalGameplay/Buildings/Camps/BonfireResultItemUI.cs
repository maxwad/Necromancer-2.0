using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using static Enums;

public class BonfireResultItemUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text amount;
    [SerializeField] private TooltipTrigger tooltip;

    public void Init(CampBonus bonus)
    {
        icon.sprite = bonus.icon;
        amount.text = bonus.amount.ToString();
        tooltip.content = bonus.name;
    }
}
