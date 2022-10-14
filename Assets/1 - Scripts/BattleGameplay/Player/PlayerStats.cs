using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class PlayerStats : MonoBehaviour
{
    #region Battle Parameters

    [Header("Battle Parameters")]

    [SerializeField] private float levelMax = 30;

    [SerializeField] private float healthBase = 100f;

    [SerializeField] private float manaBase = 20f;

    [SerializeField] private float searchRadiusBase = 2f;

    [SerializeField] private float speedBase = 200f;

    [SerializeField] private float defenceBase = 1;

    [SerializeField] private float infirmaryBase = 5f;

    [SerializeField] private float infirmaryTimeBase = 5f;

    [Header("Global Parameters")]
    [Space]
    [SerializeField] private float movementDistanceBase = 10f;

    [SerializeField] private float luckBase = 3f;

    [SerializeField] private float learningBase = 5f;

    [Header("Number Parameters")]
    [Space]
    //3, 4
    [SerializeField] private float medicTryBase = 3;

    // DESCRIPTIONS:
    //curiosity: 0 - can't see; 1 - general information; 2 - exemplary information; 3 - accurate information
    //portalSkill: 0 - can't use; 1 - to random portal; 2 - portal directly; 3 - portal to town
    //extraAfterBattleReward: 0 - can't use; 1 - ordinary resources; 2 - mana; 3 - magic

    #endregion

    private struct Stat
    {
        public PlayersStats playerStat;

        public float baseValue;
        public float currentValue;
        public float tempBoostValue;
        public float maxValue;

        public Stat(PlayersStats stat, float baseV)
        {
            playerStat     = stat;

            baseValue      = baseV;
            currentValue   = baseV;
            tempBoostValue = 0;
            maxValue       = baseV;
        }

        public void UpgradeMaxValue(float value, StatBoostType type = StatBoostType.Value)
        {
            switch(type)
            {
                case StatBoostType.Bool:
                    currentValue = 1;
                    break;

                case StatBoostType.Step:
                    currentValue++;
                    break;

                case StatBoostType.Percent:
                    currentValue = baseValue + (baseValue * value / 100);
                    break;

                case StatBoostType.Value:
                    currentValue += value;
                    break;

                default:
                    Debug.Log("Problem with boost " + playerStat);
                    break;
            }

            maxValue = currentValue + tempBoostValue;
        }

        public void SetTempBoost(float value)
        {
            tempBoostValue = value;
            maxValue = currentValue + currentValue * tempBoostValue;
        }
    }

    [SerializeField] private Dictionary<PlayersStats, Stat> allStatsDict = new Dictionary<PlayersStats, Stat>();


    private void Awake()
    {
        InitStartParameters();
    }

    private void InitStartParameters()
    {
        foreach(PlayersStats itemStat in Enum.GetValues(typeof(PlayersStats)))
        {
            float baseValue = 0;

            switch(itemStat)
            {
                case PlayersStats.Curiosity:
                    baseValue = 3;
                    break;

                case PlayersStats.ExtraMovementPoints:
                    baseValue = 5;
                    break;

                case PlayersStats.Level:
                    baseValue = levelMax;
                    break;

                case PlayersStats.Health:
                    baseValue = healthBase;
                    break;

                case PlayersStats.Mana:
                    baseValue = manaBase;
                    break;

                case PlayersStats.Speed:
                    baseValue = speedBase;
                    break;

                case PlayersStats.SearchRadius:
                    baseValue = searchRadiusBase;
                    break;

                case PlayersStats.Defence:
                    baseValue = defenceBase;
                    break;

                case PlayersStats.Infirmary:
                    baseValue = infirmaryBase;
                    break; 
                
                case PlayersStats.InfirmaryTime:
                    baseValue = infirmaryTimeBase;
                    break;

                case PlayersStats.MovementDistance:
                    baseValue = movementDistanceBase;
                    break;

                case PlayersStats.Luck:
                    baseValue = luckBase;
                    break;

                case PlayersStats.MedicTry:
                    baseValue = medicTryBase;
                    break; 
                
                case PlayersStats.Learning:
                    baseValue = learningBase;
                    break;

                default:
                    break;
            }

            allStatsDict.Add(itemStat, new Stat(itemStat, baseValue));
        }
    }

    public void UpdateMaxStat(PlayersStats stat, StatBoostType upgradeType, float value)
    {
        Stat upgradedStat = allStatsDict[stat];
        upgradedStat.UpgradeMaxValue(value, upgradeType);
        allStatsDict[stat] = upgradedStat;

        EventManager.OnSetNewPlayerStatEvent(stat, upgradedStat.maxValue);


        Debug.Log("Now " + stat + " = " + allStatsDict[stat].maxValue);
    }
    
    private void AddBoostToStat(PlayersStats stat, float value)
    {
        Stat upgradedStat = allStatsDict[stat];
        upgradedStat.SetTempBoost(value);
        allStatsDict[stat] = upgradedStat;

        EventManager.OnSetNewPlayerStatEvent(stat, upgradedStat.maxValue);
    }

    //private void UpgradeStatCurrentValue(PlayersStats stat, float maxValue, float currentValue)
    //{
    //    Stat currentStat = allStatsDict[stat];
    //    currentStat.SetCurrentValue(currentValue);
    //    allStatsDict[stat] = currentStat;
    //}

    //public void GetAllStartParameters()
    //{
    //    foreach(PlayersStats itemStat in Enum.GetValues(typeof(PlayersStats)))
    //        EventManager.OnSetNewPlayerStatEvent(itemStat, allStatsDict[itemStat].maxValue);
    //}

    public float GetCurrentParameter(PlayersStats stat)
    {
        return allStatsDict[stat].currentValue;
    }

    private void OnEnable()
    {
        EventManager.SetBoostToStat += AddBoostToStat;
        //EventManager.UpgradeStatCurrentValue += UpgradeStatCurrentValue;
    }

    private void OnDisable()
    {
        EventManager.SetBoostToStat -= AddBoostToStat;
        //EventManager.UpgradeStatCurrentValue -= UpgradeStatCurrentValue;
    }
}
