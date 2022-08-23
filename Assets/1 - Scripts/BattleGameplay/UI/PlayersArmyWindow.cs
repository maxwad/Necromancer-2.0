using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;
using System;

public class PlayersArmyWindow : MonoBehaviour
{
    [SerializeField] private GameObject playerArmyUI;

    [Header("Army")]
    [SerializeField] private ArmySlot[] armySlots;

    [Header("Reserve")]
    [SerializeField] private ArmySlot[] reserveSlots;
    [SerializeField] private GameObject reserveVeil;

    [Header("Infirmary")]
    [SerializeField] private InfirmarySlot[] infirmarySlots;
    [SerializeField] private TMP_Text infirmaryCount;


    [Header("Buttons")]
    [SerializeField] private Button fightButton;
    [SerializeField] private Button closeWindow;
    [SerializeField] private TMP_Text fightButtonText;
    private string toTheBattleText = "Fight!";
    private string toTheGlobalText = "Escape...";

    [HideInInspector] public bool isWindowOpened = false;

    #region Schemes
    public void CreateReserveScheme(Unit[] army)
    {
        for(int i = 0; i < army.Length; i++)
        {
            reserveSlots[i].FillTheArmySlot(army[i]);
        }
    }

    public void CreateArmyScheme(Unit[] army)
    {
        for (int i = 0; i < army.Length; i++)
        {
            armySlots[i].FillTheArmySlot(army[i]);
        }
    }

    public void CreateInfirmaryScheme()
    {
        List<UnitsTypes> injuredList = GlobalStorage.instance.infirmaryManager.GetCurrentInjuredList();
        List<Unit> actualUnits = GlobalStorage.instance.unitManager.GetActualArmy();
        float infarmaryCapacity = GlobalStorage.instance.infirmaryManager.GetCurrentCapacity();
        float currentInjuredCount = injuredList.Count;

        infirmaryCount.text = "[" + currentInjuredCount + "/" + infarmaryCapacity + "]";

        for(int i = 0; i < infirmarySlots.Length; i++)
        {
            infirmarySlots[i].ResetSlot();
        }

        int slotIndex = 0;
        foreach(var unit in actualUnits)
        {
            int count = 0;
            foreach(var injuredUnit in injuredList)
            {
                if(injuredUnit == unit.UnitType) count++;
            }

            if(count != 0)
            {
                infirmarySlots[slotIndex].FillTheInfarmarySlot(unit.unitIcon, count);
                slotIndex++;
            }
        }
        
    }

    #endregion

    #region Buttons
    private void ToTheBattle()
    {
        fightButton.onClick.RemoveAllListeners();
        fightButton.onClick.AddListener(ToTheGlobal);
        fightButtonText.text = toTheGlobalText;

        GlobalStorage.instance.battleManager.InitializeBattle();
        //TODO: may be here we need to give info about army future battle to the battle manager
        CloseWindow();
    }

    private void ToTheGlobal()
    {
        fightButton.onClick.RemoveAllListeners();
        fightButton.onClick.AddListener(ToTheBattle);
        fightButtonText.text = toTheBattleText;

        GlobalStorage.instance.battleManager.FinishTheBattle();
        CloseWindow();
    }


    public void OpenWindow()
    {
        if (isWindowOpened == false)
        {
            if(GlobalStorage.instance.isModalWindowOpen == false)
            {
                playerArmyUI.SetActive(true);
                isWindowOpened = true;

                UpdateArmyWindow();

                MenuManager.instance.MiniPauseOn();
                GlobalStorage.instance.isModalWindowOpen = true;
            }            
        }            
        else
            CloseWindow();
    }

    public void CloseWindow()
    {      
        for (int i = 0; i < armySlots.Length; i++)
            armySlots[i].ResetSelecting();

        playerArmyUI.SetActive(false);
        isWindowOpened = false;
        GlobalStorage.instance.isModalWindowOpen = false;
        MenuManager.instance.MiniPauseOff();

        //TODO: here we should clear info about canceled battle
    }

    private void UpdateArmyWindow()
    {
        CreateReserveScheme(GlobalStorage.instance.player.GetComponent<PlayersArmy>().reserveArmy);
        CreateArmyScheme(GlobalStorage.instance.player.GetComponent<PlayersArmy>().playersArmy);
        CreateInfirmaryScheme();

        if(GlobalStorage.instance.isGlobalMode == true)
            reserveVeil.SetActive(false);
        else
            reserveVeil.SetActive(true);
    }

    #endregion

    private void Awake()
    {
        fightButton.onClick.AddListener(ToTheBattle);
        fightButtonText.text = toTheBattleText;
        closeWindow.onClick.AddListener(CloseWindow);
    }

    private void OnDestroy()
    {
        fightButton.onClick.RemoveListener(ToTheBattle);
        closeWindow.onClick.RemoveListener(CloseWindow);
    }
}
