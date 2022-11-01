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
        {
            if((int)boost == runeIndex) result = boost;
        }


        Debug.Log("Converted " + rune + " to " + result);

        //switch(rune)
        //{
        //    case RunesType.PhysicAttack:
        //        result = BoostType.PhysicAttack;
        //        break;

        //    case RunesType.MagicAttack:
        //        result = BoostType.MagicAttack;
        //        break;

        //    case RunesType.PhysicDefence:
        //        result = BoostType.PhysicDefence;
        //        break;

        //    case RunesType.MagicDefence:
        //        result = BoostType.MagicDefence;
        //        break;

        //    case RunesType.CriticalDamage:
        //        result = BoostType.CriticalDamage;
        //        break;

        //    case RunesType.BossDamade:
        //        result = BoostType.BossDamade;
        //        break;

        //    case RunesType.MovementSpeed:
        //        result = BoostType.MovementSpeed;
        //        break;

        //    case RunesType.BonusAmount:
        //        result = BoostType.BonusAmount;
        //        break;

        //    case RunesType.BonusRadius:
        //        result = BoostType.BonusRadius;
        //        break;

        //    case RunesType.BonusOpportunity:
        //        result = BoostType.BonusOpportunity;
        //        break;

        //    case RunesType.WeaponSpeed:
        //        result = BoostType.WeaponSpeed;
        //        break;

        //    case RunesType.WeaponSize:
        //        result = BoostType.WeaponSize;
        //        break;

        //    case RunesType.CoolDown:
        //        result = BoostType.PhysicDefence;
        //        break;

        //    case RunesType.Exp:
        //        result = BoostType.Exp;
        //        break;

        //    default:
        //        break;
        //}

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
