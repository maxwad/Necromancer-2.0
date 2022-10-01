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


    private PlayersArmy playersArmy;
    private EnemyArmyOnTheMap currentEnemy;
    [HideInInspector] public bool isWindowOpened = false;
    private float playerCuriosity;
    private PlayersWindow currentMode = PlayersWindow.Nothing;
    private KeyCode lastPressedKey;

    private void Start()
    {
        autobattleButtonComponent = autobattleButton.GetComponent<Button>();
        playersArmy = GlobalStorage.instance.player.GetComponent<PlayersArmy>();
        playersArmyUIPart = GetComponent<PlayersArmyPart>();
        enemyArmyUIPart = GetComponent<EnemyArmyPart>();
    }

    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.I))
        //{
        //    if(isWindowOpened == false)
        //    {
        //        if(MenuManager.instance.isGamePaused == false && GlobalStorage.instance.isModalWindowOpen == false)
        //            OpenWindow(PlayersWindow.PlayersArmy);
        //    }
        //    else
        //        CloseWindow();
        //}

        //if(Input.GetKeyDown(KeyCode.H) && isWindowOpened == false)
        //{
        //    if(MenuManager.instance.isGamePaused == false && GlobalStorage.instance.isModalWindowOpen == false)
        //        OpenWindow(PlayersWindow.MacroLevelUp);
        //}

        //if(Input.GetKeyDown(KeyCode.U) && isWindowOpened == false)
        //{
        //    if(MenuManager.instance.isGamePaused == false && GlobalStorage.instance.isModalWindowOpen == false)
        //        OpenWindow(PlayersWindow.MicroLevelUp);
        //}

        //if(Input.GetKeyDown(KeyCode.O) && isWindowOpened == false)
        //{
        //    if(MenuManager.instance.isGamePaused == false && GlobalStorage.instance.isModalWindowOpen == false)
        //        OpenWindow(PlayersWindow.Spells);
        //}
        if(MenuManager.instance.isGamePaused == false && GlobalStorage.instance.isModalWindowOpen == false)
        {
            KeyCode currentKeyCode;
            PlayersWindow currentWindow;

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
                lastPressedKey = currentKeyCode;
                OpenWindow(currentWindow);
            }
            else
            {
                if(lastPressedKey == currentKeyCode)
                    CloseWindow();
                else
                    HandlingTabs(currentWindow);
            }


            //if(Input.GetKeyDown(KeyCode.I))
            //{
            //    if(isWindowOpened == false)
            //    {
            //        lastPressedKey = KeyCode.I;
            //        OpenWindow(PlayersWindow.PlayersArmy);
            //    }
            //    else
            //    {
            //        if(lastPressedKey == KeyCode.I)
            //            CloseWindow();
            //        else
            //            HandlingTabs(PlayersWindow.PlayersArmy);
            //    }
            //}

            //if(Input.GetKeyDown(KeyCode.H))
            //{
            //    if(isWindowOpened == false)
            //    {
            //        lastPressedKey = KeyCode.H;
            //        OpenWindow(PlayersWindow.MacroLevelUp);
            //    }
            //    else
            //    {
            //        if(lastPressedKey == KeyCode.H)
            //            CloseWindow();
            //        else
            //            HandlingTabs(PlayersWindow.MacroLevelUp);
            //    }
            //}

            //if(Input.GetKeyDown(KeyCode.U))
            //{
            //    if(isWindowOpened == false)
            //    {
            //        lastPressedKey = KeyCode.U;
            //        OpenWindow(PlayersWindow.MicroLevelUp);
            //    }
            //    else
            //    {
            //        if(lastPressedKey == KeyCode.U)
            //            CloseWindow();
            //        else
            //            HandlingTabs(PlayersWindow.MicroLevelUp);
            //    }
            //}  
        
            //if(Input.GetKeyDown(KeyCode.O))
            //{
            //    if(isWindowOpened == false)
            //    {
            //        lastPressedKey = KeyCode.O;
            //        OpenWindow(PlayersWindow.Spells);
            //    }
            //    else
            //    {
            //        if(lastPressedKey == KeyCode.O)
            //            CloseWindow();
            //        else
            //            HandlingTabs(PlayersWindow.Spells);
            //    }
            //}
        }



    }

    public void OpenWindow(PlayersWindow mode, EnemyArmyOnTheMap enemyArmy = null)
    {
        // mode = 0 - just army, 1 - battle, 2 - tomb
        currentEnemy = enemyArmy;

        MenuManager.instance.MiniPause(true);
        GlobalStorage.instance.ModalWindowOpen(true);

        isWindowOpened = true;
        currentMode = mode;
                
        Refactoring();
        HandlingTabs(currentMode);

        rootCanvas.SetActive(true);
        Fading.instance.FadeWhilePause(true, canvasGroup);

        playersArmyUIPart.UpdateArmyWindow();

        if(currentMode == PlayersWindow.Battle) enemyArmyUIPart.Init(enemyArmy);
    }

    public void CloseWindow()
    {
        playersArmy.ResetReplaceIndexes();
        playerArmyPanel.SetActive(false);
        rootCanvas.SetActive(false);

        isWindowOpened = false;
        currentMode = PlayersWindow.Nothing;

        MenuManager.instance.MiniPause(false);
        GlobalStorage.instance.ModalWindowOpen(false);
    }

    private void Refactoring()
    {
        playerCuriosity = GlobalStorage.instance.playerStats.GetCurrentParameter(PlayersStats.Curiosity);

        if(currentMode == PlayersWindow.PlayersArmy)
        {
            enemyBlock.SetActive(false);
            //tombBlock.SetActive(false);

            closeButton.SetActive(true);
            battleButton.SetActive(false);
            stepbackButton.SetActive(false);
            autobattleButton.SetActive(false);
        }

        if(currentMode == PlayersWindow.Battle)
        {
            enemyBlock.SetActive(true);
            //tombBlock.SetActive(false);

            closeButton.SetActive(false);
            battleButton.SetActive(true);
            stepbackButton.SetActive(true);

            if(playerCuriosity == 3)
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
            enemyBlock.SetActive(false);
            //tombBlock.SetActive(true);
        }
    }

    public void HandlingTabs(PlayersWindow window)
    {
        foreach(var tab in allActiveTabs)
        {
            tab.SetActive(false);
        }

        foreach(var panel in allPanels)
        {
            panel.SetActive(false);
        }

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
