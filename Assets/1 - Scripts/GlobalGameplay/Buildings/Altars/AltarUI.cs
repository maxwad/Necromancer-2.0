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
    private ResourcesManager resourcesManager;
    private GMInterface gmInterface;
    private CanvasGroup canvas;
    [HideInInspector] public Altar currentAltar;
    private AltarMiniGame gameScript;

    [SerializeField] private GameObject uiPanel;
    private float maxTry;
    private bool isResourceDeficit = false;
    private bool isMiniGameStarted = false;

    [Header("Settings")]
    [SerializeField] private GameObject settingsBlock;
    [SerializeField] private Toggle healingAll;
    [SerializeField] private TMP_Text tryCaption;
    [SerializeField] private GameObject confirmBlock;

    private Dictionary<Unit, int> units;
    private Dictionary<ResourceType, float> currentCost;
    private Dictionary<ResourceType, Sprite> resourcesIcons;

    [Header("Lists")]
    [SerializeField] private List<AltarResourceButton> resourcesPortions;
    [SerializeField] private List<AltarUnitSlotUI> unitsSlots;



    [SerializeField] private Color rigthColor;
    [SerializeField] private Color warningColor;

    private void Start()
    {
        playerStats      = GlobalStorage.instance.playerStats;
        resourcesManager = GlobalStorage.instance.resourcesManager;
        resourcesIcons   = resourcesManager.GetAllResourcesIcons();
        gmInterface      = GlobalStorage.instance.gmInterface;
        canvas           = uiPanel.GetComponent<CanvasGroup>();
        gameScript       = GetComponent<AltarMiniGame>();
    }
    
    private void Init()
    {
        settingsBlock.SetActive(true);
        healingAll.isOn = true;
        isResourceDeficit = false;

        maxTry = playerStats.GetCurrentParameter(PlayersStats.MedicTry);
        tryCaption.text = "Price per try (Total: " + maxTry + ")";

        FillUnits();

        currentCost = currentAltar.CalculatePrice();
        FillResourceButton(resourcesPortions, currentCost);
    }

    #region SETTINGS

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

    public void FillResourceButton(List<AltarResourceButton> resourcesList, Dictionary<ResourceType, float> costs, bool chooseMode = false)
    {
        if(resourcesList.Count != costs.Count)
        {
            Debug.Log("PROBLEM WITH COSTS! Buttons: " +  resourcesList.Count + " Res: " + costs.Count);
            return;
        }

        isResourceDeficit = false;

        List<ResourceType> resources = new List<ResourceType>(costs.Keys);
        for(int i = 0; i < resourcesList.Count; i++)
        {
            ResourceGiftData data = new ResourceGiftData();
            data.index = i;
            data.resourceType = resources[i];
            data.resourceIcon = resourcesIcons[resources[i]];
            data.amount = costs[resources[i]];
            data.amountTotalTry = costs[resources[i]] * maxTry;

            if(resourcesManager.CheckMinResource(data.resourceType, data.amountTotalTry) == false)
            {
                isResourceDeficit = true;
                data.tryColor = warningColor;
            }

            if(resourcesManager.CheckMinResource(data.resourceType, data.amount) == false)
            {
                data.amountColor = warningColor;
            }  

            if(chooseMode == true)
            {
                data.isActiveBtn = chooseMode;
                data.amountInStore = resourcesManager.GetResource(resources[i]);
                data.amountColor = (data.amountColor == warningColor) ? warningColor : Color.black;
                data.miniGame = gameScript;
            }

            resourcesList[i].Init(data);
        }
    }

    public void ChangeUnitsAmount(UnitsTypes unit, float newAmount)
    {
        currentAltar.ChangeUnitsAmount(unit, newAmount);

        currentCost = currentAltar.CalculatePrice();
        FillResourceButton(resourcesPortions, currentCost);
    }

    #endregion

    #region GIFTGAME





    #endregion

    #region BUTTONS

    //Toggle
    public void SwitchHealAll()
    {
        foreach(var unit in unitsSlots)
            unit.ShowSlider(!healingAll.isOn);

        if(healingAll.isOn == true)
        {
            FillUnits();

            currentCost = currentAltar.CalculatePrice();
            FillResourceButton(resourcesPortions, currentCost);
        }
    }

    //Button
    public void TryToGoToTheGame()
    {
        if(isResourceDeficit == true)
        {
            confirmBlock.SetActive(true);
        }
        else
        {
            GoToTheGame();
        }
    }

    //Button
    public void GoToTheGame()
    {
        CloseConfirm();
        settingsBlock.SetActive(false);
        isMiniGameStarted = true;

        List<ResourceType> combination = currentAltar.GenerateCombitation();

        gameScript.StartGame(this, combination);
    }

    //Button
    public void CloseConfirm()
    {
        confirmBlock.SetActive(false);
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
        //ASK CONFIRM
        isMiniGameStarted = false;
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
