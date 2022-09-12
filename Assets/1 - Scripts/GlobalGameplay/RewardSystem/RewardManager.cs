using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;


public class RewardManager : MonoBehaviour
{
    private float luck = 0f;

    private float expCalendarBonusValue = 0f;
    private float extraBoxReward = 0f;
    private float extraDoubleBonusFromBox = 0;

    private float extraExpAfterBattle = 0f;
    private float extraBonusAfterBattle = 0;

    private bool isExtraResourcesBonus = false;
    private bool isExtraManaBonus = false;
    //private bool isExtraMagicBonus = false;

    private float luckBonusValue = 100f;
    private float extraBonus = 25;

    private float difficultBattleMultiplier = 1;
    public float difficultConstant = 20f;
    private float countOfEnemySquad = 1;

    public float expBoxPortion = 10;
    public float expAfterBattlePortion = 100;
    public float manaPortion = 10;
    //public float magicPortion = 5;
    public float randomGap = 25f;
    public int portionOfBoxResources = 100;
    public int portionOfAfterBattleResources = 10;
    public int countOfResources = 2;

    private PlayerStats playerStats;

    private void Start()
    {
        playerStats = GlobalStorage.instance.playerStats;
        GetPlayersParameters();

        GetBoxReward();
    }

    #region GET/SET Parameters
    private void GetPlayersParameters()
    {
        luck = playerStats.GetCurrentParameter(PlayersStats.Luck);

        extraBoxReward = playerStats.GetCurrentParameter(PlayersStats.ExtraBoxReward);
        extraDoubleBonusFromBox = playerStats.GetCurrentParameter(PlayersStats.DoubleBonusFromBox);

        extraBonusAfterBattle = playerStats.GetCurrentParameter(PlayersStats.ExtraAfterBattleReward);
        extraExpAfterBattle = playerStats.GetCurrentParameter(PlayersStats.ExtraExpReward);

        SetExtrasResources(extraBonusAfterBattle);
    }

    private void SetExtrasResources(float mode)
    {
        if(mode == 0) return;

        if(mode > 0) isExtraResourcesBonus = true;

        if(mode > 1) isExtraManaBonus = true;
    }

    public void SetCalendarExpBonus(float value)
    {
        expCalendarBonusValue = value;
    }

    private float GetValueWithGap(float value, float gap)
    {
        float minValue = value - value * (gap / 100);
        float maxValue = value + value * (gap / 100);

        return UnityEngine.Random.Range(minValue, maxValue);
    }

    private float GetBoostValue(float value, float boost)
    {
        return Mathf.Round(value + value * (boost / 100));
    }

    private List<ResourceType> CreateBonusResources(int count)
    {
        List<ResourceType> bonusResourcesList = new List<ResourceType>();

        int index;
        for(int i = 0; i < count; i++)
        {
            index = UnityEngine.Random.Range(0, Enum.GetValues(typeof(ResourceType)).Length);
            if(index == (int)ResourceType.Magic) index = 0;
            ResourceType resource = (ResourceType)Enum.GetValues(typeof(ResourceType)).GetValue(index);            
            bonusResourcesList.Add(resource);
        }

        return bonusResourcesList;
    }

    private List<float> BoostResourceQuantity(int count, float boost)
    {
        List<float> bonusResourcesQuantityList = new List<float>();

        for(int i = 0; i < count; i++)
        {
            float quantity = GetValueWithGap(portionOfBoxResources, randomGap);
            quantity = Mathf.Round(quantity + quantity * boost);
            bonusResourcesQuantityList.Add(quantity);
        }

        return bonusResourcesQuantityList;
    }

    #endregion

    public Reward GetBoxReward()
    {
        GetPlayersParameters();

        float exp = GetValueWithGap(expBoxPortion, randomGap);
        float expBoost = 0;

        expBoost += expCalendarBonusValue;
        expBoost += extraBoxReward;

        float resourceBoost = 0;
        resourceBoost += extraBoxReward;
        List<ResourceType> bonusResources = CreateBonusResources(countOfResources);

        float randomValue = UnityEngine.Random.Range(0, 101);
        if(randomValue <= luck)
        {
            expBoost += (luckBonusValue / 100);
            resourceBoost += (luckBonusValue / 100);
        }

        exp = GetBoostValue(exp, expBoost);
        List<float> bonusResourcesQuantityList = BoostResourceQuantity(bonusResources.Count, resourceBoost);

        Reward reward = new Reward(exp, bonusResources, bonusResourcesQuantityList);


        Debug.Log(reward.exp);
        Debug.Log(reward.resourcesQuantity[0]);

        return reward;
    }
}
