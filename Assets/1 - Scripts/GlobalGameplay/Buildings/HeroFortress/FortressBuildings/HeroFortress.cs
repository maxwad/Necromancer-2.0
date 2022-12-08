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

    [SerializeField] private GameObject uiPanel;
    private OpeningBuildingWindow door;
    private Garrison garrison;

    private int marketDays = 0;
    private int maxUnitLevel = 3;
    private int levelUpMultiplier = 10;

    private bool isHeroInside = false;

    private void Start()
    {
        gmInterface = GlobalStorage.instance.gmInterface;
        canvas = uiPanel.GetComponent<CanvasGroup>();
        door = GlobalStorage.instance.fortressBuildingDoor;
        garrison = GetComponent<Garrison>();
        //TEMPER
        Close();
    }

    #region HELPERS

    public void Open(bool openByClick, ClickableObject building)
    {
        gmInterface.ShowInterfaceElements(false);

        MenuManager.instance.MiniPause(true);
        GlobalStorage.instance.ModalWindowOpen(true);

        uiPanel.SetActive(true);
        door.Close();

        isHeroInside = !openByClick;
        garrison.Init(isHeroInside);

        Fading.instance.FadeWhilePause(true, canvas);
    }

    public void Close()
    {
        gmInterface.ShowInterfaceElements(true);
        
        MenuManager.instance?.MiniPause(false);
        GlobalStorage.instance.ModalWindowOpen(false);

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

    #endregion

    private void OnEnable()
    {
        EventManager.NewMove += NewDay;
    }

    private void OnDisable()
    {
        EventManager.NewMove -= NewDay;
    }
}
