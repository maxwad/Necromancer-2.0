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
    public Garrison garrison;

    [Header("Parameters")]
    [SerializeField] private bool isRandomResource = true;
    [SerializeField] private ResourceBuildings buildingType;
    [HideInInspector] public string buildingName;
    [HideInInspector] public ResourceType resourceType;
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
    [HideInInspector] public int currentSiegeDays;
    [HideInInspector] public float resourceBonus;
    [HideInInspector] public float resourceAmount;
    [HideInInspector] public bool dailyFee = false;
    [HideInInspector] public float daysInWeek = 10;
    [HideInInspector] public bool isGarrisonThere = false;
    private float resourceMultiplier = 1;


    private void Start()
    {
        spriteRenderer   = GetComponent<SpriteRenderer>();
        resourcesManager = GlobalStorage.instance.resourcesManager;
        resourcesSources = resourcesManager.GetComponent<ResourcesSources>();
        garrison         = GetComponent<Garrison>();
        boostManager     = GlobalStorage.instance.boostManager;

        if(isRandomResource == true)
            buildingType = (ResourceBuildings)UnityEngine.Random.Range(0, Enum.GetValues(typeof(ResourceBuildings)).Length - 1);

        ResourceBuildingData data = resourcesSources.GetResourceBuildingData(buildingType);
        if(buildingType != ResourceBuildings.Castle)
        {
            buildingName = data.resourceBuilding.ToString();
            resourceType = data.resourceType;
            spriteRenderer.sprite = data.buildingSprite;
            resourceSprite = data.resourceSprite;
            resourceBaseQuote = data.resourceBaseIncome;
        }
        else
        {
            resourceType = data.resourceType;
            resourceBaseQuote = data.resourceBaseIncome;
            Register();
        }

        foreach(var upgrade in upgrades)
        {
            upgradesStatus.Add(upgrade, false);
        }

        ResetBonuses();

    }

    #region UPGRADES

    public bool CheckSiegeStatus()
    {
        return isSiege;
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

    public float GetAmount()
    {
        ResetResourceAmount();
        return resourceAmount * resourceMultiplier;
    }

    public void SetResourceMultiplier(float number)
    {
        resourceMultiplier = number;
    }

    public void ApplyUpgrade(RBUpgradeSO upgrade, bool activeMode = true)
    {
        upgradesStatus[upgrade] = activeMode;
        ResetBonuses();
    }

    private void CheckSiege()
    {
        if(isSiege == true)
        {
            currentSiegeDays--;
            if(currentSiegeDays == 0)
            {
                Unregister();
                InfotipManager.ShowWarning("You lost " + buildingName + "...");
            }
        }
        else
        {
            if(currentSiegeDays < siegeDays)
            {
                currentSiegeDays++;
            }
        }
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

        if(isSiege == false) currentSiegeDays = siegeDays;
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
        //for Castle
        if(buildingType == ResourceBuildings.Castle) return;

        isGarrisonThere = false;

        foreach(var upgrade in upgradesStatus)
        {
            if(upgrade.Key.upgradeBonus == ResourceBuildingsUpgrades.Garrison && upgrade.Value == true)
                isGarrisonThere = true;

        }
    }

    public void Register()
    {
        resourcesSources.Register(this);
    }

    public void Unregister()
    {
        resourcesSources.Unregister(this);
    }


    #endregion

    private void OnEnable()
    {
        EventManager.NewMove += CheckSiege;
    }

    private void OnDisable()
    {
        EventManager.NewMove -= CheckSiege;
    }
}
