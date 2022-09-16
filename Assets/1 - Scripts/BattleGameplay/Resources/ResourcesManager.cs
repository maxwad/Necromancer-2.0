using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class ResourcesManager : MonoBehaviour
{
    private GMInterface gmInterface;
    public float Gold { private set; get; } = 100;
    public float Food { private set; get; } = 100;
    public float Stone { private set; get; } = 10;
    public float Wood { private set; get; } = 10;
    public float Iron { private set; get; } = 10;
    public float Magic { private set; get; } = 0;
    public float Mana { private set; get; } = 10;

    public Sprite goldIcon;
    public Sprite foodIcon;
    public Sprite stoneIcon;
    public Sprite woodIcon;
    public Sprite ironIcon;
    public Sprite magicIcon;
    public Sprite expIcon;
    public Sprite manaIcon;

    public Dictionary<ResourceType, float> resourcesDict;

    public Dictionary<ResourceType, Sprite> resourcesIcons;

    private void Awake()
    {
        gmInterface = GlobalStorage.instance.gmInterface;

        resourcesIcons = new Dictionary<ResourceType, Sprite>()
        {
            [ResourceType.Gold] = goldIcon,
            [ResourceType.Food] = foodIcon,
            [ResourceType.Stone] = stoneIcon,
            [ResourceType.Wood] = woodIcon,
            [ResourceType.Iron] = ironIcon,
            [ResourceType.Magic] = magicIcon,
            [ResourceType.Exp] = expIcon,
            [ResourceType.Mana] = manaIcon
        };

        resourcesDict = new Dictionary<ResourceType, float>()
        {
            [ResourceType.Gold] = Gold,
            [ResourceType.Food] = Food,
            [ResourceType.Stone] = Stone,
            [ResourceType.Wood] = Wood,
            [ResourceType.Iron] = Iron,
            [ResourceType.Magic] = Magic,
            [ResourceType.Mana] = Mana
        };

    }
    private void Update()
    {
        //just for testing
        if(Input.GetKeyDown(KeyCode.KeypadEnter) == true)
            ChangeResource(ResourceType.Mana, -10);

        if(Input.GetKeyDown(KeyCode.KeypadPlus) == true)
            ChangeResource(ResourceType.Mana, 7);
    }

    public Dictionary<ResourceType, float> GetAllResources()
    {
        return resourcesDict;
    }

    public Dictionary<ResourceType, Sprite> GetAllResourcesIcons()
    {
        return resourcesIcons;
    }

    private void ChangeResource(ResourceType type, float value)
    {
        if(type == ResourceType.Exp) return;

        float realValue = value;

        if(value < 0)
        {
            if(CheckResource(type, value) == false) return;
        }
        else
        {
            if(type == ResourceType.Mana)
            {
                realValue = CheckMaxMana(value);
            }
        }

        resourcesDict[type] += realValue;
        gmInterface.ShowDelta(type, value);

        EventManager.OnUpgradeResourcesEvent();
    }

    private float CheckMaxMana(float value)
    {
        float maxValue = GlobalStorage.instance.playerStats.GetStartParameter(PlayersStats.Mana);
        float currentValue = resourcesDict[ResourceType.Mana];
        float result;

        Debug.Log(maxValue);
        if(maxValue - currentValue >= value)
            result = value;
        else
            result = maxValue - currentValue;      

        return result;
    }

    private void AddResourceAsBonus(BonusType type, float value)
    {
        if(type == BonusType.Gold) ChangeResource(ResourceType.Gold, value);
    }

    private bool CheckResource(ResourceType type, float value)
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
    }

    private void OnDisable()
    {
        EventManager.BonusPickedUp -= AddResourceAsBonus;
        EventManager.ResourcePickedUp -= ChangeResource;
    }
}
