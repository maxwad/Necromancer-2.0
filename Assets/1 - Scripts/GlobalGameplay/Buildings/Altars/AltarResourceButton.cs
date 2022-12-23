using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class AltarResourceButton : MonoBehaviour
{
    private ResourcesManager resourcesManager;
    private Dictionary<ResourceType, Sprite> resourcesIcons;

    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private TMP_Text maxTryText;

    private Color normalColor = Color.white;
    [SerializeField] private Color warningColor;

    public void Init(ResourceGiftData data)
    {
        if(resourcesManager == null)
        {
            resourcesManager = GlobalStorage.instance.resourcesManager;
            resourcesIcons = resourcesManager.GetAllResourcesIcons();
        }

        icon.sprite = resourcesIcons[data.resourceType];

        string amount = (data.amountInStore == 0) ? data.amount.ToString() : (data.amount + "/" + data.amountInStore);
        amountText.text = amount;

        amount = (data.amountTotalTry == 0) ? "" :  "(Max: " + data.amountTotalTry + ")";
        maxTryText.text = amount;

        maxTryText.color = (resourcesManager.CheckMinResource(data.resourceType, data.amountTotalTry) == true) ? normalColor : warningColor;
    }
}
