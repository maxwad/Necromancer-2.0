using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class HeroFortress : MonoBehaviour
{
    //private GMInterface gmInterface;
    private CanvasGroup canvas;
    private bool isWindowOpen = false;

    [SerializeField] private GameObject uiPanel;
    private OpeningBuildingWindow door;
    private FortressBuildings buildings;
    private UnitCenter unitCenter;
    private ResourceBuildingUI resourceBuildingUI;
    private ResourceBuilding resourceBuilding;

    private GMPlayerMovement gmPlayerMovement;
    private ResourcesManager resourcesManager;

    private int marketDays = 0;
    private int maxUnitLevel = 3;
    private int levelUpMultiplier = 10;
    private int seals = 0;

    private bool isHeroInside = false;
    private bool isHeroVisitedOnThisWeek = false;

    private void Awake()
    {
        //gmInterface = GlobalStorage.instance.gmInterface;
        canvas = uiPanel.GetComponent<CanvasGroup>();
        door = GlobalStorage.instance.fortressBuildingDoor;
        buildings = GetComponent<FortressBuildings>();
        unitCenter = GetComponentInChildren<UnitCenter>(true);
        resourceBuildingUI = GetComponentInChildren<ResourceBuildingUI>(true);
        resourceBuilding = GetComponent<ResourceBuilding>();

        gmPlayerMovement = GlobalStorage.instance.globalPlayer;
        resourcesManager = GlobalStorage.instance.resourcesManager;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            if(isWindowOpen == false)
            {
                if(MenuManager.instance.isGamePaused == false && GlobalStorage.instance.isModalWindowOpen == false)
                {
                    Open(true);
                }
            }
            else
            {
                Close();
            }
        }
    }
    #region HELPERS

    public void Open(bool openByClick)
    {
        MenuManager.instance.MiniPause(true);

        uiPanel.SetActive(true);
        isWindowOpen = true;
        //door.Close();
        buildings.CloseDescription();
        buildings.CloseAnotherConfirm();

        isHeroInside = !openByClick;
        resourceBuildingUI.Open(openByClick, resourceBuilding);

        if(isHeroInside == true && isHeroVisitedOnThisWeek == false)
        {
            isHeroVisitedOnThisWeek = true;
            ActivateBonusesForHero();
        }

        Fading.instance.FadeWhilePause(true, canvas);
    }

    public void Close()
    {      
        MenuManager.instance?.MiniPause(false);
        isWindowOpen = false;
        door.Close();
        uiPanel.SetActive(false);
    }

    #endregion

    #region BUILDINGS FUNCTION

    private void MarketDay()
    {
        marketDays++;
    }

    public int GetMarketPause()
    {
        int result = marketDays;
        marketDays = 0;

        return result;
    }

    public int GetMaxLevel()
    {
        return maxUnitLevel;
    }

    public int GetLevelUpMultiplier()
    {
        return levelUpMultiplier;
    }

    public int GetSealsAmount()
    {
        return seals;
    }

    public void AddSeals()
    {
        seals++;
    }

    private void ActivateBonusesForHero()
    {
        float bonusAmount = buildings.GetBonusAmount(CastleBuildingsBonuses.ShelterBonus);

        if(bonusAmount > 0)
        {
            gmPlayerMovement.ChangeMovementPoints(100);
        }

        if(bonusAmount > 1)
        {
            resourcesManager.ChangeResource(ResourceType.Health, 1000);
        }

        if(bonusAmount > 2)
        {
            resourcesManager.ChangeResource(ResourceType.Mana, 1000);
        }
    }

    #endregion

    private void UpdateVisit(int counter)
    {
        isHeroVisitedOnThisWeek = false;
    }

    private void HiringInGarrison()
    {
        unitCenter.HiringInGarrison();
    }

    public void ChangePotentialUnitsAmount(UnitsTypes unitType, int amount)
    {
        unitCenter.ChangePotentialUnitsAmount(unitType, amount);
    }

    public void SetStartGrowths(List<HiringAmount> startGrowthAmounts)
    {
        unitCenter.SetStartGrowths(startGrowthAmounts);
    }

    public void BuildStartBuildings(CastleBuildings building, int level)
    {
        for(int i = 0; i < level; i++)
        {
            buildings.BuildStartBuilding(building);
        }
    }

    private void OnEnable()
    {
        EventManager.NewMove += MarketDay;
        EventManager.NewWeek += UpdateVisit;
        EventManager.WeekEnd += HiringInGarrison;
    }

    private void OnDisable()
    {
        EventManager.NewMove -= MarketDay;
        EventManager.NewWeek -= UpdateVisit;
        EventManager.WeekEnd -= HiringInGarrison;
    }
}
