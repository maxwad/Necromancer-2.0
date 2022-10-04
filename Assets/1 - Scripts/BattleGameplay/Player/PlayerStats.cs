using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class PlayerStats : MonoBehaviour
{
    #region Battle Parameters

    [Header("Battle Parameters")]
    [SerializeField] private float globalExp = 0;

    [SerializeField] private float levelBase = 5;
    [SerializeField] private float levelMax = 30;

    [SerializeField] private float healthBase = 500f;

    [SerializeField] private float manaBase = 50f;

    [SerializeField] private float searchRadiusBase = 2f;

    [SerializeField] private float speedBase = 200f;

    [SerializeField] private float defenceBase = 1;

    [SerializeField] private float healthRegenerationBase = 0;

    [SerializeField] private float infarmaryBase = 5f;

    [SerializeField] private float luckBase = 0.03f;


    [Header("Global Parameters")]
    [Space]
    [SerializeField] private float movementDistanceBase = 10f;

    [SerializeField] private float extraMovementPointsBase = 0;

    [SerializeField] private float radiusViewBase = 10;

    //[SerializeField] private float extraResourcesProduceBase = 0;

    [SerializeField] private float extraExpRewardBase = 0;

    [SerializeField] private float extraBoxRewardBase = 0;
    


    //[Header("True-False Parameters")]
    //[Space]
    ////0 - no, 1 - yes
    //[SerializeField] private float canIUseSpellsBase = 0; 

    ////0 - no, 1 - yes
    //[SerializeField] private float canIUseNegativeCellBase = 0;

    ////0 - no, 1 - yes
    //[SerializeField] private float canIUseMedicAltarBase = 0;

    //0 - no, 1 - yes
    [SerializeField] private float doudleBonusFromBoxBase = 0;


    [Header("Number Parameters")]
    [Space]
    //3, 4
    [SerializeField] private float medicTryBase = 3;

    //0 - can't see; 1 - general information; 2 - exemplary information; 3 - accurate information
    [SerializeField] private float curiosityBase = 0;

    ////0 - can't use; 1 - to random portal; 2 - portal directly; 3 - portal to town
    [SerializeField] private float portalSkillBase = 3;

    //0 - can't use; 1 - ordinary resources; 2 - mana; 3 - magic
    [SerializeField] private float extraAfterBattleRewardBase = 0;

    ////0 - can't use; 1 - 1 mana; 2 - 2 mana; 3 - 3 mana
    //[SerializeField] private float manaRegenerationBase = 0;

    ////0 - 100%, 1 - -20%; 2 - 40%; 3 - -100%
    //[SerializeField] private float heroArmyTransformationToEnemyBase = 0;

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

    private Dictionary<PlayersStats, Stat> allStatsDict = new Dictionary<PlayersStats, Stat>();


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
                case PlayersStats.GlobalExp:
                    baseValue = globalExp;                 
                    //Create new ExpScript for handling xp
                    break;

                case PlayersStats.Level:
                    baseValue = levelBase;
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

                case PlayersStats.HealthRegeneration:
                    break;

                case PlayersStats.Infirmary:
                    baseValue = infarmaryBase;
                    break;

                case PlayersStats.MovementDistance:
                    baseValue = movementDistanceBase;
                    break;


                case PlayersStats.ExtraMovementPoints:
                    baseValue = extraMovementPointsBase;
                    break;

                case PlayersStats.RadiusView:
                    baseValue = radiusViewBase;
                    break;

                case PlayersStats.ExtraResourcesProduce:
                    break;

                case PlayersStats.Luck:
                    baseValue = luckBase;
                    break;

                case PlayersStats.ExtraBoxReward:
                    baseValue = extraBoxRewardBase;
                    break;

                case PlayersStats.ExtraExpReward:
                    baseValue = extraExpRewardBase;
                    break;

                case PlayersStats.Spell:
                    break;

                case PlayersStats.NegativeCell:
                    break;

                case PlayersStats.MedicAltar:
                    break;

                case PlayersStats.DoubleBonusFromBox:
                    baseValue = doudleBonusFromBoxBase;
                    break;

                case PlayersStats.MedicTry:
                    baseValue = medicTryBase;
                    break;

                case PlayersStats.Curiosity:
                    baseValue = curiosityBase;
                    break;

                case PlayersStats.Portal:
                    baseValue = portalSkillBase;
                    break;

                case PlayersStats.ExtraAfterBattleReward:
                    baseValue = extraAfterBattleRewardBase;
                    break;

                case PlayersStats.ManaRegeneration:
                    break;

                case PlayersStats.HeroArmyToEnemy:
                    break;

                default:
                    Debug.Log("Attention! Some stats were skip!");
                    break;
            }

            allStatsDict.Add(itemStat, new Stat(itemStat, baseValue));
        }
    }


    private void UpdateMaxStat(PlayersStats stat, float value)
    {
        Stat upgradedStat = allStatsDict[stat];
        upgradedStat.UpgradeMaxValue(value);
        allStatsDict[stat] = upgradedStat;

        EventManager.OnSetNewPlayerStatEvent(stat, upgradedStat.maxValue);
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

    public void GetAllStartParameters()
    {
        foreach(PlayersStats itemStat in Enum.GetValues(typeof(PlayersStats)))
            EventManager.OnSetNewPlayerStatEvent(itemStat, allStatsDict[itemStat].maxValue);
    }

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
