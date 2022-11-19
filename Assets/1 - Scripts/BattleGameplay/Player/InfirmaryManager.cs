using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class InfirmaryManager : MonoBehaviour
{
    private float currentCapacity;
    private float dayToDeath;

    public List<UnitsTypes> injuredList = new List<UnitsTypes>();
    public Dictionary<UnitsTypes, int> injuredDict = new Dictionary<UnitsTypes, int>();

    PlayerStats playerStats;

    private void Start()
    {
        playerStats = GlobalStorage.instance.playerStats;
        currentCapacity = playerStats.GetCurrentParameter(PlayersStats.Infirmary);
        dayToDeath = playerStats.GetCurrentParameter(PlayersStats.InfirmaryTime);
    }

    private void SetStartInfarmary(PlayersStats type, float value)
    {
        if(type == PlayersStats.Infirmary) currentCapacity = value;
    }

    public void AddUnitToInfirmary(UnitsTypes unitType)
    {
        if(injuredList.Count < currentCapacity) 
        {
            injuredList.Add(unitType);
            EventManager.OnUpdateInfirmaryUIEvent(injuredList.Count, currentCapacity);
        }
    }

    public void RemoveUnitFromInfirmary(UnitsTypes unitType)
    {
        // mode == false = death; mode == true - resurrect 
        // order == false = random; order == true - the last one 

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

        //index = (injuredList.Count - 1) >= 0 ? (injuredList.Count - 1) : 0;

        //if(mode == false)
        //{
        //    if(quantity == 0)
        //    {
        //        injuredList.Clear();
        //    }
        //    else
        //    {
        //        for(int i = 0; i < quantity; i++)
        //        {
        //            if(order == false) index = Random.Range(0, injuredList.Count);

        //            if(injuredList.Count != 0) injuredList.Remove(injuredList[index]);
        //        }
        //    }                      
        //}
        //else
        //{
        //    for(int i = 0; i < quantity; i++)
        //    {
        //        index = (injuredList.Count - 1) >= 0 ? (injuredList.Count - 1) : 0;

        //        if(order == false) index = Random.Range(0, injuredList.Count);

        //        if(injuredList.Count != 0)
        //        {
        //            EventManager.OnResurrectUnitEvent(injuredList[index]);
        //            injuredList.Remove(injuredList[index]);
        //        }   
        //    }
        //}

        //EventManager.OnUpdateInfirmaryUIEvent(injuredList.Count, currentCapacity);
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

    private void UpgradeParameter(PlayersStats stat, float value)
    {
        if(stat == PlayersStats.Infirmary)
            currentCapacity = value;

        if(stat == PlayersStats.Infirmary)
            currentCapacity = value;
    }

    private void OnEnable()
    {
        EventManager.SetNewPlayerStat += UpgradeParameter;
        //EventManager.WeLostOneUnit += AddUnitToInfirmary;
        //EventManager.RemoveUnitFromInfirmary += RemoveUnitFromInfirmary;
    }

    private void OnDisable()
    {
        EventManager.SetNewPlayerStat -= UpgradeParameter;
        //EventManager.WeLostOneUnit -= AddUnitToInfirmary;
        //EventManager.RemoveUnitFromInfirmary -= RemoveUnitFromInfirmary;
    }
}
