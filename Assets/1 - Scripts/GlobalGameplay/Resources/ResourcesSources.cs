using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

[Serializable]
public class ResourceBuildingData
{
    public ResourceType resourceType;
    public ResourceBuildings resourceBuilding;
    public Sprite resourceSprite;
    public Sprite buildingSprite;
    public float resourceBaseIncome;
}

public class ResourcesSources : MonoBehaviour
{
    private ResourcesManager resourcesManager;
    [SerializeField] private List<ResourceBuildingData> resourceBuildings;
    [SerializeField] private List<RBUpgradeSO> upgrades;

    private List<ResourceBuilding> sources = new List<ResourceBuilding>();

    private Dictionary<ResourceType, float> dailyIncome = new Dictionary<ResourceType, float>();
    private Dictionary<ResourceType, float> weeklyIncome = new Dictionary<ResourceType, float>();

    private void Start()
    {
        resourcesManager = GetComponent<ResourcesManager>();
    }

    #region GETTINGS
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
    }

    public void Unregister(ResourceBuilding building)
    {
        sources.Remove(building);        
    }



    #endregion

    private void CheckDailyIncome()
    {
        dailyIncome.Clear();

        foreach(var building in sources)
        {
            if(building.dailyFee == true)
            {
                if(dailyIncome.ContainsKey(building.resourceType))
                {
                    dailyIncome[building.resourceType] += building.GetAmount() / 10;
                }
                else
                {
                    dailyIncome.Add(building.resourceType, building.GetAmount() / 10);
                }
            }
        }

        foreach(var resource in dailyIncome)
        {
            resourcesManager.ChangeResource(resource.Key, resource.Value);
        }
    }

    private void CheckWeeklyIncome(int counter)
    {
        weeklyIncome.Clear();

        foreach(var building in sources)
        {
            if(building.dailyFee == false)
            {
                if(weeklyIncome.ContainsKey(building.resourceType))
                {
                    weeklyIncome[building.resourceType] += building.GetAmount();
                }
                else
                {
                    weeklyIncome.Add(building.resourceType, building.GetAmount());
                }
            }
        }

        foreach(var resource in weeklyIncome)
        {
            resourcesManager.ChangeResource(resource.Key, resource.Value);
        }
    }

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
