using System;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class PlayerBoostManager : MonoBehaviour
{
    private struct Boost
    {
        public BoostSender sender;
        public float value;

        public Boost(BoostSender boostSender, float boostValue)
        {
            sender = boostSender;
            value = boostValue;
        }
    }

    private Dictionary<PlayersStats, List<Boost>> allBoostDict = new Dictionary<PlayersStats, List<Boost>>();

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            float value = (float)UnityEngine.Random.Range(10, 30) / 100;
            ChangeBoost(true, BoostSender.Spell, PlayersStats.Health, value);
        }
    }

    private void ChangeBoost(bool mode, BoostSender sender, PlayersStats stats, float value)
    {
        float newBoostValue = 0;
        List<Boost> currentBoostList = allBoostDict[stats];

        if(mode == true)
        {
            currentBoostList.Add(new Boost(sender, value));
        }
        else
        {
            foreach(var item in currentBoostList)
            {
                if(item.sender == sender)
                {
                    currentBoostList.Remove(item);
                    break;
                }
            }
        }

        foreach(var item in currentBoostList)
        {
            newBoostValue += item.value;
        }

        EventManager.OnSetBoostToStatEvent(stats, newBoostValue);
    }

    private void Initialize()
    {
        foreach(PlayersStats itemStat in Enum.GetValues(typeof(PlayersStats)))
        {
            allBoostDict.Add(itemStat, new List<Boost>());
        }
    }

    private void OnEnable()
    {
        EventManager.BoostStat += ChangeBoost;
    }

    private void OnDisable()
    {
        EventManager.BoostStat -= ChangeBoost;
    }
}
