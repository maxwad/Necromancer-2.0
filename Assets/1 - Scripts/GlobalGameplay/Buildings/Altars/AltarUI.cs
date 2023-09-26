using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;
using Zenject;

public class AltarUI : MonoBehaviour
{
    private PlayerStats playerStats;
    private ResourcesManager resourcesManager;
    private CanvasGroup canvas;
    [HideInInspector] public Altar currentAltar;
    private AltarMiniGame gameScript;

    [SerializeField] private GameObject uiPanel;
    private float maxTry;
    private bool isResourceDeficit = false;
    private bool isMiniGameStarted = false;
    [SerializeField] private GameObject forceExit;

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

    [Header("Result")]
    [SerializeField] private GameObject resultBlock;
    [SerializeField] private Image resultBlockBG;
    [SerializeField] private TMP_Text resultBlockText;
    [SerializeField] private Color goodResultColor;
    [SerializeField] private Color badResultColor;
    [SerializeField] private Color neutralResultColor;
    [SerializeField] private string goodResultText;
    [SerializeField] private string badResultText;
    [SerializeField] private string neutralResultText;
    [SerializeField] private string stupidResultText;


    [SerializeField] private Color rigthColor;
    [SerializeField] private Color warningColor;

    [Inject]
    public void Construct(PlayerStats playerStats, ResourcesManager resourcesManager)
    {
        this.playerStats = playerStats;
        this.resourcesManager = resourcesManager;

        canvas = uiPanel.GetComponent<CanvasGroup>();
        gameScript = GetComponent<AltarMiniGame>();
    }

    private void Start()
    {
        resourcesIcons   = resourcesManager.GetAllResourcesIcons();
    }
    
    private void Init()
    {
        ShowExitWarning(false);
        CloseResultBlock();
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
                data.isDeficit = isResourceDeficit;
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

        if(currentAltar.IsMinPrice() == true)
        {
            ShowStupidResult();
            currentAltar.VisitRegistration();
            currentAltar.PayAllResources();
            return;
        }

        isMiniGameStarted = true;

        List<ResourceType> combination = currentAltar.GenerateCombitation();

        gameScript.StartGame(this, combination);
    }

    //Button
    public void CloseConfirm()
    {
        confirmBlock.SetActive(false);
    }

    internal void Open(bool modeClick, Altar altar)
    {
        currentAltar = altar;

        if(CheckRequirements(modeClick) == false) return;

        MenuManager.instance.MiniPause(true);
        uiPanel.SetActive(true);
        Init();

        Fading.instance.FadeWhilePause(true, canvas);
    }

    public void GameIsOver(bool visitMode)
    {
        isMiniGameStarted = false;

        if(visitMode == true)
            currentAltar.VisitRegistration();
    }

    public void ShowExitWarning(bool showMode)
    {
        forceExit.SetActive(showMode);
    }

    //Button
    public void ContinueExit()
    {
        isMiniGameStarted = false;
        Close();
    }

    //Button
    public void CancelExit()
    {
        ShowExitWarning(false);
    }

    //Button
    public void Close()
    {
        if(isMiniGameStarted == true)
        {
            ShowExitWarning(true);
            return;
        }

        MenuManager.instance?.MiniPause(false);
        uiPanel.SetActive(false);
    }

    #endregion

    private bool CheckRequirements(bool modeClick)
    {
        //if(playerStats.GetCurrentParameter(PlayersStats.MedicAltar) < 1)
        //{
        //    InfotipManager.ShowWarning("To use Altars, you need to unlock the corresponding skill!");
        //    return false;
        //}

        if(currentAltar.CheckInjured() == false)
        {
            InfotipManager.ShowMessage("You have nothing to pray for. All your units are healthy.");
            return false;
        }

        //if(modeClick == true)
        //{
        //    InfotipManager.ShowWarning("Gifts for healing are accepted only in person.");
        //    return false;
        //}

        //if(currentAltar.GetVisitStatus() == true)
        //{
        //    InfotipManager.ShowWarning("The gods are still angry with you.Try to visit next week..");
        //    return false;            
        //}

        return true;
    }

    private void ResetUnitSlots()
    {
        foreach(var unit in unitsSlots)
            unit.gameObject.SetActive(false);
    }

    public int GetMaxTry()
    {
        return (int)maxTry;
    }

    internal void ShowGoodResult()
    {
        ShowResultBlock(goodResultColor, goodResultText);
    }

    internal void ShowBadResult()
    {
        ShowResultBlock(badResultColor, badResultText);
    }

    public void ShowNeutralResult()
    {
        ShowResultBlock(neutralResultColor, neutralResultText);
    }

    public void ShowStupidResult()
    {
        ShowResultBlock(neutralResultColor, stupidResultText);
    }

    public void ShowResultBlock(Color colorBlock, string message)
    {
        resultBlock.SetActive(true);
        resultBlockBG.color = colorBlock;
        resultBlockText.text = message;
    }

    public void CloseResultBlock()
    {
        resultBlock.SetActive(false);
    }
}
