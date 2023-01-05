using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class CampGame : MonoBehaviour
{
    private CampManager campManager;
    private CampUI campUI;
    private ResourcesManager resourcesManager;
    private RewardManager rewardManager;

    private Dictionary<ResourceType, Sprite> resourcesIcons;

    [SerializeField] private List<BonfireItemUI> cells;
    [SerializeField] private List<BonfireResultItemUI> rewardsItems;
    [SerializeField] private List<CampBonus> bonusesData;
    [SerializeField] private List<ResourceType> resources;

    private CampGameParameters currentParameters;
    private List<CampReward> currentCombination;

    private void Start()
    {
        campManager = GlobalStorage.instance.campManager;
        campUI = GetComponent<CampUI>();

        resourcesManager = GlobalStorage.instance.resourcesManager;
        resourcesIcons = resourcesManager.GetAllResourcesIcons();

        rewardManager = GlobalStorage.instance.rewardManager;

        ResetCells();
        SetIndexes();
    }

    public void ResetCells()
    {
        foreach(var cell in cells)
            cell.ResetCell();

        foreach(var reward in rewardsItems)
            reward.gameObject.SetActive(false);
    }

    public void SetIndexes()
    {
        for(int i = 0; i < cells.Count; i++)
            cells[i].Init(campUI, this, i);
    }

    public void PreparedToGame(CampGameParameters parameters)
    {
        currentParameters = parameters;
        currentCombination = currentParameters.combination;
    }

    public bool CanIOpenCell()
    {
        return currentParameters.attempts > 0;
    }

    public void GetResult(int index)
    {
        currentParameters.attempts--;
        campUI.FillParameters();

        CampReward reward = currentCombination[index];
        CampBonus currentBonus = GetRewardInfo(reward);

        cells[index].SetReward(currentBonus);

        if(currentBonus.reward != CampReward.Nothing)
        {
            AddBonus(currentBonus);
        }
    }

    private CampBonus GetRewardInfo(CampReward reward)
    {
        CampBonus currentBonus = new CampBonus();

        if(reward != CampReward.Resource)
        {
            foreach(var bonus in bonusesData)
            {
                if(reward == bonus.reward)
                {
                    currentBonus = bonus;
                    break;
                }
            }
        }
        else
        {
            ResourceType resource = resources[UnityEngine.Random.Range(0, resources.Count)];

            currentBonus.reward = reward;
            currentBonus.name = resource.ToString();
            currentBonus.icon = resourcesIcons[resource];
            currentBonus.amount = (int)rewardManager.GetHeapReward(resource).resourcesQuantity[0];          
        }


        Debug.Log("You found " + currentBonus.name);
        return currentBonus;
    }

    private void AddBonus(CampBonus bonus)
    {
        foreach(var reward in rewardsItems)
        {
            if(reward.gameObject.activeInHierarchy == false)
            {
                reward.gameObject.SetActive(true);
                reward.Init(bonus);
                break;
            }
        }

        currentParameters.rewardsAmount--;
        campUI.FillParameters();

    }
}
