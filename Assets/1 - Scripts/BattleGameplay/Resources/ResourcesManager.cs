using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class ResourcesManager : MonoBehaviour
{
    public float Gold { private set; get; } = 100;
    public float Food { private set; get; } = 100;
    public float Stone { private set; get; } = 10;
    public float Wood { private set; get; } = 10;
    public float Iron { private set; get; } = 10;
    public float Magic { private set; get; } = 0;
    public float Nothing { private set; get; } = 0;    

    private void Update()
    {
        //just for testing
        if(Input.GetKeyDown(KeyCode.KeypadEnter) == true)
            DecreaseResource(ResourceType.Gold, 10);
    }

    public Dictionary<ResourceType, float> GetAllResources()
    {
        return new Dictionary<ResourceType, float>() 
        { 
            [ResourceType.Gold]    = Gold,
            [ResourceType.Food]    = Food,
            [ResourceType.Stone]   = Stone,
            [ResourceType.Wood]    = Wood,
            [ResourceType.Iron]    = Iron,
            [ResourceType.Magic]   = Magic
        };
    }

    private void AddResource(ResourceType type, float value)
    {
        switch(type)
        {
            case ResourceType.Gold:
                Gold += value;
                break;

            case ResourceType.Food:
                Food += value;
                break;

            case ResourceType.Stone:
                Stone += value;
                break;

            case ResourceType.Wood:
                Wood += value;
                break;

            case ResourceType.Iron:
                Iron += value;
                break;

            case ResourceType.Magic:
                Magic += value;
                break;

            default:
                break;
        }

        EventManager.OnUpgradeResourcesEvent();
    }

    private void AddResourceAsBonus(BonusType type, float value)
    {
        if(type == BonusType.Gold) AddResource(ResourceType.Gold, value);
    }

    private bool DecreaseResource(ResourceType type, float value)
    {
        if(CheckResource(type, value) == true)
        {
            switch(type)
            {
                case ResourceType.Gold:
                    Gold -= value;
                    break;

                case ResourceType.Food:
                    Food -= value;
                    break;

                case ResourceType.Stone:
                    Stone -= value;
                    break;

                case ResourceType.Wood:
                    Wood -= value;
                    break;

                case ResourceType.Iron:
                    Iron -= value;
                    break;

                case ResourceType.Magic:
                    Magic -= value;
                    break;

                default:
                    break;
            }

            EventManager.OnUpgradeResourcesEvent();
            return true;
            
        }

        return false;
    }

    private bool CheckResource(ResourceType type, float value)
    {
        switch(type)
        {
            case ResourceType.Gold:
                if(Gold >= value) return true;
                break;

            case ResourceType.Food:
                if(Food >= value) return true;
                break;

            case ResourceType.Stone:
                if(Stone >= value) return true;
                break;

            case ResourceType.Wood:
                if(Wood >= value) return true;
                break;

            case ResourceType.Iron:
                if(Iron >= value) return true;
                break;

            case ResourceType.Magic:
                if(Magic >= value) return true;
                break;

            default:
                break;
        }

        return false;
    }

    private void OnEnable()
    {
        EventManager.BonusPickedUp += AddResourceAsBonus;
        EventManager.ResourcePickedUp += AddResource;
    }

    private void OnDisable()
    {
        EventManager.BonusPickedUp -= AddResourceAsBonus;
        EventManager.ResourcePickedUp -= AddResource;
    }
}
