using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class AltarResourceButton : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private TMP_Text maxTryText;

    ResourceGiftData currentData;

    //private int index = 0;
    //private ResourceType resourceType;
    //private AltarMiniGame miniGame;
    //private float price;
    //private 

    public void Init(ResourceGiftData data)
    {
        currentData = data;

        icon.sprite = data.resourceIcon;

        string amount = (data.amountInStore == 0) ? data.amount.ToString() : (data.amount + "/" + data.amountInStore);
        amountText.text = amount;

        amount = (data.amountTotalTry == 0) ? "" :  "(Max: " + data.amountTotalTry + ")";
        maxTryText.text = amount;

        maxTryText.color = data.tryColor;
        amountText.color = data.amountColor;
    }

    //Button
    public void SetResource()
    {
        currentData.miniGame.SetResource(currentData);
    }
}
