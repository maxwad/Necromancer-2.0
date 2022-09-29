using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class InfirmaryManager : MonoBehaviour
{
    private float currentCapacity;

    public List<UnitsTypes> injuredList = new List<UnitsTypes>();

    PlayerStats playerStats;

    private void Start()
    {
        playerStats = GlobalStorage.instance.playerStats;
        currentCapacity = playerStats.GetMaxParameter(PlayersStats.Infirmary);
    }

    private void SetStartInfarmary(PlayersStats type, float value)
    {
        if(type == PlayersStats.Infirmary) currentCapacity = value;
    }

    private void AddUnitToInfirmary(UnitsTypes unitType)
    {
        if(injuredList.Count < currentCapacity) 
        {
            injuredList.Add(unitType);
            EventManager.OnUpdateInfirmaryUIEvent(injuredList.Count, currentCapacity);
        }
    }

    private void RemoveUnitFromInfirmary(bool mode, bool order, float quantity)
    {
        // mode == false = death; mode == true - resurrect 
        // order == false = random; order == true - the last one 

        int index = (injuredList.Count - 1) >= 0 ? (injuredList.Count - 1) : 0;

        if(mode == false)
        {
            if(quantity == 0)
            {
                injuredList.Clear();
            }
            else
            {
                for(int i = 0; i < quantity; i++)
                {
                    if(order == false) index = Random.Range(0, injuredList.Count);

                    if(injuredList.Count != 0) injuredList.Remove(injuredList[index]);
                }
            }                      
        }
        else
        {
            for(int i = 0; i < quantity; i++)
            {
                index = (injuredList.Count - 1) >= 0 ? (injuredList.Count - 1) : 0;

                if(order == false) index = Random.Range(0, injuredList.Count);

                if(injuredList.Count != 0)
                {
                    EventManager.OnResurrectUnitEvent(injuredList[index]);
                    injuredList.Remove(injuredList[index]);
                }   
            }
        }

        EventManager.OnUpdateInfirmaryUIEvent(injuredList.Count, currentCapacity);
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

    private void OnEnable()
    {
        EventManager.WeLostOneUnit += AddUnitToInfirmary;
        EventManager.RemoveUnitFromInfirmary += RemoveUnitFromInfirmary;
    }

    private void OnDisable()
    {
        EventManager.WeLostOneUnit -= AddUnitToInfirmary;
        EventManager.RemoveUnitFromInfirmary -= RemoveUnitFromInfirmary;
    }
}
