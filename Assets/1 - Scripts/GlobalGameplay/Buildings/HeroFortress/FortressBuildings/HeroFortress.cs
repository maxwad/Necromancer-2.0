using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static NameManager;
using System;

public class HeroFortress : MonoBehaviour
{
    private GMInterface gmInterface;
    private CanvasGroup canvas;
    private bool isWindowOpen = false;

    [SerializeField] private GameObject uiPanel;
    private OpeningBuildingWindow door;
    private Garrison garrison;
    private FortressBuildings buildings;

    private GMPlayerMovement gmPlayerMovement;
    private ResourcesManager resourcesManager;

    private int marketDays = 0;
    private int maxUnitLevel = 3;
    private int levelUpMultiplier = 10;

    private bool isHeroInside = false;
    private bool isHeroVisitedOnThisWeek = false;

    private void Start()
    {
        gmInterface = GlobalStorage.instance.gmInterface;
        canvas = uiPanel.GetComponent<CanvasGroup>();
        door = GlobalStorage.instance.fortressBuildingDoor;
        buildings = GetComponent<FortressBuildings>();
        garrison = GetComponent<Garrison>();

        gmPlayerMovement = GlobalStorage.instance.globalPlayer;
        resourcesManager = GlobalStorage.instance.resourcesManager;
        //TEMPER
        Close();
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
        gmInterface.ShowInterfaceElements(false);

        MenuManager.instance.MiniPause(true);
        GlobalStorage.instance.ModalWindowOpen(true);

        uiPanel.SetActive(true);
        isWindowOpen = true;
        door.Close();
        buildings.CloseDescription();
        buildings.CloseAnotherConfirm();

        isHeroInside = !openByClick;
        garrison.Init(isHeroInside);

        if(isHeroInside == true && isHeroVisitedOnThisWeek == false)
        {
            isHeroVisitedOnThisWeek = true;
            ActivateBonusesForHero();
        }

        Fading.instance.FadeWhilePause(true, canvas);
    }

    public void Close()
    {
        gmInterface.ShowInterfaceElements(true);
        
        MenuManager.instance?.MiniPause(false);
        GlobalStorage.instance.ModalWindowOpen(false);
        isWindowOpen = false;

        uiPanel.SetActive(false);
    }

    #endregion

    #region BUILDINGS FUNCTION

    private void NewDay()
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

    private void OnEnable()
    {
        EventManager.NewMove += NewDay;
        EventManager.NewWeek += UpdateVisit;
    }

    private void OnDisable()
    {
        EventManager.NewMove -= NewDay;
        EventManager.NewWeek -= UpdateVisit;
    }
}
