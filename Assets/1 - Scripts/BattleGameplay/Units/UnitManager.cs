using System;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class UnitManager : MonoBehaviour
{
    public Dictionary<UnitsTypes, int> currentLevelOfUnitsDict = new Dictionary<UnitsTypes, int>();
    public List<UnitsTypes> unitsTypesList = new List<UnitsTypes>();

    public List<UnitSO> allUnitsSO;

    public List<Unit> allUnitsBase = new List<Unit>();
    public List<Unit> allUnitsByTypes = new List<Unit>();
    public List<Unit> allCurrentBoostUnitsByTypes = new List<Unit>();

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

        //формируем список начальных уровней всех юнитов
        foreach (UnitsTypes type in unitsTypesList)
            currentLevelOfUnitsDict.Add(type, 2);
    }

    private void CreateAllUnitsBase()
    {
        //переводим все скриптеблќбджекты в дикт и получаем ¬—≈ юниты базовых параметров
        foreach (UnitSO item in allUnitsSO)
            allUnitsBase.Add(new Unit(item));

        CreateAllCurrentBaseUnitsByTypes();
    }

    private void CreateAllCurrentBaseUnitsByTypes()
    {
        //формируем список из юнитов (по одному каждого типа)
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

        CreateAllCurrentBoostUnitsByTypes();
    }

    private void CreateAllCurrentBoostUnitsByTypes()
    {
        //накладываем эффекты на все базовые юниты
        foreach (var item in allUnitsByTypes)
        {
            //allCurrentBoostUnitsByTypes.Add(boostManager.AddBonusStatsToUnit(item));
            allUnitsIconsDict.Add(item.unitType, item.unitIcon);
        }

        EventManager.OnAllUnitsIsReadyEvent();
    }

    public Dictionary<UnitsTypes, Sprite> GetUnitsIcons()
    {
        return allUnitsIconsDict;
    }

    public Unit[] GetAllUnits()
    {
        Unit[] army = new Unit[unitsTypesList.Count];
        for (int i = 0; i < army.Length; i++)
        {
            foreach(var item in allUnitsByTypes)
            {
                if(unitsTypesList[i] == item.unitType)
                {
                    army[i] = item;
                    break;
                }
            }
        }

        return army;
    }

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

        //    if(level == 1)
        //    {
        //        foreach(var unit in allUnitsByTypes)
        //        {
        //            if(unit.unitType == type) return unit;
        //        }
        //    }
        //    else
        //    {
        //        if(currentLevelOfUnitsDict[type] >= level)
        //        {
        //            foreach(var unit in allUnitsBase)
        //            {
        //                if(unit.unitType == type && unit.level == level)
        //                {
        //                    return unit;
        //                }
        //            }
        //        }            
        //    }        

        return null;
    }

    public void UpgradeUnitLevel(UnitsTypes unitType, int level)
    {
        currentLevelOfUnitsDict[unitType] = level;
    }

    public List<Unit> GetActualArmy()
    {
        return allUnitsByTypes;
    }

    public List<UnitsTypes> GetUnitsTypesList()
    {
        return unitsTypesList;
    }
}
