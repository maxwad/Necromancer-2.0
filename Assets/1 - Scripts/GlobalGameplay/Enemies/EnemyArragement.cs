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
        if(enemyManager == null) enemyManager = manager;
        if(gmManager == null) gmManager = GlobalStorage.instance.gmManager;
        if(poolManager == null) poolManager = GlobalStorage.instance.objectsPoolManager;

        enterPointsDict = gmManager.GetEnterPoints();

        GenerateEnterEnemies();
        GenerateRandomEnemies();

        enemyManager.SetEnemiesPointsDict(enemiesPointsDict);
    }

    #region USUAL ENEMIES

    private void GenerateEnterEnemies()
    {
        foreach(var enterPoint in enterPointsDict)
        {
            if(CanICreateEnterEnemy(enterPoint.Key, enterPoint.Value) == true)
            {
                CreateEnemy(enterPoint.Value);
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
                    CreateEnemy(tempWorldPositions[i]);
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

    private void CreateEnemy(Vector3 position)
    {
        GameObject enemyOnTheMap = poolManager.GetObjectFromPool(ObjectPool.EnemyOnTheMap);
        enemyOnTheMap.transform.SetParent(enemiesMap.transform);
        enemyOnTheMap.transform.position = position;
        enemyOnTheMap.SetActive(true);

        EnemyArmyOnTheMap army = enemyOnTheMap.GetComponent<EnemyArmyOnTheMap>();
        enemiesPointsDict.Add(army, position);
    }
}
