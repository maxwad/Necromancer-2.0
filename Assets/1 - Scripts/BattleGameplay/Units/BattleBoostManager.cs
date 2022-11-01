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

    private BoostType[] boostTypes;
    private Dictionary<BoostType, List<Boost>> boostItemsDict = new Dictionary<BoostType, List<Boost>>();
    private Dictionary<BoostType, float> commonBoostDict = new Dictionary<BoostType, float>();


    private void Awake()
    {
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


    public void SetBoost(BoostType type, BoostSender sender,BoostEffect effect, float value)
    {
        if(type == BoostType.Nothing) return;

        boostItemsDict[type].Add(new Boost(sender, effect, value));
        RecalculateBoost(type);
    }

    public void DeleteBoost(BoostType type, BoostSender sender, float value)
    {
        if(type == BoostType.Nothing) return;
        List<Boost> boostList = boostItemsDict[type];

        foreach(var boost in boostList)
        {
            if(sender == BoostSender.Spell)
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


    private void RecalculateBoost(BoostType type)
    {
        float result = 0f;
        for(int i = 0; i < boostItemsDict[type].Count; i++)
        {
            result += boostItemsDict[type][i].value;
        }

        commonBoostDict[type] = result;

        //Debug.Log(type + " now is " + result);
        EventManager.OnSetBattleBoostEvent(type, result / 100);
    }

    public Dictionary<BoostType, List<Boost>> GetBoostDict()
    {
        return boostItemsDict;
    }

    public float GetBoost(BoostType boostType)
    {
        return (boostType == BoostType.Nothing) ? 0 : commonBoostDict[boostType] / 100;
    }

    private void ClearBattleBonuses()
    {
        foreach(var boostList in boostItemsDict)
        {
            foreach(var boost in boostList.Value)
            {
                if(boost.sender == BoostSender.Spell || boost.sender == BoostSender.Rune)
                {
                    boostList.Value.Remove(boost);
                }
            }
            RecalculateBoost(boostList.Key);
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
