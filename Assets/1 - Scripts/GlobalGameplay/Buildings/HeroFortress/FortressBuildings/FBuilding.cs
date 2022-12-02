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
    public bool isSpecialBuilding = true;
    [HideInInspector] public int level = 0;

    [Header("UI Elements")]
    [SerializeField] private Button buildingButton;
    [SerializeField] private Image buildingsIcon;
    [SerializeField] private GameObject levelBlock;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TooltipTrigger description;
    [SerializeField] private InfotipTrigger costDescription;

    private OpeningBuildingWindow door;
    private FortressBuildings allBuildings;
    private ResourcesManager resourcesManager;
    private HeroFortress fortress;

    private void Start()
    { 
        door = GlobalStorage.instance.fortressBuildingDoor;
        resourcesManager = GlobalStorage.instance.resourcesManager;
        fortress = GlobalStorage.instance.heroFortress;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Delete))
        {
            Downgrade();
        }
    }

    private void OnEnable()
    {
        Init();
    }

    private void Init()
    {
        if(allBuildings == null) allBuildings = GlobalStorage.instance.fortressBuildings;
        //we don't care about level, we need common info
        FortressUpgradeSO bonus = allBuildings.GetBuildingBonus(building, 1);

        upgradeButton.gameObject.SetActive(true);

        description.header = bonus.buildingName;
        description.content = bonus.BuildingDescription;

        if(level == 0)
        {
            buildingButton.interactable = false;
            buildingsIcon.sprite = bonus.inActiveIcon;
            levelBlock.SetActive(false);
        }
        else
        {
            buildingButton.interactable = true;
            buildingsIcon.sprite = bonus.activeIcon;
            levelBlock.SetActive(true);
            levelText.text = LevelConverter();
            if(level >= allBuildings.GetMaxLevel()) upgradeButton.gameObject.SetActive(false);
        }
    }

    public List<Cost> GetCost()
    {
        return allBuildings.GetCost(building);
    }

    private string LevelConverter()
    {
        string levelText = "";

        if(level == 1) levelText = "I";
        if(level == 2) levelText = "II";
        if(level == 3) levelText = "III";

        return levelText;
    }
    
    public void Upgrade()
    {
        if(CanIBuild() == true)
        {
            //Build();

            if(level < allBuildings.GetMaxLevel()) level++;

            allBuildings.UpgradeBuilding(building, level);
            if(level == 1) allBuildings.RegisterBuilding(building, this);

            Init();
        }
        else
        {
            InfotipManager.ShowWarning("You need to dig up resources.");
        }
    }

    public void Downgrade()
    {
        if(level > 0) level--;

        allBuildings.UpgradeBuilding(building, level);
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

    private void Build()
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
            fortress.ShowAllBuildings(false);
            door.Open(this);
        }
        else
            InfotipManager.ShowMessage("This building has not yet been built.");
    }    
}
