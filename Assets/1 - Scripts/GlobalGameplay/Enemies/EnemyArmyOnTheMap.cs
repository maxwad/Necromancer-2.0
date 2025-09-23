using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static Enums;

public class EnemyArmyOnTheMap : MonoBehaviour
{
    private BattleManager battleManager;
    private EnemyManager enemyManager;
    private EnemySquadGenerator enemySquadGenerator;

    [HideInInspector] public Army army;
    [HideInInspector] public int commonCount = 0;
    private float splitPercent = 0.7f;
    private float decreaseSiegePercent = 0.1f;
    private float decreasePortion;

    public TypeOfArmy typeOfArmy = TypeOfArmy.OnTheMap;
    public bool isEnemyGarrison = false;

    private SpriteRenderer sprite;

    [Inject]
    public void Construct(BattleManager battleManager, EnemyManager enemyManager)
    {
        this.battleManager = battleManager;
        this.enemyManager = enemyManager;

        sprite = GetComponent<SpriteRenderer>();
        enemySquadGenerator = enemyManager.enemySquadGenerator;
    }

    private void ResetGarrison()
    {
        if(isEnemyGarrison == true)
            Birth();
    }

    public void Birth(Army sourceArmy = null) 
    {
        StartCoroutine(Initialize(sourceArmy));

        if(typeOfArmy == TypeOfArmy.NearUsualObjects || typeOfArmy == TypeOfArmy.OnTheMap)
        {
            sprite.color = new Color(Random.value, Random.value, Random.value);
            StartCoroutine(Blink(true));
        }
    }

    private IEnumerator Initialize(Army sourceArmy)
    {
        while(GlobalStorage.instance == null || GlobalStorage.instance.isGameLoaded == false)
        {
            yield return null;
        }

        if(sourceArmy == null)
            army = enemyManager.enemySquadGenerator.GenerateArmy(typeOfArmy);
        else
            army = sourceArmy;

        commonCount = 0;

        for(int i = 0; i < army.squadList.Count; i++)
            commonCount += army.quantityList[i];
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
        float growUpConst = percent / 100;
        commonCount = 0;

        for(int i = 0; i < army.squadList.Count; i++)
        {
            army.quantityList[i] += Mathf.RoundToInt(army.quantityList[i] * growUpConst);
            commonCount += army.quantityList[i];
        }
    }

    public void DecreaseSquads(int siegeLevel)
    {
        if(enemySquadGenerator == null)
        {
            enemySquadGenerator = enemyManager.enemySquadGenerator;
            decreasePortion = enemySquadGenerator.GetPortionAmount();
        }
        siegeLevel = (siegeLevel == 0) ? 1 : siegeLevel;
        float decreaseAmount = decreasePortion * siegeLevel * decreaseSiegePercent * 0.1f;
        commonCount = 0;

        for(int i = 0; i < army.squadList.Count; i++)
        {
            army.quantityList[i] = Mathf.RoundToInt(army.quantityList[i] - decreaseAmount);
            
            if(army.quantityList[i] <= 0)
                army.quantityList[i] = 1;

            commonCount += army.quantityList[i];
        }
    }

    public int GetCommonAmountArmy()
    {
        return commonCount;
    }

    private IEnumerator Blink(bool isBorning)
    {
        WaitForSeconds appearDelay = new WaitForSeconds(0.02f);
        float alfaFrom = 0;
        float alfaTo = 1;
        float step = 0.075f;

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

    public void PrepairToTheBattle(bool enemyInitiative = false)
    {
        battleManager.PrepairToTheBattle(army, this, enemyInitiative);
    }

    public void Splitting()
    {
        CreateFromVassalsArmy();

        Army newArmy = Army.Splitting(army, 1 - splitPercent, true);
        army = newArmy;
    }

    private void CreateFromVassalsArmy()
    {
        EnemyArmyOnTheMap newSquad = enemyManager.CreateEnemyOnTheMap(transform.position);
        Army newArmy = Army.Splitting(army, splitPercent, false);
        newSquad.Birth(newArmy);
    }

    private void OnEnable()
    {
        EventManager.ResetGarrisons += ResetGarrison;
    }

    private void OnDisable()
    {
        EventManager.ResetGarrisons -= ResetGarrison;
    }

    public EnemySD SaveEnemy()
    {
        EnemySD saveData = new EnemySD();

        saveData.position = transform.position.ToVec3();
        saveData.army = army;
        saveData.typeOfArmy = typeOfArmy;
        saveData.isEnemyGarrison = isEnemyGarrison;
        saveData.color = new Vector3(sprite.color.r, sprite.color.g, sprite.color.b).ToVec3();

        return saveData;
    }

    public void LoadEnemy(EnemySD saveData)
    {
        army = saveData.army;
        isEnemyGarrison = saveData.isEnemyGarrison;
        typeOfArmy = saveData.typeOfArmy;
        commonCount = 0;

        if(army != null)
        {
            for(int i = 0; i < army.squadList.Count; i++)
                commonCount += army.quantityList[i];
        }

        sprite.color = new Color(saveData.color.x, saveData.color.y, saveData.color.z);
    }
}
