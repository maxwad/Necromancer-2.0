using System;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class UnitManager : MonoBehaviour
{
    [HideInInspector] public int maxUnitLevel = 3;
    public List<UnitSO> allUnitsSO;

    public List<Unit> allUnitsBase = new List<Unit>();
    public List<Unit> allUnitsByTypes = new List<Unit>();

    public Dictionary<UnitsTypes, int> currentLevelOfUnitsDict = new Dictionary<UnitsTypes, int>();
    public List<UnitsTypes> unitsTypesList = new List<UnitsTypes>();
    private Dictionary<UnitsTypes, Sprite> allUnitsIconsDict = new Dictionary<UnitsTypes, Sprite>();

    public void LoadUnits()
    {
        StartCreatingPlayersArmy();
        //we return signal to next step load in PlayersArmy script
    } 

    private void StartCreatingPlayersArmy()
    {
        CreateCurrentLevelOfUnits();
        CreateAllUnitsBase();
    }

    private void CreateCurrentLevelOfUnits()
    {
        foreach(UnitsTypes item in Enum.GetValues(typeof(UnitsTypes)))
            unitsTypesList.Add(item);

        foreach (UnitsTypes type in unitsTypesList)
            currentLevelOfUnitsDict.Add(type, 1);
    }

    private void CreateAllUnitsBase()
    {
        foreach (UnitSO item in allUnitsSO)
            allUnitsBase.Add(new Unit(item));

        CreateAllCurrentBaseUnitsByTypes();
    }

    private void CreateAllCurrentBaseUnitsByTypes()
    {
        foreach (UnitsTypes type in unitsTypesList)
        {
            foreach (var itemUnit in allUnitsBase)
            {
                if (itemUnit.unitType == type && itemUnit.level == 1)
                {
                    allUnitsByTypes.Add(itemUnit);
                    break;
                }
            }
        }

        CreateIconDict();

        EventManager.OnAllUnitsIsReadyEvent();
    }

    private void CreateIconDict()
    {
        foreach (var item in allUnitsByTypes)
            allUnitsIconsDict.Add(item.unitType, item.unitIcon);
    }

    public Sprite GetUnitsIcon(UnitsTypes type)
    {
        return allUnitsIconsDict[type];
    }

    //public Unit[] GetAllUnits()
    //{
    //    Unit[] army = new Unit[unitsTypesList.Count];
    //    for (int i = 0; i < army.Length; i++)
    //    {
    //        foreach(var item in allUnitsByTypes)
    //        {
    //            if(unitsTypesList[i] == item.unitType)
    //            {
    //                army[i] = item;
    //                break;
    //            }
    //        }
    //    }

    //    return army;
    //}

    public Unit GetNewSquad(UnitsTypes type, int level = 1)
    {
        if(currentLevelOfUnitsDict[type] >= level)
        {
            foreach(var unit in allUnitsBase)
            {
                if(unit.unitType == type && unit.level == level)
                {
                    unit.isUnitActive = true;
                    return unit;
                }
            }
        }     

        return null;
    }

    public Unit GetUnitForTip(UnitsTypes type, int level = 1)
    {
        foreach(var unit in allUnitsBase)
        {
            if(unit.unitType == type && unit.level == level)
            {
                return unit;
            }
        }

        return null;
    }

    public void UpgradeUnitLevel(UnitsTypes unitType, int level)
    {
        currentLevelOfUnitsDict[unitType] = level;
    }

    public bool IsUnitOpen(UnitsTypes unitType, int level)
    {
        return currentLevelOfUnitsDict[unitType] >= level;
    }

    public List<Unit> GetActualArmy()
    {
        return allUnitsByTypes;
    }

    public List<UnitsTypes> GetUnitsTypesList()
    {
        return unitsTypesList;
    }

    public List<UnitsTypes> GetUnitsByBuildings(CastleBuildings building)
    {
        List<UnitsTypes> unitsList = new List<UnitsTypes>();

        foreach(var unit in allUnitsByTypes)
        {
            if(unit.unitHome == building) unitsList.Add(unit.unitType);
        }

        return unitsList;
    }
}
