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
    [HideInInspector] public Garrison garrison;
    private FortressBuildings heroCastle;

    [Header("Parameters")]
    private ObjectOwner owner;
    [SerializeField] private bool isRandomResource = true;
    public ResourceBuildings buildingType;
    [HideInInspector] public string buildingName;
    [HideInInspector] public ResourceType resourceType;
    private SpriteRenderer spriteRenderer;
    [HideInInspector] public Sprite resourceSprite;
    private float resourceBaseQuote = 0;
    private bool isSiege = false;

    [Header("Upgrades")]
    [SerializeField] private List<RBUpgradeSO> builtInUpgrades;
    [SerializeField] private List<RBUpgradeSO> commonUpgrades;
    [SerializeField] private List<RBUpgradeSO> rbUpgrades;
    [SerializeField] private List<RBUpgradeSO> outpostUpgrades;
    private Dictionary<RBUpgradeSO, UpgradeStatus> upgradesDict = new Dictionary<RBUpgradeSO, UpgradeStatus>();
    public int maxCountUpgrades = 3;

    [Header("Bonuses")]
    [HideInInspector] public int siegeDaysBase = 3;
    [HideInInspector] public int siegeDays;
    [HideInInspector] public int currentSiegeDays;
    [HideInInspector] public float resourceBonus;
    [HideInInspector] public float resourceAmount;
    [HideInInspector] public bool dailyFee = false;
    [HideInInspector] public float daysInWeek = 10;
    private bool isGarrisonThere = false;
    private float resourceMultiplier = 1;

    private void Start()
    {
        owner            = GetComponent<ObjectOwner>();
        spriteRenderer   = GetComponent<SpriteRenderer>();
        resourcesManager = GlobalStorage.instance.resourcesManager;
        resourcesSources = resourcesManager.GetComponent<ResourcesSources>();
        garrison         = GetComponent<Garrison>();
        boostManager     = GlobalStorage.instance.boostManager;
        heroCastle       = GlobalStorage.instance.fortressBuildings;

        // Length - 2 because we don't need in resource buildings Castle or outposts
        if(isRandomResource == true)
            buildingType = (ResourceBuildings)UnityEngine.Random.Range(0, Enum.GetValues(typeof(ResourceBuildings)).Length - 2);

        ResourceBuildingData data = resourcesSources.GetResourceBuildingData(buildingType);
        buildingName              = data.resourceBuilding.ToString();
        resourceType              = data.resourceType;
        resourceSprite            = data.resourceSprite;
        resourceBaseQuote         = data.resourceBaseIncome;

        if(buildingType == ResourceBuildings.Castle)
        {
            Register();
            owner = GlobalStorage.instance.fortressGO;
        }
        else
        {
            spriteRenderer.sprite = data.buildingSprite;
            spriteRenderer.color = data.buildingColor;
            owner.Init(buildingType, resourceType);
            owner.ChangeLevel(0);
        }

        GetUpgrades();
        ResetBonuses();

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.PageUp))
        {
            StartSiege();
        }
    }

    #region UPGRADES

    private void GetUpgrades()
    {
        List<RBUpgradeSO> upgradesList = new List<RBUpgradeSO>();

        foreach(var item in builtInUpgrades)
        {
            upgradesList.Add(item);
        }

        foreach(var item in commonUpgrades)
        {
            upgradesList.Add(item);
        }

        if(buildingType == ResourceBuildings.Outpost)
        {
            foreach(var item in outpostUpgrades)
            {
                upgradesList.Add(item);
            }
        }
        else
        {
            foreach(var item in rbUpgrades)
            {
                upgradesList.Add(item);
            }
        }

        foreach(var upgrade in upgradesList)
        {
            if(upgradesDict.ContainsKey(upgrade) == false)
            {
                upgradesDict.Add(upgrade, new UpgradeStatus());
            }
        }

        foreach(var item in builtInUpgrades)
        {
            UpgradeStatus upgradeStatus = upgradesDict[item];
            upgradeStatus.isHidden = true;
            upgradesDict[item] = upgradeStatus;

            ApplyUpgrade(item);
        }
    }

    public int GetCountOfActiveUpgrades()
    {
        int count = 0;
        foreach(var upgrade in upgradesDict)
        {
            if(upgrade.Value.isEnable == true) count++;
        }

        return count - builtInUpgrades.Count;
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
        UpgradeStatus upgradeStatus = upgradesDict[upgrade];
        upgradeStatus.isEnable = true;
        upgradesDict[upgrade] = upgradeStatus;

        ResetBonuses();

        int level = GetCountOfActiveUpgrades();
        owner.ChangeLevel(level);
    }

    public Dictionary<RBUpgradeSO, UpgradeStatus> GetUpgradesStatuses()
    {
        return upgradesDict;
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
        if(isSiege == true) return;

        siegeDays = siegeDaysBase;

        if(buildingType == ResourceBuildings.Castle)
            siegeDays += (int)heroCastle.GetBonusAmount(CastleBuildingsBonuses.SiegeTime);
        else
        {
            foreach(var upgrade in upgradesDict)
            {
                if(upgrade.Value.isEnable == true && upgrade.Key.upgradeBonus == ResourceBuildingsUpgrades.SiegeTime)
                    siegeDays += (int)upgrade.Key.value;
            }
        }

        currentSiegeDays = siegeDays + (int)garrison.GetGarrisonAmount();
    }

    public void ResetResourceAmount()
    {
        resourceAmount = resourceBaseQuote;

        if(buildingType != ResourceBuildings.Outpost)
            resourceBonus = boostManager.GetBoost(BoostType.ExtraResourcesProduce);
        else
            resourceBonus = 0;

        foreach(var upgrade in upgradesDict)
        {
            if(upgrade.Value.isEnable == true && upgrade.Key.upgradeBonus == ResourceBuildingsUpgrades.ResourceBonus)
                resourceBonus += (upgrade.Key.value * 0.01f);
        }

        resourceAmount += resourceAmount * resourceBonus;
        resourceAmount = Mathf.RoundToInt(resourceAmount);
    }

    public void ResetDailyPortion()
    {
        dailyFee = false;

        foreach(var upgrade in upgradesDict)
        {
            if(upgrade.Key.upgradeBonus == ResourceBuildingsUpgrades.DailyResources && upgrade.Value.isEnable == true)
                dailyFee = true;
        }
    }

    public void ResetGarrisonStatus()
    {
        isGarrisonThere = false;

        foreach(var upgrade in upgradesDict)
        {
            if(upgrade.Key.upgradeBonus == ResourceBuildingsUpgrades.Garrison && upgrade.Value.isEnable == true)
                isGarrisonThere = true;
        }
    }

    public bool GetGarrisonStatus()
    {
        return isGarrisonThere;
    }

    public void Register()
    {
        resourcesSources.Register(this);

        if(owner != null)
            owner.ChangeOwner(TypeOfObjectsOwner.Player);
    }

    public void Unregister()
    {
        resourcesSources.Unregister(this);

        if(owner != null)
            owner.ChangeOwner(TypeOfObjectsOwner.Enemy);
    }

    public bool CheckOwner()
    {
        return owner.CheckOwner();
    }
    #endregion

    #region SIEGE

    public void StartSiege(bool siegeMode = true)
    {
        ResetSiegeDays();
        isSiege = !isSiege;

        owner.StartSiege(isSiege);
        owner.UpdateSiegeTerm(currentSiegeDays + "/" + siegeDays);
    }

    public bool CheckSiegeStatus()
    {
        return isSiege;
    }

    private void UpdateSiege()
    {
        if(isSiege == true)
        {
            currentSiegeDays--;
            garrison.DecreaseGarrisonUnits();
            owner.UpdateSiegeTerm(currentSiegeDays + "/" + siegeDays);
            if(currentSiegeDays <= 0)
            {
                StartSiege();

                if(buildingType != ResourceBuildings.Castle)
                {
                    Unregister();
                    InfotipManager.ShowWarning("You lost " + buildingName + "...");
                }
                else
                {
                    heroCastle.DestroyBuildings();
                    InfotipManager.ShowWarning("A few buildings in your Castle was destroyed.");
                }
            }
        }
        else
        {
            currentSiegeDays = siegeDays;
        }
    }

    #endregion

    private void OnEnable()
    {
        EventManager.NewMove += UpdateSiege;
    }

    private void OnDisable()
    {
        EventManager.NewMove -= UpdateSiege;
    }
}
