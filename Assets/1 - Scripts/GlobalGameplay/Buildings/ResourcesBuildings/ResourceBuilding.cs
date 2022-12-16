using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class ResourceBuilding : MonoBehaviour
{
    private ResourcesManager resourcesManager;
    private ResourcesSources resourcesSources;
    private BoostManager boostManager;
    public BuildingGarrison garrison;

    [Header("Parameters")]
    [SerializeField] private bool isRandomResource = true;
    [SerializeField] private ResourceBuildings buildingType;
    [HideInInspector] public string buildingName;
    private ResourceType resourceType;
    private SpriteRenderer spriteRenderer;
    [HideInInspector] public Sprite resourceSprite;
    private float resourceBaseQuote = 0;
    private bool isSiege = false;


    [Header("Upgrades")]
    [SerializeField] private List<RBUpgradeSO> upgrades;
    [HideInInspector] public Dictionary<RBUpgradeSO, bool> upgradesStatus = new Dictionary<RBUpgradeSO, bool>();
    [HideInInspector] public int maxCountUpgrades = 3;

    [Header("Bonuses")]
    [HideInInspector] public int siegeDaysBase = 3;
    [HideInInspector] public int siegeDays;
    [HideInInspector] public float resourceBonus;
    [HideInInspector] public float resourceAmount;
    [HideInInspector] public bool dailyFee = false;
    [HideInInspector] public float daysInWeek = 10;
    [HideInInspector] public bool isGarrisonThere = false;


    private void Start()
    {
        spriteRenderer   = GetComponent<SpriteRenderer>();
        resourcesManager = GlobalStorage.instance.resourcesManager;
        resourcesSources = resourcesManager.GetComponent<ResourcesSources>();
        garrison         = GetComponent<BuildingGarrison>();
        boostManager     = GlobalStorage.instance.boostManager;

        if(isRandomResource == true)
            buildingType = (ResourceBuildings)UnityEngine.Random.Range(0, Enum.GetValues(typeof(ResourceBuildings)).Length);

        ResourceBuildingData data = resourcesSources.GetResourceBuildingData(buildingType);
        if(data != null)
        {
            buildingName = data.resourceBuilding.ToString();
            resourceType = data.resourceType;
            spriteRenderer.sprite = data.buildingSprite;
            resourceSprite = data.resourceSprite;
            resourceBaseQuote = data.resourceBaseIncome;
        }

        foreach(var upgrade in upgrades)
        {
            upgradesStatus.Add(upgrade, false);
        }

        ResetBonuses();

    }

    #region UPGRADES

    public bool CheckUpgrade(RBUpgradeSO upgrade)
    {
        return upgradesStatus[upgrade];
    }

    public int GetCountOfActiveUpgrades()
    {
        int count = 0;
        foreach(var upgrade in upgradesStatus)
        {
            if(upgrade.Value == true) count++;
        }

        return count;
    }

    public void ApplyUpgrade(RBUpgradeSO upgrade, bool activeMode = true)
    {
        upgradesStatus[upgrade] = activeMode;
        ResetBonuses();
    }

    #endregion

    #region Bonuses

    public void ResetBonuses()
    {
        ResetSiegeDays();
        ResetResourceAmount();
        ResetDailyPortion();
        ResetGarrisonStatus();
    }

    public void ResetSiegeDays()
    {
        siegeDays = siegeDaysBase;
        foreach(var upgrade in upgradesStatus)
        {
            if(upgrade.Value == true && upgrade.Key.upgradeBonus == ResourceBuildingsUpgrades.SiegeTime)
                siegeDays += (int)upgrade.Key.value;
        }
    }

    public void ResetResourceAmount()
    {
        resourceAmount = resourceBaseQuote;
        resourceBonus = boostManager.GetBoost(BoostType.ExtraResourcesProduce);

        foreach(var upgrade in upgradesStatus)
        {
            if(upgrade.Value == true && upgrade.Key.upgradeBonus == ResourceBuildingsUpgrades.ResourceBonus)
                resourceBonus += (upgrade.Key.value * 0.01f);
        }

        resourceAmount += resourceAmount * resourceBonus;
        resourceAmount = Mathf.RoundToInt(resourceAmount);
    }

    public void ResetDailyPortion()
    {
        dailyFee = false;

        foreach(var upgrade in upgradesStatus)
        {
            if(upgrade.Key.upgradeBonus == ResourceBuildingsUpgrades.DailyResources && upgrade.Value == true)
                dailyFee = true;
        }
    }

    public void ResetGarrisonStatus()
    {
        isGarrisonThere = false;

        foreach(var upgrade in upgradesStatus)
        {
            if(upgrade.Key.upgradeBonus == ResourceBuildingsUpgrades.Garrison && upgrade.Value == true)
                isGarrisonThere = true;
        }

        garrison.enabled = isGarrisonThere;
    }

    #endregion

    private void OnEnable()
    {
        //EventManager.NewMove += NewDay;

    }

    private void OnDisable()
    {
        //EventManager.NewMove -= NewDay;

    }
}
