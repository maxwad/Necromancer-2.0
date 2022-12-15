using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class ResourceBuilding : MonoBehaviour
{
    private ResourcesSources resourcesSources;
    public BuildingGarrison garrison;

    [Header("Parameters")]
    [SerializeField] private bool isRandomResource = true;
    [SerializeField] private ResourceBuildings buildingType;
    private ResourceType resourceType;
    private SpriteRenderer spriteRenderer;
    private Sprite resourceSprite;
    private float sourceBaseQuote = 0;

    [Header("Upgrades")]
    [SerializeField] private List<GameObject> upgrades;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        resourcesSources = GlobalStorage.instance.resourcesManager.GetComponent<ResourcesSources>();
        garrison = GetComponent<BuildingGarrison>();

        if(isRandomResource == true)
            buildingType = (ResourceBuildings)UnityEngine.Random.Range(0, Enum.GetValues(typeof(ResourceBuildings)).Length);

        ResourceBuildingData data = resourcesSources.GetResourceBuildingData(buildingType);
        if(data != null)
        {
            resourceType = data.resourceType;
            spriteRenderer.sprite = data.buildingSprite;
            resourceSprite = data.resourceSprite;
            sourceBaseQuote = data.resourceBaseIncome;
        }
    }

    private void OnEnable()
    {
        //EventManager.NewMove += NewDay;

    }

    private void OnDisable()
    {
        //EventManager.NewMove -= NewDay;

    }
}
