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


    private void Update()
    {
        //just for testing
        if(Input.GetKeyDown(KeyCode.KeypadEnter) == true)
            DecreaseResource(ResourceType.Gold, 10);
    }

    private void AddResource(ResourceType type, float value)
    {
        switch(type)
        {
            case ResourceType.Gold:
                Gold += value;
                EventManager.OnUpgradeGoldEvent(Gold);
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

            case ResourceType.Nothing:
                break;
            default:
                break;
        }
    }

    private void AddResourceAsBonus(BonusType type, float value)
    {
        if(type == BonusType.Gold)
        {
            Gold += value;
            EventManager.OnUpgradeGoldEvent(Gold);
        }
    }

    private bool DecreaseResource(ResourceType type, float value)
    {
        if(CheckResource(type, value) == true)
        {
            switch(type)
            {
                case ResourceType.Gold:
                    Gold -= value;
                    EventManager.OnUpgradeGoldEvent(Gold);
                    return true;

                case ResourceType.Food:
                    Food -= value;
                    return true;

                case ResourceType.Stone:
                    Stone -= value;
                    return true;

                case ResourceType.Wood:
                    Wood -= value;
                    return true;

                case ResourceType.Iron:
                    Iron -= value;
                    return true;

                case ResourceType.Magic:
                    Magic -= value;
                    return true;

                case ResourceType.Nothing:
                    break;
                default:
                    break;
            }
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

            case ResourceType.Nothing:
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
