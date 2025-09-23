using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static Enums;

public class ObjectsPoolManager : MonoBehaviour
{
    //public ObjectsPoolManager instance;
    public DiContainer container;

    public List<ObjectPoolObjects> objectsList;
    private Dictionary<ObjectPool, List<GameObject>> allObjectsDict = new Dictionary<ObjectPool, List<GameObject>>();

    public List<GameObject> enemyPrefabsList = new List<GameObject>();
    private Dictionary<EnemiesTypes, List<GameObject>> enemiesDict = new Dictionary<EnemiesTypes, List<GameObject>>();

    public List<ObjectPoolWeapon> weaponList;
    private Dictionary<UnitsAbilities, List<GameObject>> allWeaponsDict = new Dictionary<UnitsAbilities, List<GameObject>>();

    public List<ObjectPoolBossWeapon> bossWeaponList;
    private Dictionary<BossWeapons, List<GameObject>> allBossWeaponList = new Dictionary<BossWeapons, List<GameObject>>();

    private Dictionary<string, List<GameObject>> unusualPrefabs = new Dictionary<string, List<GameObject>>();

    private int minElementsCount = 5;
    private List<GameObject> currentObjectsList = new List<GameObject>();

    [Inject]
    public void Construct(DiContainer container)
    {
        this.container = container;

        //Initialize();
    }

    public void Initialize()
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

        for(int x = 0; x < bossWeaponList.Count; x++)
        {
            List<GameObject> objectList = new List<GameObject>();
            for(int i = 0; i < minElementsCount; i++)
                objectList.Add(CreateObject(bossWeaponList[x].weapon));

            allBossWeaponList.Add(bossWeaponList[x].type, objectList);
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
        //GameObject obj = Instantiate(prefab);
        //Debug.Log("Create " + prefab);
        GameObject obj = container.InstantiatePrefab(prefab);
        obj.name = prefab.name;
        obj.transform.SetParent(transform);
        obj.SetActive(false);      

        return obj;
    }

    //NOTICE:
    // in GET-functions we need condition:
    // currentObjectsList[i].activeInHierarchy == false && i != 0
    // because otherwise we take GO which is on the Scene with different parameters.
    // With this condition we always will be have dafault object

    public GameObject GetEnemy(EnemiesTypes enemyTypes)
    {
        GameObject obj = null;
        currentObjectsList = enemiesDict[enemyTypes];

        for(int i = 0; i < currentObjectsList.Count; i++)
        {
            if(currentObjectsList[i].activeInHierarchy == false && i != 0)
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
            if(currentObjectsList[i].activeInHierarchy == false && i != 0)
                obj = currentObjectsList[i];
        }

        if(obj == null)
            obj = AddObject(currentObjectsList);

        return obj;
    }

    public GameObject GetBossWeapon(BossWeapons type)
    {
        GameObject obj = null;

        currentObjectsList = allBossWeaponList[type];

        for(int i = 0; i < currentObjectsList.Count; i++)
        {
            if(currentObjectsList[i].activeInHierarchy == false && i != 0)
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

        currentObjectsList.RemoveAll(c => c == null);

        for (int i = 0; i < currentObjectsList.Count; i++)
        {
            //if(currentObjectsList[i] == null)
            //{
            //    currentObjectsList.Remove(currentObjectsList[i]);
            //    continue;
            //}

            if (currentObjectsList[i].activeSelf == false && i != 0)
            {
                obj = currentObjectsList[i];
                break;
            }
        }

        if (obj == null)
            obj = AddObject(currentObjectsList);

        obj.transform.rotation = Quaternion.identity;

        return obj;
    }

    private GameObject AddObject(List<GameObject> list)
    {
        GameObject newObj;

        newObj = CreateObject(list[0]);
        list.Add(newObj);

        return newObj;
    }

    // This method was created after reworking project for using Zenject
    // I know, that is ugly, but someday i will rework the whole poll manager
    // I hope...
    public GameObject GetUnusualPrefab(GameObject prefab, bool needActive = true)
    {
        GameObject obj = null;

        if(unusualPrefabs.ContainsKey(prefab.name))
        {
            currentObjectsList = unusualPrefabs[prefab.name];

            currentObjectsList.RemoveAll(c => c == null);

            for(int i = 0; i < currentObjectsList.Count; i++)
            {
                if(currentObjectsList[i].activeSelf == false && i != 0)
                {
                    obj = currentObjectsList[i];
                    break;
                }
            }

            if(obj == null)
                obj = AddObject(currentObjectsList);
        }
        else
        {
            List<GameObject> list = new List<GameObject>();
            obj = CreateObject(prefab);
            list.Add(obj);
            unusualPrefabs[prefab.name] = list;

            obj = GetUnusualPrefab(prefab);
        }

        obj.transform.rotation = Quaternion.identity;
        return obj;
    }
}
