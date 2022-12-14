using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static NameManager;

public class PlayerPersonalWindow : MonoBehaviour
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
    //[SerializeField] private GameObject tombBlock;

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


    private GMInterface gmInterface;
    private EnemyArmyOnTheMap currentEnemy;
    private MacroLevelWindow macroLevelUI;
    private RunesWindow runesWindow;
    [HideInInspector] public bool isWindowOpened = false;
    private float playerCuriosity;


    private PlayersWindow currentMode = PlayersWindow.Nothing;
    private KeyCode lastPressedKey;

    private KeyCode currentKeyCode;
    private PlayersWindow currentWindow;

    private void Start()
    {
        autobattleButtonComponent = autobattleButton.GetComponent<Button>();
        playersArmyUIPart = GetComponent<PlayersArmyPart>();
        enemyArmyUIPart = GetComponent<EnemyArmyPart>();
        macroLevelUI = GetComponentInChildren<MacroLevelWindow>();
        runesWindow = GetComponentInChildren<RunesWindow>();
        gmInterface = GlobalStorage.instance.gmInterface;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            currentKeyCode = KeyCode.I;
            currentWindow = PlayersWindow.PlayersArmy;
        }
        else if(Input.GetKeyDown(KeyCode.H))
        {
            currentKeyCode = KeyCode.H;
            currentWindow = PlayersWindow.MacroLevelUp;
        }
        else if(Input.GetKeyDown(KeyCode.U))
        {
            currentKeyCode = KeyCode.U;
            currentWindow = PlayersWindow.MicroLevelUp;
        }
        else if(Input.GetKeyDown(KeyCode.O))
        {
            currentKeyCode = KeyCode.O;
            currentWindow = PlayersWindow.Spells;
        }
        else
        {
            return;
        }

        if(isWindowOpened == false)
        {
            if(MenuManager.instance.isGamePaused == false && GlobalStorage.instance.isModalWindowOpen == false)
            {
                lastPressedKey = currentKeyCode;
                OpenWindow(currentWindow);
            }
        }
        else
        {
            if(lastPressedKey == currentKeyCode)
                CloseWindow();
            else
            {
                HandlingTabs(currentWindow, currentKeyCode);
            }
        }
    }

    public void PressButton(KeyCode key) 
    {
        currentKeyCode = key;
        if(key == KeyCode.I)
        {
            currentWindow = PlayersWindow.PlayersArmy;
        }
        else if(key == KeyCode.H)
        {
            currentWindow = PlayersWindow.MacroLevelUp;
        }
        else if(key == KeyCode.U)
        {
            currentWindow = PlayersWindow.MicroLevelUp;
        }
        else if(key == KeyCode.O)
        {
            currentWindow = PlayersWindow.Spells;
        }
        else
        {
            return;
        }

        if(isWindowOpened == false)
        {
            if(MenuManager.instance.isGamePaused == false && GlobalStorage.instance.isModalWindowOpen == false)
            {
                lastPressedKey = currentKeyCode;
                OpenWindow(currentWindow);
            }
        }
    }

    public void OpenWindow(PlayersWindow mode, EnemyArmyOnTheMap enemyArmy = null)
    {
        currentEnemy = enemyArmy;

        MenuManager.instance.MiniPause(true);
        //GlobalStorage.instance.ModalWindowOpen(true);
        //gmInterface.ShowInterfaceElements(false);

        isWindowOpened = true;
        currentMode = mode;
        rootCanvas.SetActive(true);
        Fading.instance.FadeWhilePause(true, canvasGroup);
                
        Refactoring();
        HandlingTabs(currentMode, lastPressedKey);

        playersArmyUIPart.UpdateArmyWindow();

        enemyArmyUIPart.Init(enemyArmy);
        macroLevelUI.Init();
    }

    public void CloseWindow()
    {
        playerArmyPanel.SetActive(false);
        rootCanvas.SetActive(false);

        isWindowOpened = false;
        currentMode = PlayersWindow.Nothing;
        currentEnemy = null;

        MenuManager.instance.MiniPause(false);
        //GlobalStorage.instance.ModalWindowOpen(false);
        //gmInterface.ShowInterfaceElements(true);
    }

    private void Refactoring()
    {
        playerCuriosity = GlobalStorage.instance.playerStats.GetCurrentParameter(PlayersStats.Curiosity);

        enemyBlock.SetActive(false);
        //tombBlock.SetActive(false);

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

            closeButton.SetActive(false);
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
    }

    public void HandlingTabs(PlayersWindow window, KeyCode key)
    {
        foreach(var tab in allActiveTabs)
        {
            tab.SetActive(false);
        }

        foreach(var panel in allPanels)
        {
            panel.SetActive(false);
        }

        lastPressedKey = key;

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

    public void ToTheBattle()
    {
        CloseWindow();
        GlobalStorage.instance.battleManager.InitializeBattle();
    }

    public void AutoBattle()
    {
        GlobalStorage.instance.battleManager.AutoBattle();
        CloseWindow();
    }
}
