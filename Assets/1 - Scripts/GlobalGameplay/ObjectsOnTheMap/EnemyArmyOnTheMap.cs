using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArmyOnTheMap : MonoBehaviour
{
    private BattleManager battleManager;

    public List<GameObject> currentEnemiesList = new List<GameObject>();
    public List<int> currentEnemiesQuantityList = new List<int>();
    private Army army;
    [HideInInspector] public int commonCount = 0;

    private void OnEnable()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        while(GlobalStorage.instance == null || GlobalStorage.instance.isGameLoaded == false)
        {
            yield return null;
        }

        if(battleManager == null) battleManager = GlobalStorage.instance.battleManager;

        army = battleManager.GenerateArmy();

        currentEnemiesList = army.squadList;
        currentEnemiesQuantityList = army.quantityList;

        for(int i = 0; i < currentEnemiesList.Count; i++)
        {
            commonCount += currentEnemiesQuantityList[i];
        }

        Debug.Log("Count of army = " + commonCount);
    }

    public void GenerateNewArmy()
    {
        StartCoroutine(Initialize());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(TagManager.T_PLAYER) == true)
        {
            PrepairToTheBattle();
        }
    }
    private void PrepairToTheBattle()
    {
        battleManager.PrepairToTheBattle(army);
    }

}
