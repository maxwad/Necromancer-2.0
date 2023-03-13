using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public partial class ResourcesManager : MonoBehaviour
{
    private GMInterface gmInterface;
    private PlayerStats playerStats;
    private MacroLevelUpManager macroLevelUpManager;

    //public float startGold   = 100;
    //public float startFood   = 100;
    //public float startStone  = 10;
    //public float startWood   = 10;
    //public float startIron   = 10;
    //public float startMana   = 10;
    //public float startHealth = 100;

    public float maxMana;
    public float maxHealth;

    public Sprite goldIcon;
    public Sprite foodIcon;
    public Sprite stoneIcon;
    public Sprite woodIcon;
    public Sprite ironIcon;
    public Sprite magicIcon;
    public Sprite expIcon;
    public Sprite manaIcon;
    public Sprite healthIcon;
    public Sprite unitIcon;
    public Sprite shardsIcon;

    public Dictionary<ResourceType, float> resourcesDict;

    public Dictionary<ResourceType, Sprite> resourcesIcons;

    private void Awake()
    {
        gmInterface = GlobalStorage.instance.gmInterface;
        playerStats = GlobalStorage.instance.playerStats;
        macroLevelUpManager = GlobalStorage.instance.macroLevelUpManager;

        resourcesIcons = new Dictionary<ResourceType, Sprite>()
        {
            [ResourceType.Gold]   = goldIcon,
            [ResourceType.Food]   = foodIcon,
            [ResourceType.Stone]  = stoneIcon,
            [ResourceType.Wood]   = woodIcon,
            [ResourceType.Iron]   = ironIcon,
            [ResourceType.Exp]    = expIcon,
            [ResourceType.Mana]   = manaIcon,
            [ResourceType.Health] = healthIcon,
            [ResourceType.Shards] = shardsIcon,
            [ResourceType.Units]  = unitIcon
        };

        resourcesDict = new Dictionary<ResourceType, float>()
        {
            [ResourceType.Gold]   = 0,
            [ResourceType.Food]   = 0,
            [ResourceType.Stone]  = 0,
            [ResourceType.Wood]   = 0,
            [ResourceType.Iron]   = 0,
            [ResourceType.Mana]   = 0,
            [ResourceType.Health] = 0,
            [ResourceType.Shards] = 0,
            [ResourceType.Units]  = 0

        };
    }

    private void Start()
    {
        maxMana = playerStats.GetCurrentParameter(PlayersStats.Mana);
        resourcesDict[ResourceType.Mana] = maxMana;
        EventManager.OnUpgradeResourceEvent(ResourceType.Mana, resourcesDict[ResourceType.Mana]);

        maxHealth = playerStats.GetCurrentParameter(PlayersStats.Health);
        resourcesDict[ResourceType.Health] = maxHealth;
        EventManager.OnUpgradeResourceEvent(ResourceType.Health, resourcesDict[ResourceType.Health]);
    }

    private void Update()
    {
        //just for testing
        if(Input.GetKeyDown(KeyCode.KeypadEnter) == true)
            ChangeResource(ResourceType.Exp, 500);

        if(Input.GetKeyDown(KeyCode.KeypadPlus) == true)
            ChangeResource(ResourceType.Exp, 1000);

        if(Input.GetKeyDown(KeyCode.PageUp) == true)
            ChangeResource(ResourceType.Mana, 100);

        if(Input.GetKeyDown(KeyCode.PageDown) == true)
            ChangeResource(ResourceType.Mana, -5);

        if(Input.GetKeyDown(KeyCode.F1) == true)
            ChangeResource(ResourceType.Health, 5);

        if(Input.GetKeyDown(KeyCode.F2) == true)
            ChangeResource(ResourceType.Health, -5);
    }

    #region GETTING

    public float GetResource(ResourceType type)
    {
        return resourcesDict[type];
    }

    public float GetMaxMana()
    {
        return maxMana;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public Dictionary<ResourceType, float> GetAllResources()
    {
        return resourcesDict;
    }

    public Dictionary<ResourceType, Sprite> GetAllResourcesIcons()
    {
        return resourcesIcons;
    }

    #endregion

    private void UpgrateMaxManaHealth(PlayersStats stat, float maxValue)
    {
        ResourceType resource = ResourceType.Mana;
        bool isUpgraded = false;

        if(stat == PlayersStats.Mana) 
        {
            resource = ResourceType.Mana;
            maxMana = maxValue;
            isUpgraded = true;
        }

        if(stat == PlayersStats.Health) 
        {
            resource = ResourceType.Health;
            maxHealth = maxValue;
            isUpgraded = true;
        }

        if(isUpgraded == true)
        {
            resourcesDict[resource] = maxValue;
            EventManager.OnUpgradeResourceEvent(resource, resourcesDict[resource]);
        }
    }

    public void ChangeResource(ResourceType type, float value)
    {
        if(value == 0) return;

        if(type == ResourceType.Units) return;

        if(type == ResourceType.Exp)
        {
            macroLevelUpManager.AddExp(value);
            return;
        }

        float realValue = value;

        if(value < 0)
        {
            if(type == ResourceType.Health)
            {
                DecreaseHealth(value);
                return;
            }
            else
            {
                if(CheckMinResource(type, value) == false) return;
            }                
        }
        else
        {
            if(type == ResourceType.Mana || type == ResourceType.Health)
            {
                realValue = CheckMaxResource(type, value);
            }
        }

        resourcesDict[type] += realValue;

        if(type != ResourceType.Health && type != ResourceType.Mana)
            gmInterface.resourcesPart.ShowDelta(type, value);

        EventManager.OnUpgradeResourceEvent(type, resourcesDict[type]);
    }

    public void LoadResource(ResourceType type, float value)
    {
        resourcesDict[type] = value;

        EventManager.OnUpgradeResourceEvent(type, resourcesDict[type]);
    }

    public void DecreaseHealth(float value)
    {
        ResourceType type = ResourceType.Health;

        if(CheckMinResource(type, value) == false)
            resourcesDict[type] = 0;
        else
            resourcesDict[type] += value;

        EventManager.OnUpgradeResourceEvent(type, resourcesDict[type]);

    }

    private float CheckMaxResource(ResourceType type, float value)
    {
        float maxValue;

        if(type == ResourceType.Mana)
            maxValue = maxMana;
        else
            maxValue = maxHealth;

        float currentValue = resourcesDict[type];
        float result;

        if(maxValue - currentValue >= value)
            result = value;
        else
            result = maxValue - currentValue;

        return result;
    }

    private void AddResourceAsBonus(BonusType type, float value)
    {
        if(type == BonusType.Gold) 
            ChangeResource(ResourceType.Gold, value);

        if(type == BonusType.Mana) 
            ChangeResource(ResourceType.Mana, value);

        if(type == BonusType.Health) 
            ChangeResource(ResourceType.Health, value);
    }

    public bool CheckMinResource(ResourceType type, float value)
    {
        if(resourcesDict[type] >= Mathf.Abs(value)) 
            return true;
        else
            return false;
    }

    private void OnEnable()
    {
        EventManager.BonusPickedUp += AddResourceAsBonus;
        EventManager.ResourcePickedUp += ChangeResource;
        EventManager.SetNewPlayerStat += UpgrateMaxManaHealth;
    }

    private void OnDisable()
    {
        EventManager.BonusPickedUp -= AddResourceAsBonus;
        EventManager.ResourcePickedUp -= ChangeResource;
        EventManager.SetNewPlayerStat -= UpgrateMaxManaHealth;
    }
}
