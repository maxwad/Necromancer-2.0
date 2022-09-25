using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private BattleResult battleResultUI;
    private Autobattle autobattle;
    private PlayerMilitaryWindow playerArmyWindow;

    [SerializeField] private int sizeMapX = 50;
    [SerializeField] private int sizeMapY = 50;
    private int positionZ = 40; // we should send double size of Z because dividing by 2

    [Header("Army")]
    [SerializeField] private EnemySpawner enemySpawner;

    //for testing!
    public List<GameObject> falsEnemyArmy;
    public List<int> falseEnemiesQuantity;

    private Army currentArmy = new Army();
    private GameObject currentEnemySquad;
    private EnemyArmyOnTheMap currentEnemyArmyOnTheMap;
    private List<GameObject> currentEnemies = new List<GameObject>();
    private List<int> currentEnemiesQuantity = new List<int>();


    private void Start()
    {
        battleResultUI = GlobalStorage.instance.battleResultUI;
        autobattle = GetComponent<Autobattle>();
        playerArmyWindow = GlobalStorage.instance.playerMilitaryWindow;
    }

    #region Starting Initialisation

    public void InitializeBattle()
    {
        GlobalStorage.instance.ChangePlayMode(false);

        //for testing!
        if (currentEnemies.Count == 0)
        {
            currentEnemies = falsEnemyArmy;
            currentEnemiesQuantity = falseEnemiesQuantity;
        }

        enemySpawner.Initialize(currentEnemies, currentEnemiesQuantity);
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

    public void FinishTheBattle(int result)
    {
        GlobalStorage.instance.ChangePlayMode(true);

        battleResultUI.Init(result, currentEnemyArmyOnTheMap, currentArmy);
    }    

    #endregion

    public void PrepairToTheBattle(Army army, EnemyArmyOnTheMap currentEnemyArmy)
    {
        currentArmy = army;
        currentEnemyArmyOnTheMap = currentEnemyArmy;
        currentEnemySquad = currentEnemyArmy.gameObject;
        currentEnemies = army.squadList;
        currentEnemiesQuantity = army.quantityList;

        playerArmyWindow.OpenWindow(1, currentEnemyArmy);
    }

    public void ReopenPreBattleWindow()
    {
        playerArmyWindow.Reopen();
    }

    public void AutoBattle()
    {
        autobattle.Preview(currentArmy);
        Debug.Log("Autobattle");
    }
}
