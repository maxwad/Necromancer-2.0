using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class Altar : MonoBehaviour
{
    private InfirmaryManager infirmary;
    private UnitManager unitManager;
    private ResourcesManager resourcesManager;
    private BoostManager boostManager;

    private ObjectOwner objectOwner;
    private bool isVisited = false;

    private Dictionary<UnitsTypes, float> injuredUnitsDict = new Dictionary<UnitsTypes, float>();
    private float percentCommonCost = 0.15f;
    private float minCost = 50f;

    private void Start()
    {
        infirmary = GlobalStorage.instance.infirmaryManager;
        unitManager = GlobalStorage.instance.unitManager;
        resourcesManager = GlobalStorage.instance.resourcesManager;
        boostManager = GlobalStorage.instance.boostManager;

        objectOwner = GetComponent<ObjectOwner>();
    }

    public Dictionary<ResourceType, float> GetPrice()
    {
        injuredUnitsDict.Clear();
        injuredUnitsDict = GetUnitsFromInfirmary();
        Dictionary<ResourceType, float> finalCommonSum = new Dictionary<ResourceType, float>();
        float discount = boostManager.GetBoost( BoostType.Altar);

        float commonQuantity = 0;
        foreach(var unit in injuredUnitsDict)
        {
            commonQuantity += injuredUnitsDict[unit.Key];

            List<Cost> unitCost = unitManager.GetUnitForTip(unit.Key).costs;

            for(int i = 0; i < unitCost.Count; i++)
            {
                float amount = unitCost[i].amount * injuredUnitsDict[unit.Key] * (1 + discount);

                if(finalCommonSum.ContainsKey(unitCost[i].type) == true)
                    finalCommonSum[unitCost[i].type] += amount;
                else
                    finalCommonSum.Add(unitCost[i].type, amount);

            }
        }

        List<ResourceType> costList = new List<ResourceType>(finalCommonSum.Keys);
        foreach(var itemCost in costList)
        {
            finalCommonSum[itemCost] = 
                (finalCommonSum[itemCost] == 0) 
                ? minCost
                : Mathf.Round(finalCommonSum[itemCost] * percentCommonCost);

            Debug.Log(itemCost + " = " + finalCommonSum[itemCost]);
        }

        return finalCommonSum;
    }

    private Dictionary<UnitsTypes, float> GetUnitsFromInfirmary()
    {
        injuredUnitsDict.Clear();
        Dictionary<UnitsTypes, float> tempDict = new Dictionary<UnitsTypes, float>();

        Dictionary<UnitsTypes, float> units = infirmary.GetCurrentInjuredDict();
        foreach(var item in units)
        {
            tempDict.Add(item.Key, infirmary.GetCurrentInjuredQuantityByType(item.Key));
        }

        return tempDict;
    }

    private void ResetVisit(int counter)
    {
        isVisited = false;
        objectOwner.SetVisitStatus(isVisited);
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
