using System;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class BattleManager : MonoBehaviour
{
    //for testing!
    public List<GameObject> falsEnemyArmy;
    public List<int> falseEnemiesQuantity;

    public List<GameObject> allEnemiesList = new List<GameObject>();
    private List<int> enemiesQuantity = new List<int>();
    private List<GameObject> enemiesArmy = new List<GameObject>();

    [SerializeField] private int sizeMapX = 50;
    [SerializeField] private int sizeMapY = 50;
    private int positionZ = 40; // we should send double size of Z because dividing by 2

    [SerializeField] private EnemySpawner enemySpawner;

    public void SetAllEnemiesList(List<GameObject> enemies)
    {
        allEnemiesList = enemies;
    }

    //this info should give object of battle
    public void GetCurrentEnemiesArmy(EnemiesTypes[] enemyType, List<int> quantity)
    {
        foreach (EnemiesTypes itemType in enemyType)
        {
            for (int i = 0; i < allEnemiesList.Count; i++)
            {
                Enemy enemy = allEnemiesList[i].GetComponent<Enemy>();
                if (enemy.EnemiesType == itemType)
                {
                    enemiesArmy.Add(allEnemiesList[i]);
                    enemiesQuantity.Add(quantity[i]);
                }
            }            
        }
    }

    public void InitializeBattle()
    {
        GlobalStorage.instance.ChangePlayMode(false);

        //for testing!
        if (enemiesArmy.Count == 0)
        {
            enemiesArmy = falsEnemyArmy;
            enemiesQuantity = falseEnemiesQuantity;
        }

        enemySpawner.Initialize(enemiesArmy, enemiesQuantity);
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

    public void FinishTheBattle()
    {
        GlobalStorage.instance.ChangePlayMode(true);
    }
    
    public void ClearEnemyArmy()
    {
        enemiesArmy.Clear();
        enemiesQuantity.Clear();
    }

    //public void ChangePlayer(bool mode)
    //{
    //    Debug.Log("SwitchPlayer");
    //}

    //private void OnEnable()
    //{
    //    EventManager.ChangePlayMode += ChangePlayer;
    //}

    //private void OnDisable()
    //{
    //    EventManager.ChangePlayMode -= ChangePlayer;
    //}
}
