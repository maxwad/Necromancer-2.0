using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<EnemySO> allEnemiesSO;

    public List<Enemy> allEnemiesBase = new List<Enemy>();
    public List<Enemy> allBoostEnemies = new List<Enemy>();
    public List<GameObject> finalEnemiesListGO = new List<GameObject>();

    private UnitBoostManager boostManager;

    private void Start()
    {
        boostManager = GlobalStorage.instance.unitBoostManager;

        GetAllEnemiesBase();
    }

    private void GetAllEnemiesBase()
    {
        foreach (var item in allEnemiesSO)
            allEnemiesBase.Add(new Enemy(item));

        GetAllEnemiesBoost();
    }

    private void GetAllEnemiesBoost()
    {
        foreach (Enemy item in allEnemiesBase)
            allBoostEnemies.Add(boostManager.AddBonusStatsToEnemy(item));

        GetFinalEnemiesList();
    }

    private void GetFinalEnemiesList()
    {
        foreach (Enemy item in allBoostEnemies)
        {
            GameObject enemy = item.enemyGO;
            enemy.GetComponent<EnemyController>().Initialize(item);
            finalEnemiesListGO.Add(enemy);            
        }

        GlobalStorage.instance.battleManager.SetAllEnemiesList(finalEnemiesListGO);
    }

}
