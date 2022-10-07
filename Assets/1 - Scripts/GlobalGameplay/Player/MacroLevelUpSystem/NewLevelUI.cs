using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class NewLevelUI : MonoBehaviour
{
    private PlayerPersonalWindow playerMilitaryWindow;

    [SerializeField] private GameObject uiPanel;
    [SerializeField] private CanvasGroup canvas;

    [SerializeField] private TMP_Text newLevelText;
    [SerializeField] private TMP_Text newLevelBonusText;
    [SerializeField] private Image newLevelBonus;
    [SerializeField] private Sprite manaBonus;
    [SerializeField] private Sprite healthBonus;

    [SerializeField] private TooltipTrigger bonusTip;
    private PlayersStats stat;

    private float fadeStep = 0.1f;
    private float fadeDelay = 5f;

    private void Start()
    {
        playerMilitaryWindow = GlobalStorage.instance.playerMilitaryWindow;
    }

    public void Init(PlayersStats bonusStat, float amount, float level)
    {
        MenuManager.instance.MiniPause(true);
        GlobalStorage.instance.ModalWindowOpen(true);

        Sprite sprite;

        if(bonusStat == PlayersStats.Health)
            sprite = healthBonus;
        else
            sprite = manaBonus;

        stat = bonusStat;
        bonusTip.content = "You get " + amount + " points of " + stat;

        newLevelText.text = level.ToString();
        newLevelBonus.sprite = sprite;

        newLevelBonusText.text = "+" + amount;

        uiPanel.SetActive(true);
        Fading.instance.FadeWhilePause(true, canvas, fadeStep, fadeDelay);
    }

    //Button
    public void CloseWindow()
    {
        MenuManager.instance.MiniPause(false);
        GlobalStorage.instance.ModalWindowOpen(false);
        uiPanel.SetActive(false);
    }

    //Button
    public void ToTheSkillsWindow()
    {
        CloseWindow();
        playerMilitaryWindow.OpenWindow(PlayersWindow.MacroLevelUp);
    }

}
