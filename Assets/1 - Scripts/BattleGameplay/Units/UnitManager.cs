using System;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class UnitManager : MonoBehaviour
{
    public Dictionary<UnitsTypes, int> currentLevelOfUnitsDict = new Dictionary<UnitsTypes, int>();

    public List<UnitSO> allUnitsSO;

    public List<Unit> allUnitsBase = new List<Unit>();
    public List<Unit> allCurrentBaseUnitsByTypes = new List<Unit>();
    public List<Unit> allCurrentBoostUnitsByTypes = new List<Unit>();

    private Dictionary<UnitsTypes, Sprite> allUnitsIconsDict = new Dictionary<UnitsTypes, Sprite>();

    private UnitBoostManager boostManager;

    private void Awake()
    {
        boostManager = GlobalStorage.instance.unitBoostManager;        
    }

    public void LoadUnits()
    {
        StartCreatingPlayersArmy();
        //we return signal to next step load in PlayersArmy script
    } 

    #region Start army creation 
    private void StartCreatingPlayersArmy()
    {
        CreateCurrentLevelOfUnits();
        CreateAllUnitsBase();
    }

    private void CreateCurrentLevelOfUnits()
    {
        //формируем список начальных уровней всех юнитов
        foreach (UnitsTypes item in Enum.GetValues(typeof(UnitsTypes)))
            currentLevelOfUnitsDict.Add(item, 2);
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
        foreach (UnitsTypes itemType in Enum.GetValues(typeof(UnitsTypes)))
        {
            foreach (var itemUnit in allUnitsBase)
            {
                if (itemUnit.unitType == itemType && itemUnit.level == 2)
                {
                    allCurrentBaseUnitsByTypes.Add(itemUnit);
                    break;
                }
            }
        }

        CreateAllCurrentBoostUnitsByTypes();
    }

    private void CreateAllCurrentBoostUnitsByTypes()
    {
        //накладываем эффекты на все базовые юниты
        foreach (var item in allCurrentBaseUnitsByTypes)
        {
            allCurrentBoostUnitsByTypes.Add(boostManager.AddBonusStatsToUnit(item));

            allUnitsIconsDict.Add(item.unitType, item.unitIcon);
        }
            
        EventManager.OnAllUnitsIsReadyEvent();
    }

    public Dictionary<UnitsTypes, Sprite> GetUnitsIcons()
    {
        return allUnitsIconsDict;
    }

    #endregion


    public Unit[] GetUnitsForPlayersArmy(UnitsTypes[] playersArmy)
    {
        Unit[] army = new Unit[Enum.GetValues(typeof(UnitsTypes)).Length];

        for (int i = 0; i < army.Length; i++)
        {
            if (i < playersArmy.Length)
            {
                foreach (var item in allCurrentBoostUnitsByTypes)
                {
                    if (playersArmy[i] == item.unitType)
                    {
                        army[i] = item;
                    }
                }
            }
            else
            {
                army[i] = null;
            }
        }         
        
        return army;
    }

    public Unit GetNewSquad(UnitsTypes type)
    {
        foreach(var unit in allCurrentBoostUnitsByTypes)
        {
            if(unit.unitType == type)
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

    public List<Unit> GetActualArmy()
    {
        return allCurrentBoostUnitsByTypes;
    }
}
