using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class Boost
{
    public BoostSender sender;
    public BoostEffect effect;
    public float value;

    public Boost(BoostSender boostSender, BoostEffect boostEffect, float boostValue)
    {
        sender = boostSender;
        effect = boostEffect;
        value = boostValue;

    }
}

public class BattleBoostManager : MonoBehaviour
{
    private PlayerStats playerStats;
    private BoostType[] boostTypes;
    private Dictionary<BoostType, List<Boost>> boostItemsDict = new Dictionary<BoostType, List<Boost>>();
    private Dictionary<BoostType, float> commonBoostDict = new Dictionary<BoostType, float>();

    int count = 0;
    private void Awake()
    {
        playerStats = GlobalStorage.instance.playerStats;
        boostTypes = new BoostType[Enum.GetValues(typeof(BoostType)).Length];

        int counter = 0;
        foreach(BoostType itemType in Enum.GetValues(typeof(BoostType)))
        {
            boostTypes[counter] = itemType;
            counter++;
        }

        foreach(var itemType in boostTypes)
        {
            boostItemsDict[itemType] = new List<Boost>();
            commonBoostDict[itemType] = 0f;
        }
    }


    public void SetBoost(BoostType type, BoostSender sender, BoostEffect effect, float value)
    {
        if(type == BoostType.Nothing) return;

        boostItemsDict[type].Add(new Boost(sender, effect, value));
        RecalculateBoost(type);

        if(GlobalStorage.instance.isGlobalMode == false) EventManager.OnShowBoostEffectEvent(sender, type, value);
    }

    public void DeleteBoost(BoostType type, BoostSender sender, float value)
    {
        if(type == BoostType.Nothing) return;
        List<Boost> boostList = boostItemsDict[type];

        foreach(var boost in boostList)
        {
            if(sender == BoostSender.Spell || sender == BoostSender.Calendar)
            {
                if(boost.sender == sender)
                {
                    boostList.Remove(boost);
                    break;
                }
            }
            else
            {
                if(boost.sender == sender && boost.value == value)
                {
                    boostList.Remove(boost);
                    break;
                }
            }
        }

        boostItemsDict[type] = boostList;
        RecalculateBoost(type);
    }


    private void RecalculateBoost(BoostType type, bool sendMode = true)
    {
        float result = 0f;
        for(int i = 0; i < boostItemsDict[type].Count; i++)
        {
            result += boostItemsDict[type][i].value;
        }

        commonBoostDict[type] = result;

        //Debug.Log("Now " + type + " = " + result);
        if(sendMode == true)
        {
            if(result < -99) result = -99f;
            EventManager.OnSetBattleBoostEvent(type, result / 100);

            PlayersStats stat = BoostConverter.instance.BoostTypeToPlayerStat(type);
            if(stat != PlayersStats.Level) playerStats.ForceUpdateStat(stat);
        }
    }

    public Dictionary<BoostType, List<Boost>> GetBoostDict()
    {
        return boostItemsDict;
    }

    public float GetBoost(BoostType boostType)
    {
        float result = 0;

        if(boostType != BoostType.Nothing && commonBoostDict.ContainsKey(boostType) == true)
            result = commonBoostDict[boostType] / 100;

        if(result < -0.99) result = -0.99f;
        count++;

        //Debug.Log(count);
        return result;
    }

    private void ClearBattleBonuses()
    {
        foreach(var boostList in boostItemsDict)
        {
            List<Boost> tempList = boostList.Value;

            for(int i = tempList.Count - 1; i >= 0; i--)
            {
                if(tempList[i].sender != BoostSender.Calendar)
                {
                    tempList.Remove(tempList[i]);
                }
            }
            RecalculateBoost(boostList.Key, false);
        }
    }

    private void OnEnable()
    {
        //EventManager.BoostUnitStat += SetBoost;
        EventManager.EndOfBattle += ClearBattleBonuses;
    }


    private void OnDisable()
    {
        //EventManager.BoostUnitStat -= SetBoost;
        EventManager.EndOfBattle -= ClearBattleBonuses;
    }
}
