using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class FBuilding : MonoBehaviour, IPointerClickHandler
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

    [Header("UI Elements")]
    [SerializeField] private Button buildingButton;
    [SerializeField] private Image buildingsIcon;
    [SerializeField] private GameObject levelBlock;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TooltipTrigger description;
    [SerializeField] private InfotipTrigger costDescription;

    [Header("BuildProcess")]
    [SerializeField] private GameObject processBlock;
    [SerializeField] private Image processScale;
    [SerializeField] private TMP_Text processText;

    private OpeningBuildingWindow door;
    private FortressBuildings allBuildings;
    private ResourcesManager resourcesManager;

    private void Start()
    { 
        door = GlobalStorage.instance.fortressBuildingDoor;
        resourcesManager = GlobalStorage.instance.resourcesManager;
    }

    private void OnEnable()
    {
        Init();
    }

    private void Init()
    {
        if(allBuildings == null) allBuildings = GlobalStorage.instance.fortressBuildings;
        //we don't care about level, we need common info
        currentBonus = allBuildings.GetBuildingBonus(building, 1);

        upgradeButton.gameObject.SetActive(true);

        buildingName = currentBonus.buildingName;
        buildingDescr = currentBonus.BuildingDescription;
        description.header = currentBonus.buildingName;
        description.content = currentBonus.BuildingDescription;

        if(level == 0)
        {
            buildingButton.interactable = false;
            buildingsIcon.sprite = currentBonus.inActiveIcon;
            levelBlock.SetActive(false);
        }
        else
        {
            buildingButton.interactable = true;
            buildingsIcon.sprite = currentBonus.activeIcon;
            levelBlock.SetActive(true);
            levelText.text = LevelConverter();
            if(level >= allBuildings.GetMaxLevel()) 
                upgradeButton.gameObject.SetActive(false);
        }
    }

    public List<Cost> GetCost()
    {
        return allBuildings.GetBuildingCost(building);
    }

    private string LevelConverter()
    {
        string levelText = "";

        if(level == 1) levelText = "I";
        if(level == 2) levelText = "II";
        if(level == 3) levelText = "III";

        return levelText;
    }
    
    public void StartToBuild()
    {
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

        constructionTime = allBuildings.StartBuildingBuilding(building);
        Pay();
        StartBuildingProcess();        
    }

    private void StartBuildingProcess()
    {
        processBlock.SetActive(true);
        processScale.fillAmount = 0;
        processText.text = constructionTime.daysLeft.ToString();

        upgradeButton.gameObject.SetActive(false);
    }

    public void UpdateBuildingProcess(ConstructionTime term)
    {
        processScale.fillAmount = (float)(term.term - term.daysLeft) / (float)term.term;

        processText.text = term.daysLeft.ToString();

        if(term.daysLeft <= 0)
        {
            processBlock.SetActive(false);
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
        if(level > 0) level--;

        allBuildings.UpgradeBuilding(building, level, buildingBonus);
        Init();
    }

    private bool CanIBuild()
    {
        List<Cost> prices = GetCost();

        for(int i = 0; i < prices.Count; i++)
        {
            if(resourcesManager.CheckMinResource(prices[i].type, prices[i].amount) == false)
                return false;
        }

        return true;
    }

    private void Pay()
    {
        List<Cost> prices = GetCost();

        for(int i = 0; i < prices.Count; i++)
        {
            resourcesManager.ChangeResource(prices[i].type, -prices[i].amount);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(level > 0)
        {
            allBuildings.ShowAllBuildings(false);
            door.Open(this);
        }
        else
            InfotipManager.ShowMessage("This building has not yet been built.");
    }    
}
