using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static NameManager;


public class RewardManager : MonoBehaviour
{
    private float extraBoxReward = 0f;

    private float extraExpAfterBattle = 0f;
    private float extraBonusAfterBattle = 0;

    private bool isExtraResourcesBonus = false;
    private bool isExtraManaBonus = false;

    private float difficultBattleMultiplier = 0;
    public float difficultConstant = 20f;
    private float countOfEnemySquad = 1;

    public float expBoxPortion = 50;
    public float expAfterBattlePortion = 50;
    public float manaPortion = 10;

    public float randomGap = 25f;
    public int portionOfBoxResources = 100;
    public int portionOfAfterBattleResources = 10;
    public int portionOfTombResources = 500;
    public float portionMultiplier = 0.5f;
    public int usualCountOfResources = 3;
    public int tombCountOfResources = 3;

    private PlayerStats playerStats;
    private MacroLevelUpManager levelUpManager;

    private float luck;
    private float playerLevel;

    [Inject]
    public void Construct(PlayerStats playerStats, MacroLevelUpManager levelUpManager)
    {
        this.playerStats = playerStats;
        this.levelUpManager = levelUpManager;
    }

    #region GET/SET Parameters
    private void GetPlayersParameters()
    {
        //if(playerStats == null)
        //{
        //    levelUpManager = GlobalStorage.instance.macroLevelUpManager;
        //    playerStats = GlobalStorage.instance.playerStats;
        //}

        extraBoxReward = playerStats.GetCurrentParameter(PlayersStats.ExtraBoxReward);
        extraBonusAfterBattle = playerStats.GetCurrentParameter(PlayersStats.ExtraAfterBattleReward);
        extraExpAfterBattle = playerStats.GetCurrentParameter(PlayersStats.ExtraExpReward);

        luck = playerStats.GetCurrentParameter(PlayersStats.Luck);
        playerLevel = levelUpManager.GetCurrentLevel();

        SetExtrasResources(extraBonusAfterBattle);
    }


    private void SetExtrasResources(float mode)
    {
        if(mode == 0) return;

        if(mode > 0) isExtraResourcesBonus = true;

        if(mode > 1) isExtraManaBonus = (UnityEngine.Random.Range(0, 101) <= luck) ? true : false;        
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
            if(index > 4) index = 0;
            ResourceType resource = (ResourceType)Enum.GetValues(typeof(ResourceType)).GetValue(index);            
            bonusResourcesList.Add(resource);
        }

        return bonusResourcesList;
    }


    private List<float> BoostResourceQuantity(int count, float portion, float boost)
    {
        List<float> bonusResourcesQuantityList = new List<float>();

        for(int i = 0; i < count; i++)
        {
            float quantity = GetValueWithGap(portion, randomGap);
            quantity = Mathf.Round(quantity + quantity * (boost / 100));
            bonusResourcesQuantityList.Add(quantity);
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
        float portion = portionOfBoxResources * playerLevel * portionMultiplier;

        List<float> bonusResourcesQuantityList = BoostResourceQuantity(bonusResources.Count, portion, resourceBoost);

        Reward reward = new Reward(bonusResources, bonusResourcesQuantityList);

        return reward;
    }


    public Reward GetBoxReward()
    {
        GetPlayersParameters();

        float exp = GetValueWithGap(expBoxPortion, randomGap);
        float expBoost = 0;
        expBoost += extraBoxReward;
     
        float resourceBoost = 0;
        resourceBoost += extraBoxReward;

        exp = GetBoostValue(exp, expBoost);

        List<ResourceType> bonusResources = CreateBonusResources(usualCountOfResources);
        bonusResources.Add(ResourceType.Exp);

        float portion = portionOfBoxResources * playerLevel * portionMultiplier;

        List<float> bonusResourcesQuantityList = BoostResourceQuantity(usualCountOfResources, portion, resourceBoost);
        bonusResourcesQuantityList.Add(exp);

        bonusResources.Reverse();
        bonusResourcesQuantityList.Reverse();

        Reward reward = new Reward(bonusResources, bonusResourcesQuantityList);

        return reward;
    }

    public Reward GetBattleReward(Army army)
    {
        GetPlayersParameters();

        difficultBattleMultiplier = (int)army.strength;
        float strenghtBoost = army.squadList.Count * difficultBattleMultiplier * difficultConstant;

        float exp = GetValueWithGap(expAfterBattlePortion, randomGap);
        float expBoost = 0;

        expBoost += extraExpAfterBattle;
        expBoost += strenghtBoost;

        exp = GetBoostValue(exp, expBoost);

        List<ResourceType> bonusResources = new List<ResourceType>();
        bonusResources.Add(ResourceType.Gold);

        if(isExtraResourcesBonus == true)
        {
            bonusResources.AddRange(CreateBonusResources(usualCountOfResources));
        }

        float portion = portionOfAfterBattleResources * playerLevel * portionMultiplier;
        portion *= countOfEnemySquad;
        portion = GetBoostValue(portion, strenghtBoost);

        List<float> bonusResourcesQuantityList = BoostResourceQuantity(bonusResources.Count, portion, 0);

        if(isExtraManaBonus == true) 
        {
            float mana = GetValueWithGap(manaPortion, randomGap);
            bonusResources.Add(ResourceType.Mana);
            bonusResourcesQuantityList.Add(mana);
        }

        bonusResources.Add(ResourceType.Exp);
        bonusResourcesQuantityList.Add(exp);

        bonusResources.Reverse();
        bonusResourcesQuantityList.Reverse();

        Reward reward = new Reward(bonusResources, bonusResourcesQuantityList);

        return reward;
    }

    public Reward GetTombReward()
    {
        GetPlayersParameters();

        List<ResourceType> bonusResources = CreateBonusResources(tombCountOfResources);
        List<float> bonusResourcesQuantityList = BoostResourceQuantity(bonusResources.Count, playerLevel * portionOfTombResources, 0);

        float expBoost = extraExpAfterBattle;
        float exp = playerLevel * portionOfTombResources * portionMultiplier;
        exp = GetValueWithGap(exp, randomGap);
        exp = GetBoostValue(exp, expBoost);

        bonusResources.Add(ResourceType.Exp);
        bonusResourcesQuantityList.Add(exp);

        bonusResources.Reverse();
        bonusResourcesQuantityList.Reverse();

        Reward reward = new Reward(bonusResources, bonusResourcesQuantityList);

        return reward;
    }
}
