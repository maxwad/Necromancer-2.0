using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static NameManager;

public class PlayerPersonalWindow : MonoBehaviour, IInputableKeys
{
    [SerializeField] private GameObject rootCanvas;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Panels")]
    [SerializeField] private List<GameObject> allPanels;
    [SerializeField] private GameObject playerArmyPanel;
    [SerializeField] private GameObject macroLvlPanel;
    [SerializeField] private GameObject microLvlPanel;
    [SerializeField] private GameObject spellsPanel;

    [Header("Player Part")]
    private PlayersArmyPart playersArmyUIPart;

    [Header("Enemy Part")]
    [SerializeField] private EnemyArmyUI enemyArmyUI;
    [SerializeField] private GameObject enemyBlock;
    private EnemyArmyPart enemyArmyUIPart;

    [Header("Hero Part")]
    [SerializeField] private GameObject heroBlock;

    [Header("Spell Part")]
    [SerializeField] private SpellWindow spellUI;

    [Header("Buttons")]
    [SerializeField] private GameObject commonButtonsBlock;
    [SerializeField] private GameObject closeButton;
    [SerializeField] private GameObject battleButton;
    [SerializeField] private GameObject autobattleButton;
    [SerializeField] private GameObject stepbackButton;
    private Button autobattleButtonComponent;

    [Header("Tabs")]
    [SerializeField] private List<GameObject> allActiveTabs;
    [SerializeField] private GameObject activeArmyTab;
    [SerializeField] private GameObject activeMacroTab;
    [SerializeField] private GameObject activeMicroTab;
    [SerializeField] private GameObject activeSpellsTab;


    //private GMInterface gmInterface;
    private InputSystem inputSystem;
    private EnemyArmyOnTheMap currentEnemy;
    private MacroLevelWindow macroLevelUI;
    private RunesWindow runesWindow;
    [HideInInspector] public bool isWindowOpened = false;
    private bool canICloseWindow = true;
    private float playerCuriosity;


    private PlayersWindow currentMode = PlayersWindow.Nothing;
    private KeyActions lastKeyAction;

    private KeyActions currentKeyAction;
    private PlayersWindow currentWindow;

    private void Start()
    {
        autobattleButtonComponent = autobattleButton.GetComponent<Button>();
        playersArmyUIPart = GetComponent<PlayersArmyPart>();
        enemyArmyUIPart = GetComponent<EnemyArmyPart>();
        macroLevelUI = GetComponentInChildren<MacroLevelWindow>();
        runesWindow = GetComponentInChildren<RunesWindow>();
        //gmInterface = GlobalStorage.instance.gmInterface;
        RegisterInputKeys();
    }

    public void RegisterInputKeys()
    {
        inputSystem = GlobalStorage.instance.inputSystem;
        inputSystem.RegisterInputKeys(KeyActions.Army, this);
        inputSystem.RegisterInputKeys(KeyActions.Skills, this);
        inputSystem.RegisterInputKeys(KeyActions.Runes, this);
        inputSystem.RegisterInputKeys(KeyActions.Spells, this);
    }

    public void InputHandling(KeyActions keyAction)
    {
        if(canICloseWindow == false) return;

        currentKeyAction = keyAction;

        if(currentKeyAction == KeyActions.Army)
            currentWindow = PlayersWindow.PlayersArmy;

        else if(currentKeyAction == KeyActions.Skills)
            currentWindow = PlayersWindow.MacroLevelUp;

        else if(currentKeyAction == KeyActions.Runes)
            currentWindow = PlayersWindow.MicroLevelUp;

        else if(currentKeyAction == KeyActions.Spells)
            currentWindow = PlayersWindow.Spells;

        else
            return;

        if(isWindowOpened == false)
        {
            if(MenuManager.instance.isGamePaused == false && GlobalStorage.instance.isModalWindowOpen == false)
            {
                lastKeyAction = currentKeyAction;
                OpenWindow(currentWindow);
            }
        }
        else
        {
            if(lastKeyAction == currentKeyAction)
                CloseWindow();
            else
            {
                HandlingTabs(currentWindow, currentKeyAction);
            }
        }
    }

    public void PressButton(KeyActions keyAction) 
    {
        if(canICloseWindow == false) return;

        currentKeyAction = keyAction;

        if(currentKeyAction == KeyActions.Army)
            currentWindow = PlayersWindow.PlayersArmy;

        else if(currentKeyAction == KeyActions.Skills)
            currentWindow = PlayersWindow.MacroLevelUp;

        else if(currentKeyAction == KeyActions.Runes)
            currentWindow = PlayersWindow.MicroLevelUp;

        else if(currentKeyAction == KeyActions.Spells)
            currentWindow = PlayersWindow.Spells;

        else
            return;

        if(isWindowOpened == false)
        {
            if(MenuManager.instance.isGamePaused == false && GlobalStorage.instance.isModalWindowOpen == false)
            {
                lastKeyAction = currentKeyAction;
                OpenWindow(currentWindow);
            }
        }
    }

    public void OpenWindow(PlayersWindow mode, EnemyArmyOnTheMap enemyArmy = null, bool enemyInitiative = false)
    {
        currentEnemy = enemyArmy;
        canICloseWindow = !enemyInitiative;

        MenuManager.instance.MiniPause(true);

        isWindowOpened = true;
        currentMode = mode;
        rootCanvas.SetActive(true);
        Fading.instance.FadeWhilePause(true, canvasGroup);
                
        Refactoring();
        HandlingTabs(currentMode, lastKeyAction);

        playersArmyUIPart.UpdateArmyWindow();

        enemyArmyUIPart.Init(enemyArmy);
        macroLevelUI.Init();
        spellUI.Init();
    }

    public void CloseWindow()
    {
        playerArmyPanel.SetActive(false);
        rootCanvas.SetActive(false);

        isWindowOpened = false;
        currentMode = PlayersWindow.Nothing;
        currentEnemy = null;

        MenuManager.instance.MiniPause(false);
    }

    private void Refactoring()
    {
        playerCuriosity = GlobalStorage.instance.playerStats.GetCurrentParameter(PlayersStats.Curiosity);

        enemyBlock.SetActive(false);

        closeButton.SetActive(true);
        battleButton.SetActive(false);
        stepbackButton.SetActive(false);
        autobattleButton.SetActive(false);

        if(currentMode == PlayersWindow.PlayersArmy)
        {
            enemyBlock.SetActive(false);
            heroBlock.SetActive(true);
        }

        if(currentMode == PlayersWindow.Battle)
        {
            heroBlock.SetActive(false);
            enemyBlock.SetActive(true);

            //closeButton.SetActive(false);
            battleButton.SetActive(true);
            stepbackButton.SetActive(true);

            if(playerCuriosity >= 3)
            {
                autobattleButton.SetActive(true);

                if(currentEnemy.army.isAutobattlePosible == true)
                    autobattleButtonComponent.interactable = true;
                else
                    autobattleButtonComponent.interactable = false;
            }
                
            else
                autobattleButton.SetActive(false);
        }

        if(currentMode == PlayersWindow.Tomb)
        {
            //tombBlock.SetActive(true);
        }

        if(canICloseWindow == false)
        {
            closeButton.SetActive(false);
            stepbackButton.SetActive(false);
        }
    }

    public void HandlingTabs(PlayersWindow window, KeyActions keyAction)
    {
        foreach(var tab in allActiveTabs)
        {
            tab.SetActive(false);
        }

        foreach(var panel in allPanels)
        {
            panel.SetActive(false);
        }

        lastKeyAction = keyAction;

        switch(window)
        {
            case PlayersWindow.Nothing:
            case PlayersWindow.PlayersArmy:
            case PlayersWindow.Battle:
            case PlayersWindow.Tomb:
                activeArmyTab.SetActive(true);
                playerArmyPanel.SetActive(true);
                break;

            case PlayersWindow.Spells:
                activeSpellsTab.SetActive(true);
                spellsPanel.SetActive(true);
                spellUI.Init();
                break;

            case PlayersWindow.MicroLevelUp:
                activeMicroTab.SetActive(true);
                microLvlPanel.SetActive(true);
                runesWindow.UpdateWindow();
                break;

            case PlayersWindow.MacroLevelUp:
                activeMacroTab.SetActive(true);
                macroLvlPanel.SetActive(true);
                break;
            default:
                break;
        }
    }

    //Button
    public void ToTheBattle()
    {
        CloseWindow();
        GlobalStorage.instance.battleManager.InitializeBattle();
    }

    //Button
    public void AutoBattle()
    {
        GlobalStorage.instance.battleManager.AutoBattle();
        CloseWindow();
    }
}
