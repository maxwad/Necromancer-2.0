using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class UnitInMarketUI : MonoBehaviour
{
    private ResourcesManager resourcesManager;
    private Dictionary<ResourceType, Sprite> resourcesIcons;
    private FortressBuildings allBuildings;
    private Military root;

    [SerializeField] private TMP_Text unitName;
    [SerializeField] private Image iconBG;
    [SerializeField] private Image unitIcon;

    [SerializeField] private GameObject nextBlock;
    [SerializeField] private GameObject costsBlock;
    [SerializeField] private GameObject nextMark;
    [SerializeField] private Button nextBtn;
    [SerializeField] private List<GameObject> costs;
    [SerializeField] private InfotipTrigger tip;

    [SerializeField] private Color normalColor;
    [SerializeField] private Color warningColor;
    [SerializeField] private Color openedColor;

    private int levelUpMultiplier;
    private Dictionary<ResourceType, float> costList = new Dictionary<ResourceType, float>();
    private bool canIUpgradeUnit = true;
    private UnitsTypes unitsType;
    private int level;


    public void Init(Military military, Unit unit, bool nextLevel)
    {
        root = military;
        unitsType = unit.unitType;
        level = unit.level;

        if(resourcesManager == null)
        {
            resourcesManager = GlobalStorage.instance.resourcesManager;
            resourcesIcons = resourcesManager.GetAllResourcesIcons();
            allBuildings = GlobalStorage.instance.fortressBuildings;

            levelUpMultiplier = root.fortress.GetLevelUpMultiplier();
        }

        gameObject.SetActive(true);
        tip.SetUnit(unit);

        FillUnit(unit);

        if(nextLevel == false)
        {
            nextBlock.SetActive(false);
        }
        else
        {
            nextBlock.SetActive(true);

            if(root.IsUnitOpen(unit.unitType, unit.level + 1) == true)
            {
                costsBlock.SetActive(false);
            }
            else
            {
                costsBlock.SetActive(true);
                FillCost(unit);
            }
        }

        iconBG.color = (root.IsUnitOpen(unit.unitType, unit.level)) ? openedColor : warningColor;
        nextMark.SetActive(false);

        if((root.IsUnitOpen(unit.unitType, unit.level) == true))
        {
            if(root.IsUnitOpen(unit.unitType, unit.level + 1) == true)
            {
                nextBtn.gameObject.SetActive(false);
                nextMark.SetActive(true);
            }
            else
            {
                nextBtn.gameObject.SetActive(true);
                nextBtn.interactable = true;
            }
        }
        else
        {
            nextBtn.gameObject.SetActive(true);
            nextBtn.interactable = false;
        }

    }

    private void FillUnit(Unit unit) 
    {
        unitName.text = unit.unitName;
        unitIcon.sprite = unit.unitIcon;
    }

    private void FillCost(Unit unit)
    {
        costList.Clear();
        canIUpgradeUnit = true;

        float discount = allBuildings.GetBonusAmount(CastleBuildingsBonuses.HiringDiscount);

        for(int i = 0; i < costs.Count; i++)
        {
            costs[i].SetActive(!(i >= unit.costs.Count));
        }

        for(int i = 0; i < unit.costs.Count; i++)
        {
            ResourceType resourceType = unit.costs[i].type;
            TMP_Text amount = costs[i].GetComponentInChildren<TMP_Text>();

            float price = 1f;
            if(resourceType == ResourceType.Gold) price = 1 - discount;
            if(resourceType == ResourceType.Food) price = 1 - discount;

            float resumeCost = Mathf.Round(unit.costs[i].amount * (unit.level + 1) * levelUpMultiplier * price);
            costList.Add(unit.costs[i].type, resumeCost);

            bool checkCost = resourcesManager.CheckMinResource(resourceType, resumeCost);
            if(checkCost == false) canIUpgradeUnit = false;

            amount.color = (checkCost == true) ? normalColor : warningColor;
            amount.text = resumeCost.ToString();
            costs[i].GetComponentInChildren<Image>().sprite = resourcesIcons[resourceType];
        }
    }

    public void Deal()
    {
        if(canIUpgradeUnit == true)
        {
            foreach(var costItem in costList)
            {
                resourcesManager.ChangeResource(costItem.Key, -costItem.Value);
            }

            root.UpgradeUnitLevel(unitsType, level + 1);
            root.Init(root.currentBuilding);
        }
        else
        {
            InfotipManager.ShowWarning("You don't have enough resources to upgrade a unit.");
        }
    }
}
