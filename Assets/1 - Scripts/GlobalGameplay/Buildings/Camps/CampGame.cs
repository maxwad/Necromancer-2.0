using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static NameManager;

public class CampGame : MonoBehaviour
{
    private CampManager campManager;
    private CampUI campUI;
    private ResourcesManager resourcesManager;
    private MacroLevelUpManager levelUpManager;
    private RunesManager runesManager;

    [Header("Parameters")]
    [SerializeField] private List<BonfireItemUI> cells;
    [SerializeField] private List<BonfireResultItemUI> rewardsItems;
    [SerializeField] private List<CampBonus> bonusesData;
    [SerializeField] private List<ResourceType> resources;

    [Header("Tips")]
    [SerializeField] private Button doubleBonus;
    [SerializeField] private Button showCells;
    [SerializeField] private Button showAsh;
    private bool isDoubleBonus = false;
    private int showCellsCount = 3;
    private int showAshCount = 5;

    private CampGameParameters currentParameters;
    private List<CampBonus> currentCombination;

    private bool isGameStarted = false;
    private bool isGameFinished = false;

    private bool shouldCoroutineStop = false;
    private WaitForSecondsRealtime bigPause = new WaitForSecondsRealtime(1f);
    private WaitForSecondsRealtime smallPause = new WaitForSecondsRealtime(0.25f);

    private void Start()
    {
        campManager = GlobalStorage.instance.campManager;
        campUI = GetComponent<CampUI>();

        resourcesManager = GlobalStorage.instance.resourcesManager;
        levelUpManager = GlobalStorage.instance.macroLevelUpManager;
        runesManager = GlobalStorage.instance.runesManager;

        ResetCells();
        SetIndexes();
    }

    #region PREPARING

    public void ResetCells()
    {
        foreach(var cell in cells)
            cell.ResetCell();

        foreach(var reward in rewardsItems)
            reward.gameObject.SetActive(false);

        ActivateTips(true);
        isGameStarted = false;
        isGameFinished = false;
        shouldCoroutineStop = false;
    }

    public void SetIndexes()
    {
        for(int i = 0; i < cells.Count; i++)
            cells[i].Init(this, i);
    }

    public void PreparedToGame(CampGameParameters parameters)
    {
        currentParameters = parameters;
        currentCombination = currentParameters.combination;
    }

    public bool CanIOpenCell()
    {
        return currentParameters.attempts > 0;
    }

    public bool CheckGameStatus()
    {
        return isGameStarted;
    }

    public void UpdateGameStatus()
    {
        if(currentParameters.attempts > 0)
        {
            isGameStarted = true;
        }
        else
        {
            if(isGameStarted == true)
            {
                isGameStarted = false;
                isGameFinished = true;
                ActivateTips(false);
                EndOfGame();
            }
            else
            {
                return;
            }
        }
    }

    private void ChangeHelpPointsAmount(int delta)
    {
        campManager.ChangeHelpPointsAmount(delta);
        currentParameters.helps += delta;
        campUI.FillParameters();
    }

    #endregion

    #region GAME

    public void ShowResult(int index, bool tipMode)
    {
        CampBonus currentBonus = currentCombination[index];

        cells[index].SetReward(currentBonus);

        if(tipMode == false)
        {
            currentParameters.attempts--;

            AddBonusUI(currentBonus, false);
            if(isDoubleBonus == true)
                AddBonusUI(currentBonus, true);

            ApplyBonus(currentBonus);
            if(isDoubleBonus == true)
                ApplyBonus(currentBonus);

            UpdateGameStatus();
        }

        currentParameters.cellsAmount--;
        campUI.FillParameters();
    }

    private void AddBonusUI(CampBonus bonus, bool doubleMode)
    {
        if(bonus.reward == CampReward.Nothing) return;

        foreach(var reward in rewardsItems)
        {
            if(reward.gameObject.activeInHierarchy == false)
            {
                reward.gameObject.SetActive(true);
                reward.Init(bonus);
                break;
            }
        }

        if(doubleMode == false)
        {
            currentParameters.rewardsAmount--;
            campUI.FillParameters();
        }
    }

    private void ApplyBonus(CampBonus bonus)
    {
        switch(bonus.reward)
        {
            case CampReward.Resource:
                resourcesManager.ChangeResource(bonus.resource, bonus.amount);                
                break;

            case CampReward.AbilityPoint:
                levelUpManager.ChangeAbilityPoints(1);
                break;

            case CampReward.RuneDrawing:
                runesManager.ChangeShardAmount(1);
                break;

            case CampReward.Mana:
                resourcesManager.ChangeResource(ResourceType.Mana, bonus.amount);
                break;

            case CampReward.Health:
                resourcesManager.ChangeResource(ResourceType.Health, bonus.amount);
                break;

            case CampReward.Help:
                ChangeHelpPointsAmount(1);
                break;

            case CampReward.Nothing:
                break;

            default:
                break;
        }

        isDoubleBonus = false;
    }

    private void EndOfGame()
    {
        campUI.CloseCamp();
        StartCoroutine(ShowAllCells());
    }

    private IEnumerator ShowAllCells()
    {
        yield return bigPause;

        for(int i = 0; i < currentCombination.Count; i++)
        {
            if(shouldCoroutineStop == true) break;

            if(cells[i].GetCellStatus() == false)
            {
                cells[i].ActivateCell(true);
                yield return smallPause;
            }
        }
    }

    public void CancelCorounite()
    {
        shouldCoroutineStop = true;
    }

    #endregion


    #region TIP BUTTONS
    private void ActivateTips(bool activateMode)
    {
        doubleBonus.interactable = activateMode;
        showCells.interactable = activateMode;
        showAsh.interactable = activateMode;
    }

    private bool UseHelp(Button button)
    {
        if(CanIUseHelp(button) == false) return false;

        UpdateGameStatus();
        ChangeHelpPointsAmount(-1);
        button.interactable = false;

        return true;
    }

    private bool CanIUseHelp(Button button)
    {
        if(isGameFinished == true)
        {
            InfotipManager.ShowWarning("You have no more attempts left to continue.");
            return false;
        }

        if(campManager.GetHelpsCount() <= 0)
        {
            InfotipManager.ShowWarning("You have no more Help Points.");
            return false;
        }

        if(button.interactable == false)
        {
            InfotipManager.ShowWarning("You have already used this type of Help.");
            return false;
        }

        return true;
    }

    //Button
    public void ShowAsh()
    {
        if(UseHelp(showAsh) == false) return;

        GetAshCells();
    }

    private void GetAshCells()
    {
        List<int> ashCells = new List<int>();

        for(int i = 0; i < currentCombination.Count; i++)
        {
            if(currentCombination[i].reward == CampReward.Nothing && cells[i].GetCellStatus() == false)
                ashCells.Add(i);
        }

        int count = (ashCells.Count > showAshCount) ? showAshCount : ashCells.Count;

        for(int i = 0; i < count; i++)
        {
            int index = ashCells[UnityEngine.Random.Range(0, ashCells.Count)];
            cells[index].ActivateCell(true);
            ashCells.Remove(index);
        }
    }

    //Button
    public void DoubleBonus()
    {
        if(UseHelp(doubleBonus) == false) return;

        isDoubleBonus = true;
    }

    //Button
    public void ShowCells()
    {
        if(UseHelp(showCells) == false) return;

        List<int> toShowCells = new List<int>();

        for(int i = 0; i < currentCombination.Count; i++)
        {
            if(cells[i].GetCellStatus() == false)
                toShowCells.Add(i);
        }

        int count = (toShowCells.Count > showCellsCount) ? showCellsCount : toShowCells.Count;

        for(int i = 0; i < count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, toShowCells.Count);
            int index = toShowCells[randomIndex];

            CampBonus currentBonus = currentCombination[index];
            cells[index].ShowCell(currentBonus);
            toShowCells.Remove(randomIndex);
        }
    }

    #endregion

}
