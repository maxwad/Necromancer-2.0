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
    //public List<UnitsTypes> injuredList = new List<UnitsTypes>();
    //public Dictionary<UnitsTypes, float> injuredTimersDict = new Dictionary<UnitsTypes, float>();
    public Dictionary<UnitsTypes, InjuredUnitData> injuredDict = new Dictionary<UnitsTypes, InjuredUnitData>();

    private PlayerStats playerStats;
    private GameObject player;

    private void Start()
    {
        playerStats = GlobalStorage.instance.playerStats;
        player = GlobalStorage.instance.globalPlayer.gameObject;
        currentCapacity = playerStats.GetCurrentParameter(PlayersStats.Infirmary);
        dayToDeath = playerStats.GetCurrentParameter(PlayersStats.InfirmaryTime);


        //FOR ALTAR TESTING
        injuredDict.Add(UnitsTypes.Barbarians, new InjuredUnitData(3, 60));
        injuredDict.Add(UnitsTypes.Mercenaries, new InjuredUnitData(7, 60));
        injuredDict.Add(UnitsTypes.Paladins, new InjuredUnitData(5, 60));

    }

    public void AddUnitToInfirmary(UnitsTypes unitType)
    {
        //if(injuredList.Count < currentCapacity) 
        //{
        //    injuredList.Add(unitType);
        //    EventManager.OnUpdateInfirmaryUIEvent(injuredList.Count, currentCapacity);

        //    if(injuredTimersDict.ContainsKey(unitType) == false)
        //        injuredTimersDict.Add(unitType, dayToDeath);
        //}

        if(GetInjuredCount() < currentCapacity)
        {
            if(injuredDict.ContainsKey(unitType) == false)
            {
                injuredDict.Add(unitType, new InjuredUnitData(1, dayToDeath));
            }
            else
            {
                injuredDict[unitType].quantity++;
            }
        }
    }

    public int GetInjuredCount()
    {
        int count = 0;

        foreach(var unit in injuredDict)
        {
            count += unit.Value.quantity;
        }

        return count;
    }

    public void RemoveUnitFromInfirmary(UnitsTypes unitType)
    {
        //int index = injuredList.Count - 1;
        //while(index >= 0)
        //{
        //    if(injuredList[index] == unitType)
        //    {
        //        injuredList.RemoveAt(index);
        //        break;
        //    }

        //    index--;
        //}

        //List<UnitsTypes> typeList = new List<UnitsTypes>(injuredDict.Keys);
        if(injuredDict.ContainsKey(unitType) == true)
        {
            injuredDict[unitType].quantity--;
            injuredDict[unitType].term = dayToDeath;

            if(injuredDict[unitType].quantity == 0)
                injuredDict.Remove(unitType);
        }
        else
        {
            return;
        }

        EventManager.OnUpdateInfirmaryUIEvent(GetInjuredCount(), currentCapacity);

        //bool unitFinded = false;
        //foreach(var unit in injuredList)
        //{
        //    if(unit == unitType)
        //    {
        //        unitFinded = true;
        //        break;
        //    }
        //}

        //if(unitFinded == false) injuredTimersDict.Remove(unitType);

    }

    //public int GetCurrentInjuredQuantity()
    //{
    //    return injuredList.Count;
    //}

    //public int GetCurrentInjuredQuantityByType(UnitsTypes unitType)
    //{
    //    int count = 0;

    //    if(injuredDict.ContainsKey(unitType) == true)
    //        count = injuredDict[unitType].quantity;

    //    return count;
    //}

    //public List<UnitsTypes> GetCurrentInjuredList()
    //{
    //    return injuredList;
    //}

    public Dictionary<UnitsTypes, InjuredUnitData> GetCurrentInjuredDict()
    {
        return injuredDict;
    }

    public float GetCurrentCapacity()
    {
        return currentCapacity;
    }

    public int GetEmptySpaces()
    {
        return (int)currentCapacity - GetInjuredCount();
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
        List<UnitsTypes> tempList = new List<UnitsTypes>(injuredDict.Keys);

        foreach(var unit in tempList)
        {
            injuredDict[unit].term--; 
        }

        int countOfUnitsDeath = 0;

        foreach(var unit in tempList)
        {
            if(injuredDict[unit].term <= 0) 
            {
                countOfUnitsDeath++;
                //injuredList.Remove(unit);
                RemoveUnitFromInfirmary(unit);
                //injuredDict[unit].quantity--;


                //int unitsLeft = GetCurrentInjuredQuantityByType(unit);
                //if(unitsLeft == 0)
                //{
                //    injuredTimersDict.Remove(unit);
                //}
                //else
                //{
                //    injuredTimersDict[unit] = dayToDeath;
                //}
            }
        }

        if(countOfUnitsDeath > 0)
        {
            BonusTipUIManager.ShowVisualEffect(player.transform.position, VisualEffects.Death, countOfUnitsDeath);
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
