using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static NameManager;
using System;

public class BattleUIManager : MonoBehaviour
{
    public Canvas uiCanvas;
    [HideInInspector] public bool isBattleOver = false;

    private float blinkTime = 0.005f;

    [Header("End of Battle")]
    [SerializeField] private int timer = 10;
    [SerializeField] private int currentTime = 10;
    private Coroutine redirectCoroutine;
    
    [SerializeField] private GameObject leaveBlock;
    private bool isLeaveBlockOpened = false;
    [SerializeField] private GameObject victoryBlock;
    [SerializeField] private TMP_Text victoryTimer;
    [SerializeField] private CanvasGroup victoryCanvasGroup;

    [SerializeField] private GameObject defeatBlock;
    [SerializeField] private TMP_Text defeatTimer;
    [SerializeField] private CanvasGroup defeatCanvasGroup;
    
    [HideInInspector] public BattleUIExpPart expPart;
    [HideInInspector] public BattleUIHeroPart heroPart;
    [HideInInspector] public BattleUIInfirmaryPart infirmaryPart;
    [HideInInspector] public BattleUISpellPart spellPart;
    [HideInInspector] public BattleUIEnemyPart enemyPart;
    [HideInInspector] public BattleUIBoostPart boostPart;

    private void Start()
    {
        expPart = GetComponent<BattleUIExpPart>();
        expPart.Init(this);

        heroPart = GetComponent<BattleUIHeroPart>();
        heroPart.Init(this);

        infirmaryPart = GetComponent<BattleUIInfirmaryPart>();
        infirmaryPart.Init(this);

        spellPart = GetComponent<BattleUISpellPart>();
        spellPart.Init(this);

        enemyPart = GetComponent<BattleUIEnemyPart>();
        enemyPart.Init(this);

        boostPart = GetComponent<BattleUIBoostPart>();
        boostPart.Init(this);
    }

    #region Helpers

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            if(GlobalStorage.instance.isGlobalMode == false) OpenLeaveBlock(!isLeaveBlockOpened);
        }

        if(Input.GetKeyDown(KeyCode.End))
        {
            if(GlobalStorage.instance.isGlobalMode == false && isBattleOver == false) Victory();
        }
        if(Input.GetKeyDown(KeyCode.Delete))
        {
            if(GlobalStorage.instance.isGlobalMode == false && isBattleOver == false) Defeat();
        }
        if(Input.GetKeyDown(KeyCode.Home))
        {
            if(GlobalStorage.instance.isGlobalMode == false && isBattleOver == false) LeaveTheBattle();
        }
    }

    public void Inizialize(bool mode)
    {   
        uiCanvas.gameObject.SetActive(!mode);
        isBattleOver = false;

        if(mode == false) ResetCanvas();
    }
    
    private void ResetCanvas()
    {
        expPart.FillRigthTempLevelScale();

        infirmaryPart.FillInfirmary();

        heroPart.GetMaxManaHealth();
        heroPart.FillHealth();
        heroPart.FillMana();
        heroPart.FillGold();

        spellPart.FillSpells(-1);

        enemyPart.FillDeadEnemiesBar(null);

        boostPart.FillPlayerBoost();
    }

    public void Blink(Image panel, Color effectColor, Color normalColor, float divider = 5)
    {
        panel.color = effectColor;
        StartCoroutine(ColorBack(panel, normalColor, divider));
    }

    private IEnumerator ColorBack(Image panel, Color normalColor, float divider)
    {
        //the bigger divider the slower animation
        float time = 0;

        while(panel.color != normalColor)
        {
            time += Time.deltaTime;
            panel.color = Color.Lerp(panel.color, normalColor, time / divider);
            yield return new WaitForSeconds(blinkTime);
        }
    }

    public void OpenLeaveBlock(bool mode)
    {
        if(isBattleOver == true) return;

        isLeaveBlockOpened = mode;
        GlobalStorage.instance.ModalWindowOpen(mode);
        MenuManager.instance.MiniPause(mode);
        leaveBlock.SetActive(mode);
    }

    public void LeaveTheBattle()
    {
        OpenLeaveBlock(false);
        GlobalStorage.instance.battleManager.FinishTheBattle(false, -1);
    }

    public void ShowVictoryBlock()
    {
        isBattleOver = true;
        victoryBlock.SetActive(true);
        EventManager.OnVictoryEvent();
        Fading.instance.Fade(true, victoryCanvasGroup, 0.005f, activeMode: false);
        redirectCoroutine = StartCoroutine(Timer(victoryTimer));
    }

    public void Victory()
    {
        victoryBlock.SetActive(false);
        if(redirectCoroutine != null) StopCoroutine(redirectCoroutine);
        GlobalStorage.instance.battleManager.FinishTheBattle(false, 1);
    }

    public void ShowDefeatBlock()
    {
        isBattleOver = true;
        defeatBlock.SetActive(true);
        Fading.instance.Fade(true, defeatCanvasGroup, 0.005f, activeMode: false);
        redirectCoroutine = StartCoroutine(Timer(defeatTimer));
    }

    public void Defeat()
    {
        defeatBlock.SetActive(false);
        if(redirectCoroutine != null) StopCoroutine(redirectCoroutine);
        GlobalStorage.instance.battleManager.FinishTheBattle(false, 0);
    }


    private IEnumerator Timer(TMP_Text label)
    {
        WaitForSeconds delay = new WaitForSeconds(1f);

        for(int i = timer; i >= 0; i--)
        {
            label.text = "(" + i + ")";
            yield return delay;
        }

        if(label == victoryTimer)
            Victory();
        else
            Defeat();
    }

    #endregion

    private void OnEnable()
    {
        EventManager.SwitchPlayer   += Inizialize;
    }

    private void OnDisable()
    {
        EventManager.SwitchPlayer   -= Inizialize;
    }
}
