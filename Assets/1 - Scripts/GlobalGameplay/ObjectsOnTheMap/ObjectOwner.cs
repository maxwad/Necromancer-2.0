using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static NameManager;
using System;

public class ObjectOwner : MonoBehaviour
{
    public TypeOfObjectsOwner owner;
    public bool isGuardNeeded = true;
    public float probabilityGuard = 100;

    private Color currentColor;
    public Color neutralColor;
    public Color enemyColor;
    public Color playerColor;

    public SpriteRenderer flagSpite;

    [SerializeField] private GameObject siegeBlock;
    [SerializeField] private TMP_Text term;

    private TooltipTrigger tooltip;

    private void Awake()
    {
        tooltip = GetComponent<TooltipTrigger>();
        ChangeFlag(owner);
    }

    public void ChangeOwner(TypeOfObjectsOwner newOwner)
    {
        if(newOwner != owner)
        {
            owner = newOwner;
            ChangeFlag(owner);
        }
    }

    private void ChangeFlag(TypeOfObjectsOwner newOwner)
    {
        if(flagSpite == null) return;

        switch(newOwner)
        {
            case TypeOfObjectsOwner.Player:
                currentColor = playerColor;
                break;

            case TypeOfObjectsOwner.Enemy:
                currentColor = enemyColor;
                break;

            case TypeOfObjectsOwner.Nobody:
                currentColor = neutralColor;
                break;

            default:
                break;
        }

        if(tooltip != null) tooltip.SetStatus(newOwner.ToString());
        flagSpite.color = currentColor;
    }

    public void StartSiege(bool siegeMode)
    {
        siegeBlock.SetActive(siegeMode);
    }

    public void UpdateSiegeTerm(string termStr)
    {
        term.text = termStr;
    }

    public void Init(ResourceBuildings buildingType, ResourceType resourceType)
    {
        if(tooltip == null) return;

        tooltip.header = buildingType.ToString();

        string content;

        if(buildingType == ResourceBuildings.Outpost)
        {
           content = "Military building. " + resourceType.ToString();
        }
        else if(buildingType == ResourceBuildings.Castle)
        {
            content = "Your main building.";
        }
        else
        {
            content = "Resource building. " + resourceType.ToString();
        }

        tooltip.content = content;
        tooltip.SetStatus(owner.ToString());
    }

    public bool CheckOwner()
    {
        return owner == TypeOfObjectsOwner.Player;
    }
}
