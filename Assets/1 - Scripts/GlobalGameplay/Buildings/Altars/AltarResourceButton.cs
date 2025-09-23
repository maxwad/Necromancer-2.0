using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Enums;
using System;

public class AltarResourceButton : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image bgIcon;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private TMP_Text maxTryText;

    ResourceGiftData currentData;

    public void Init(ResourceGiftData data)
    {
        currentData = data;

        icon.sprite = data.resourceIcon;

        string amount = data.amount.ToString();
        amountText.text = amount;

        amount = (data.amountTotalTry == 0) ? "" :  "(Max: " + data.amountTotalTry + ")";
        maxTryText.text = amount;

        maxTryText.color = data.tryColor;
        amountText.color = data.amountColor;
    }

    //Button
    public void SetResource()
    {
        if(currentData.isDeficit == true)
        {
            InfotipManager.ShowWarning("You do not have a selected resource. Choose a different one or take a prayer.");
            return;
        }
        else
        {
            currentData.miniGame.SetResource(currentData);
        }
    }

    public void SetResource(Sprite tryIcon)
    {
        icon.sprite = tryIcon;
    }

    internal void SetBGColor(Color tryColor)
    {
        bgIcon.color = tryColor;
    }
}
