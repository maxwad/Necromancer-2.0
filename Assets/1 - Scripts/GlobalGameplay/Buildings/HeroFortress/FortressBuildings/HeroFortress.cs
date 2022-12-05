using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static NameManager;
using System;

public class HeroFortress : MonoBehaviour
{
    private OpeningBuildingWindow door;
    private GMInterface gmInterface;
    private CanvasGroup canvas;

    [SerializeField] private TMP_Text fortressLevel;

    [SerializeField] private GameObject uiPanel;
    [SerializeField] private List<GameObject> allBuildings;
    [SerializeField] private List<FBuilding> activeBuildings;

    private int marketDays = 0;
    private int maxUnitLevel = 3;
    private int LevelUpMultiplier = 10;

    private void Start()
    {
        door = GlobalStorage.instance.fortressBuildingDoor;
        gmInterface = GlobalStorage.instance.gmInterface;
        canvas = uiPanel.GetComponent<CanvasGroup>();
        UpgradeFortressLevel(0);
        //TEMPER
        Close();
    }

    #region HELPERS

    public void Open(bool openMode, ClickableObject building)
    {
        gmInterface.ShowInterfaceElements(false);

        MenuManager.instance.MiniPause(true);
        GlobalStorage.instance.ModalWindowOpen(true);

        uiPanel.SetActive(true);
        Fading.instance.FadeWhilePause(true, canvas);
    }

    public void Close()
    {
        gmInterface.ShowInterfaceElements(true);
        
        MenuManager.instance?.MiniPause(false);
        GlobalStorage.instance.ModalWindowOpen(false);

        door.Close();
        uiPanel.SetActive(false);
    }

    public void UpgradeFortressLevel(int level)
    {
        fortressLevel.text = level.ToString();
    }

    public void ShowAllBuildings(bool showMode)
    {
        foreach(var building in allBuildings)
        {
            building.SetActive(showMode);
        }
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
        return LevelUpMultiplier;
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
