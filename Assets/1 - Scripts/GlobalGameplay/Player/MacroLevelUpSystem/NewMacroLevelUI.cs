using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewMacroLevelUI : MonoBehaviour
{
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private CanvasGroup canvas;

    [Header("Caption")]
    [SerializeField] private TMP_Text newLevelText;
    [SerializeField] private TMP_Text newLevelBonusText;
    [SerializeField] private Image newLevelBonus;
    [SerializeField] private Sprite manaBonus;
    [SerializeField] private Sprite healthBonus;
    [SerializeField] private float newLevelBonusAmount = 5f;


    [SerializeField] private List<LevelUpCard> cardList;
    private List<MacroAbilitySO> abilitiesList;

    private AbilitiesStorage abilitiesStorage;

    private float fadeStep = 0.1f;
    private float fadeDelay = 10f;

    private int countOfVariants = 3;

    private void Start()
    {
        abilitiesStorage = GlobalStorage.instance.macroLevelUpManager.GetComponentInChildren<AbilitiesStorage>();
    }

    public void Init(float level)
    {
        MenuManager.instance.MiniPause(true);
        GlobalStorage.instance.ModalWindowOpen(true);

        uiPanel.SetActive(true);
        Fading.instance.FadeWhilePause(true, canvas, fadeStep, fadeDelay);

        FillHead(level, newLevelBonus, newLevelBonusText);
        FillCards(cardList);
    }

    public void FillHead(float level, Image icon, TMP_Text bonusText)
    {
        Sprite sprite = healthBonus;

        if(level % 2 != 0) sprite = manaBonus;

        newLevelText.text = level.ToString();
        icon.sprite = sprite;

        bonusText.text = "+" + newLevelBonusAmount;
    }

    public void FillCards(List<LevelUpCard> list)
    {
        abilitiesList = abilitiesStorage.GetAbilitiesForNewLevel(countOfVariants);

        for(int i = 0; i < abilitiesList.Count; i++)
        {
            bool mode = (i == abilitiesList.Count - 1) ? false : true;
            list[i].Init(mode, abilitiesList[i]);
        }
    }

    public void CloseWindow()
    {
        MenuManager.instance.MiniPause(false);
        GlobalStorage.instance.ModalWindowOpen(false);
        uiPanel.SetActive(false);
    }
}
