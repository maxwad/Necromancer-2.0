using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class AltarUI : MonoBehaviour
{
    private PlayerStats playerStats;
    private GMInterface gmInterface;
    private CanvasGroup canvas;
    private Altar currentAltar; 

    [SerializeField] private GameObject uiPanel;
    private float maxTry;

    [Header("Settings")]
    [SerializeField] private GameObject settingsBlock;
    [SerializeField] private Toggle healingAll;
    [SerializeField] private TMP_Text tryCaption;

    private Dictionary<Unit, int> units;
    private Dictionary<ResourceType, float> currentCost;

    [Header("Lists")]
    [SerializeField] private List<AltarResourceButton> resourcesPortions;
    [SerializeField] private List<AltarUnitSlotUI> unitsSlots;



    [SerializeField] private Color rigthColor;

    private void Start()
    {
        playerStats = GlobalStorage.instance.playerStats;
        gmInterface = GlobalStorage.instance.gmInterface;
        canvas = uiPanel.GetComponent<CanvasGroup>();
    }
    
    private void Init()
    {
        settingsBlock.SetActive(true);
        healingAll.isOn = true;

        maxTry = playerStats.GetCurrentParameter(PlayersStats.MedicTry);
        tryCaption.text = "Price per try (Total: " + maxTry + ")";

        FillUnits();

        currentCost = currentAltar.GetPrice();
        FillResourceButton(resourcesPortions, currentCost);















    }

    private void FillUnits()
    {
        ResetUnitSlots();

        units = currentAltar.GetUnits();
        int index = 0;
        foreach(var unit in units)
        {
            unitsSlots[index].Init(this, unit.Key, unit.Value);
            index++;
        }
    }

    private void FillResourceButton(List<AltarResourceButton> resourcesList, Dictionary<ResourceType, float> costs)
    {
        if(resourcesList.Count != costs.Count)
        {
            Debug.Log("PROBLEM WITH COSTS! Buttons: " +  resourcesList.Count + " Res: " + costs.Count);
            return;
        }

        List<ResourceType> resources = new List<ResourceType>(costs.Keys);
        for(int i = 0; i < resourcesList.Count; i++)
        {
            ResourceGiftData data = new ResourceGiftData();
            data.resourceType = resources[i];
            data.amount = costs[resources[i]];
            data.amountTotalTry = costs[resources[i]] * maxTry;

            resourcesList[i].Init(data);
        }
    }


    public void ChangeUnitsAmount(UnitsTypes unit, float newAmount)
    {
        currentAltar.ChangeUnitsAmount(unit, newAmount);

        currentCost = currentAltar.GetPrice();
        FillResourceButton(resourcesPortions, currentCost);
    }

    #region BUTTONS

    //Toggle
    public void SwitchHealAll()
    {
        foreach(var unit in unitsSlots)
            unit.ShowSlider(!healingAll.isOn);

        if(healingAll.isOn == true)
        {
            FillUnits();

            currentCost = currentAltar.GetPrice();
            FillResourceButton(resourcesPortions, currentCost);
        }
    }

    #endregion

    #region HELPERS

    internal void Open(bool modeClick, Altar altar)
    {
        currentAltar = altar;

        CheckRequirements(modeClick);

        gmInterface.ShowInterfaceElements(false);

        MenuManager.instance.MiniPause(true);
        GlobalStorage.instance.ModalWindowOpen(true);

        uiPanel.SetActive(true);
        Init();

        Fading.instance.FadeWhilePause(true, canvas);
    }

    public void Close()
    {
        gmInterface.ShowInterfaceElements(true);

        MenuManager.instance?.MiniPause(false);
        GlobalStorage.instance.ModalWindowOpen(false);

        uiPanel.SetActive(false);
    }

    private bool CheckRequirements(bool modeClick)
    {
        //if(playerStats.GetCurrentParameter(PlayersStats.MedicAltar) < 1)
        //{
        //    InfotipManager.ShowWarning("To use Altars, you need to unlock the corresponding skill!");
        //    return false;
        //}

        //if(modeClick == true)
        //{
        //    InfotipManager.ShowWarning("Gifts for healing are accepted only in person.");
        //    return false;
        //}

        return currentAltar.CheckInjured();
    }

    private void ResetUnitSlots()
    {
        foreach(var unit in unitsSlots)
            unit.gameObject.SetActive(false);
    }
    #endregion
}
