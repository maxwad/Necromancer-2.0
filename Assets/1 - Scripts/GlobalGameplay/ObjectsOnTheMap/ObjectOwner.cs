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
    public bool isOwnerRequired = false;
    private bool isVisited = false;

    private Color currentColor;
    public Color neutralColor;
    public Color enemyColor;
    public Color playerColor;

    public Color visitedColor;
    public Color notVisitedColor;

    public SpriteRenderer flagSpite;

    [SerializeField] private GameObject siegeBlock;
    [SerializeField] private TMP_Text term;

    private TooltipTrigger tooltip;
    //TypeOfObjectOnTheMap test;

    private void Awake()
    {
        tooltip = GetComponent<TooltipTrigger>();

        //test = GetComponent<ClickableObject>().objectType;

        if(isOwnerRequired == true)
            ChangeFlag(owner);
        else
            SetVisitStatus(false);
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

    public void SetVisitStatus(bool statusMode)
    {
        if(flagSpite == null) return;

        ChangeFlag(statusMode);
    }

    private void ChangeFlag(bool statusMode)
    {
        //string status;

        if(statusMode == true)
        {
            currentColor = visitedColor;
            //status = "You have already brought gifts here this week.";
        }
        else
        {
            currentColor = notVisitedColor;
            //status = "You can heal your units here.";
        }

        flagSpite.color = currentColor;
        //tooltip.SetStatus(status);
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

    public bool GetVisitStatus()
    {
        return isVisited;
    }

    public void SetVisitStatus()
    {
        isVisited = true;
    }
}
