using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Enums;

public class UpgradeButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    private IUpgradable parent;
    private InfotipTrigger costDescription;

    private void Start()
    {
        parent = transform.parent.GetComponent<IUpgradable>();
        costDescription = GetComponent<InfotipTrigger>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {      
        parent.TryToBuild();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        costDescription.SetCost(parent.GetRequirements());
    }
}
