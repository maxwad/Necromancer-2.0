using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class EnemyArmyOnTheMap : MonoBehaviour
{
    private BattleManager battleManager;
    private EnemyManager enemyManager;

    public List<GameObject> currentEnemiesList = new List<GameObject>();
    public List<int> currentEnemiesQuantityList = new List<int>();
    private Army army;
    [HideInInspector] public int commonCount = 0;

    public ArmyStrength armyStrength = ArmyStrength.Low;
    public TypeOfArmy typeOfArmy = TypeOfArmy.OnTheMap;

    private SpriteRenderer sprite;

    private void OnEnable()
    {
        if(sprite == null)
        {
            sprite = GetComponent<SpriteRenderer>();
            sprite.color = new Color(Random.value, Random.value, Random.value);
        } 

        Birth();
    }

    private IEnumerator Initialize()
    {
        while(GlobalStorage.instance == null || GlobalStorage.instance.isGameLoaded == false)
        {
            yield return null;
        }

        if(battleManager == null) battleManager = GlobalStorage.instance.battleManager;
        if(enemyManager == null) enemyManager = GlobalStorage.instance.enemyManager;

        army = enemyManager.enemySquadGenerator.GenerateArmy(armyStrength, typeOfArmy);

        currentEnemiesList = army.squadList;
        currentEnemiesQuantityList = army.quantityList;

        for(int i = 0; i < currentEnemiesList.Count; i++)
        {
            commonCount += currentEnemiesQuantityList[i];
        }
    }

    private void Birth() 
    {
        StartCoroutine(Initialize());
        StartCoroutine(Blink(true));
    }

    public void Death()
    {
        StartCoroutine(Blink(false));
    }

    private IEnumerator Blink(bool isBorning)
    {
        WaitForSeconds appearDelay = new WaitForSeconds(0.05f);
        float alfaFrom = 0;
        float alfaTo = 1;
        float step = 0.05f;

        if(isBorning == false)
        {
            alfaFrom = 1;
            alfaTo = 0;
            step = -step;
        }

        Color currentColor = sprite.color;
        currentColor.a = alfaFrom;
        sprite.color = currentColor;

        bool stop = false;

        while(stop == false)
        {
            alfaFrom += step;
            currentColor.a = alfaFrom;
            sprite.color = currentColor;

            if(step > 0)
            {
                if(alfaFrom >= alfaTo) stop = true;
            }
            else
            {
                if(alfaFrom <= alfaTo) stop = true;
            }

            yield return appearDelay;
        }

        if(isBorning == false) gameObject.SetActive(false);
    }

    //for new Month
    public void GenerateNewEnemiesOnTheMap()
    {
        StartCoroutine(Initialize());
    }

    public void PrepairToTheBattle()
    {
        battleManager.PrepairToTheBattle(army, this);
    }

}
