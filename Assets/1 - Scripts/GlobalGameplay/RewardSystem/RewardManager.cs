using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;


public class RewardManager : MonoBehaviour
{
    private float expCalendarBonusValue = 0f;
    private float extraBoxReward = 0f;

    private float extraExpAfterBattle = 0f;
    private float extraBonusAfterBattle = 0;

    private bool isExtraResourcesBonus = false;
    private bool isExtraManaBonus = false;
    //private bool isExtraMagicBonus = false;

    private float difficultBattleMultiplier = 0;
    public float difficultConstant = 20f;
    private float countOfEnemySquad = 1;

    public float expBoxPortion = 50;
    public float expAfterBattlePortion = 50;
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

        //GetBattleReward(ArmyStrength.Middle, UnityEngine.Random.Range(1, 13));
    }


    #region GET/SET Parameters
    private void GetPlayersParameters()
    {
        playerStats = GlobalStorage.instance.playerStats;//DELETE LATER

        extraBoxReward = playerStats.GetCurrentParameter(PlayersStats.ExtraBoxReward);

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

        return Mathf.Round(UnityEngine.Random.Range(minValue, maxValue));
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


    private List<float> BoostResourceQuantity(List<ResourceType> list, int count, float portion, float boost)
    {
        List<float> bonusResourcesQuantityList = new List<float>();

        for(int i = 0; i < count; i++)
        {
            float quantity = GetValueWithGap(portion, randomGap);
            quantity = Mathf.Round(quantity + quantity * (boost / 100));
            bonusResourcesQuantityList.Add(quantity);
            //Debug.Log(list[i] + " quantity = " + quantity);
        }

        return bonusResourcesQuantityList;
    }


    #endregion

    public Reward GetHeapReward(ResourceType type)
    {
        GetPlayersParameters();

        List<ResourceType> bonusResources = new List<ResourceType>();
        bonusResources.Add(type);

        float resourceBoost = 0;
        resourceBoost += extraBoxReward;
        
        List<float> bonusResourcesQuantityList = BoostResourceQuantity(bonusResources, bonusResources.Count, portionOfBoxResources, resourceBoost);

        Reward reward = new Reward(0, bonusResources, bonusResourcesQuantityList);

        return reward;
    }


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

        Debug.Log("exp " + exp + ". expBosst " + expBoost);
        exp = GetBoostValue(exp, expBoost);
        Debug.Log("resourceBoost " + resourceBoost);
        List<float> bonusResourcesQuantityList = BoostResourceQuantity(bonusResources, bonusResources.Count, portionOfBoxResources, resourceBoost);

        Reward reward = new Reward(exp, bonusResources, bonusResourcesQuantityList);


        Debug.Log(reward.exp);
        Debug.Log(reward.resourcesQuantity[0]);

        return reward;
    }

    public Reward GetBattleReward(ArmyStrength strength, int countOfSquad)
    {
        GetPlayersParameters();

        difficultBattleMultiplier = (int)strength;
        float strenghtBoost = countOfSquad * difficultBattleMultiplier * difficultConstant;

        //Debug.Log("countOfSquad = " + countOfSquad + ". strenghtBoost = " + strenghtBoost);

        float exp = GetValueWithGap(expAfterBattlePortion, randomGap);
        float expBoost = 0;

        expBoost += expCalendarBonusValue;
        expBoost += extraExpAfterBattle;
        expBoost += strenghtBoost;

        exp = GetBoostValue(exp, expBoost);

        List<ResourceType> bonusResources = new List<ResourceType>();
        bonusResources.Add(ResourceType.Gold);

        if(isExtraResourcesBonus == true)
        {
            bonusResources.AddRange(CreateBonusResources(countOfResources));
        }

        float portion = portionOfAfterBattleResources;
        portion *= countOfEnemySquad;
        portion = GetBoostValue(portion, strenghtBoost);

        List<float> bonusResourcesQuantityList = BoostResourceQuantity(bonusResources, bonusResources.Count, portion, 0);

        float mana = 0f;
        if(isExtraManaBonus == true) mana = GetValueWithGap(manaPortion, randomGap);

        Reward reward = new Reward(exp, bonusResources, bonusResourcesQuantityList, mana);

        return reward;
    }
}
