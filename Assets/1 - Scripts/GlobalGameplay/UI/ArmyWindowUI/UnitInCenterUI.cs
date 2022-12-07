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
    private Dictionary<ResourceType, Sprite> resourcesIcons;
    private FortressBuildings allBuildings;
    private Garrison garrison;
    private UnitCenter uCenter;

    [SerializeField] private TMP_Text unitName;
    [SerializeField] private Image unitIcon;

    [SerializeField] private TMP_Text unitAmount;
    [SerializeField] private TMP_Text unitGrowth;

    [SerializeField] private List<GameObject> costs;
    [SerializeField] private InfotipTrigger tip;

    private List<Cost> realCosts = new List<Cost>();
    private Unit currentUnit;

    public void Init(Unit unit, UnitCenter center)
    {
        if(resourcesManager == null)
        {
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
        int amountPerWeek = garrison.GetHiringGrowth(unit.unitHome);
        unitGrowth.text = amountPerWeek.ToString();

        int amount = garrison.GetHiringAmount(unit.unitType);
        unitAmount.text = amount.ToString();
    }

    private void FillCost(Unit unit)
    {
        realCosts.Clear();

        float goldDiscount = allBuildings.GetBonusAmount(CastleBuildings.TaxService);
        float foodDiscount = allBuildings.GetBonusAmount(CastleBuildings.Tavern);

        for(int i = 0; i < costs.Count; i++)
        {
            costs[i].SetActive(!(i >= unit.costs.Count));
        }

        for(int i = 0; i < unit.costs.Count; i++)
        {
            ResourceType resourceType = unit.costs[i].type;
            TMP_Text amount = costs[i].GetComponentInChildren<TMP_Text>();

            float discount = 1f;
            if(resourceType == ResourceType.Gold) discount = 1 - goldDiscount;
            if(resourceType == ResourceType.Food) discount = 1 - foodDiscount;

            float resumeCost = Mathf.Round(unit.costs[i].amount * discount);
            
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
        uCenter.SetUnitForHiring(currentUnit, realCosts);
    }    
}
