using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class ObjectsPoolManager : MonoBehaviour
{
    public static ObjectsPoolManager instance;

    [Serializable]
    public class ObjectPoolObjects
    {
        public ObjectPool type;
        public GameObject obj;
    }

    [Serializable]
    public class ObjectPoolWeapon
    {
        public UnitsAbilities type;
        public GameObject weapon;
    }

    public List<ObjectPoolObjects> objectsList;
    private Dictionary<ObjectPool, List<GameObject>> allObjectsDict = new Dictionary<ObjectPool, List<GameObject>>();

    public List<GameObject> enemyPrefabsList = new List<GameObject>();
    private Dictionary<EnemiesTypes, List<GameObject>> enemiesDict = new Dictionary<EnemiesTypes, List<GameObject>>();

    public List<ObjectPoolWeapon> weaponList;
    private Dictionary<UnitsAbilities, List<GameObject>> allWeaponsDict = new Dictionary<UnitsAbilities, List<GameObject>>();

    private int minElementsCount = 10;
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
        CreateDicts();
    }

    private void CreateDicts()
    {
        for (int x = 0; x < enemyPrefabsList.Count; x++)
        {
            EnemiesTypes type = enemyPrefabsList[x].GetComponent<EnemyController>().enemiesType;
            List<GameObject> enemiesList = new List<GameObject>();

            for (int i = 0; i < minElementsCount; i++)
                enemiesList.Add(CreateObject(enemyPrefabsList[x]));

            enemiesDict.Add(type, enemiesList);
        }

        for(int x = 0; x < weaponList.Count; x++)
        {
            List<GameObject> objectList = new List<GameObject>();
            for(int i = 0; i < minElementsCount; i++)
                objectList.Add(CreateObject(weaponList[x].weapon));

            allWeaponsDict.Add(weaponList[x].type, objectList);
        }

        for(int x = 0; x < objectsList.Count; x++)
        {
            List<GameObject> objectList = new List<GameObject>();
            for(int i = 0; i < minElementsCount; i++)
                objectList.Add(CreateObject(objectsList[x].obj));

            allObjectsDict.Add(objectsList[x].type, objectList);
        }

        //end of creating objects
        GlobalStorage.instance.LoadNextPart();
    }

    private GameObject CreateObject(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab);
        obj.name = prefab.name;
        obj.transform.SetParent(transform);
        obj.SetActive(false);      

        return obj;
    }

    public GameObject GetEnemy(EnemiesTypes enemyTypes)
    {
        GameObject obj = null;
        currentObjectsList = enemiesDict[enemyTypes];

        for(int i = 0; i < currentObjectsList.Count; i++)
        {
            if(currentObjectsList[i].activeInHierarchy == false)
                obj = currentObjectsList[i];
        }

        if(obj == null)
            obj = AddObject(currentObjectsList);

        return obj;
    }

    public GameObject GetWeapon(UnitsAbilities type)
    {
        GameObject obj = null;

        currentObjectsList = allWeaponsDict[type];

        for(int i = 0; i < currentObjectsList.Count; i++)
        {
            if(currentObjectsList[i].activeInHierarchy == false)
                obj = currentObjectsList[i];
        }

        if(obj == null)
            obj = AddObject(currentObjectsList);

        return obj;
    }

    public GameObject GetObject(ObjectPool type)
    {
        GameObject obj = null;        

        currentObjectsList = allObjectsDict[type];

        for (int i = 0; i < currentObjectsList.Count; i++)
        {
            if (currentObjectsList[i].activeInHierarchy == false)
            {
                obj = currentObjectsList[i];
                break;
            }
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
}
