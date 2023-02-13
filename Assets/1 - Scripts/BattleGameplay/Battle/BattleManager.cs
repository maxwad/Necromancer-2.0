using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class BattleManager : MonoBehaviour
{
    private BattleResult battleResultUI;
    private Autobattle autobattle;
    private PlayerPersonalWindow playerArmyWindow;
    private AISystem aiSystem;

    [SerializeField] private int sizeMapX = 50;
    [SerializeField] private int sizeMapY = 50;
    private int positionZ = 40; // we should send double size of Z because dividing by 2

    [Header("Army")]
    [SerializeField] private EnemySpawner enemySpawner;

    //for testing!
    public List<GameObject> falsEnemyArmy;
    public List<int> falseEnemiesQuantity;

    private Army currentArmy = new Army();
    private EnemyArmyOnTheMap currentEnemyArmyOnTheMap;

    private bool isFightWithVassal = false;
    private bool isVassalWin = false;


    private void Start()
    {
        autobattle       = GetComponent<Autobattle>();
        battleResultUI   = GlobalStorage.instance.battleResultUI;
        playerArmyWindow = GlobalStorage.instance.playerMilitaryWindow;
        aiSystem         = GlobalStorage.instance.aiSystem;
    }

    #region Starting Initialisation

    public void InitializeBattle()
    {
        if (currentArmy == null)
        {
            Debug.Log("We don't have an army!");
            return;
        }

        GlobalStorage.instance.ChangePlayMode(false);        
    }

    public Army GetArmy()
    {
        return currentArmy;
    }

    public Vector3Int GetBattleMapSize()
    {
        return new Vector3Int(sizeMapX, sizeMapY, positionZ);
    }

    //this info should give object of battle
    public void SetBattleMapSize(int width = 10, int heigth = 10)
    {
        sizeMapX = width;
        sizeMapY = heigth;
    }

    public void FinishTheBattle(bool isAutobattle, int result, float percentOfReward = 100)
    {
        if(isAutobattle == false) 
            GlobalStorage.instance.ChangePlayMode(true);

        StartCoroutine(WaitGlobalMode(result, percentOfReward));
    }

    private IEnumerator WaitGlobalMode(int result, float percentOfReward)
    {
        while(GlobalStorage.instance.isGlobalMode == false)
        {
            yield return null;
        }

        isVassalWin = ((result == 0 || result == -1) && isFightWithVassal == true);

        battleResultUI.Init(result, percentOfReward, currentEnemyArmyOnTheMap, currentArmy);
    }

    #endregion

    public void PrepairToTheBattle(Army army, EnemyArmyOnTheMap currentEnemyArmy, bool enemyInitiative = false)
    {
        currentArmy = army;
        currentEnemyArmyOnTheMap = currentEnemyArmy;
        isFightWithVassal = enemyInitiative;

        playerArmyWindow.OpenWindow(PlayersWindow.Battle, currentEnemyArmy, enemyInitiative);
    }

    public void ReopenPreBattleWindow()
    {
        playerArmyWindow.OpenWindow(PlayersWindow.Battle, currentEnemyArmyOnTheMap);
    }

    public void AutoBattle()
    {
        autobattle.Preview(currentArmy);
    }

    public void SetVassalVictory(bool victoryMode)
    {
        isVassalWin = victoryMode;
    }

    public void TryToContinueEnemysTurn()
    {
        aiSystem.HandleBattleResult(isVassalWin);

        isVassalWin = false;
    }
}
