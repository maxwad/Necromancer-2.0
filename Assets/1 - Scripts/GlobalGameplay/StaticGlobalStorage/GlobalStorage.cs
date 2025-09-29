using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

public class GlobalStorage : MonoBehaviour
{
    [HideInInspector] public static GlobalStorage instance;

    private GameStarter gameStarter;
    private UnitManager unitManager;
    private ObjectsPoolManager objectsPoolManager;
    private EnemyManager enemyManager;
    private MacroLevelUpManager macroLevelUpManager;
    private RunesSystem runesSystem;
    private GlobalMapTileManager gmManager;
    private MapBonusManager mapBonusManager;
    private CalendarManager calendarManager;
    public LoaderUI loaderUI;

    [HideInInspector] public bool isGlobalMode = true;

    [HideInInspector] public bool isModalWindowOpen = false;

    [HideInInspector] public bool canILoadNextPart = true;

    [HideInInspector] public bool isGameLoaded = false;

    private CameraSwitcher cameraSwitcher;

    [Inject]
    public void Construct
        (
        GameStarter gameStarter,
        UnitManager unitManager,
        ObjectsPoolManager objectsPoolManager,
        EnemyManager enemyManager,
        MacroLevelUpManager macroLevelUpManager,
        RunesSystem runesSystem,
        GlobalMapTileManager gmManager,
        MapBonusManager mapBonusManager,
        CalendarManager calendarManager,
        LoaderUI loaderUI
        )
    {
        this.gameStarter = gameStarter;
        this.unitManager = unitManager;
        this.objectsPoolManager = objectsPoolManager;
        this.enemyManager = enemyManager;
        this.macroLevelUpManager = macroLevelUpManager;
        this.runesSystem = runesSystem;
        this.gmManager = gmManager;
        this.mapBonusManager = mapBonusManager;
        this.calendarManager = calendarManager;
        this.loaderUI = loaderUI;
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartGame(bool createMode)
    {
        cameraSwitcher = Camera.main.GetComponent<CameraSwitcher>();
        StartCoroutine(LoadTheGame(createMode));
    }

    private IEnumerator LoadTheGame(bool createMode)
    {
        //1
        canILoadNextPart = false;
        objectsPoolManager.Initialize();
        while(canILoadNextPart == false)
        {
            yield return null;
        }

        //2
        canILoadNextPart = false;
        gmManager.Init(createMode);
        while(canILoadNextPart == false)
        {
            yield return null;
        }

        //3
        canILoadNextPart = false;
        unitManager.LoadUnits();
        while(canILoadNextPart == false)
        {
            yield return null;
        }

        //4
        canILoadNextPart = false;
        enemyManager.InitializeEnemies(createMode);
        while(canILoadNextPart == false)
        {
            yield return null;
        }

        //5
        canILoadNextPart = false;
        mapBonusManager.InitializeHeaps(createMode);
        while(canILoadNextPart == false)
        {
            yield return null;
        }

        //6
        canILoadNextPart = false;
        macroLevelUpManager.Init();
        while(canILoadNextPart == false)
        {
            yield return null;
        }

        //7
        canILoadNextPart = false;
        runesSystem.Init();
        while(canILoadNextPart == false)
        {
            yield return null;
        }

        //8
        canILoadNextPart = false;
        calendarManager.Init(createMode);
        while(canILoadNextPart == false)
        {
            yield return null;
        }

        //9
        canILoadNextPart = false;
        gameStarter.Init(createMode);
        while(canILoadNextPart == false)
        {
            yield return null;
        }

        isGameLoaded = true;

        if(createMode == true)
            loaderUI.Close();


        Debug.Log("GAME IS LOADED!");        
    }


    public void LoadNextPart() => canILoadNextPart = true;

    public void SetGlobalMode(bool mode)
    {
        isGlobalMode = mode;
        EventManager.OnSwitchPlayerEvent(mode);
    }

    public bool IsGlobalMode() => isGlobalMode;

    public void ChangePlayMode(bool mode) => cameraSwitcher.FadeIn(mode);

    public void ModalWindowOpen(bool mode) => isModalWindowOpen = mode;

}
