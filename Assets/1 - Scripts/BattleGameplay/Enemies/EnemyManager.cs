using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class EnemyManager : MonoBehaviour
{
    public List<EnemySO> allEnemiesSO;
    public List<EnemiesTypes> allEnemiesTypes;
    public Dictionary<EnemiesTypes, EnemySO> allEnemiesSODict = new Dictionary<EnemiesTypes, EnemySO>();

    [HideInInspector] public EnemyArragement enemyArragement;
    private Dictionary<EnemyArmyOnTheMap, Vector3> enemiesPointsDict = new Dictionary<EnemyArmyOnTheMap, Vector3>();

    [HideInInspector] public EnemySquadGenerator enemySquadGenerator;

    public float growUpConst = 10;
    public int weeksInMonth = 3;

    public void InitializeEnemies()
    {
        enemyArragement = GetComponent<EnemyArragement>();
        enemySquadGenerator = GetComponent<EnemySquadGenerator>();

        CreateEnemiesDict();

        enemySquadGenerator.SetAllEnemiesList(allEnemiesTypes);
        enemyArragement.GenerateEnemiesOnTheMap(this);

        GlobalStorage.instance.LoadNextPart();
    }

    private void CreateEnemiesDict()
    {
        foreach(EnemiesTypes type in Enum.GetValues(typeof(EnemiesTypes)))
        {
            for(int i = 0; i < allEnemiesSO.Count; i++)
            {
                if(allEnemiesSO[i].EnemiesType == type)
                {
                    allEnemiesTypes.Add(type);
                    allEnemiesSODict[type] = allEnemiesSO[i];
                    break;
                }
            }
        }            
    }

    public EnemySO GetEnemySO(EnemiesTypes enemiesTypes)
    {
        if(allEnemiesSODict.ContainsKey(enemiesTypes) == true)
        {
            return allEnemiesSODict[enemiesTypes];
        }
        else
        {
            return null;
        }
    }

    public List<EnemiesTypes> GetEnemiesList()
    {
        return allEnemiesTypes;
    }

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
        enemiesPointsDict.Remove(enemyGO);
        enemySquadGenerator.RemoveArmy(army);
        enemyGO.Death();
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
            enemy.Key.Death(true);

        yield return delay;

        //wait till all enemies dissapear
        bool canIContinue = false;
        while(canIContinue == false)
        {
            canIContinue = true;
            foreach(var enemy in enemiesPointsDict)
            {
                if(enemy.Key.isEnemyGarrison == false && enemy.Key.gameObject.activeInHierarchy == true)
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


    public EnemyArmyOnTheMap CreateEnemyOnTheMap(Vector3 position)
    {
        return enemyArragement.CreateEnemyOnTheMap(position);
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
