using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class AltarMiniGame : MonoBehaviour
{
    private Altar currentAltar;
    private AltarUI currentAltarUI;

    [SerializeField] private TMP_Text tryAmount;
    [SerializeField] private List<AltarGiftColumnUI> godsColumns;
    [SerializeField] private Button checkGifts;
    [SerializeField] private GameObject choiceBlock;
    [SerializeField] private CanvasGroup choiceBlockCanvas;
    [SerializeField] private List<AltarResourceButton> choiceButtons;
    [SerializeField] private GameObject winBlock;
    [SerializeField] private GameObject loseBlock;
    [SerializeField] private GameObject cancelBlock;

    private List<ResourceType> questCombination;
    Dictionary<ResourceType, float> price;
    private Dictionary<int, TryData> triesInfo = new Dictionary<int, TryData>();
    private int currentColumn = 0;
    private List<int> finishIndexes = new List<int>();
    private List<int> checkIndexes = new List<int>();

    private int maxTry;
    private int currentTry;

    private Coroutine coroutine;
    private WaitForSecondsRealtime delay = new WaitForSecondsRealtime(0.25f);

    // 0 - fail - RED   1 - success - green   2 - try again - yellow
    [SerializeField] private List<Color> resultColors;

    public void StartGame(AltarUI altarUI, List<ResourceType> combination)
    {
        currentAltarUI = altarUI;
        currentAltar = altarUI.currentAltar;

        maxTry = currentAltarUI.GetMaxTry();
        currentTry = 0;
        UpdateTries();

        questCombination = combination;

        Debug.Log(questCombination[0] + " - " + questCombination[1] + " - " + questCombination[2] + " - " +
            questCombination[3] + " - " + questCombination[4]);
        price = currentAltar.GetPrice();

        EnableGiftBtn(false);
        winBlock.SetActive(false);
        loseBlock.SetActive(false);
        cancelBlock.SetActive(false);

        ResetTriesData();
        CloseChoiceBlock();
        ResetColumns();

        finishIndexes.Clear();
        checkIndexes.Clear();
    }

    #region RESETS
    private void ResetColumns()
    {
        for(int i = 0; i < godsColumns.Count; i++)
            godsColumns[i].ResetColumn(this, i);
    }

    private void ResetChoiceBlock()
    {
        currentAltarUI.FillResourceButton(choiceButtons, price, true);
    }

    private void ResetTriesData()
    {
        triesInfo.Clear();

        int index = 0;
        foreach(var itemPrice in price)
        {
            triesInfo.Add(index, new TryData());
            index++;
        }
    }

    #endregion

    public void ShowChoiceBlock(int godIndex)
    {
        choiceBlock.SetActive(true);
        Fading.instance.FadeWhilePause(true, choiceBlockCanvas);
        ResetChoiceBlock();

        currentColumn = godIndex;
    }

    public void SetResource(ResourceGiftData data)
    {
        godsColumns[currentColumn].SetChoiceIcon(data.resourceIcon);
        choiceBlock.SetActive(false);

        triesInfo[currentColumn].resourceType = data.resourceType;
        triesInfo[currentColumn].resourceIcon = data.resourceIcon;
        triesInfo[currentColumn].amount = data.amount;

        checkIndexes.Add(currentColumn);

        CheckChoiceComplete();
    }

    private void CheckChoiceComplete()
    {
        if(finishIndexes.Count + checkIndexes.Count == questCombination.Count)
            EnableGiftBtn(true);
    }

    //Button
    public void ApplyAttempt()
    {
        EnableGiftBtn(false);
        coroutine = StartCoroutine(ApplyAttemptCoroutine());        
    }

    private IEnumerator ApplyAttemptCoroutine()
    {
        int index = 0;
        foreach(var item in triesInfo)
        {
            if(item.Value.isComplete == false)
            {
                yield return delay;
                godsColumns[index].ResetChoiceBtn();
                godsColumns[index].ShowTry(currentTry, item.Value.resourceIcon);
                item.Value.tempStatus = CheckTryStatus(index, item.Value.resourceType);

                currentAltar.Pay(item.Value.resourceType, item.Value.amount);
                index++;
            }
            else
            {
                index++;
                continue;
            }
        }

        index = 0;
        foreach(var item in triesInfo)
        {
            if(item.Value.isComplete == false)
            {
                int colorIndex = GetTryColor(index, item.Value.resourceType);
                Color tryColor = resultColors[colorIndex];
                godsColumns[index].SetChoiceBtnColor(currentTry, tryColor);
                if(colorIndex == 1)
                {
                    godsColumns[index].SelectButtonDisable();
                    item.Value.isComplete = true;
                    finishIndexes.Add(index);
                    checkIndexes.Remove(index);
                }                

                index++;
                yield return delay;
            }
            else
            {
                index++;
                continue;
            }
        }

        foreach(var item in triesInfo)
            item.Value.tempStatus = false;

        currentTry++;

        yield return delay;

        if(finishIndexes.Count == questCombination.Count)
        {
            winBlock.SetActive(true);
            yield return false;
        }
        else
        {
            if(currentTry >= maxTry)
            {
                loseBlock.SetActive(true);
                yield return false;
            }
            else
            {
                UpdateTries();
                ResetChoiceBlock();
                checkIndexes.Clear();
            }
        }
    }

    private bool CheckTryStatus(int index, ResourceType resourceType)
    {
        return questCombination[index] == resourceType;
    }

    private int GetTryColor(int checkIndex, ResourceType resourceType)
    {
        if(CheckTryStatus(checkIndex, resourceType) == true) return 1;

        foreach(var item in triesInfo)
        {
            if(item.Value.tempStatus == false)
            {
                if(CheckTryStatus(item.Key, resourceType) == true) return 2;



                //if(finishIndexes.Count == questCombination.Count - 1)
                //{
                //    if(CheckTryStatus(item.Key, resourceType) == true) return 0;
                //}
                //else
                //{
                //    if(item.Key != checkIndex)
                //    {
                //        if(CheckTryStatus(item.Key, resourceType) == true) return 2;
                //    }
                //}
            }
        }

        return 0;
    }

    //Button
    public void CloseChoiceBlock() 
    {
        choiceBlock.SetActive(false);
    }

    public void UpdateTries()
    {
        tryAmount.text = currentTry + "/" + maxTry + ")";
    }

    private void EnableGiftBtn(bool enableMode)
    {
        checkGifts.interactable = enableMode;
    }
}
