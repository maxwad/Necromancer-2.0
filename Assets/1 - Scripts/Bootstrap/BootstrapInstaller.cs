using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

public class BootstrapInstaller : MonoInstaller
{
    #region MANAGERS
    [Header("Managers")]
	[SerializeField] private InputSystem inputSystem;
    [SerializeField] private GameStarter gameStarter;
	[SerializeField] private GameMaster gameMaster;
    [SerializeField] private ObjectsPoolManager objectsPoolManager;
    [SerializeField] private MenuManager menuManager;
    [SerializeField] private UnitManager unitManager;
    [SerializeField] private BoostManager boostManager;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private MacroLevelUpManager macroLevelUpManager;
    [SerializeField] private RunesSystem runesSystem;
    [SerializeField] private RunesManager runesManager;
    [SerializeField] private BonusManager bonusManager;
    [SerializeField] private ResourcesManager resourcesManager;
    [SerializeField] private InfirmaryManager infirmaryManager;
    [SerializeField] private SpellManager spellManager;
    [SerializeField] private CursorManager cursorManager;
    [SerializeField] private GlobalMapTileManager gmManager;
    [SerializeField] private PortalsManager portalsManager;
    [SerializeField] private RewardManager rewardManager;
    [SerializeField] private MapBonusManager mapBonusManager;
    [SerializeField] private MapBoxesManager mapBoxesManager;
    [SerializeField] private CalendarManager calendarManager;
    [SerializeField] private TombsManager tombsManager;
    [SerializeField] private CampManager campManager;
    [SerializeField] private AISystem aiSystem;
    [SerializeField] private SaveLoadManager saveManager;
    #endregion

    #region UI
    [Header("UI")]
    [SerializeField] private BattleUIManager battleIUManager;
    [SerializeField] private BattleResult battleResultUI;
    [SerializeField] private AutobattleUI autobattleUI;
    [SerializeField] private GMInterface gmInterface;
    [SerializeField] private BonusOnTheMapUI bonusOnTheMapUI;
    [SerializeField] private EnemyArmyUI enemyArmyUI;
    [SerializeField] private PlayerPersonalWindow playerMilitaryWindow;
    [SerializeField] private TemperCommonUIManager commonUIManager;
    [SerializeField] private OpeningBuildingWindow fortressBuildingDoor;
    [SerializeField] private ResourceBuildingUI resourceBuildingDoor;
    [SerializeField] private AltarUI altarDoor;
    [SerializeField] private TombUI tombDoor;
    [SerializeField] private CampUI campDoor;
    [SerializeField] private PortalsWindowUI portalDoor;
    [SerializeField] private LoaderUI loaderUI;
    #endregion

    #region OTHER
    [Header("Player")]
    [SerializeField] private GameObject player;
    [SerializeField] private GMPlayerMovement globalPlayer;
    [SerializeField] private BattleArmyController battlePlayer;
    [SerializeField] private HeroController hero;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayersArmy playersArmy;
    [SerializeField] private WeaponStorage weaponStorage;

    [Header("Buildings")]
    [SerializeField] private HeroFortress heroFortress;
    [SerializeField] private FortressBuildings fortressBuildings;
    [SerializeField] private GameObject fortressGO;

    [Header("Maps")]
    [SerializeField] private GameObject globalMap;
    [SerializeField] private GameObject battleMap;
    [SerializeField] private Tilemap roadMap;
    [SerializeField] private Tilemap overlayMap;

    [Header("Containers")]
    [SerializeField] private GameObject effectsContainer;
    [SerializeField] private GameObject bonusesContainer;

    #endregion

    public override void InstallBindings()
    {
        var stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();

        BindGameManagers();

        BindPlayer();

        BindMap();

        BindUI();

        BindOther();

        stopWatch.Stop();
    }

    private void BindService<T>(T instance)
    {
        Container.
            Bind<T>().
            FromInstance(instance).
            AsSingle().
            NonLazy();
    }

    private void BindGameObject<T>(T go, string name)
    {
        Container.Bind<T>().
            WithId(name).
            FromInstance(go).
            //AsSingle().
            NonLazy();
    }

    protected void BindGameManagers()
	{
        BindService(gameMaster);
        BindService(inputSystem);
        BindService(objectsPoolManager);
        BindService(gameStarter);
        BindService(menuManager);
        BindService(aiSystem);
        BindService(saveManager);
        BindService(cursorManager);

        BindService(boostManager);
        BindService(unitManager);
        BindService(battleManager);
        BindService(enemyManager);

        BindService(playerManager);
        BindService(macroLevelUpManager);
        BindService(runesSystem);
        BindService(runesManager);
        BindService(infirmaryManager);
        BindService(spellManager);

        BindService(bonusManager);
        BindService(rewardManager);
        BindService(mapBoxesManager);
        BindService(mapBonusManager);
        BindService(resourcesManager);

        BindService(gmManager);
        BindService(calendarManager);
        BindService(portalsManager);
        BindService(tombsManager);
        BindService(campManager);
    }

    private void BindPlayer()
    {
        //BindGameObject(player, Constants.PLAYER);
        BindService(globalPlayer);
        BindService(battlePlayer);
        BindService(hero);

        BindService(playerStats);
        BindService(playersArmy);
        BindService(weaponStorage);
    }

    private void BindMap()
    {
        BindGameObject(globalMap, Constants.GLOBAL_MAP);
        BindGameObject(battleMap, Constants.BATTLE_MAP);
        BindGameObject(roadMap, Constants.ROAD_MAP);
        BindGameObject(overlayMap, Constants.OVERLAY_MAP);

        BindService(heroFortress);
        BindService(fortressBuildings);
        BindGameObject(fortressGO, Constants.FORTRESS);
    }

    private void BindUI()
    {
        BindService(battleIUManager);
        BindService(battleResultUI);
        BindService(autobattleUI);

        BindService(playerMilitaryWindow);
        BindService(enemyArmyUI);

        BindService(gmInterface);
        BindService(bonusOnTheMapUI);

        BindService(fortressBuildingDoor);
        BindService(resourceBuildingDoor);
        BindService(altarDoor);
        BindService(tombDoor);
        BindService(campDoor);
        BindService(portalDoor);

        BindService(loaderUI);
        BindService(commonUIManager);
    }

    private void BindOther()
    {
        BindGameObject(effectsContainer, Constants.EFFECTS_CONTAINER);
        BindGameObject(bonusesContainer, Constants.BONUS_CONTAINER);
    }
}
