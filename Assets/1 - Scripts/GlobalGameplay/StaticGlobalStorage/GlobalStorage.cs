using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GlobalStorage : MonoBehaviour
{
    [HideInInspector] public static GlobalStorage instance;

    [Header("Managers")]
    public InputSystem inputSystem;
    public GameStarter gameStarter;
    public GameMaster gameMaster;
    public ObjectsPoolManager objectsPoolManager;
    public MenuManager menuManager;
    public UnitManager unitManager;
    public BoostManager boostManager;
    public BattleManager battleManager;
    public EnemyManager enemyManager;
    public PlayerManager playerManager;
    public MacroLevelUpManager macroLevelUpManager;
    public RunesSystem runesSystem;
    public RunesManager runesManager;
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
    public TombsManager tombsManager;
    public CampManager campManager;
    public AISystem aiSystem;


    [Header("UI")]
    public BattleUIManager battleIUManager;
    public BattleResult battleResultUI;
    public AutobattleUI autobattleUI;
    public GMInterface gmInterface;
    public BonusOnTheMapUI bonusOnTheMapUI;
    public EnemyArmyUI enemyArmyUI;
    public PlayerPersonalWindow playerMilitaryWindow;
    public TemperCommonUIManager commonUIManager;
    public OpeningBuildingWindow fortressBuildingDoor;
    public ResourceBuildingUI resourceBuildingDoor;
    public AltarUI altarDoor;
    public TombUI tombDoor;
    public CampUI campDoor;

    [Header("Player")]
    public GameObject player;
    public GMPlayerMovement globalPlayer;
    public BattleArmyController battlePlayer;
    public HeroController hero;
    public PlayerStats playerStats;
    public PlayersArmy playersArmy;

    [Header("Buildings")]
    public HeroFortress heroFortress;
    public FortressBuildings fortressBuildings;
    public GameObject fortressGO;

    [Header("Maps")]
    public GameObject globalMap;
    public GameObject battleMap;
    public Tilemap roadMap;
    public Tilemap overlayMap;

    [Header("Containers")]
    public GameObject effectsContainer;
    public GameObject bonusesContainer;

    [HideInInspector] public bool isGlobalMode = true;

    [HideInInspector] public bool isEnoughTempExp = false;

    [HideInInspector] public bool isModalWindowOpen = false;

    [HideInInspector] public bool canILoadNextPart = true;

    [HideInInspector] public bool isGameLoaded = false;

    private CameraSwitcher cameraSwitcher;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        cameraSwitcher = Camera.main.GetComponent<CameraSwitcher>();
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

        canILoadNextPart = false;
        macroLevelUpManager.Init();
        while(canILoadNextPart == false)
        {
            yield return null;
        }
        
        canILoadNextPart = false;
        runesSystem.Init();
        while(canILoadNextPart == false)
        {
            yield return null;
        }

        canILoadNextPart = false;
        calendarManager.Init();
        while(canILoadNextPart == false)
        {
            yield return null;
        }

        canILoadNextPart = false;
        gameStarter.Init();
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

    public void SetGlobalMode(bool mode)
    {
        isGlobalMode = mode;
        EventManager.OnSwitchPlayerEvent(mode);
    }

    public bool IsGlobalMode()
    {
        return isGlobalMode;
    }

    public void ChangePlayMode(bool mode)
    {
        //isGlobalMode = mode;
        cameraSwitcher.FadeIn(mode);
        //EventManager.OnChangePlayModeEvent(mode);
    }

    public void ModalWindowOpen(bool mode)
    {
        isModalWindowOpen = mode;
    }
}
