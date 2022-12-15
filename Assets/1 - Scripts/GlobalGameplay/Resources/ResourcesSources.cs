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
    [SerializeField] private List<ResourceBuildingData> resourceBuildings;

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
}
