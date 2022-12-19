using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class Garrison : MonoBehaviour
{
    private Dictionary<UnitsTypes, int> currentAmounts = new Dictionary<UnitsTypes, int>();
    public int defendersPerDay = 10;

    public void AddUnits(UnitsTypes unit, int amount, bool addMode = true)
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

        EventManager.OnUpdateSiegeTermEvent(this);
    }

    public Dictionary<UnitsTypes, int> GetUnitsInGarrison()
    {
        return currentAmounts;
    }

    public int GetUnitAmount(UnitsTypes unit)
    {
        return (currentAmounts.ContainsKey(unit) == true) ? currentAmounts[unit] : 0;
    }

    public float GetGarrisonAmount(bool intMode = false)
    {
        float amount = 0;

        foreach(var squad in currentAmounts)
            amount += squad.Value;

        return (intMode == false) ? Mathf.FloorToInt(amount / defendersPerDay) : amount / defendersPerDay;
    }

    public bool DecreaseGarrisonUnits()
    {
        if(GetGarrisonAmount(true) > 1)
        {
            DeleteUnits();
            return true;
        }
        else
        {
            currentAmounts.Clear();
            return false;
        }
    }

    public void DeleteUnits()
    {
        for(int i = defendersPerDay; i > 0;)
        {
            List<UnitsTypes> units = new List<UnitsTypes>(currentAmounts.Keys);
            foreach(var squad in units)
            {
                currentAmounts[squad]--;
                i--;

                if(currentAmounts[squad] == 0)
                    currentAmounts.Remove(squad);

                if(i == 0) break;
            }
        }
    }
}
