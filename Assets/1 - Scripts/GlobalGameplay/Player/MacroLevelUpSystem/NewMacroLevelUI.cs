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

    [SerializeField] private List<LevelUpCard> cardList;
    private List<MacroAbilitySO> abilitiesList;

    private AbilitiesStorage abilitiesStorage;
    private MacroLevelUpManager macroLevelUpManager;

    private float fadeStep = 0.1f;
    private float fadeDelay = 10f;

    private int countOfVariants = 3;
    private bool isAbilityTaken = false;
    private int readyForRepalce = 0;
    private bool isReplacingStarted = false;


    private void Start()
    {
        abilitiesStorage = GlobalStorage.instance.macroLevelUpManager.GetComponentInChildren<AbilitiesStorage>();
    }

    public void Init(bool bonusesMode, float newLevel, MacroLevelUpManager manager)
    {
        MenuManager.instance.MiniPause(true);
        GlobalStorage.instance.ModalWindowOpen(true);

        macroLevelUpManager = manager;

        uiPanel.SetActive(true);
        Fading.instance.FadeWhilePause(true, canvas, fadeStep, fadeDelay);

        isAbilityTaken = false;

        if(bonusesMode == true)
        {
            bonusesBlock.SetActive(true);
            FillHead(newLevel, newLevelBonus, newLevelBonusText);
        }
        else
        {
            bonusesBlock.SetActive(false);
        }
        
        FillCards(cardList);
    }

    public void FillHead(float level, Image icon, TMP_Text bonusText)
    {
        Sprite sprite;

        if(level % 2 == 0) 
        {
            sprite = healthBonus;
            stat = PlayersStats.Health;
        }
        else
        {
            sprite = manaBonus;
            stat = PlayersStats.Mana;
        }

        bonusTip.content = "You get " + newLevelBonusAmount + " points of " + stat;


        newLevelText.text = level.ToString();
        icon.sprite = sprite;

        bonusText.text = "+" + newLevelBonusAmount;

        //ADD IF
        replaceButton.SetActive(true);
    }

    public void FillCards(List<LevelUpCard> list)
    {
        readyForRepalce = 0;
        isReplacingStarted = false;

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
        StartCoroutine(Close());

        IEnumerator Close()
        {
            yield return new WaitForSecondsRealtime(1f);
            Fading.instance.FadeWhilePause(false, canvas);
            yield return new WaitForSecondsRealtime(1f);
            CloseWindow();
        }
    }

    public void ReplaceAbilities()
    {
        if(isAbilityTaken == true) return;

        replaceButton.SetActive(false);
        isReplacingStarted = true;
        foreach(var card in cardList)
        {
            card.Replace();
        }

    }

    public void ReadyToReplace()
    {
        readyForRepalce++;
        if(readyForRepalce == cardList.Count)
        {
            FillCards(cardList);
        }
    }

    public void CloseWindow()
    {
        if(isReplacingStarted == true) return;

        if(isAbilityTaken == false)
        {

        }

        MenuManager.instance.MiniPause(false);
        GlobalStorage.instance.ModalWindowOpen(false);
        uiPanel.SetActive(false);
    }
}
