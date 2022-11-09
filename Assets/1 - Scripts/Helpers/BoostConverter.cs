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
        //BoostType result = BoostType.Nothing;
        //int statIndex = (int)stat;

        //foreach(BoostType boost in Enum.GetValues(typeof(BoostType))) 
        //{
        //    if((int)boost == statIndex) result = boost;
        //    break;
        //}

        //if(stat == PlayersStats.SearchRadius) result = BoostType.BonusRadius;
        //if(stat == PlayersStats.Luck) result = BoostType.CriticalDamage;
        //if(stat == PlayersStats.Defence) result = BoostType.HeroDefence;
        //if(stat == PlayersStats.ExtraResourcesProduce) result = BoostType.ExtraResourcesProduce;
        //if(stat == PlayersStats.HealthRegeneration) result = BoostType.HealthRegeneration;
        //if(stat == PlayersStats.MovementDistance) result = BoostType.MovementPoints;
        //if(stat == PlayersStats.ExtraExpReward) result = BoostType.ExpAfterBattle;

        //if(result == BoostType.Nothing) Debug.Log("FAIL: Converted " + stat + " to " + result);
        BoostType result = ((int)stat > 1000) ? BoostType.Nothing : (BoostType)stat;
        //Debug.Log("FAIL: Converted " + stat + " to " + result);
        return result;
    }

    public PlayersStats BoostTypeToPlayerStat(BoostType boost)
    {
        PlayersStats result = PlayersStats.Level;
        int boostIndex = (int)boost;

        foreach(PlayersStats stat in Enum.GetValues(typeof(PlayersStats)))
        {
            if((int)stat == boostIndex) 
            { 
                result = stat;
                break;            
            }
        }

        //if(result == PlayersStats.Level) Debug.Log("FAIL: Converted " + boost + " to " + result);

        return result;
    }

    public BoostType RuneToBoostType(RunesType rune)
    {
        BoostType result = BoostType.Nothing;

        int runeIndex = (int)rune;

        foreach(BoostType boost in Enum.GetValues(typeof(BoostType)))
        {
            if((int)boost == runeIndex) 
            {
                result = boost;
                break;            
            }
        }

        if(result == BoostType.Nothing) Debug.Log("FAIL: Converted " + rune + " to " + result);       

        return result;
    }

    public RunesType BoostTypeToRune(BoostType boost)
    {
        RunesType result = RunesType.WeaponSpeed; //parameter which useless (like BoostType.Nothing)

        int boostIndex = (int)boost;

        foreach(RunesType rune in Enum.GetValues(typeof(RunesType)))
        {
            if((int)rune == boostIndex) 
            {
                result = rune;
                break;            
            }
        }

        if(result == RunesType.WeaponSpeed) Debug.Log("FAIL: Converted " + boost + " to " + result);

        return result;
    }
}
