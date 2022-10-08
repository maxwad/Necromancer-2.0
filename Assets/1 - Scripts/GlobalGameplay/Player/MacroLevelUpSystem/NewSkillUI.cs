using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewSkillUI : MonoBehaviour
{
    [SerializeField] private GameObject uiPanel;
    private CanvasGroup canvas;

    [SerializeField] private Button replaceButton;
    [SerializeField] private Button hideButton;

    [SerializeField] private List<LevelUpCard> cardList;
    private List<MacroAbilitySO> abilitiesList = new List<MacroAbilitySO>();

    private MacroLevelUpManager macroLevelUpManager;
    private MacroLevelWindow macroLevelUI;

    private float fadeStep = 0.1f;
    private float fadeDelay = 5f;

    private bool isAbilityTaken = false;
    private int readyForReplace = 0;
    private bool isChecking = false;

    private void Start()
    {
        macroLevelUpManager = GlobalStorage.instance.macroLevelUpManager;
        macroLevelUI = GetComponent<MacroLevelWindow>();
        canvas = uiPanel.GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if(isChecking == true)
        {
            hideButton.interactable = false;
            foreach(var card in cardList)
                if(card.CanIClickAnyButton() == false) return;

            isChecking = false;
            hideButton.interactable = true;
        }
    }

    public void OpenWindow()
    {
        isAbilityTaken = false;
        isChecking = false;

        FillCards(cardList);

        ShowBattons();

        uiPanel.SetActive(true);
        Fading.instance.FadeWhilePause(true, canvas, fadeStep, fadeDelay);
    }

    private void ShowBattons()
    {
        if(macroLevelUpManager.CanIReplaceCards() == true)
            replaceButton.interactable = true;
    }

    private void GetAbilities()
    {
        if(abilitiesList.Count != 0) return;
        abilitiesList = macroLevelUpManager.GetCurrentAbilities();
    }

    public void FillCards(List<LevelUpCard> list)
    {
        GetAbilities();

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

        macroLevelUpManager.OpenAbility(ability);
        macroLevelUpManager.ChangeAbilityPoints(false);
        macroLevelUI.UpdateAbilityBlock();
        isAbilityTaken = true;
        abilitiesList.Clear();

        replaceButton.interactable = false;
    }

    //button
    public void ReplaceAbilities()
    {
        if(isAbilityTaken == true) return;

        isChecking = true;

        replaceButton.interactable = false;

        abilitiesList.Clear();
        GetAbilities();
        macroLevelUpManager.CardsReplaced();

        readyForReplace = 0;

        foreach(var card in cardList)
            card.Replace();
    }

    public void ReadyToReplace()
    {
        readyForReplace++;

        if(readyForReplace == cardList.Count)
            FillCards(cardList);
    }

    public bool CheckTakenAbility()
    {
        return isAbilityTaken;
    }

    public bool TryToHideWindow()
    {
        if(hideButton.interactable == false) 
            return false;
        else
        {
            HideWindow();
            return true;
        }
    }

    public void HideWindow()
    {
        macroLevelUI.UpdateAbilityBlock();
        uiPanel.SetActive(false);
    }
}
