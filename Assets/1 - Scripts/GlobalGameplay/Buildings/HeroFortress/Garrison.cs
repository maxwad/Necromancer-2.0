using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class Garrison : MonoBehaviour
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

    public int GetUnitAmount(UnitsTypes unit)
    {
        return (currentAmounts.ContainsKey(unit) == true) ? currentAmounts[unit] : 0;
    }
}
