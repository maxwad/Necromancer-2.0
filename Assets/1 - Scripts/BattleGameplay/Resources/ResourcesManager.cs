using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class ResourcesManager : MonoBehaviour
{
    private GMInterface gmInterface;
    private PlayerStats playerStats;
    private MacroLevelUpManager macroLevelUpManager;

    public float startGold   = 100;
    public float startFood   = 100;
    public float startStone  = 10;
    public float startWood   = 10;
    public float startIron   = 10;
    public float startMagic  = 0;
    public float startMana   = 10;
    public float startHealth = 100;

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
            [ResourceType.Magic]  = magicIcon,
            [ResourceType.Exp]    = expIcon,
            [ResourceType.Mana]   = manaIcon,
            [ResourceType.Health] = healthIcon
        };

        resourcesDict = new Dictionary<ResourceType, float>()
        {
            [ResourceType.Gold]   = startGold,
            [ResourceType.Food]   = startFood,
            [ResourceType.Stone]  = startStone,
            [ResourceType.Wood]   = startWood,
            [ResourceType.Iron]   = startIron,
            [ResourceType.Magic]  = startMagic,
            [ResourceType.Mana]   = startMana,
            [ResourceType.Health] = startHealth
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
            ChangeResource(ResourceType.Exp, 100);

        if(Input.GetKeyDown(KeyCode.KeypadPlus) == true)
            ChangeResource(ResourceType.Exp, 500);
    }

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
    
    private void UpgrateMaxManaHealth(PlayersStats stat, float maxValue)
    {
        if(stat == PlayersStats.Mana) maxMana = maxValue;
        if(stat == PlayersStats.Health) maxHealth = maxValue;
    }

    public Dictionary<ResourceType, float> GetAllResources()
    {
        return resourcesDict;
    }

    public Dictionary<ResourceType, Sprite> GetAllResourcesIcons()
    {
        return resourcesIcons;
    }

    public void ChangeResource(ResourceType type, float value)
    {
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
        if(type != ResourceType.Health) gmInterface.ShowDelta(type, value);

        EventManager.OnUpgradeResourceEvent(type, resourcesDict[type]);
    }

    public void DecreaseHealth(float value)
    {
        ResourceType type = ResourceType.Health;

        if(CheckMinResource(type, value) == false)
        {
            resourcesDict[type] = 0;
        }
        else
        {
            resourcesDict[type] += value;
        }

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
        if(type == BonusType.Gold) ChangeResource(ResourceType.Gold, value);
        if(type == BonusType.Mana) ChangeResource(ResourceType.Mana, value);
        if(type == BonusType.Health) ChangeResource(ResourceType.Health, value);
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
