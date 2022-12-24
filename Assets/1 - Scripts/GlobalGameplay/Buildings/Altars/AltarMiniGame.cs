using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static NameManager;

public class AltarMiniGame : MonoBehaviour
{
    private Altar currentAltar;
    private AltarUI currentAltarUI;

    [SerializeField] private List<AltarGiftColumnUI> godsColumns;
    [SerializeField] private Button checkGifts;
    [SerializeField] private GameObject choiceBlock;
    [SerializeField] private CanvasGroup choiceBlockCanvas;
    [SerializeField] private List<AltarResourceButton> choiceButtons;

    private List<ResourceType> questCombination;
    private int currentColumn = 0;
    private bool[] tryStatuses = new bool[5];
    //private Dictionary<>



    public void StartGame(AltarUI altarUI, List<ResourceType> combination)
    {
        currentAltarUI = altarUI;
        currentAltar = altarUI.currentAltar;

        questCombination = combination;

        checkGifts.interactable = false;
        CloseChoiceBlock();
        ResetColumns();
    }

    private void ResetColumns()
    {
        for(int i = 0; i < godsColumns.Count; i++)
            godsColumns[i].ResetColumn(this, i);
    }

    private void ResetChoiceBlock()
    {
        Dictionary<ResourceType, float> price = currentAltar.GetPrice();

        currentAltarUI.FillResourceButton(choiceButtons, price, true);
    }

    public void SetResource(ResourceGiftData data)
    {
        godsColumns[currentColumn].SetChoiceIcon(data.resourceIcon);
        choiceBlock.SetActive(false);
    }

    public void ShowChoiceBlock(int godIndex)
    {
        choiceBlock.SetActive(true);
        Fading.instance.FadeWhilePause(true, choiceBlockCanvas);
        ResetChoiceBlock();

        currentColumn = godIndex;
    }

    //Button
    public void CloseChoiceBlock() 
    {
        choiceBlock.SetActive(false);
    }

}
