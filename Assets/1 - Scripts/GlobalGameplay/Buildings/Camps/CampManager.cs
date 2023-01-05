using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class CampManager : MonoBehaviour
{
    private ResourcesManager resourcesManager;
    private RewardManager rewardManager;

    private Dictionary<GameObject, bool> campsRewardsDict = new Dictionary<GameObject, bool>();

    [SerializeField] private int cellsAmount = 25;
    [SerializeField] private int rewardsAmount = 10;
    [SerializeField] private int attempts = 10;
    [SerializeField] private int helps = 4;
    [SerializeField] private int runeDrawnings = 3;

    public void Register(GameObject building)
    {
        campsRewardsDict.Add(building, true);
    }

    #region GETTINGS

    public bool GetCampStatus(GameObject building)
    {
        return (campsRewardsDict.ContainsKey(building) == true) ? campsRewardsDict[building] : false;
    }

    public CampGameParameters GetStartParameters()
    {
        CampGameParameters gameParameters = new CampGameParameters();

        gameParameters.cellsAmount = cellsAmount;
        gameParameters.rewardsAmount = rewardsAmount;
        gameParameters.attempts = attempts;
        gameParameters.helps = helps;
        gameParameters.combination = GetBonfireCombination();

        return gameParameters;
    }

    public List<CampReward> GetBonfireCombination()
    {
        List<CampReward> rewardList = new List<CampReward>();
        List<int> variants = new List<int>();
        int bonusAmount = rewardsAmount;

        for(int i = 0; i < cellsAmount; i++)
        {
            rewardList.Add(CampReward.Nothing);
            variants.Add(i);
        }

        FillRandomElementBy(CampReward.AbilityPoint);
        FillRandomElementBy(CampReward.Health);
        FillRandomElementBy(CampReward.Mana);
        FillRandomElementBy(CampReward.Help);

        for(int i = 0; i < runeDrawnings; i++)
        {
            FillRandomElementBy(CampReward.RuneDrawing);
        }

        for(int i = 0; i < bonusAmount; i++)
        {
            FillRandomElementBy(CampReward.Resource);
        }

        return rewardList;


        void FillRandomElementBy(CampReward reward)
        {
            int randomIndex = variants[UnityEngine.Random.Range(0, variants.Count)];            
            rewardList[randomIndex] = reward;
            variants.Remove(randomIndex);

            bonusAmount--;
        }
    }

    public void DestroyCamp(GameObject building)
    {
        campsRewardsDict[building] = false;
        Debug.Log("Close Camp");
    }

    #endregion

    public void SpendHelp()
    {
        helps--;
    }
}
