using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class FBuilding : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IUpgradable
{
    [Header("Parameters")]
    public CastleBuildings building;
    public CastleBuildingsBonuses buildingBonus;
    public bool isSpecialBuilding = true;
    public bool isMilitarySource = false;
    [HideInInspector] public int level = 0;
    [HideInInspector] public string buildingName;
    [HideInInspector] public string buildingDescr;
    private FortressUpgradeSO currentBonus;
    private ConstructionTime constructionTime;
    private bool canIBuild = true;

    [Header("UI Elements")]
    [SerializeField] private GameObject borderBlock;
    [SerializeField] private Image buildingsBG;
    [SerializeField] private Image buildingsIcon;
    [SerializeField] private GameObject statusBuild;
    [SerializeField] private GameObject statusUpgrade;
    [SerializeField] private Button buildingButton;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private UpgradeButton upgradeButtonC;
    [SerializeField] private TMP_Text caption;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private InfotipTrigger costDescription;
    [SerializeField] private string zeroStatus = "Not built";

    [Header("ConfirmBlock")]
    [SerializeField] private GameObject confirmBlock;

    [Header("Warnings")]
    [SerializeField] private GameObject levelBlock;
    [SerializeField] private GameObject warningBlock;
    [SerializeField] private GameObject warningLevel;
    [SerializeField] private GameObject warningQueue;
    [SerializeField] private GameObject warningCost;
    [SerializeField] private GameObject warningSiege;
    [SerializeField] private Color warningColor;
    [SerializeField] private Color normalColor;

    [Header("BuildProcess")]
    [SerializeField] private GameObject processBlock;
    [SerializeField] private Image processScale;
    [SerializeField] private TMP_Text processText;
    [SerializeField] private TMP_Text levelFromText;
    [SerializeField] private TMP_Text levelToText;

    private OpeningBuildingWindow door;
    private FortressBuildings allBuildings;
    private ResourcesManager resourcesManager;

    private void Start()
    { 
        door = GlobalStorage.instance.fortressBuildingDoor;
    }

    private void OnEnable()
    {
        Init();
    }

    private void Init()
    {
        if(allBuildings == null)
        {
            allBuildings = GlobalStorage.instance.fortressBuildings;
            resourcesManager = GlobalStorage.instance.resourcesManager;
            door = GlobalStorage.instance.fortressBuildingDoor;
        }
        //we don't care about level, we need common info
        currentBonus = allBuildings.GetBuildingBonus(building, 1);

        upgradeButton.gameObject.SetActive(true);

        buildingName = currentBonus.buildingName;
        caption.text = buildingName;

        buildingDescr = currentBonus.buildingDescription;

        if(allBuildings.GetConstructionStatus(building) == true)
        {
            processBlock.SetActive(true);
            levelBlock.SetActive(false);
            warningBlock.SetActive(false);
            upgradeButton.gameObject.SetActive(false);
        }
        else
        {
            processBlock.SetActive(false);

            levelBlock.SetActive(true);
            levelText.text = (level == 0) ? zeroStatus : ("Level " + level);
            warningBlock.SetActive(true);
            CheckRequirements();

            if(level == 0)
            {
                buildingsBG.color = new Color(buildingsBG.color.r, buildingsBG.color.g, buildingsBG.color.b, 0.9f);
                buildingsIcon.sprite = currentBonus.inActiveIcon;
                levelText.color = warningColor;
                borderBlock.SetActive(false);
                statusBuild.SetActive(true);
                statusUpgrade.SetActive(false);
            }
            else
            {
                levelText.color = Color.white;
                buildingsBG.color = new Color(buildingsBG.color.r, buildingsBG.color.g, buildingsBG.color.b, 1f);
                buildingsIcon.sprite = currentBonus.activeIcon;
                borderBlock.SetActive(true);
                statusBuild.SetActive(false);
                statusUpgrade.SetActive(true);

                if(level >= allBuildings.GetMaxLevel())
                {
                    upgradeButton.gameObject.SetActive(false);
                }
            }
        }      
    }

    public BuildingsRequirements GetRequirements()
    {
        BuildingsRequirements requirements = new BuildingsRequirements();
        requirements.costs = allBuildings.GetBuildingCost(building);
        requirements.fortressLevel = allBuildings.GetRequiredLevel(building);
        requirements.canIBuild = canIBuild;

        return requirements;
    }
    
    public void CheckRequirements()
    {
        BuildingsRequirements requirements = GetRequirements();

        bool costFlag = true;
        for(int i = 0; i < requirements.costs.Count; i++)
        {
            if(resourcesManager.CheckMinResource(requirements.costs[i].type, requirements.costs[i].amount) == false)
            {
                costFlag = false;
                break;
            }            
        }

        warningCost.SetActive(!costFlag);

        bool levelFlag = requirements.canIBuild;
        warningLevel.SetActive(!levelFlag);

        bool queueFlag = allBuildings.CanIBuild();
        warningQueue.SetActive(!queueFlag);

        bool siegeFlag = allBuildings.GetSiegeStatus();
        warningSiege.SetActive(siegeFlag);

        bool permission = true;
        if(costFlag == false || levelFlag == false || queueFlag == false || siegeFlag == true)
        {
            permission = false;
        }

        var colors = upgradeButton.colors;
        colors.normalColor = (permission == true) ? normalColor : warningColor;
        colors.selectedColor = (permission == true) ? normalColor : warningColor;
        upgradeButton.colors = colors;
    }

    public void TryToBuild()
    {
        if(allBuildings.GetSiegeStatus() == true)
        {
            InfotipManager.ShowWarning("You can't build anything while your Castle is under siege.");
            return;
        }

        if(allBuildings.CheckNeededLevel(building) == false)
        {
            InfotipManager.ShowWarning("First you need to increase the level of hero's fortress.");
            return;
        }

        if(allBuildings.CanIBuild() == false)
        {
            InfotipManager.ShowWarning("There are no free slots for building. Wait for the construction of current buildings to finish.");
            return;
        }

        if(CanIBuild() == false)
        {
            InfotipManager.ShowWarning("You need to dig up resources.");
            return;
        }

        ShowConfirm();   
    }

    //Button
    public void Confirm()
    {
        constructionTime = allBuildings.StartBuildingBuilding(building);
        Pay();
        StartBuildingProcess();
        CloseConfirm();
    }

    private void StartBuildingProcess()
    {
        processBlock.SetActive(true);
        processScale.fillAmount = 0;
        processText.text = constructionTime.daysLeft.ToString();

        levelBlock.SetActive(false);
        levelFromText.text = level.ToString();
        levelToText.text = (level + 1).ToString();

        warningBlock.SetActive(false);

        upgradeButton.gameObject.SetActive(false);

        allBuildings.UpdateBuildingsStatus();
    }

    public void UpdateBuildingProcess(ConstructionTime term)
    {
        processScale.fillAmount = (float)(term.term - term.daysLeft) / (float)term.term;

        processText.text = term.daysLeft.ToString();

        if(term.daysLeft <= 0)
        {
            Upgrade();
        }
    }

    public void Upgrade()
    {
        if(level < allBuildings.GetMaxLevel()) level++;

        allBuildings.UpgradeBuilding(building, level, buildingBonus);
        if(level == 1) allBuildings.RegisterBuilding(building, this);

        Init();
    }

    public void Downgrade()
    {
        if(level > 0) 
            level--;

        allBuildings.UpgradeBuilding(building, level, buildingBonus);
        Init();
    }

    public void UpdateStatus(bool canIBeBuilt)
    {
        canIBuild = canIBeBuilt;
    }

    private bool CanIBuild()
    {
        List<Cost> prices = allBuildings.GetBuildingCost(building);

        for(int i = 0; i < prices.Count; i++)
        {
            if(resourcesManager.CheckMinResource(prices[i].type, prices[i].amount) == false)
                return false;
        }

        return true;
    }

    private void Pay()
    {
        List<Cost> prices = allBuildings.GetBuildingCost(building);

        for(int i = 0; i < prices.Count; i++)
        {
            resourcesManager.ChangeResource(prices[i].type, -prices[i].amount);
        }
    }

    public void ShowConfirm()
    {
        allBuildings.CloseAnotherConfirm();
        confirmBlock.SetActive(true);
    }

    public void CloseConfirm()
    {
        confirmBlock.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(level > 0)
        {
            if(allBuildings.GetSiegeStatus() == true)
            {
                InfotipManager.ShowWarning("You can't use any buildings while your Castle is under siege.");
                return;
            }

            allBuildings.ShowAllBuildings(false);
            door.Open(this);
        }
        else
            InfotipManager.ShowMessage("This building has not yet been built.");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        allBuildings.ShowDescription(building);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        allBuildings.CloseDescription();
    }
}
