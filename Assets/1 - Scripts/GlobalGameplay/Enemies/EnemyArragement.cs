using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static NameManager;

public class EnemyArragement : MonoBehaviour
{
    private GlobalMapTileManager gmManager;
    private EnemyManager enemyManager;
    private ObjectsPoolManager poolManager;
    private TombsManager tombsManager;
    private AISystem aiSystem;

    public GameObject enemiesMap;
    public Tilemap roadMap;

    public int distanceBetweenEnemies = 15;
    public int countEnemiesPerVertical = 3;
    public int enemiesGap = 5;
    private int randomStartPointX;
    public Tile fogTile;
    //public Tile testTile;

    private Dictionary<GameObject, Vector3> enterPointsDict = new Dictionary<GameObject, Vector3>();
    private Dictionary<EnemyArmyOnTheMap, Vector3> enemiesPointsDict = new Dictionary<EnemyArmyOnTheMap, Vector3>();

    public void GenerateEnemiesOnTheMap(EnemyManager manager)
    {
        if(enemyManager == null)
        {
            enemyManager = manager;
            gmManager = GlobalStorage.instance.gmManager;
            poolManager = GlobalStorage.instance.objectsPoolManager;
            tombsManager = GlobalStorage.instance.tombsManager;
            aiSystem = GlobalStorage.instance.aiSystem;
        }

        enterPointsDict = gmManager.GetEnterPoints();

        GenerateEnterEnemies();
        GenerateRandomEnemies();

        //GenerateGarrison(new List<GameObject>(tombsManager.GetTombs().Keys), TypeOfArmy.InTomb);
        //GenerateGarrison(new List<GameObject>(aiSystem.GetCastles()), TypeOfArmy.InCastle);
        //GenerateTombsGarrisons();

        enemyManager.SetEnemiesPointsDict(enemiesPointsDict);
    }

    //private void GenerateTombsGarrisons()
    //{
    //    Dictionary<GameObject, TombInfo> tombsDict = tombsManager.GetTombs();

    //    foreach(var tomb in tombsDict)
    //    {
    //        ObjectOwner objectStatus = tomb.Key.GetComponent<ObjectOwner>();
    //        EnemyArmyOnTheMap enemyGarrison = tomb.Key.GetComponent<EnemyArmyOnTheMap>();

    //        if(enemyGarrison == null)
    //        {
    //            if(objectStatus.GetVisitStatus() == false)
    //            {
    //                EnemyArmyOnTheMap newGarrison = tomb.Key.AddComponent(typeof(EnemyArmyOnTheMap)) as EnemyArmyOnTheMap;
    //                newGarrison.typeOfArmy = TypeOfArmy.InTomb;
    //                newGarrison.isEnemyGarrison = true;

    //                RegisterEnemy(newGarrison, tomb.Value.position);
    //            }
    //        }
    //        else
    //        {
    //            enemyGarrison.Birth();
    //        }
    //    }
    //}

    private void GenerateGarrison(List<GameObject> garrissonObjects, TypeOfArmy typeOfArmy)
    {
        foreach(var building in garrissonObjects)
        {
            ObjectOwner objectStatus = building.GetComponent<ObjectOwner>();
            EnemyArmyOnTheMap enemyGarrison = building.GetComponent<EnemyArmyOnTheMap>();

            if(enemyGarrison == null)
            {
                if(objectStatus.GetVisitStatus() == false)
                {
                    //EnemyArmyOnTheMap newGarrison = building.AddComponent(typeof(EnemyArmyOnTheMap)) as EnemyArmyOnTheMap;

                    Debug.Log("Create");
                    EnemyArmyOnTheMap newGarrison = new EnemyArmyOnTheMap();
                    Debug.Log("After");
                    newGarrison.typeOfArmy = typeOfArmy;
                    newGarrison.isEnemyGarrison = true;
                    newGarrison = building.AddComponent(typeof(EnemyArmyOnTheMap)) as EnemyArmyOnTheMap;

                    RegisterEnemy(newGarrison, building.transform.position);
                }
            }
            else
            {
                enemyGarrison.Birth();
            }
        }
    }
    #region GUARD ENEMIES

    private void GenerateEnterEnemies()
    {
        foreach(var enterPoint in enterPointsDict)
        {
            if(CanICreateEnterEnemy(enterPoint.Key, enterPoint.Value) == true)
            {
                CreateUsualEnemy(enterPoint.Value);
            }
        }
    }

    private bool CanICreateEnterEnemy(GameObject building, Vector3 position)
    {
        bool canICreateEnemy = false;

        ObjectOwner objectInfo = building.GetComponent<ObjectOwner>();

        if(objectInfo != null)
        {
            if(objectInfo.isGuardNeeded == true)
            {
                if(objectInfo.probabilityGuard >= Random.Range(0, 101))
                {
                    canICreateEnemy = true;
                }
            }
        }

        if(canICreateEnemy == true) canICreateEnemy = CheckPlayerPosition(position);

        return canICreateEnemy;
    }

    #endregion

    #region RANDOM ENEMIES

    private void GenerateRandomEnemies()
    {
        int width = roadMap.size.x;
        int height = roadMap.size.y;
        randomStartPointX = Random.Range(10, 20); 

        List<Vector3> tempWorldPositions = new List<Vector3>();
        List<Vector3Int> tempCellPositions = new List<Vector3Int>();

        for(int currentX = randomStartPointX; currentX < width; currentX += distanceBetweenEnemies)
        {
            tempWorldPositions.Clear();
            tempCellPositions.Clear();

            for(int currentY = 0; currentY <= height; currentY++)
            {
                Vector3Int cellPosition = new Vector3Int(currentX, currentY, 0);
                if(roadMap.HasTile(cellPosition))
                {
                    Vector3 checkPosition = roadMap.CellToWorld(cellPosition);
                    tempCellPositions.Add(cellPosition);
                    tempWorldPositions.Add(checkPosition);                    
                }                
            }

            int verticalEnemyCount = tempWorldPositions.Count / countEnemiesPerVertical;
            verticalEnemyCount = (verticalEnemyCount == 0) ? 1 : verticalEnemyCount;            

            for(int i = 0; i < tempCellPositions.Count; i += verticalEnemyCount)
            {
                //roadMap.SetTile(tempCellPositions[i], testTile);                
                if(CheckPosition(tempWorldPositions[i]) == true)
                {
                    CreateUsualEnemy(tempWorldPositions[i]);
                }
            }
        }
    }

    private bool CheckPosition(Vector3 position)
    {
        bool isPositionFree = false;        
        int currentSearchIndex = 0;
        foreach(var point in enterPointsDict)
        {
            if(Vector3.Distance(point.Value, position) < enemiesGap)
            {
                //roadMap.SetTile(roadMap.WorldToCell(position), fogTile);
                isPositionFree = false;
                break;
            }
            else
            {
                isPositionFree = true;
                currentSearchIndex++;                
            }
        }

        if(isPositionFree == true) isPositionFree = CheckPlayerPosition(position);

        return isPositionFree;        
    }

    #endregion

    private bool CheckPlayerPosition(Vector3 position)
    {
        return !(GlobalStorage.instance.globalPlayer.transform.position == position);
    }

    private void CreateUsualEnemy(Vector3 position)
    {
        GameObject enemyOnTheMap = poolManager.GetObject(ObjectPool.EnemyOnTheMap);
        enemyOnTheMap.transform.SetParent(enemiesMap.transform);
        enemyOnTheMap.transform.position = position;
        enemyOnTheMap.SetActive(true);

        EnemyArmyOnTheMap army = enemyOnTheMap.GetComponent<EnemyArmyOnTheMap>();
        //enemiesPointsDict.Add(army, position);
        RegisterEnemy(army, position);
    }

    //private void CreateEnemyGarrison(Vector3 position, GameObject building)
    //{
    //    EnemyArmyOnTheMap army = building.GetComponent<EnemyArmyOnTheMap>();
    //    if(army != null)
    //    {
    //        army.Birth();
    //    }
    //}

    public void RegisterEnemy(EnemyArmyOnTheMap enemyArmyOnTheMap, Vector3 position)
    {
        enemiesPointsDict.Add(enemyArmyOnTheMap, position);
    }
}
