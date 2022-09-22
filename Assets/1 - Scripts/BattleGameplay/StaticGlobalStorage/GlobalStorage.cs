using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalStorage : MonoBehaviour
{
    [HideInInspector] public static GlobalStorage instance;

    [Header("Managers")]
    public MenuManager menuManager;
    public UnitManager unitManager;
    public UnitBoostManager unitBoostManager;
    public PlayerBoostManager playerBoostManager;
    public BattleManager battleManager;
    public EnemyManager enemyManager;
    public PlayerManager playerManager;
    public ObjectsPoolManager objectsPoolManager;
    public BonusManager bonusManager;
    public ResourcesManager resourcesManager;
    public InfirmaryManager infirmaryManager;
    public SpellManager spellManager;
    public CursorManager cursorManager;
    public GlobalMapTileManager gmManager;
    public PortalsManager portalsManager;
    public RewardManager rewardManager;
    public MapBonusManager mapBonusManager;
    public MapBoxesManager mapBoxesManager;
    public CalendarManager calendarManager;


    [Header("UI")]
    public BattleUIManager battleIUManager;
    public BattleResult battleResultUI;
    public GMInterface gmInterface;
    public BonusOnTheMapUI bonusOnTheMapUI;
    public EnemyArmyUI enemyArmyUI;
    public PlayerMilitaryWindow playerMilitaryWindow;
    public TemperCommonUIManager commonUIManager;

    [Header("Player")]
    public GameObject player;
    public GMPlayerMovement globalPlayer;
    public BattleArmyController battlePlayer;
    public HeroController hero;
    public PlayerStats playerStats;

    [Header("Maps")]
    public GameObject globalMap;
    public GameObject battleMap;

    [Header("Containers")]
    public GameObject effectsContainer;
    public GameObject bonusesContainer;

    [HideInInspector] public bool isGlobalMode = true;

    [HideInInspector] public bool isEnoughTempExp = false;

    [HideInInspector] public bool isModalWindowOpen = false;

    [HideInInspector] public bool canILoadNextPart = true;

    [HideInInspector] public bool isGameLoaded = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(LoadTheGame());
    }

    private IEnumerator LoadTheGame()
    {
        canILoadNextPart = false;
        objectsPoolManager.Initialize();
        while(canILoadNextPart == false)
        {
            yield return null;
        }

        canILoadNextPart = false;
        gmManager.Load();
        while(canILoadNextPart == false)
        {
            yield return null;
        }

        canILoadNextPart = false;
        unitManager.LoadUnits();
        while(canILoadNextPart == false)
        {
            yield return null;
        }

        canILoadNextPart = false;
        enemyManager.InitializeEnemies();
        while(canILoadNextPart == false)
        {
            yield return null;
        }

        canILoadNextPart = false;
        mapBonusManager.InitializeHeaps();
        while(canILoadNextPart == false)
        {
            yield return null;
        }

        isGameLoaded = true;
        Debug.Log("GAME IS LOADED!");        
    }

    public void LoadNextPart()
    {
        canILoadNextPart = true;
    }

    public void ChangePlayMode(bool mode)
    {
        isGlobalMode = mode;
        EventManager.OnChangePlayModeEvent(mode);
    }

    public void ModalWindowOpen(bool mode)
    {
        isModalWindowOpen = mode;
    }
}
