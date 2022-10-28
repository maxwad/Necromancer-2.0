using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class BattleBoostManager : MonoBehaviour
{    private class Boost
    {
        public BoostSender sender;
        public float value;

        public Boost(BoostSender boostSender, float boostValue)
        {
            sender = boostSender;
            value = boostValue;
        }
    }

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


    public void SetBoost(BoostType type, BoostSender sender, float value)
    {
        boostItemsDict[type].Add(new Boost(sender, value));
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

        EventManager.OnSetBattleBoostEvent(type, result);
    }

    private void SetBoost(bool boostAll, bool addBoost, BoostSender sender, UnitStats stat, float value, UnitsTypes types = UnitsTypes.Militias)
    {
        Debug.Log("We set " + addBoost + " to " + stat + " boost = " + value);
    }

    public float GetBoost(BoostType boostType)
    {
        return commonBoostDict[boostType];
    }

    private void OnEnable()
    {
        EventManager.BoostUnitStat += SetBoost;
    }

    private void OnDisable()
    {
        EventManager.BoostUnitStat -= SetBoost;
    }

}
