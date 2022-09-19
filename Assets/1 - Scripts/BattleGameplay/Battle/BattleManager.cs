using System;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class BattleManager : MonoBehaviour
{
    private BattleResult battleResultUI;
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

        if(result == 1)
        {
            //reward window

            Debug.Log("Victory");
            GlobalStorage.instance.enemyManager.DeleteArmy(currentEnemyArmyOnTheMap, currentArmy);
        }

        if(result == 0)
        {
            EventManager.OnDefeatEvent();
            GlobalStorage.instance.player.GetComponent<PlayersArmy>().DefeatDamage();
        }

        if(result == -1)
        {
            GlobalStorage.instance.player.GetComponent<PlayersArmy>().EscapeDamage();
        }

        battleResultUI.Init(result, currentArmy);
    }
    
    //public void ClearEnemyArmy()
    //{
    //    currentEnemies.Clear();
    //    currentEnemiesQuantity.Clear();
    //}

    #endregion

    public void PrepairToTheBattle(Army army, EnemyArmyOnTheMap currentEnemyArmy)
    {
        currentArmy = army;
        currentEnemyArmyOnTheMap = currentEnemyArmy;
        currentEnemySquad = currentEnemyArmy.gameObject;
        //ClearEnemyArmy();
        currentEnemies = army.squadList;
        currentEnemiesQuantity = army.quantityList;

        GlobalStorage.instance.enemyArmyUI.OpenWindow(false, currentEnemyArmy);
    }

    public void AutoBattle()
    {
        //here will be the AutobattleScript

        Debug.Log("Autobattle");
    }
}
