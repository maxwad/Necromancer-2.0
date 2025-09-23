using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using static Enums;
using Zenject;

public class RBUpgradeItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IUpgradable
{
    private FortressBuildings fortressBuildings;
    private ResourcesManager resourcesManager;
    private ResourceBuildingUI currentBuilding;
    private RBUpgradeSO currentUpgrade;

    private List<Cost> price = new List<Cost>();

    [SerializeField] private Image border;
    [SerializeField] private TMP_Text caption;
    [SerializeField] private Image icon;
    [SerializeField] private GameObject descriptionBG;
    [SerializeField] private TMP_Text description;
    [SerializeField] private GameObject status;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private GameObject confirmBlock;
    [SerializeField] Color warningColor;
    [SerializeField] Color normalColor;

    [Inject]
    public void Construct(ResourcesManager resourcesManager, FortressBuildings allBuildings)
    {
        this.resourcesManager = resourcesManager;
        this.fortressBuildings = allBuildings;
    }

    public void Init(ResourceBuildingUI building, RBUpgradeSO upgrade, bool activeMode) 
    {
        currentBuilding = building;
        currentUpgrade = upgrade;

        border.enabled = !activeMode;

        caption.text = upgrade.upgradeName;
        icon.sprite = (activeMode == true) ? upgrade.activeIcon : upgrade.inActiveIcon;

        descriptionBG.SetActive(false);
        description.text = upgrade.description.Replace("$V", upgrade.value.ToString());

        status.SetActive(activeMode);
        upgradeButton.gameObject.SetActive(!activeMode);

        price = CheckPriceDiscount(upgrade.cost);
        confirmBlock.SetActive(false);

        var colors = upgradeButton.colors;
        colors.normalColor = (currentBuilding.CheckSiegeStatus() == false && CanIUpgrade() == true) ? normalColor : warningColor;
        colors.selectedColor = (currentBuilding.CheckSiegeStatus() == false && CanIUpgrade() == true) ? normalColor : warningColor;
        upgradeButton.colors = colors;

        ItemOff(true);
    }

    public void ItemOff(bool showMode)
    {
        gameObject.SetActive(showMode);
    }

    private List<Cost> CheckPriceDiscount(List<Cost> oldCost)
    {
        List<Cost> newPrice = new List<Cost>();
        float discount = fortressBuildings.GetBonusAmount(CastleBuildingsBonuses.BuildingDiscount);

        if(discount != 0)
        {
            for(int i = 0; i < oldCost.Count; i++)
            {
                Cost cost = new Cost();
                cost.type = oldCost[i].type;
                cost.amount = oldCost[i].amount * (1 + discount);

                newPrice.Add(cost);
            }
        }
        else
        {
            return oldCost;
        }        

        return newPrice;
    }

    private bool CanIUpgrade()
    {
        for(int i = 0; i < price.Count; i++)
        {
            if(resourcesManager.CheckMinResource(price[i].type, price[i].amount) == false)
                return false;
        }

        return true;
    }

    //Button
    public void TryToBuild()
    {
        if(currentBuilding.CheckSiegeStatus() == true)
        {
            InfotipManager.ShowWarning("You can't upgrade this building during the siege!");
            return;
        }

        if(CanIUpgrade() == false)
        {
            InfotipManager.ShowWarning("You need to dig up resources.");
            return;
        }
        else
        {
            ShowConfirm();
        }
    }

    public void ShowConfirm()
    {
        confirmBlock.SetActive(true);
    }

    //Button
    public void CloseConfirm()
    {
        confirmBlock.SetActive(false);
    }

    //Button
    public void Upgrade()
    {
        Pay();
        CloseConfirm();
        currentBuilding.Upgrade(currentUpgrade);
    }

    private void Pay()
    {
        for(int i = 0; i < price.Count; i++)
        {
            resourcesManager.ChangeResource(price[i].type, -price[i].amount);
        }
    }

    public BuildingsRequirements GetRequirements()
    {
        BuildingsRequirements requirements = new BuildingsRequirements();
        requirements.isCostForCastle = false;
        requirements.costs = price;

        return requirements;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowDescription(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ShowDescription(false);
    }

    private void ShowDescription(bool showMode)
    {
        descriptionBG.gameObject.SetActive(showMode);
    }

}
