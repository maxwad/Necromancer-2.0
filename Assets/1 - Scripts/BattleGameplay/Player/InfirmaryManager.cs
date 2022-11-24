using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class InfirmaryManager : MonoBehaviour
{
    private float currentCapacity;
    private float dayToDeath;

    //we have 2 structure for convenience: dict for timeCount and list for simple information about capacity
    public List<UnitsTypes> injuredList = new List<UnitsTypes>();
    public Dictionary<UnitsTypes, float> injuredTimersDict = new Dictionary<UnitsTypes, float>();

    private PlayerStats playerStats;
    private GameObject player;

    private void Start()
    {
        playerStats = GlobalStorage.instance.playerStats;
        player = GlobalStorage.instance.player;
        currentCapacity = playerStats.GetCurrentParameter(PlayersStats.Infirmary);
        dayToDeath = playerStats.GetCurrentParameter(PlayersStats.InfirmaryTime);
    }

    public void AddUnitToInfirmary(UnitsTypes unitType)
    {
        if(injuredList.Count < currentCapacity) 
        {
            injuredList.Add(unitType);
            EventManager.OnUpdateInfirmaryUIEvent(injuredList.Count, currentCapacity);

            if(injuredTimersDict.ContainsKey(unitType) == false)
                injuredTimersDict.Add(unitType, dayToDeath);
        }
    }

    public void RemoveUnitFromInfirmary(UnitsTypes unitType)
    {
        int index = injuredList.Count - 1;
        while(index >= 0)
        {
            if(injuredList[index] == unitType)
            {
                injuredList.RemoveAt(index);
                break;
            }

            index--;
        }

        EventManager.OnUpdateInfirmaryUIEvent(injuredList.Count, currentCapacity);

        bool unitFinded = false;
        foreach(var unit in injuredList)
        {
            if(unit == unitType)
            {
                unitFinded = true;
                break;
            }
        }

        if(unitFinded == false) injuredTimersDict.Remove(unitType);

    }

    public int GetCurrentInjuredQuantity()
    {
        return injuredList.Count;
    }

    public List<UnitsTypes> GetCurrentInjuredList()
    {
        return injuredList;
    }

    public float GetCurrentCapacity()
    {
        return currentCapacity;
    }

    public int GetEmptySpaces()
    {
        return (int)currentCapacity - injuredList.Count;
    }

    private void UpgradeParameter(PlayersStats stat, float value)
    {
        if(stat == PlayersStats.Infirmary)
            currentCapacity = value;

        if(stat == PlayersStats.InfirmaryTime)
            dayToDeath = value;
    }

    private void NewDay()
    {
        foreach(var unit in injuredTimersDict)
        {
            injuredTimersDict[unit.Key]--;
        }

        int countOfDeath = 0;
        Dictionary<UnitsTypes, float> tempDict = injuredTimersDict;
        foreach(var unit in tempDict)
        {
            if(unit.Value <= 0) 
            {
                injuredTimersDict.Remove(unit.Key);
                injuredList.Remove(unit.Key);
                countOfDeath++;               
            }
        }

        if(countOfDeath > 0)
        {
            BonusTipUIManager.ShowVisualEffectInBattle(player.transform.position, VisualEffects.Death, countOfDeath);
        }
    }

    private void OnEnable()
    {
        EventManager.SetNewPlayerStat += UpgradeParameter;
        EventManager.NewMove += NewDay;
    }

    private void OnDisable()
    {
        EventManager.SetNewPlayerStat -= UpgradeParameter;
        EventManager.NewMove -= NewDay;
    }
}
