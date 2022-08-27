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
    public PlayerManager playerManager;
    public ObjectsPoolManager objectsPoolManager;
    public BonusManager bonusManager;
    public ResourcesManager resourcesManager;
    public InfirmaryManager infirmaryManager;
    public SpellManager spellManager;
    public CursorManager cursorManager;
    public GlobalMapTileManager gmManager;
    public PortalsManager portalsManager;

    [Header("UI")]
    public BattleUIManager battleIUManager;
    public PlayerGMInterface gmInterface;
    public TemperCommonUIManager commonUIManager;

    [Header("Player")]
    public GameObject player;
    public GMPlayerMovement globalPlayer;
    public BattleArmyController battlePlayer;
    public HeroController hero;

    [Header("Maps")]
    public GameObject globalMap;
    public GameObject battleMap;

    [Header("Containers")]
    public GameObject effectsContainer;
    public GameObject bonusesContainer;

    [HideInInspector] public bool isGlobalMode = true;

    [HideInInspector] public bool isEnoughTempExp = false;

    [HideInInspector] public bool isModalWindowOpen = false;

    [HideInInspector] public bool canILoadNextStep = true;

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
        canILoadNextStep = false;
        gmManager.Load();        

        while(canILoadNextStep == false)
        {
            yield return null;
        }

        canILoadNextStep = false;
        unitManager.LoadUnits();        

        while(canILoadNextStep == false)
        {
            yield return null;
        }

        Debug.Log("GAME IS LOADED!");        
    }

    public void ChangePlayMode(bool mode)
    {
        isGlobalMode = mode;
        EventManager.OnChangePlayModeEvent(mode);
    }
}
