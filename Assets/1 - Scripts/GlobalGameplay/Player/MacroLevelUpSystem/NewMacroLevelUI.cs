using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class NewMacroLevelUI : MonoBehaviour
{
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private CanvasGroup canvas;

    [Header("Caption")]
    [SerializeField] private GameObject bonusesBlock;
    [SerializeField] private TooltipTrigger bonusTip;
    [SerializeField] private TMP_Text newLevelText;
    [SerializeField] private TMP_Text newLevelBonusText;
    [SerializeField] private Image newLevelBonus;
    [SerializeField] private Sprite manaBonus;
    [SerializeField] private Sprite healthBonus;
    [SerializeField] private float newLevelBonusAmount = 5f;
    private PlayersStats stat;

    [SerializeField] private GameObject replaceButton;
    [SerializeField] private GameObject laterButton;
    [SerializeField] private GameObject closeButton;

    [SerializeField] private List<LevelUpCard> cardList;
    private List<MacroAbilitySO> abilitiesList;

    private AbilitiesStorage abilitiesStorage;
    private MacroLevelUpManager macroLevelUpManager;

    private float fadeStep = 0.1f;
    private float fadeDelay = 10f;

    private int countOfVariants = 3;
    private bool isAbilityTaken = false;
    private int readyForReplace = 0;
    private int readyForTaken = 0;

    private void Start()
    {
        abilitiesStorage = GlobalStorage.instance.macroLevelUpManager.GetComponentInChildren<AbilitiesStorage>();
    }

    public void Init(PlayersStats bonusStat, float newLevel, MacroLevelUpManager manager)
    {
        MenuManager.instance.MiniPause(true);
        GlobalStorage.instance.ModalWindowOpen(true);

        macroLevelUpManager = manager;

        uiPanel.SetActive(true);
        Fading.instance.FadeWhilePause(true, canvas, fadeStep, fadeDelay);

        isAbilityTaken = false;

        if(bonusStat == PlayersStats.Level)
        {
            bonusesBlock.SetActive(false);
        }
        else
        {
            bonusesBlock.SetActive(true);
            FillHead(newLevel, bonusStat, newLevelBonus, newLevelBonusText);
        }
        
        FillCards(cardList);
    }

    public void FillHead(float level, PlayersStats bonus, Image icon, TMP_Text bonusText)
    {
        Sprite sprite;

        if(bonus == PlayersStats.Health)
            sprite = healthBonus;
        else
            sprite = manaBonus;

        stat = bonus;
        bonusTip.content = "You get " + newLevelBonusAmount + " points of " + stat;

        newLevelText.text = level.ToString();
        icon.sprite = sprite;

        bonusText.text = "+" + newLevelBonusAmount;

        //ADD IF
        replaceButton.SetActive(true);
        laterButton.SetActive(true);
        closeButton.SetActive(false);
    }

    public void FillCards(List<LevelUpCard> list)
    {
        readyForReplace = 0;
        readyForTaken = 0;

        abilitiesList = abilitiesStorage.GetAbilitiesForNewLevel(countOfVariants);

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

        macroLevelUpManager.GetNewAbility(ability);
        isAbilityTaken = true;
        macroLevelUpManager.ChangeAbilityPoints(false);

        replaceButton.SetActive(false);
        laterButton.SetActive(false);
        closeButton.SetActive(true);
    }

    //button
    public void ReplaceAbilities()
    {
        if(isAbilityTaken == true) return;

        replaceButton.SetActive(false);
        readyForReplace = 0;
        readyForTaken = 0;

        foreach(var card in cardList)
            card.Replace();
    }

    public void ReadyToReplace()
    {
        readyForReplace++;
        if(readyForReplace == cardList.Count)
            FillCards(cardList);
    }

    public void ReadyForTaken()
    {
        readyForTaken++;
    }

    public bool CheckTakenAbility()
    {
        return isAbilityTaken;
    }


    public void CloseWindow()
    {
        if(readyForTaken != cardList.Count) return;

        MenuManager.instance.MiniPause(false);
        GlobalStorage.instance.ModalWindowOpen(false);
        uiPanel.SetActive(false);
    }
}
