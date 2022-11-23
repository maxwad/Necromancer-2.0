using System;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class EnumConverter : MonoBehaviour
{
    public static EnumConverter instance;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public BoostType PlayerStatToBoostType(PlayersStats stat)
    {
        BoostType result = ((int)stat > 1000) ? BoostType.Nothing : (BoostType)stat;
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

    public List<BoostType> SpellToBoost(Spells spell)
    {
        List<BoostType> boostList = new List<BoostType>();

        switch(spell)
        {
            case Spells.SpeedUp:
                boostList.Add(BoostType.MovementSpeed);
                break;

            case Spells.AttackUp:
                boostList.Add(BoostType.MagicAttack);
                boostList.Add(BoostType.PhysicAttack);
                break;

            case Spells.DoubleCrit:
                boostList.Add(BoostType.CriticalDamage);
                break;

            case Spells.DoubleBonuses:
                boostList.Add(BoostType.BonusAmount);
                break;

            case Spells.WeaponSize:
                boostList.Add(BoostType.WeaponSize);
                break;

            case Spells.Immortal:
                boostList.Add(BoostType.MagicDefence);
                boostList.Add(BoostType.PhysicDefence);
                break;

            case Spells.EnemiesStop:
                boostList.Add(BoostType.EnemyMovementSpeed);
                break;

            default:
                break;
        }

        return boostList;
    }

    public PreSpells SpellToPreEpell(Spells spell)
    {
        PreSpells result = (PreSpells)spell;
        return result;
    }
}
