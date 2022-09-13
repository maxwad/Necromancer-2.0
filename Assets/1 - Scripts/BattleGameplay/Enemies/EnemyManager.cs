using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class EnemyManager : MonoBehaviour
{
    public List<EnemySO> allEnemiesSO;

    public List<Enemy> allEnemiesBase = new List<Enemy>();
    public List<Enemy> allBoostEnemies = new List<Enemy>();
    public List<GameObject> finalEnemiesListGO = new List<GameObject>();

    private UnitBoostManager boostManager;

    private EnemyArragement enemyArragement;
    private Dictionary<EnemyArmyOnTheMap, Vector3> enemiesPointsDict = new Dictionary<EnemyArmyOnTheMap, Vector3>();

    [HideInInspector] public EnemySquadGenerator enemySquadGenerator;

    public void InitializeEnemies()
    {
        boostManager = GlobalStorage.instance.unitBoostManager;
        enemyArragement = GetComponent<EnemyArragement>();
        enemySquadGenerator = GetComponent<EnemySquadGenerator>();

        GetAllEnemiesBase();
        enemyArragement.GenerateEnemiesOnTheMap(this);

        GlobalStorage.instance.LoadNextPart();
    }

    #region CREATE ENEMY FOR BATTLE (GO)

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

        GetFinalEnemiesListGO();
    }

    private void GetFinalEnemiesListGO()
    {
        foreach (Enemy item in allBoostEnemies)
        {
            GameObject enemy = item.enemyGO;
            enemy.GetComponent<EnemyController>().Initialize(item);
            finalEnemiesListGO.Add(enemy);            
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



    private void OnEnable()
    {
        EventManager.NewMonth += ReGenerateEnemiesOnTheMap;
    }

    private void OnDisable()
    {
        EventManager.NewMonth -= ReGenerateEnemiesOnTheMap;
    }
}
