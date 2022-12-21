using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class ResourcesSources : MonoBehaviour
{
    private ResourcesManager resourcesManager;
    [SerializeField] private List<ResourceBuildingData> resourceBuildings;
    [SerializeField] private List<RBUpgradeSO> upgrades;

    private List<ResourceBuilding> sources = new List<ResourceBuilding>();

    private Dictionary<ResourceType, float> dailyIncome = new Dictionary<ResourceType, float>();
    private Dictionary<ResourceType, float> weeklyIncome = new Dictionary<ResourceType, float>();
    private int dailyPortion = 10;

    private void Start()
    {
        resourcesManager = GetComponent<ResourcesManager>();
    }


    #region REGISTERS

    public void Register(ResourceBuilding building)
    {
        bool isBuildingInList = false;
        for(int i = 0; i < sources.Count; i++)
        {
            if(sources[i] == building)
            {
                isBuildingInList = true;
                break;
            }
        }

        if(isBuildingInList == false)
        {
            sources.Add(building);
        }

        UpdateIncomes(dailyIncome, true);
        UpdateIncomes(weeklyIncome, false);
    }

    public void Unregister(ResourceBuilding building)
    {
        sources.Remove(building);

        UpdateIncomes(dailyIncome, true);
        UpdateIncomes(weeklyIncome, false);
    }

    #endregion

    #region INCOMES
    private void UpdateIncomes(Dictionary<ResourceType, float> income, bool dailyMode )
    {
        int divider = (dailyMode == true) ? dailyPortion : 1;

        income.Clear();

        foreach(var building in sources)
        {
            if(building.dailyFee == dailyMode)
            {
                if(income.ContainsKey(building.resourceType))
                {
                    income[building.resourceType] += building.GetAmount() / divider;
                }
                else
                {
                    income.Add(building.resourceType, building.GetAmount() / divider);
                }
            }
        }
    }

    private void CheckDailyIncome()
    {
        UpdateIncomes(dailyIncome, true);

        foreach(var resource in dailyIncome)
        {
            resourcesManager.ChangeResource(resource.Key, resource.Value);
        }
    }

    private void CheckWeeklyIncome(int counter)
    {
        UpdateIncomes(weeklyIncome, false);

        foreach(var resource in weeklyIncome)
        {
            resourcesManager.ChangeResource(resource.Key, resource.Value);
        }
    }

    #endregion

    #region GETTINGS

    public float GetResourceGrowth(ResourceType resourceType)
    {
        return (weeklyIncome.ContainsKey(resourceType) == true) ? weeklyIncome[resourceType] : 0;
    }

    public ResourceBuildingData GetResourceBuildingData(ResourceBuildings buildingType)
    {
        foreach(var building in resourceBuildings)
        {
            if(building.resourceBuilding == buildingType)
            {
                return building;
            }
        }

        return null;
    }

    #endregion

    private void OnEnable()
    {
        EventManager.NewMove += CheckDailyIncome;
        EventManager.NewWeek += CheckWeeklyIncome;
    }

    private void OnDisable()
    {
        EventManager.NewMove -= CheckDailyIncome;
        EventManager.NewWeek -= CheckWeeklyIncome;
    }
}
