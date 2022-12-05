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
    [SerializeField] private Image unitIcon;

    [SerializeField] private GameObject nextBlock;
    [SerializeField] private GameObject costsBlock;
    [SerializeField] private Button nextBtn;
    [SerializeField] private List<GameObject> costs;
    [SerializeField] private InfotipTrigger tip;

    [SerializeField] private Color normalColor;
    [SerializeField] private Color warningColor;

    private int levelUpMultiplier;
    private List<float> costList = new List<float>();
    private bool canIUpgradeUnit = true;

    public void Init(Military military, Unit unit, bool nextLevel)
    {
        root = military;

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

            if(root.IsUnitOpen(unit.unitType, unit.level) == true)
            {
                costsBlock.SetActive(false);
            }
            else
            {
                costsBlock.SetActive(true);
                FillCost(unit);
            }
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

        float goldDiscount = allBuildings.GetBonusAmount(CastleBuildings.TaxService);
        float foodDiscount = allBuildings.GetBonusAmount(CastleBuildings.Tavern);

        for(int i = 0; i < costs.Count; i++)
        {
            costs[i].SetActive(!(i > unit.costs.Count));
        }

        for(int i = 0; i < unit.costs.Count; i++)
        {
            ResourceType resourceType = unit.costs[i].type;
            TMP_Text amount = costs[i].GetComponent<TMP_Text>();

            float discount = 1f;
            if(resourceType == ResourceType.Gold) discount = goldDiscount;
            if(resourceType == ResourceType.Food) discount = foodDiscount;

            float resumeCost = Mathf.Round(unit.costs[i].amount * (unit.level + 1) * levelUpMultiplier * discount);
            costList.Add(resumeCost);

            bool checkCost = resourcesManager.CheckMinResource(resourceType, resumeCost);
            if(checkCost == false) canIUpgradeUnit = false;

            amount.color = (checkCost == true) ? normalColor : warningColor;
            amount.text = resumeCost.ToString();
            costs[i].GetComponent<Image>().sprite = resourcesIcons[resourceType];
        }
    }

    public void Deal()
    {
        if(canIUpgradeUnit == true)
        {
            
        }
        else
        {
            InfotipManager.ShowWarning("You don't have enough resources to upgrade a unit");
        }
    }
}
