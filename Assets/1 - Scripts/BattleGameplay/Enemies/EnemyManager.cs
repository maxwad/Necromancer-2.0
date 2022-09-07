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

    private EnemySquadGenerator enemySquadGenerator;

    public void InitializeEnemies()
    {
        boostManager = GlobalStorage.instance.unitBoostManager;
        enemyArragement = GetComponent<EnemyArragement>();
        enemySquadGenerator = GetComponent<EnemySquadGenerator>();

        GetAllEnemiesBase();
        enemyArragement.GenerateEnemiesOnTheMap(this);

        GlobalStorage.instance.canILoadNextStep = true;
    }

    #region CREATE ENEMY (GO)

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

    public void DeleteArmy(EnemyArmyOnTheMap enemyGO, Army army)
    {
        Destroy(enemyGO.gameObject);
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

    public void ReGenerateEnemiesGO()
    {

    }

    public Army GenerateArmy(ArmyStrength strength)
    {
        Army army = enemySquadGenerator.GenerateArmy(strength);
        return army;
    }
}
