using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class CampManager : MonoBehaviour
{
    private ResourcesManager resourcesManager;
    private RewardManager rewardManager;

    private Dictionary<ResourceType, Sprite> resourcesIcons;
    private Dictionary<GameObject, bool> campsRewardsDict = new Dictionary<GameObject, bool>();

    [Header("Parameters")]
    [SerializeField] private int cellsAmount = 25;
    [SerializeField] private int rewardsAmount = 10;
    [SerializeField] private int attempts = 10;
    [SerializeField] private int helps = 4;
    [SerializeField] private int runeDrawnings = 3;

    [SerializeField] private List<CampBonus> bonusesData;
    [SerializeField] private List<ResourceType> resources;

    private void Start()
    {
        resourcesManager = GlobalStorage.instance.resourcesManager;
        resourcesIcons = resourcesManager.GetAllResourcesIcons();

        rewardManager = GlobalStorage.instance.rewardManager;
    }

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

    public List<CampBonus> GetBonfireCombination()
    {
        List<CampReward> rewardNamesList = new List<CampReward>();
        List<int> variants = new List<int>();
        int bonusAmount = rewardsAmount;

        for(int i = 0; i < cellsAmount; i++)
        {
            rewardNamesList.Add(CampReward.Nothing);
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

        int counter = bonusAmount;
        for(int i = 0; i < counter; i++)
        {
            FillRandomElementBy(CampReward.Resource);
        }

        return CreateBonusList(); ;

        void FillRandomElementBy(CampReward reward)
        {
            int randomIndex = variants[UnityEngine.Random.Range(0, variants.Count)];            
            rewardNamesList[randomIndex] = reward;
            variants.Remove(randomIndex);

            bonusAmount--;
        }

        List<CampBonus> CreateBonusList()
        {
            List<CampBonus> list = new List<CampBonus>();

            for(int i = 0; i < rewardNamesList.Count; i++)
            {
                CampBonus currentBonus = new CampBonus();

                if(rewardNamesList[i] != CampReward.Resource)
                {
                    foreach(var bonus in bonusesData)
                    {
                        if(rewardNamesList[i] == bonus.reward)
                        {
                            currentBonus = bonus;
                            break;
                        }
                    }
                }
                else
                {
                    ResourceType resource = resources[UnityEngine.Random.Range(0, resources.Count)];

                    currentBonus.reward = rewardNamesList[i];
                    currentBonus.resource = resource;
                    currentBonus.name = resource.ToString();
                    currentBonus.icon = resourcesIcons[resource];
                    currentBonus.amount = (int)rewardManager.GetHeapReward(resource).resourcesQuantity[0];
                }

                list.Add(currentBonus);
            }

            return list;
        }
    }

    public int GetHelpsCount()
    {
        return helps;
    }

    public void CloseCamp(GameObject building)
    {
        campsRewardsDict[building] = false;
        Debug.Log("Close Camp");
    }

    #endregion

    public void ChangeHelpPointsAmount(int delta)
    {
        helps += delta;
    }
}
