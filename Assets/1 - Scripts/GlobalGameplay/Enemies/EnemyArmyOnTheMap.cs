using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class EnemyArmyOnTheMap : MonoBehaviour
{
    private BattleManager battleManager;
    private EnemyManager enemyManager;

    [HideInInspector] public Army army;
    [HideInInspector] public int commonCount = 0;

    public TypeOfArmy typeOfArmy = TypeOfArmy.OnTheMap;
    public bool isEnemyGarrison = false;

    private SpriteRenderer sprite;

    private void OnEnable()
    {
        EventManager.ResetGarrisons += ResetGarrison;

        if(sprite == null)
        {
            battleManager = GlobalStorage.instance.battleManager;
            enemyManager = GlobalStorage.instance.enemyManager;

            sprite = GetComponent<SpriteRenderer>();
        } 

        Birth();
    }

    private void OnDisable()
    {
        EventManager.ResetGarrisons -= ResetGarrison;
    }

    private void ResetGarrison()
    {
        if(typeOfArmy != TypeOfArmy.NearUsualObjects 
            && typeOfArmy != TypeOfArmy.OnTheMap 
            && typeOfArmy != TypeOfArmy.Vassals)
        {
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }
    }

    public void Birth() 
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        while(GlobalStorage.instance == null || GlobalStorage.instance.isGameLoaded == false)
        {
            yield return null;
        }

        if(typeOfArmy != TypeOfArmy.Vassals)
            enemyManager.enemyArragement.RegisterEnemy(this);

        army = enemyManager.enemySquadGenerator.GenerateArmy(typeOfArmy);
        commonCount = 0;

        for(int i = 0; i < army.squadList.Count; i++)
        {
            commonCount += army.quantityList[i];
        }

        if(typeOfArmy == TypeOfArmy.NearUsualObjects || typeOfArmy == TypeOfArmy.OnTheMap)
        {
            sprite.color = new Color(Random.value, Random.value, Random.value);
            StartCoroutine(Blink(true));
        }
    }

    public void Death(bool resetMode = false)
    {
        if(isEnemyGarrison == false)
        {
            if(typeOfArmy != TypeOfArmy.Vassals)
                StartCoroutine(Blink(false));
        }
        else
        {
            if(resetMode == false)
            {
                Destroy(this);
            }
        }
    }

    public void GrowUpSquads(float percent)
    {
        if(typeOfArmy == TypeOfArmy.InCastle)
        {

            Debug.Log("Castle Grow");
        }

        if(typeOfArmy == TypeOfArmy.Vassals)
        {

            Debug.Log("Vassal Grow");
        }
        float growUpConst = percent / 100;
        commonCount = 0;

        for(int i = 0; i < army.squadList.Count; i++)
        {
            army.quantityList[i] += Mathf.RoundToInt(army.quantityList[i] * growUpConst);
            commonCount += army.quantityList[i];
        }
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

    public void PrepairToTheBattle()
    {
        battleManager.PrepairToTheBattle(army, this);
    }

}
