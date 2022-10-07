using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class NewSkillUI : MonoBehaviour
{
    [SerializeField] private GameObject uiPanel;
    private CanvasGroup canvas;

    [SerializeField] private Button replaceButton;

    [SerializeField] private List<LevelUpCard> cardList;
    private List<MacroAbilitySO> abilitiesList;

    private MacroLevelUpManager macroLevelUpManager;
    private MacroLevelWindow macroLevelUI;

    private float fadeStep = 0.1f;
    private float fadeDelay = 5f;

    private bool isAbilityTaken = false;
    private int readyForReplace = 0;
    //private int readyForTaken = 0;

    private void Start()
    {
        macroLevelUpManager = GlobalStorage.instance.macroLevelUpManager;
        macroLevelUI = GetComponent<MacroLevelWindow>();
        canvas = uiPanel.GetComponent<CanvasGroup>();
    }

    public void OpenWindow()
    {
        isAbilityTaken = false;

        FillCards(cardList);

        ShowBattons();

        uiPanel.SetActive(true);
        Fading.instance.FadeWhilePause(true, canvas, fadeStep, fadeDelay);
    }

    private void ShowBattons()
    {
        //ADD IF
        if(macroLevelUpManager.CanIReplaceCards() == true)
            replaceButton.interactable = true;
    }

    public void FillCards(List<LevelUpCard> list)
    {
        //readyForTaken = list.Count;
        abilitiesList = macroLevelUpManager.GetCurrentAbilities();

        for(int i = 0; i < abilitiesList.Count; i++)
        {
            bool mode = (i == abilitiesList.Count - 1) ? false : true;
            list[i].Init(mode, abilitiesList[i], i, this);
        }
    }

    public void Result(MacroAbilitySO ability)
    {
        foreach(var card in cardList)
            card.LockCard();

        isAbilityTaken = true;
        macroLevelUpManager.GetNewAbility(ability);
        macroLevelUpManager.ChangeAbilityPoints(false);
        macroLevelUI.UpdateAbilityBlock();

        Debug.Log("Taken");

        replaceButton.interactable = false;
    }

    //button
    public void ReplaceAbilities()
    {
        if(isAbilityTaken == true) return;

        replaceButton.interactable = false;
        readyForReplace = 0;
        //readyForTaken = 0;

        foreach(var card in cardList)
            card.Replace();
    }

    public void ReadyToReplace()
    {
        readyForReplace++;
        if(readyForReplace == cardList.Count)
            FillCards(cardList);

        macroLevelUpManager.CardsReplaced();
    }

    public void ReadyForTaken()
    {
        //readyForTaken++;
    }

    public bool CheckTakenAbility()
    {
        return isAbilityTaken;
    }

    public void GetOneMoreAbility()
    {
        if(macroLevelUpManager.GetAbilityPoints() == 0) return;

        isAbilityTaken = false;
        readyForReplace = 0;
        //readyForTaken = 0;

        foreach(var card in cardList)
            card.Replace();
    }

    public void HideWindow()
    {
        //if(readyForTaken != cardList.Count) return;
        foreach(var card in cardList)
            if(card.CanIClickAnyButton() == false) return;

        macroLevelUI.UpdateAbilityBlock();
        uiPanel.SetActive(false);
    }
}
