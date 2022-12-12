using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using static NameManager;
using System;

public class UnitInCenterUI : MonoBehaviour, IPointerClickHandler
{
    private ResourcesManager resourcesManager;
    private BoostManager boostManager;
    private Dictionary<ResourceType, Sprite> resourcesIcons;
    private FortressBuildings allBuildings;
    private Garrison garrison;
    private UnitCenter uCenter;

    [SerializeField] private Button thisButton;
    [SerializeField] private TMP_Text unitName;
    [SerializeField] private Image unitIcon;

    [SerializeField] private TMP_Text unitAmount;
    [SerializeField] private TMP_Text unitGrowth;

    [SerializeField] private List<GameObject> costs;
    [SerializeField] private InfotipTrigger tip;

    private List<Cost> realCosts = new List<Cost>();
    private Unit currentUnit;
    private int currentAmount;

    public void Init(Unit unit, UnitCenter center)
    {
        if(resourcesManager == null)
        {
            boostManager = GlobalStorage.instance.boostManager;
            resourcesManager = GlobalStorage.instance.resourcesManager;
            resourcesIcons = resourcesManager.GetAllResourcesIcons();
            allBuildings = GlobalStorage.instance.fortressBuildings;
            garrison = allBuildings.GetComponent<Garrison>();
        }

        gameObject.SetActive(true);
        tip.SetUnit(unit);
        uCenter = center;
        currentUnit = unit;

        FillUnit(unit);
        FillGrowth(unit);
        FillCost(unit);
    }

    private void FillUnit(Unit unit)
    {
        unitName.text = unit.unitName;
        unitIcon.sprite = unit.unitIcon;
    }

    private void FillGrowth(Unit unit)
    {
        int amountPerWeek = garrison.GetHiringGrowth(unit.unitType);
        unitGrowth.text = "+" + amountPerWeek.ToString();

        int amount = garrison.GetHiringAmount(unit.unitType);
        unitAmount.text = amount.ToString();
        currentAmount = amount;
        thisButton.interactable = (currentAmount != 0);
    }

    private void FillCost(Unit unit)
    {
        realCosts.Clear();

        float discount = allBuildings.GetBonusAmount(CastleBuildingsBonuses.HiringDiscount);

        for(int i = 0; i < costs.Count; i++)
        {
            costs[i].SetActive(!(i >= unit.costs.Count));
        }

        for(int i = 0; i < unit.costs.Count; i++)
        {
            ResourceType resourceType = unit.costs[i].type;
            TMP_Text amount = costs[i].GetComponentInChildren<TMP_Text>();

            float price = 1f;
            //"+" becouse discount has negative  value
            if(resourceType == ResourceType.Gold) price = 1 + discount;
            if(resourceType == ResourceType.Food) price = 1 + discount;

            float resumeCost = Mathf.Round(unit.costs[i].amount * price);
            
            costs[i].GetComponentInChildren<Image>().sprite = resourcesIcons[resourceType];
            amount.text = resumeCost.ToString();

            Cost itemCost = new Cost
            {
                type = resourceType,
                amount = resumeCost
            };

            realCosts.Add(itemCost);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        uCenter.SetUnitForHiring(currentUnit, currentAmount, realCosts);
    }    
}
