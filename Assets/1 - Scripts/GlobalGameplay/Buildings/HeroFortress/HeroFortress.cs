using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Enums;

public partial class HeroFortress : MonoBehaviour, IInputableKeys
{
    private InputSystem inputSystem;
    private CanvasGroup canvas;
    private bool isWindowOpen = false;

    [SerializeField] private GameObject uiPanel;
    private OpeningBuildingWindow fortressBuildingDoor;
    private FortressBuildings buildings;
    private UnitCenter unitCenter;
    private ResourceBuildingUI resourceBuildingUI;
    private ResourceBuilding resourceBuilding;

    private GMPlayerMovement globalPlayer;
    private ResourcesManager resourcesManager;

    private int marketDays = 0;
    private int maxUnitLevel = 3;
    private int levelUpMultiplier = 10;
    private int seals = 0;

    private bool isHeroInside = false;
    private bool isHeroVisitedOnThisWeek = false;

    [Inject]
    public void Construct
        (
        [Inject(Id = Constants.FORTRESS)]
        GameObject fortressGO,
        InputSystem inputSystem,
        OpeningBuildingWindow fortressBuildingDoor,
        GMPlayerMovement globalPlayer,
        ResourcesManager resourcesManager
        )
    {
        this.inputSystem          = inputSystem;
        this.fortressBuildingDoor = fortressBuildingDoor;
        this.globalPlayer         = globalPlayer;
        this.resourcesManager     = resourcesManager;

        canvas             = uiPanel.GetComponent<CanvasGroup>();
        buildings          = GetComponent<FortressBuildings>();
        unitCenter         = GetComponentInChildren<UnitCenter>(true);
        resourceBuildingUI = GetComponentInChildren<ResourceBuildingUI>(true);
        resourceBuilding   = fortressGO.GetComponent<ResourceBuilding>();
    }

    private void Start()
    {
        RegisterInputKeys();
    }

    public void RegisterInputKeys()
    {
        inputSystem.RegisterInputKeys(KeyActions.Castle, this);
    }

    public void InputHandling(KeyActions keyAction)
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

    #region HELPERS

    public void Open(bool openByClick)
    {
        MenuManager.instance.MiniPause(true);

        uiPanel.SetActive(true);
        isWindowOpen = true;
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
        fortressBuildingDoor.Close();
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

    public int GetMaxLevel() => maxUnitLevel;

    public int GetLevelUpMultiplier() => levelUpMultiplier;

    public int GetSealsAmount() => seals;

    public ResourceBuilding GetCastleMint() => resourceBuilding;

    public void AddSeals() => seals++;

    private void ActivateBonusesForHero()
    {
        float bonusAmount = buildings.GetBonusAmount(CastleBuildingsBonuses.ShelterBonus);

        if(bonusAmount > 0)
            globalPlayer.ChangeMovementPoints(100);

        if(bonusAmount > 1)
            resourcesManager.ChangeResource(ResourceType.Health, 1000);

        if(bonusAmount > 2)
            resourcesManager.ChangeResource(ResourceType.Mana, 1000);
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
