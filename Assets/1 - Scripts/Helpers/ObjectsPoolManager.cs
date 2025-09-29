using Enums;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ObjectsPoolManager : MonoBehaviour
{
    //public ObjectsPoolManager instance;
    public DiContainer diContainer;
    public Transform root;

    private List<MonoBehaviour> instances;

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
        this.diContainer = container;
    }

    public void Initialize()
    {
        for(int x = 0; x < enemyPrefabsList.Count; x++)
        {
            EnemiesTypes type = enemyPrefabsList[x].GetComponent<EnemyController>().enemiesType;
            List<GameObject> enemiesList = new List<GameObject>();

            for(int i = 0; i < minElementsCount; i++)
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
        GameObject obj = diContainer.InstantiatePrefab(prefab);
        obj.name = prefab.name;
        obj.transform.SetParent(transform);
        obj.SetActive(false);

        return obj;
    }

    //NOTICE:
    // in GET-functions we need condition:
    // currentObjectsList[i].activeInHierarchy == false && i != 0
    // because otherwise we'll take GO which is on the Scene with different parameters.
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

        for(int i = 0; i < currentObjectsList.Count; i++)
        {
            if(currentObjectsList[i].activeSelf == false && i != 0)
            {
                obj = currentObjectsList[i];
                break;
            }
        }

        if(obj == null)
        {
            obj = AddObject(currentObjectsList);
        }

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

    private Dictionary<MonoBehaviour, Stack<MonoBehaviour>> discardedPrefabsPools = new();
    private Dictionary<MonoBehaviour, List<(MonoBehaviour instance, MonoBehaviour prefab)>> usedInstancesPool = new Dictionary<MonoBehaviour, List<(MonoBehaviour, MonoBehaviour)>>();

    public MonoBehaviour GetOrCreateElement(MonoBehaviour proto, MonoBehaviour forSource, Transform parent, bool setActive = true)
    {
        if(parent == null)
        {
            parent = root;
        }

        MonoBehaviour instance = GetInstance<MonoBehaviour>(proto, forSource, parent, setActive);
        instance.gameObject.SetActive(setActive);
        instances.Add(instance);

        return instance;
    }

    public T GetInstance<T>(MonoBehaviour prefab, MonoBehaviour forSource, Transform parent, bool setItemActive = true) where T : class
    {
        if(prefab == null)
        {
            Debug.LogError("Cannot get instance of a null prefab.");
            return null;
        }

        //Пробуем забрать инстанс из пула
        if(!discardedPrefabsPools.TryGetValue(prefab, out Stack<MonoBehaviour> discardedPool))
        {
            discardedPool = new Stack<MonoBehaviour>();
            discardedPrefabsPools[prefab] = discardedPool;
        }

        MonoBehaviour instance;
        T newInstance;

        if(discardedPool.Count > 0)
        {
            instance = discardedPool.Pop();
            instance.transform.SetParent(parent, false);
            newInstance = instance as T;
        }
        else
        {
            // В пуле таких нет, так что создаём новый
            newInstance = diContainer.InstantiatePrefab(prefab, parent).GetComponent<T>();
            instance = newInstance as MonoBehaviour;
        }

        //Закрепляем инстанс за родителем
        if(!usedInstancesPool.ContainsKey(forSource))
        {
            usedInstancesPool[forSource] = new List<(MonoBehaviour, MonoBehaviour)>();
        }

        //Закрепляем префаб за инстансом
        usedInstancesPool[forSource].Add((instance, prefab));

        instance.gameObject.SetActive(setItemActive);
        TryCallOnBeginUse(instance);

        return newInstance;
    }

    public void TryCallOnBeginUse<T>(T instance) where T : MonoBehaviour
    {
        IDiscardableInstance discardableInstance = instance as IDiscardableInstance;

        if(discardableInstance != null)
        {
            discardableInstance.OnBeginUse();
        }
    }

    public void TryCallOnDiscard<T>(T instance) where T : MonoBehaviour
    {
        IDiscardableInstance discardableInstance = instance as IDiscardableInstance;

        if(discardableInstance != null)
        {
            discardableInstance.OnDiscard();
        }
    }

    public void DiscardAll(MonoBehaviour forSource)
    {
        if(!usedInstancesPool.ContainsKey(forSource))
        {
            Debug.Log("THERE IS NO PREFABS FOR " + forSource.gameObject.name);
            return;
        }

        foreach(var item in usedInstancesPool[forSource])
        {
            DiscardByInstance(item.instance, forSource);
        }
    }

    public void DiscardByInstance<T>(T instance, MonoBehaviour forSource) where T : MonoBehaviour
    {
        if(!usedInstancesPool.ContainsKey(forSource))
        {
            Debug.Log("THERE IS NO INSTANCE " + instance.gameObject.name + " FOR " + forSource.gameObject.name);
            return;
        }

        (MonoBehaviour instance, MonoBehaviour prefab) foundedPair = (null, null);
        foreach(var pair in usedInstancesPool[forSource])
        {
            if(pair.instance == instance)
            {
                foundedPair = pair;
                break;
            }
        }

        if(foundedPair.prefab != null)
        {
            TryCallOnDiscard(instance);
            usedInstancesPool[forSource].Remove(foundedPair);
            discardedPrefabsPools[foundedPair.prefab].Push(instance);

            instance.gameObject.SetActive(false);
            instance.transform.SetParent(root, false);
        }
        else
        {
            Debug.Log("THERE IS NO INSTANCE " + instance.gameObject.name + " FOR " + forSource.gameObject.name);
        }
    }

    public void DiscardByPrefab<T>(T prefab, MonoBehaviour forSource) where T : MonoBehaviour
    {
        if(!usedInstancesPool.ContainsKey(forSource))
        {
            Debug.Log("THERE IS NO PREFAB " + prefab.gameObject.name + " FOR " + forSource.gameObject.name);
            return;
        }

        (MonoBehaviour instance, MonoBehaviour prefab) foundedPair = (null, null);
        foreach(var pair in usedInstancesPool[forSource])
        {
            if(pair.prefab == prefab)
            {
                foundedPair = pair;
                break;
            }
        }

        if(foundedPair.instance != null)
        {
            TryCallOnDiscard(foundedPair.instance);
            usedInstancesPool[forSource].Remove(foundedPair);
            discardedPrefabsPools[foundedPair.prefab].Push(foundedPair.instance);

            foundedPair.instance.gameObject.SetActive(false);
            foundedPair.instance.transform.SetParent(root, false);
        }
        else
        {
            Debug.Log("THERE IS NO INSTANCE " + foundedPair.instance.gameObject.name + " FOR " + forSource.gameObject.name);
        }
    }
}
