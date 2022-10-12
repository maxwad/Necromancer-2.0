using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static NameManager;

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

    private bool isCardHidden = true;
    private bool canIReplace = false;

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
        if(canIReplace == true)
        {
            replaceButton.gameObject.SetActive(true);

            if(macroLevelUpManager.CanIReplaceCards() == true)
                replaceButton.interactable = true;
        }
        else
        {
            replaceButton.gameObject.SetActive(false);
        }
    }

    private void GetAbilities()
    {
        if(abilitiesList.Count != 0) return;
        abilitiesList = macroLevelUpManager.GetCurrentAbilities();
    }

    public void FillCards(List<LevelUpCard> list)
    {
        GetAbilities();

        for(int i = 0; i < list.Count; i++)
        {
            if(i >= abilitiesList.Count)
            {
                list[i].gameObject.SetActive(false);
            }
            else
            {
                bool mode;
                if(isCardHidden == true)
                {
                    mode = (i == abilitiesList.Count - 1) ? false : true;
                }
                else
                {
                    mode = true;
                }
                list[i].Init(mode, abilitiesList[i], i, this);
            }
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

    private void UpgradeParameter(PlayersStats stat, float value)
    {
        if(stat == PlayersStats.Learning)
        {
            canIReplace = (value > 0) ? true : false;
            isCardHidden = (value > 1) ? false : true;
        }
    }

    private void OnEnable()
    {
        EventManager.SetNewPlayerStat += UpgradeParameter;
    }

    private void OnDisable()
    {
        EventManager.SetNewPlayerStat -= UpgradeParameter;
    }
}
