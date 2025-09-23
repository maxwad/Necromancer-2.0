using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static Enums;

public partial class InfirmaryManager : MonoBehaviour
{
    private float currentCapacity;
    private float dayToDeath;

    public Dictionary<UnitsTypes, InjuredUnitData> injuredDict = new Dictionary<UnitsTypes, InjuredUnitData>();

    private PlayerStats playerStats;
    private GameObject player;
    
    [Inject]
    public void Construct(PlayerStats playerStats, GMPlayerMovement globalPlayer)
    {
        this.playerStats = playerStats;
        player = globalPlayer.gameObject;
    }

    private void Start()
    {
        currentCapacity = playerStats.GetCurrentParameter(PlayersStats.Infirmary);
        dayToDeath = playerStats.GetCurrentParameter(PlayersStats.InfirmaryTime);


        //FOR ALTAR TESTING
        injuredDict.Add(UnitsTypes.Barbarians, new InjuredUnitData(3, 6));
        injuredDict.Add(UnitsTypes.Mercenaries, new InjuredUnitData(7, 6));
        injuredDict.Add(UnitsTypes.Paladins, new InjuredUnitData(5, 6));
    }

    public void AddUnitToInfirmary(UnitsTypes unitType)
    {
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
        if(injuredDict.ContainsKey(unitType) == false)
            return;

        injuredDict[unitType].quantity--;
        injuredDict[unitType].term = dayToDeath;

        if(injuredDict[unitType].quantity == 0)
            injuredDict.Remove(unitType);

        EventManager.OnUpdateInfirmaryUIEvent(GetInjuredCount(), currentCapacity);
    }

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
                RemoveUnitFromInfirmary(unit);
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
