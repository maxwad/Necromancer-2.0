using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static NameManager;

public class TypesConverter : MonoBehaviour
{
    public static TypesConverter instance;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public static BoostType PlayerStatToBoostType(PlayersStats stat)
    {
        BoostType result = ((int)stat > 1000) ? BoostType.Nothing : (BoostType)stat;
        return result;
    }

    public static PlayersStats BoostTypeToPlayerStat(BoostType boost)
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

    public static BoostType RuneToBoostType(RunesType rune)
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

    public static RunesType BoostTypeToRune(BoostType boost)
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

    public static List<BoostType> SpellToBoost(Spells spell)
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

    public static PreSpells SpellToPreEpell(Spells spell)
    {
        PreSpells result = (PreSpells)spell;
        return result;
    }




    #region SAVE|LOAD

    public static (List<T>, List<V>) SplitDictionary<T, V>(Dictionary<T, V> dict)
    {
        List<T> keysList = new List<T>(dict.Keys);
        List<V> valuesList = new List<V>(dict.Values);

        return (keysList, valuesList);
    }

    public static Dictionary<T, V> CreateDictionary<T, V>(List<T> keys, List<V> values)
    {
        Dictionary<T, V> dict = new Dictionary<T, V>();

        for(int i = 0; i < keys.Count; i++)
            dict.Add(keys[i], values[i]);

        return dict;
    }


    //public static Vector3 GetVector3(Vec3 falseVector)
    //{
    //    return new Vector3(falseVector.x, falseVector.y, falseVector.z);
    //}

    //public static Vec3 GetVec3(Vector3 realVector)
    //{
    //    return new Vec3(realVector);
    //}

    //public static List<Vector3> GetVector3List(List<Vec3> falseVectorList)
    //{
    //    List<Vector3> newList = new List<Vector3>();

    //    foreach(var oldVector in falseVectorList)
    //        newList.Add(GetVector3(oldVector));

    //    return newList;
    //}

    //public static List<Vec3> GetVec3List(List<Vector3> vectorList)
    //{
    //    List<Vec3> newList = new List<Vec3>();

    //    foreach(var falseVector in vectorList)
    //        newList.Add(GetVec3(falseVector));

    //    return newList;
    //}


    public static T ConvertToRequiredType<T>(object data)
    {
        JObject tempObject = (JObject)data;
        T convertedData = tempObject.ToObject<T>();
        return convertedData;
    }


    #endregion
}
