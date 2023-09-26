using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static NameManager;

public class Altar : MonoBehaviour
{
    private InfirmaryManager infirmary;
    private UnitManager unitManager;
    private ResourcesManager resourcesManager;
    private BoostManager boostManager;
    private PlayersArmy playersArmy;

    private ObjectOwner objectOwner;
    private bool isVisited = false;

    private Dictionary<UnitsTypes, int> injuredUnitsDict = new Dictionary<UnitsTypes, int>();
    private Dictionary<ResourceType, float> currentPrice;

    private List<ResourceType> defaultResList = new List<ResourceType>();
    private float percentCommonCost = 0.15f;
    private float minCost = 50f;

    [Inject]
    public void Construct
        (
        InfirmaryManager infirmary,
        UnitManager unitManager,
        ResourcesManager resourcesManager,
        BoostManager boostManager,
        PlayersArmy playersArmy
        )
    {
        this.infirmary = infirmary;
        this.unitManager = unitManager;
        this.resourcesManager = resourcesManager;
        this.boostManager = boostManager;
        this.playersArmy = playersArmy;

        objectOwner = GetComponent<ObjectOwner>();

        defaultResList = new List<ResourceType>()
        {
            ResourceType.Wood,
            ResourceType.Food,
            ResourceType.Gold,
            ResourceType.Iron,
            ResourceType.Stone
        };
    }

    private void Start()
    {
        //infirmary = GlobalStorage.instance.infirmaryManager;
        //unitManager = GlobalStorage.instance.unitManager;
        //resourcesManager = GlobalStorage.instance.resourcesManager;
        //boostManager = GlobalStorage.instance.boostManager;
        //playersArmy = GlobalStorage.instance.playersArmy;

        //objectOwner = GetComponent<ObjectOwner>();

        //defaultResList = new List<ResourceType>()
        //{
        //    ResourceType.Wood,
        //    ResourceType.Food,
        //    ResourceType.Gold,
        //    ResourceType.Iron,
        //    ResourceType.Stone
        //};
    }

    #region SETTINGS

    public Dictionary<Unit, int> GetUnits()
    {
        injuredUnitsDict.Clear();
        injuredUnitsDict = GetUnitsFromInfirmary();

        Dictionary<Unit, int> units = new Dictionary<Unit, int>();
        foreach(var unit in injuredUnitsDict)
        {
            units.Add(unitManager.GetUnitForTip(unit.Key), unit.Value);
        }

         return units;
    }

    public Dictionary<ResourceType, float> CalculatePrice()
    {
        Dictionary<ResourceType, float> finalCommonSum = new Dictionary<ResourceType, float>()
        {
            [ResourceType.Gold] = 0,
            [ResourceType.Food] = 0,
            [ResourceType.Wood] = 0,
            [ResourceType.Stone] = 0,
            [ResourceType.Iron] = 0
        };

        float discount = boostManager.GetBoost(BoostType.AltarTryCost);
        float commonQuantity = 0;
        foreach(var unit in injuredUnitsDict)
        {
            commonQuantity += injuredUnitsDict[unit.Key];

            List<Cost> unitCost = unitManager.GetUnitForTip(unit.Key).costs;

            for(int i = 0; i < unitCost.Count; i++)
            {
                float amount = unitCost[i].amount * injuredUnitsDict[unit.Key] * (1 + discount);

                finalCommonSum[unitCost[i].type] += amount;
            }
        }

        List<ResourceType> costList = new List<ResourceType>(finalCommonSum.Keys);
        foreach(var itemCost in costList)
        {
            finalCommonSum[itemCost] = 
                (finalCommonSum[itemCost] == 0) 
                ? minCost
                : Mathf.Round(finalCommonSum[itemCost] * percentCommonCost);
        }

        currentPrice = finalCommonSum;

        return finalCommonSum;
    }

    public bool IsMinPrice()
    {
        foreach(var price in currentPrice)
        {
            if(price.Value != minCost) return false;
        }

        return true;
    }

    public Dictionary<ResourceType, float> GetPrice()
    {
        return currentPrice;
    }

    public void ChangeUnitsAmount(UnitsTypes unit, float newAmount)
    {
        injuredUnitsDict[unit] = (int)newAmount;
    }

    internal void Pay(ResourceType resourceType, float amount)
    {
        resourcesManager.ChangeResource(resourceType, -amount);
    }

    public void PayAllResources()
    {
        foreach(var price in currentPrice)
            Pay(price.Key, price.Value);
    }

    public bool CheckInjured()
    {
        return infirmary.GetInjuredCount() > 0;
    }

    public Dictionary<UnitsTypes, int> GetUnitsFromInfirmary()
    {
        Dictionary<UnitsTypes, int> tempDict = new Dictionary<UnitsTypes, int>();

        Dictionary<UnitsTypes, InjuredUnitData> units = infirmary.GetCurrentInjuredDict();
        foreach(var item in units)
            tempDict.Add(item.Key, item.Value.quantity);

        return tempDict;
    }

    public void HealUnits()
    {       
        foreach(var unit in injuredUnitsDict)
        {
            for(int i = 0; i < unit.Value; i++)
                playersArmy.ResurrectionUnit(unit.Key);
        }
    }

    #endregion

    public List<ResourceType> GenerateCombitation()
    {
        List<ResourceType> resourcesList = new List<ResourceType>();

        for(int i = 0; i < defaultResList.Count; i++)
            resourcesList.Add(defaultResList[UnityEngine.Random.Range(0, defaultResList.Count)]);

        return resourcesList;
    }

    public void VisitRegistration()
    {
        isVisited = true;
        objectOwner.SetVisitStatus(isVisited);
    }

    private void ResetVisit(int counter)
    {
        isVisited = false;
        objectOwner.SetVisitStatus(isVisited);
    }

    public bool GetVisitStatus()
    {
        return isVisited;
    }

    private void OnEnable()
    {
        EventManager.NewWeek += ResetVisit;
    }


    private void OnDisable()
    {
        EventManager.NewWeek -= ResetVisit;
    }
}
