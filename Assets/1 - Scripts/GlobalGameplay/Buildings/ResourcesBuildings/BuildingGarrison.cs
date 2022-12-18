using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class BuildingGarrison : MonoBehaviour
{
    private Dictionary<UnitsTypes, int> currentAmounts = new Dictionary<UnitsTypes, int>();


    public Dictionary<UnitsTypes, int> AddUnits(UnitsTypes unit, int amount, bool addMode = true)
    {
        if(currentAmounts.ContainsKey(unit) == true)
        {
            currentAmounts[unit] = (addMode == true) ? (currentAmounts[unit] + amount) : amount;
        }
        else
        {
            currentAmounts.Add(unit, amount);
        }

        if(currentAmounts[unit] == 0)
            currentAmounts.Remove(unit);

        return currentAmounts;
    }

    public Dictionary<UnitsTypes, int> GetUnitsInGarrison()
    {
        return currentAmounts;
    }

    //private void HiringInGarrison()
    //{
    //    foreach(var unit in growthAmounts)
    //    {
    //        int amount = GetHiringGrowth(unit.unitType);
    //        potentialAmounts[unit.unitType] += amount;
    //    }
    //}

    //private void OnEnable()
    //{
    //    EventManager.WeekEnd += HiringInGarrison;
    //}

    //private void OnDisable()
    //{
    //    EventManager.WeekEnd -= HiringInGarrison;
    //}

}
