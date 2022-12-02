using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static NameManager;

public class UpgradeButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private FBuilding building;
    [SerializeField] private InfotipTrigger costDescription;

    public void OnPointerClick(PointerEventData eventData)
    {
        building.Upgrade();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        costDescription.SetCost(building.GetCost());
    }
}
