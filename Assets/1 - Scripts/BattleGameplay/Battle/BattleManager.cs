using System;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class Army
{
    public List<GameObject> squadList = new List<GameObject>();
    public List<int> quantityList = new List<int>();
    public bool isThisASiege = false;
    // more parameters
}
public class BattleManager : MonoBehaviour
{
    [SerializeField] private int sizeMapX = 50;
    [SerializeField] private int sizeMapY = 50;
    private int positionZ = 40; // we should send double size of Z because dividing by 2

    [Header("Army")]
    [SerializeField] private EnemySpawner enemySpawner;

    //for testing!
    public List<GameObject> falsEnemyArmy;
    public List<int> falseEnemiesQuantity;

    public List<GameObject> allEnemiesList = new List<GameObject>();
    private Army currentArmy = new Army();
    private List<GameObject> currentEnemies = new List<GameObject>();
    private List<int> currentEnemiesQuantity = new List<int>();


    private List<Army> allArmies = new List<Army>();

    private float playerLevel;
    private float countOfSquad;
    float sizeOfSquadMultiplier;
    float enemyQuantityDivider = 2;
    float enemySizeDivider = 5;
    float enemyPortion = 300;
    float percentGap = 0.2f;


    #region Starting Initialisation

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
                    currentEnemies.Add(allEnemiesList[i]);
                    currentEnemiesQuantity.Add(quantity[i]);
                }
            }            
        }
    }

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

    public void FinishTheBattle()
    {
        GlobalStorage.instance.ChangePlayMode(true);
    }
    
    public void ClearEnemyArmy()
    {
        currentEnemies.Clear();
        currentEnemiesQuantity.Clear();
    }

    #endregion

    #region Army's Generation

    public Army GenerateArmy()
    { 
        playerLevel = GlobalStorage.instance.playerStats.GetCurrentParameter(PlayersStats.Level);

        countOfSquad = Mathf.Ceil(playerLevel / enemyQuantityDivider);
        sizeOfSquadMultiplier = Mathf.Ceil(playerLevel / enemySizeDivider);

        Army newArmy = new Army();

        for(int i = 0; i < countOfSquad; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, allEnemiesList.Count);
            GameObject randomSquad = allEnemiesList[randomIndex];
            newArmy.squadList.Add(randomSquad);

            int randomQuantity = (int)sizeOfSquadMultiplier * Mathf.RoundToInt(
                UnityEngine.Random.Range((enemyPortion - enemyPortion * percentGap), (enemyPortion + enemyPortion * percentGap))
                );
            newArmy.quantityList.Add(randomQuantity);
        }

        allArmies.Add(newArmy);
        return newArmy;
    }

    public void PrepairToTheBattle(Army army)
    {
        currentArmy = army;

        ClearEnemyArmy();
        currentEnemies = army.squadList;
        currentEnemiesQuantity = army.quantityList;
    }

    #endregion
}
