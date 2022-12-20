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
    [HideInInspector] public ResourceBuildings buildingType;
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
    public bool isGarrisonThere = false;
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

        if(isRandomResource == true)
            buildingType = (ResourceBuildings)UnityEngine.Random.Range(0, Enum.GetValues(typeof(ResourceBuildings)).Length - 1);

        ResourceBuildingData data = resourcesSources.GetResourceBuildingData(buildingType);
        buildingName = data.resourceBuilding.ToString();
        resourceType = data.resourceType;
        resourceSprite = data.resourceSprite;
        resourceBaseQuote = data.resourceBaseIncome;

        if(buildingType == ResourceBuildings.Castle)
        {
            Register();
            owner = GlobalStorage.instance.fortressGO;
        }
        else
            spriteRenderer.sprite = data.buildingSprite;


        foreach(var upgrade in upgrades)
        {
            upgradesStatus.Add(upgrade, false);
        }

        ResetBonuses();

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.PageUp))
        {
            StartSiege();

            //isSiege = !isSiege;
            //Debug.Log("is siege = " + isSiege);
        }
    }

    #region UPGRADES

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
            foreach(var upgrade in upgradesStatus)
            {
                if(upgrade.Value == true && upgrade.Key.upgradeBonus == ResourceBuildingsUpgrades.SiegeTime)
                    siegeDays += (int)upgrade.Key.value;
            }
        }

        currentSiegeDays = siegeDays + (int)garrison.GetGarrisonAmount();
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

        if(owner != null)
            owner.ChangeOwner(TypeOfObjectsOwner.Player);
    }

    public void Unregister()
    {
        resourcesSources.Unregister(this);

        if(owner != null)
            owner.ChangeOwner(TypeOfObjectsOwner.Enemy);
    }

    #endregion

    #region SIEGE

    public void StartSiege(bool siegeMode = true, bool switchMode = true)
    {
        ResetSiegeDays();
        isSiege = (switchMode == true) ? !isSiege : siegeMode;

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
