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
        public float boostValue;
        public float maxValue;
        public bool boostType;

        public Stat(PlayersStats stat, float baseV = 0, float boost = 0, float max = 0, bool type = false)
        {
            playerStat   = stat;

            baseValue    = baseV;
            currentValue = baseV;
            boostValue   = boost;
            maxValue     = max;
            boostType    = type;
        }

        public void UpgradeValue()
        {
            if(boostType == false)
                maxValue = baseValue + (baseValue * boostValue);
            else
                maxValue = baseValue + boostValue;
        }

        public void SetNewBoost(float value)
        {
            boostValue = value;
            UpgradeValue();
        }

        public void SetCurrentValue(float value)
        {
            currentValue = value;
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
            float maxValue = 0;
            // type = true - "+"; type = false - "*"
            bool type = true;

            switch(itemStat)
            {
                case PlayersStats.GlobalExp:
                    baseValue = globalExp;
                    maxValue = globalExp;
                    //Create new ExpScript for handling xp
                    break;

                case PlayersStats.Level:
                    baseValue = levelBase;
                    maxValue = levelBase;
                    break;

                case PlayersStats.Health:
                    baseValue = healthBase;
                    maxValue = healthBase;
                    type = false;
                    break;

                case PlayersStats.Mana:
                    baseValue = manaBase;
                    maxValue = manaBase;
                    type = false;
                    break;

                case PlayersStats.Speed:
                    baseValue = speedBase;
                    maxValue = speedBase;
                    type = false;
                    break;

                case PlayersStats.SearchRadius:
                    baseValue = searchRadiusBase;
                    maxValue = searchRadiusBase;
                    type = false;
                    break;

                case PlayersStats.Defence:
                    baseValue = defenceBase;
                    maxValue = defenceBase;
                    type = false;
                    break;

                case PlayersStats.HealthRegeneration:
                    type = false;
                    break;

                case PlayersStats.Infirmary:
                    baseValue = infarmaryBase;
                    maxValue = infarmaryBase;
                    break;

                case PlayersStats.MovementDistance:
                    baseValue = movementDistanceBase;
                    maxValue = movementDistanceBase;
                    break;

                case PlayersStats.RadiusView:
                    baseValue = radiusViewBase;
                    maxValue = radiusViewBase;
                    type = false;
                    break;

                case PlayersStats.ExtraResourcesProduce:
                    type = false;
                    break;

                case PlayersStats.Luck:
                    baseValue = luckBase;
                    maxValue = luckBase;
                    break;

                case PlayersStats.ExtraBoxReward:
                    baseValue = extraBoxRewardBase;
                    maxValue = extraBoxRewardBase;
                    type = false;
                    break;

                case PlayersStats.ExtraExpReward:
                    baseValue = extraExpRewardBase;
                    maxValue = extraExpRewardBase;
                    type = false;
                    break;

                case PlayersStats.Spell:
                    break;

                case PlayersStats.NegativeCell:
                    break;

                case PlayersStats.MedicAltar:
                    break;

                case PlayersStats.DoubleBonusFromBox:
                    baseValue = doudleBonusFromBoxBase;
                    maxValue = doudleBonusFromBoxBase;
                    break;

                case PlayersStats.MedicTry:
                    baseValue = medicTryBase;
                    maxValue = medicTryBase;
                    break;

                case PlayersStats.Curiosity:
                    baseValue = curiosityBase;
                    maxValue = curiosityBase;
                    break;

                case PlayersStats.Portal:
                    baseValue = portalSkillBase;
                    maxValue = portalSkillBase;
                    break;

                case PlayersStats.ExtraAfterBattleReward:
                    baseValue = extraAfterBattleRewardBase;
                    maxValue = extraAfterBattleRewardBase;
                    break;

                case PlayersStats.ManaRegeneration:
                    break;

                case PlayersStats.HeroArmyToEnemy:
                    type = false;
                    break;

                default:
                    Debug.Log("Attention! Some stats were skip!");
                    break;
            }

            allStatsDict.Add(itemStat, new Stat(itemStat, baseValue, 0, maxValue, type));
        }
    }


    private void UpdateBoost(PlayersStats stat, float value)
    {
        Stat currentStat = allStatsDict[stat];
        currentStat.SetNewBoost(value);
        allStatsDict[stat] = currentStat;

        EventManager.OnSetNewPlayerStatEvent(stat, currentStat.maxValue);
    }

    private void UpgradeStatCurrentValue(PlayersStats stat, float maxValue, float currentValue)
    {
        Stat currentStat = allStatsDict[stat];
        currentStat.SetCurrentValue(currentValue);
        allStatsDict[stat] = currentStat;
    }

    public void GetAllStartParameters()
    {
        foreach(PlayersStats itemStat in Enum.GetValues(typeof(PlayersStats)))
            EventManager.OnSetNewPlayerStatEvent(itemStat, allStatsDict[itemStat].maxValue);
    }

    public float GetMaxParameter(PlayersStats stat)
    {
        return allStatsDict[stat].maxValue;
    }

    public float GetCurrentParameter(PlayersStats stat)
    {
        return allStatsDict[stat].currentValue;
    }

    private void OnEnable()
    {
        EventManager.SetBoostToStat += UpdateBoost;
        EventManager.UpgradeStatCurrentValue += UpgradeStatCurrentValue;
    }

    private void OnDisable()
    {
        EventManager.SetBoostToStat -= UpdateBoost;
        EventManager.UpgradeStatCurrentValue -= UpgradeStatCurrentValue;
    }
}
