using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

[Serializable]
public class HiringAmount
{
    public CastleBuildings building;
    public int amount;
}

public class Garrison : MonoBehaviour
{
    private UnitManager unitManager;
    private FortressBuildings fortressBuildings;

    public List<HiringAmount> hiringAmounts;

    private Dictionary<UnitsTypes, int> potentialAmounts = new Dictionary<UnitsTypes, int>();
    private Dictionary<UnitsTypes, int> currentAmounts = new Dictionary<UnitsTypes, int>();

    private void Awake()
    {
        foreach(UnitsTypes item in Enum.GetValues(typeof(UnitsTypes)))
            potentialAmounts.Add(item, 0);

        foreach(UnitsTypes item in Enum.GetValues(typeof(UnitsTypes)))
            currentAmounts.Add(item, 0);
    }

    private void Start()
    {
        unitManager = GlobalStorage.instance.unitManager;
        fortressBuildings = GlobalStorage.instance.fortressBuildings;
    }

    public int GetHiringGrowth(CastleBuildings building)
    {
        int amount = 0;
        for(int i = 0; i < hiringAmounts.Count; i++)
        {
            if(hiringAmounts[i].building == building)
            {
                amount = hiringAmounts[i].amount;
                break;
            }
        }

        amount += (int)fortressBuildings.GetBonusAmount(CastleBuildings.RecruitmentCenter);

        return amount;
    }

    public int GetHiringAmount(UnitsTypes unitType)
    {
        return potentialAmounts[unitType];
    }

    public void ChangeUnitsAmount(UnitsTypes unit, int amount)
    {
        potentialAmounts[unit] += amount;

        if(potentialAmounts[unit] < 0) potentialAmounts[unit] = 0;
    }
}
