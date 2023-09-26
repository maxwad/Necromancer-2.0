using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
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

    [SerializeField] private float spellBase = 1;

    [Header("Global Parameters")]
    [Space]
    [SerializeField] private float movementDistanceBase = 10f;

    [SerializeField] private float luckBase = 3f;

    [SerializeField] private float learningBase = 5f;

    [SerializeField] private float squadMaxSizeBase = 30;

    [SerializeField] private float extraResources = 1;

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
        public float maxValue;

        public Stat(PlayersStats stat, float baseV)
        {
            playerStat     = stat;
            baseValue      = baseV;
            maxValue       = baseV;
        }

        public void UpgradeMaxValue(float value, StatBoostType type = StatBoostType.Value)
        {
            switch(type)
            {
                case StatBoostType.Bool:
                    maxValue = 1;
                    break;

                case StatBoostType.Step:
                    maxValue++;
                    break;

                case StatBoostType.Percent:
                    maxValue = baseValue + (baseValue * value / 100);
                    break;

                case StatBoostType.Value:
                    maxValue += value;
                    break;

                default:
                    Debug.Log("Problem with boost " + playerStat);
                    break;
            }
        }
    }

    private BoostManager boostManager;
    [SerializeField] private Dictionary<PlayersStats, Stat> allStatsDict = new Dictionary<PlayersStats, Stat>();

    [Inject]
    public void Construct(BoostManager boostManager)
    {
        this.boostManager = boostManager;
    }

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

                case PlayersStats.NegativeCell:
                    baseValue = 1;
                    break;

                case PlayersStats.SquadMaxSize:
                    baseValue = squadMaxSizeBase;
                    break;

                case PlayersStats.ExtraResourcesProduce:
                    baseValue = extraResources;
                    break;

                case PlayersStats.Spell:
                    baseValue = spellBase;
                    break;

                case PlayersStats.Portal:
                    baseValue = 4;
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

        EventManager.OnSetNewPlayerStatEvent(stat, GetCurrentParameter(stat));
        //Debug.Log("Now " + stat + " = " + GetCurrentParameter(stat));
    }
    
    public void ForceUpdateStat(PlayersStats stat)
    {
        EventManager.OnSetNewPlayerStatEvent(stat, GetCurrentParameter(stat));
        //Debug.Log("ForceUpdate: Now " + stat + " = " + GetCurrentParameter(stat));
    }

    public float GetCurrentParameter(PlayersStats stat)
    {
        float result = allStatsDict[stat].maxValue;
        float boost = boostManager.GetBoost(TypesConverter.PlayerStatToBoostType(stat));

        return result + (result * boost);
    }
}
