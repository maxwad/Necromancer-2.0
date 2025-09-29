using UnityEngine;
using TMPro;
using Enums;

public class ObjectOwner : MonoBehaviour
{
    public TypeOfObjectsOwner owner;
    public bool isGuardNeeded = true;
    public float probabilityGuard = 100;
    public bool isOwnerRequired = false;
    [HideInInspector] public bool isVisited = false;

    private Color currentColor;
    public Color neutralColor;
    public Color enemyColor;
    public Color playerColor;

    public Color visitedColor;
    public Color notVisitedColor;

    public SpriteRenderer flagSpite;
    [SerializeField] private TMP_Text level;

    [SerializeField] private GameObject siegeBlock;
    [SerializeField] private TMP_Text term;

    private TooltipTrigger tooltip;
    //TypeOfObjectOnTheMap test;

    private void Awake()
    {
        tooltip = GetComponent<TooltipTrigger>();

        //test = GetComponent<ClickableObject>().objectType;

        if(isOwnerRequired == true)
            ChangeOwnerFlag(owner);
        else
            SetVisitStatus(false);
    }

    public void ChangeOwner(TypeOfObjectsOwner newOwner)
    {
        if(newOwner != owner)
        {
            owner = newOwner;
            ChangeOwnerFlag(owner);
        }
    }

    private void ChangeOwnerFlag(TypeOfObjectsOwner newOwner)
    {
        if(flagSpite == null) return;

        switch(newOwner)
        {
            case TypeOfObjectsOwner.Player:
                currentColor = playerColor;
                isGuardNeeded = false;
                break;

            case TypeOfObjectsOwner.Enemy:
                currentColor = enemyColor;
                isGuardNeeded = true;
                break;

            case TypeOfObjectsOwner.Nobody:
                currentColor = neutralColor;
                break;

            default:
                break;
        }

        if(tooltip != null) tooltip.SetOwner(newOwner.ToString());
        flagSpite.color = currentColor;
    }

    public void SetVisitStatus(bool statusMode)
    {
        isVisited = statusMode;

        if(flagSpite == null) return;

        ChangeVisitFlag(statusMode);
    }

    private void ChangeVisitFlag(bool statusMode)
    {
        currentColor = (statusMode == true) ? visitedColor : notVisitedColor;
        flagSpite.color = currentColor;
        tooltip.SetStatus(statusMode);
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
        if(tooltip == null)
        {
            Debug.Log("Tiiltip in owner = null");
            return;
        }

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
        tooltip.SetOwner(owner.ToString());
    }

    public bool CheckOwner(TypeOfObjectsOwner checkingOwner = TypeOfObjectsOwner.Player)
    {
        return owner == checkingOwner;
    }

    public bool GetVisitStatus()
    {
        return isVisited;
    }

    public void ChangeLevel(int newLevel)
    {
        if(level == null) return;

        if(newLevel == 0) 
        {
            level.gameObject.SetActive(false);
        }
        else
        {
            level.gameObject.SetActive(true);
            level.text = newLevel.ToString();
        }
    }
}
