using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class PlayerMilitaryWindow : MonoBehaviour
{
    [Header("Window Parts")]
    private PlayersArmy playersArmy;
    [SerializeField] private GameObject playerArmyUI;
    [SerializeField] private EnemyArmyUI enemyArmyUI;
    [SerializeField] private RectTransform rectTransformUI;
    [SerializeField] private CanvasGroup canvasGroup;
    private PlayersArmyPart playersArmyUIPart;
    //private EnemyArmyPart enemyArmyUIPart;

    [HideInInspector] public bool isWindowOpened = false;

    [SerializeField] private float minWidth;
    [SerializeField] private float maxWidth;
    private float currentWidth;

    //[SerializeField] private GameObject tombBlock;

    private Coroutine coroutine;
    private float step = 0.1f;

    [Header("Buttons")]
    [SerializeField] private GameObject commonButtonsBlock;
    [SerializeField] private GameObject closeButton;
    [SerializeField] private GameObject battleButton;
    [SerializeField] private GameObject autobattleButton;
    [SerializeField] private GameObject stepbackButton;

    private float playerCuriosity;
    private int currentMode = 0;

    private void Start()
    {
        playersArmy = GlobalStorage.instance.player.GetComponent<PlayersArmy>();
        playersArmyUIPart = GetComponent<PlayersArmyPart>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            if(isWindowOpened == false)
            {
                if(MenuManager.instance.isGamePaused == false && GlobalStorage.instance.isModalWindowOpen == false)
                    OpenWindow(0);
            }
            else
                CloseWindow();
        }
    }

    public void OpenWindow(int mode, EnemyArmyOnTheMap enemyArmy = null)
    {
        // mode = 0 - just army, 1 - battle, 2 - tomb

        MenuManager.instance.MiniPause(true);
        GlobalStorage.instance.ModalWindowOpen(true);

        playerArmyUI.SetActive(true);
        isWindowOpened = true;
        currentMode = mode;

        Resizing(mode);
        playersArmyUIPart.UpdateArmyWindow();

        if(currentMode == 1) enemyArmyUI.OpenWithPlayerArmy(enemyArmy);
    }

    public void CloseWindow()
    {
        playersArmy.ResetReplaceIndexes();
        playerArmyUI.SetActive(false);

        if(currentMode == 1) enemyArmyUI.CloseWithPlayerArmy();

        isWindowOpened = false;

        MenuManager.instance.MiniPause(false);
        GlobalStorage.instance.ModalWindowOpen(false);
    }

    private void Resizing(int mode)
    {
        playerCuriosity = GlobalStorage.instance.playerStats.GetCurrentParameter(PlayersStats.Curiosity);

        if(mode == 0)
        {
            //enemyBlock.SetActive(false);
            //tombBlock.SetActive(false);
            currentWidth = minWidth;

            closeButton.SetActive(true);
            battleButton.SetActive(false);
            stepbackButton.SetActive(false);
            autobattleButton.SetActive(false);
        }

        if(mode == 1)
        {
            //enemyBlock.SetActive(true);
            //tombBlock.SetActive(false);
            currentWidth = maxWidth;

            closeButton.SetActive(false);
            battleButton.SetActive(true);
            stepbackButton.SetActive(true);

            if(playerCuriosity == 3)
                autobattleButton.SetActive(true);
            else
                autobattleButton.SetActive(false);

            //enemyArmyUIPart.Init();
        }

        if(mode == 2)
        {
            //enemyBlock.SetActive(false);
            //tombBlock.SetActive(true);
            currentWidth = maxWidth;
        }

        rectTransformUI.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentWidth);
        if(coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(ShowWindow(true));
    }

    private IEnumerator ShowWindow(bool mode)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(step * 0.1f);

        float start;
        float end;

        if(mode == true)
        {
            commonButtonsBlock.SetActive(true);

            start = 0;
            end = 1;
            canvasGroup.alpha = start;

            while(canvasGroup.alpha < end)
            {
                canvasGroup.alpha += step;
                yield return delay;
            }
        }
        else
        {
            commonButtonsBlock.SetActive(false);

            start = 1;
            end = 0;
            canvasGroup.alpha = start;

            while(canvasGroup.alpha > end)
            {
                canvasGroup.alpha -= step;
                yield return delay;
            }
        }
    }

    public void Reopen()
    {
        if(coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(ShowWindow(true));
    }

    public void ToTheBattle()
    {
        CloseWindow();
        GlobalStorage.instance.battleManager.InitializeBattle();
    }

    public void AutoBattle()
    {
        GlobalStorage.instance.battleManager.AutoBattle();
        if(coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(ShowWindow(false));
    }
}
