using System;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class BoostConverter : MonoBehaviour
{
    public static BoostConverter instance;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public BoostType PlayerStatToBoostType(PlayersStats stat)
    {
        BoostType result = BoostType.Nothing;

        if(stat == PlayersStats.SearchRadius) result = BoostType.BonusRadius;
        if(stat == PlayersStats.Luck) result = BoostType.CriticalDamage;
        if(stat == PlayersStats.Defence) result = BoostType.HeroDefence;

        return result;
    }

    public BoostType RuneToBoostType(RunesType rune)
    {
        BoostType result = BoostType.Nothing;

        int runeIndex = (int)rune;

        foreach(BoostType boost in Enum.GetValues(typeof(BoostType)))
            if((int)boost == runeIndex) result = boost;

        if(result == BoostType.Nothing) Debug.Log("Converted " + rune + " to " + result);       

        return result;
    }

    public RunesType BoostTypeToRune(BoostType boost)
    {
        RunesType result = RunesType.WeaponSpeed;

        int boostIndex = (int)boost;

        foreach(RunesType rune in Enum.GetValues(typeof(RunesType)))
        {
            if((int)rune == boostIndex) result = rune;
        }

        return result;
    }
}
