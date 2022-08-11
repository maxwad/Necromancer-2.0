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
    public BattleUIManager battleIUManager;
    public PlayerManager playerManager;
    public ObjectsPoolManager objectsPoolManager;
    public BonusManager bonusManager;
    public ResourcesManager resourcesManager;
    public InfirmaryManager infirmaryManager;
    public SpellManager spellManager;
    public CursorManager cursorManager;

    [Header("Player")]
    public GameObject player;
    public GameObject globalPlayer;
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

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void ChangePlayMode(bool mode)
    {
        isGlobalMode = mode;
        EventManager.OnChangePlayModeEvent(mode);
    }

    private void ExpEnough(bool mode)
    {
        isEnoughTempExp = mode;
    }

    private void OnEnable()
    {
        EventManager.ExpEnough += ExpEnough;
    }

    private void OnDisable()
    {
        EventManager.ExpEnough -= ExpEnough;
    }
}
