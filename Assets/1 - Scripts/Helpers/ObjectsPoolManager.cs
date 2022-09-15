using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class ObjectsPoolManager : MonoBehaviour
{
    public static ObjectsPoolManager instance;


    //prefabs for Instantiating    
    public List<GameObject> enemyPrefabsList = new List<GameObject>();
    public GameObject damageText;
    public GameObject torch;
    public GameObject bonusExp;
    public GameObject bonusGold;
    public GameObject enemyDeath;
    public GameObject bloodSpot;
    public GameObject enemyOnTheMap;
    public GameObject resourceOnTheMap;
    public GameObject bonusText;

    //storages for created objects
    private List<List<GameObject>> listsToDisableAfterBattle = new List<List<GameObject>>();
    private Dictionary<EnemiesTypes, List<GameObject>> enemiesDict = new Dictionary<EnemiesTypes, List<GameObject>>();
    private List<GameObject> damageTextList = new List<GameObject>();
    private List<GameObject> torchesList    = new List<GameObject>();
    private List<GameObject> bonusExpList   = new List<GameObject>();
    private List<GameObject> bonusGoldList  = new List<GameObject>();
    private List<GameObject> enemyDeathList = new List<GameObject>();
    private List<GameObject> bloodSpotList  = new List<GameObject>();
    private List<GameObject> enemyOnTheMapList = new List<GameObject>();
    private List<GameObject> resourceOnTheMapList = new List<GameObject>();
    private List<GameObject> bonusTextList = new List<GameObject>();


    private int elementsCount = 10;
    private List<GameObject> currentObjectsList = new List<GameObject>();

    private void Awake()
    {
        if(instance != null)
            Destroy(gameObject);
        else
            instance = this;
    }

    public void Initialize()
    {
        CreateObjects();
    }

    private void CreateObjects()
    {
        //creating Dictionary with enemy Lists
        for (int x = 0; x < enemyPrefabsList.Count; x++)
        {
            EnemiesTypes type = enemyPrefabsList[x].GetComponent<EnemyController>().enemiesType;
            List<GameObject> enemiesList = new List<GameObject>();

            for (int i = 0; i < elementsCount; i++)
            {
                enemiesList.Add(CreateObject(enemyPrefabsList[x]));
            }

            enemiesDict.Add(type, enemiesList);
        }


        //creating List with damageText
        for (int i = 0; i < elementsCount; i++)
        {
            damageTextList.Add(CreateObject(damageText));
        }
        listsToDisableAfterBattle.Add(damageTextList);


        //creating List with tourches
        for (int i = 0; i < elementsCount; i++)
        {
            torchesList.Add(CreateObject(torch));
        }
        listsToDisableAfterBattle.Add(torchesList);


        //creating List with bonusExp
        for (int i = 0; i < elementsCount; i++)
        {
            bonusExpList.Add(CreateObject(bonusExp));
        }
        listsToDisableAfterBattle.Add(bonusExpList);


        //creating List with bonusGold
        for(int i = 0; i < elementsCount; i++)
        {
            bonusGoldList.Add(CreateObject(bonusGold));
        }
        listsToDisableAfterBattle.Add(bonusGoldList);


        //creating List with enemyDeath
        for(int i = 0; i < elementsCount; i++)
        {
            enemyDeathList.Add(CreateObject(enemyDeath));
        }
        listsToDisableAfterBattle.Add(enemyDeathList);


        //creating List with bloodSpot
        for(int i = 0; i < elementsCount; i++)
        {
            bloodSpotList.Add(CreateObject(bloodSpot));
        }
        listsToDisableAfterBattle.Add(bloodSpotList);

        //creating List with enemyOnTheMap
        for(int i = 0; i < elementsCount; i++)
        {
            enemyOnTheMapList.Add(CreateObject(enemyOnTheMap));
        }

        //creating List with resourceOnTheMap
        for(int i = 0; i < elementsCount; i++)
        {
            resourceOnTheMapList.Add(CreateObject(resourceOnTheMap));
        }

        //creating List with bonusText
        for(int i = 0; i < elementsCount; i++)
        {
            bonusTextList.Add(CreateObject(bonusText));
        }

        //end of creating objects
        GlobalStorage.instance.LoadNextPart();
    }

    private GameObject CreateObject(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab);
        obj.transform.SetParent(transform);
        obj.SetActive(false);      

        return obj;
    }

    public GameObject GetObjectFromPool(ObjectPool type, EnemiesTypes enemiesTypes = EnemiesTypes.Bat)
    {
        GameObject obj = null;

        switch (type)
        {
            case ObjectPool.Enemy:
                currentObjectsList = enemiesDict[enemiesTypes];
                break;
            case ObjectPool.DamageText:
                currentObjectsList = damageTextList;
                break;
            case ObjectPool.Torch:
                currentObjectsList = torchesList;
                break;

            case ObjectPool.BonusExp:
                currentObjectsList = bonusExpList;
                break;

            case ObjectPool.BonusGold:
                currentObjectsList = bonusGoldList;
                break;

            case ObjectPool.EnemyDeath:
                currentObjectsList = enemyDeathList;
                break;

            case ObjectPool.BloodSpot:
                currentObjectsList = bloodSpotList;
                break;

            case ObjectPool.EnemyOnTheMap:
                currentObjectsList = enemyOnTheMapList;
                break;

            case ObjectPool.ResourceOnTheMap:
                currentObjectsList = resourceOnTheMapList;
                break;

            case ObjectPool.BonusText:
                currentObjectsList = bonusTextList;
                break;
        }

        for (int i = 0; i < currentObjectsList.Count; i++)
        {
            if (currentObjectsList[i].activeInHierarchy == false)
                obj = currentObjectsList[i];
        }

        if (obj == null)
            obj = AddObject(currentObjectsList);

        return obj;
    }

    private GameObject AddObject(List<GameObject> list)
    {
        GameObject newObj;

        newObj = CreateObject(list[0]);
        list.Add(newObj);

        return newObj;
    }

    //TODO: check later how many objects we have in pull after few battle
    //if it's too match - clear it to initial value
    private void AllObjectsFalseAfterBattle()
    {
        //clear enemy after battle
        for (int x = 0; x < enemyPrefabsList.Count; x++)
        {
            EnemiesTypes type = enemyPrefabsList[x].GetComponent<EnemyController>().enemiesType;
            List<GameObject> enemiesList = enemiesDict[type];

            for (int i = 0; i < enemiesList.Count; i++)
            {
                enemiesList[i].SetActive(false);
            }
        }

        foreach(var itemList in listsToDisableAfterBattle)
        {
            for(int i = 0; i < itemList.Count; i++)
            {
                itemList[i].SetActive(false);
            }
        }
    }

    private void OnEnable()
    {
        EventManager.EndOfBattle += AllObjectsFalseAfterBattle;
    }

    private void OnDisable()
    {
        EventManager.EndOfBattle -= AllObjectsFalseAfterBattle;
    }
}
