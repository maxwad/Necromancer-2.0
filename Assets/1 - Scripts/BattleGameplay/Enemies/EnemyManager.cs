using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<EnemySO> allEnemiesSO;

    public List<Enemy> allEnemiesBase = new List<Enemy>();
    public List<Enemy> allBoostEnemies = new List<Enemy>();
    public List<GameObject> finalEnemiesListGO = new List<GameObject>();
    public List<EnemyController> finalEnemiesListEC = new List<EnemyController>();

    private UnitBoostManager boostManager;

    private EnemyArragement enemyArragement;
    private Dictionary<EnemyArmyOnTheMap, Vector3> enemiesPointsDict = new Dictionary<EnemyArmyOnTheMap, Vector3>();

    [HideInInspector] public EnemySquadGenerator enemySquadGenerator;

    public float growUpConst = 10;
    public int weeksInMonth = 3;

    public void InitializeEnemies()
    {
        boostManager = GlobalStorage.instance.unitBoostManager;
        enemyArragement = GetComponent<EnemyArragement>();
        enemySquadGenerator = GetComponent<EnemySquadGenerator>();

        CreateAllEnemiesBase();
        enemyArragement.GenerateEnemiesOnTheMap(this);

        GlobalStorage.instance.LoadNextPart();
    }

    #region CREATE ENEMY FOR BATTLE (GO)

    private void CreateAllEnemiesBase()
    {
        foreach (var item in allEnemiesSO)
            allEnemiesBase.Add(new Enemy(item));

        CreatetAllEnemiesBoost();
    }

    private void CreatetAllEnemiesBoost()
    {
        foreach (Enemy item in allEnemiesBase)
            allBoostEnemies.Add(boostManager.AddBonusStatsToEnemy(item));

        CreateFinalEnemiesListGO();
    }

    private void CreateFinalEnemiesListGO()
    {
        foreach (Enemy item in allBoostEnemies)
        {
            GameObject enemy = item.enemyGO;
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            enemyController.Initialize(item);
            finalEnemiesListGO.Add(enemy);
            finalEnemiesListEC.Add(enemyController);
        }

        enemySquadGenerator.SetAllEnemiesList(finalEnemiesListGO);
    }

    #endregion

    public void SetEnemiesPointsDict(Dictionary<EnemyArmyOnTheMap, Vector3> points)
    {
        enemiesPointsDict = points;
    }

    public Dictionary<EnemyArmyOnTheMap, Vector3> GetEnemiesPointsDict()
    {
        return enemiesPointsDict;
    }

    public void DeleteArmy(EnemyArmyOnTheMap enemyGO, Army army)
    {
        enemyGO.Death();
        enemiesPointsDict.Remove(enemyGO);
        enemySquadGenerator.RemoveArmy(army);
    }

    public EnemyArmyOnTheMap CheckPositionInEnemyPoints(Vector3 position)
    {
        EnemyArmyOnTheMap enemyArmy = null;
        foreach(var army in enemiesPointsDict)
        {
            if(army.Value == position) 
            {
                enemyArmy = army.Key;
                break;
            }
        }
        return enemyArmy;
    }



    public void ReGenerateEnemiesOnTheMap()
    {
        StartCoroutine(ResetEnemies());
    }

    private IEnumerator ResetEnemies()
    {
        WaitForSeconds delay = new WaitForSeconds(0.1f);

        foreach(var enemy in enemiesPointsDict)
            enemy.Key.Death();

        yield return delay;

        //wait till all enemies dissapear
        bool canIContinue = false;
        while(canIContinue == false)
        {
            canIContinue = true;
            foreach(var enemy in enemiesPointsDict)
            {
                if(enemy.Key.gameObject.activeInHierarchy == true)
                {
                    canIContinue = false;
                    break;
                }
            }

            yield return delay;
        }

        enemiesPointsDict.Clear();
        enemySquadGenerator.ClearAllArmies();

        enemyArragement.GenerateEnemiesOnTheMap(this);
    }

    private void GrowUp(int weekCounter)
    {
        //every new month we regenerate Armies, so we don't need to this action
        if(weekCounter % weeksInMonth != 0)
        {
            foreach(var enemy in enemiesPointsDict)
                enemy.Key.GrowUpSquads(growUpConst);
        }        
    }

    private void OnEnable()
    {
        EventManager.NewMonth += ReGenerateEnemiesOnTheMap;
        EventManager.NewWeek += GrowUp;
    }

    private void OnDisable()
    {
        EventManager.NewMonth -= ReGenerateEnemiesOnTheMap;
        EventManager.NewWeek -= GrowUp;
    }
}
