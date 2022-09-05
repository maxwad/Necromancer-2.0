using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class EnemyArmyOnTheMap : MonoBehaviour
{
    private BattleManager battleManager;

    public List<GameObject> currentEnemiesList = new List<GameObject>();
    public List<int> currentEnemiesQuantityList = new List<int>();
    private Army army;
    [HideInInspector] public int commonCount = 0;

    public ArmyStrength armyStrength = ArmyStrength.Low;

    private SpriteRenderer sprite;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = new Color(Random.value, Random.value, Random.value);
    }

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

        army = battleManager.GenerateArmy(armyStrength);

        currentEnemiesList = army.squadList;
        currentEnemiesQuantityList = army.quantityList;

        for(int i = 0; i < currentEnemiesList.Count; i++)
        {
            commonCount += currentEnemiesQuantityList[i];
        }
    }

    //for new Month
    public void GenerateNewArmy()
    {
        StartCoroutine(Initialize());
    }

    public void PrepairToTheBattle()
    {
        battleManager.PrepairToTheBattle(army, this);
    }

}
