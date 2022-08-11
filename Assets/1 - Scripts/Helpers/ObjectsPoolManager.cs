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

    //storages for created objects
    private List<List<GameObject>> allLists = new List<List<GameObject>>();
    private Dictionary<EnemiesTypes, List<GameObject>> enemiesDict = new Dictionary<EnemiesTypes, List<GameObject>>();
    private List<GameObject> damageTextList = new List<GameObject>();
    private List<GameObject> torchesList    = new List<GameObject>();
    private List<GameObject> bonusExpList   = new List<GameObject>();
    private List<GameObject> bonusGoldList  = new List<GameObject>();
    private List<GameObject> enemyDeathList = new List<GameObject>();
    private List<GameObject> bloodSpotList  = new List<GameObject>();


    private int elementsCount = 10;
    private List<GameObject> currentObjectsList = new List<GameObject>();

    private void Start()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;

        Initialize();
    }

    private void Initialize()
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
        allLists.Add(damageTextList);


        //creating List with tourches
        for (int i = 0; i < elementsCount; i++)
        {
            torchesList.Add(CreateObject(torch));
        }
        allLists.Add(torchesList);


        //creating List with bonusExp
        for (int i = 0; i < elementsCount; i++)
        {
            bonusExpList.Add(CreateObject(bonusExp));
        }
        allLists.Add(bonusExpList);


        //creating List with bonusGold
        for(int i = 0; i < elementsCount; i++)
        {
            bonusGoldList.Add(CreateObject(bonusGold));
        }
        allLists.Add(bonusGoldList);


        //creating List with enemyDeath
        for(int i = 0; i < elementsCount; i++)
        {
            enemyDeathList.Add(CreateObject(enemyDeath));
        }
        allLists.Add(enemyDeathList);


        //creating List with bloodSpot
        for(int i = 0; i < elementsCount; i++)
        {
            bloodSpotList.Add(CreateObject(bloodSpot));
        }
        allLists.Add(bloodSpotList);

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

        foreach(var itemList in allLists)
        {
            for(int i = 0; i < itemList.Count; i++)
            {
                itemList[i].SetActive(false);
            }
        }

        ////clear damageText after battle
        //for (int i = 0; i < damageTextList.Count; i++)
        //{
        //    damageTextList[i].SetActive(false);
        //}

        ////clear torches after battle
        //for (int i = 0; i < torchesList.Count; i++)
        //{
        //    torchesList[i].SetActive(false);
        //}

        ////clear bonusExp after battle
        //for (int i = 0; i < bonusExpList.Count; i++)
        //{
        //    bonusExpList[i].SetActive(false);
        //}

        ////clear bonusGold after battle
        //for(int i = 0; i < bonusGoldList.Count; i++)
        //{
        //    bonusGoldList[i].SetActive(false);
        //}
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
