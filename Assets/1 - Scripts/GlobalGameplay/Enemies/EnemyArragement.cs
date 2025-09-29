using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;
using Enums;

public class EnemyArragement : MonoBehaviour
{
    private GlobalMapTileManager gmManager;
    private EnemyManager enemyManager;
    private ObjectsPoolManager poolManager;
    private GMPlayerMovement globalPlayer;

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

    [Inject]
    public void Construct(
        GlobalMapTileManager gmManager,
        EnemyManager enemyManager,
        ObjectsPoolManager poolManager,
        GMPlayerMovement globalPlayer
        )
    {
        this.gmManager = gmManager;
        this.enemyManager = enemyManager;
        this.poolManager = poolManager;
        this.globalPlayer = globalPlayer;
    }

    public void GenerateEnemiesOnTheMap()
    {
        enterPointsDict = gmManager.GetEnterPoints();

        GenerateEnterEnemies();
        GenerateRandomEnemies();

        EventManager.OnResetGarrisonsEvent();
        enemyManager.SetEnemiesPointsDict(enemiesPointsDict);
    }

    #region GUARD ENEMIES

    private void GenerateEnterEnemies()
    {
        foreach(var enterPoint in enterPointsDict)
        {
            if(CanICreateEnterEnemy(enterPoint.Key, enterPoint.Value) == true)
                CreateUsualEnemy(enterPoint.Value, true);
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
                    canICreateEnemy = true;
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
                    CreateUsualEnemy(tempWorldPositions[i], true);
            }
        }
    }

    private bool CheckPosition(Vector3 position)
    {
        foreach(var point in enterPointsDict)
        {
            if(Vector3.Distance(point.Value, position) < enemiesGap)
            {
                //roadMap.SetTile(roadMap.WorldToCell(position), fogTile);
                return false;
            }
        }

        return CheckPlayerPosition(position);     
    }

    #endregion

    private bool CheckPlayerPosition(Vector3 position)
    {
        return !(globalPlayer.transform.position == position);
    }

    public EnemyArmyOnTheMap CreateUsualEnemy(Vector3 position, bool createMode = true)
    {
        GameObject enemy = poolManager.GetObject(ObjectPool.EnemyOnTheMap);
        enemy.transform.SetParent(enemiesMap.transform);
        enemy.transform.position = position;
        enemy.SetActive(true);
        EnemyArmyOnTheMap enemyOnTheMap  = enemy.GetComponent<EnemyArmyOnTheMap>();
        if(createMode == true)
            enemyOnTheMap.Birth();

        if(enemyOnTheMap.typeOfArmy != TypeOfArmy.Vassals)
            enemyManager.enemyArragement.RegisterEnemy(enemyOnTheMap);

        return enemyOnTheMap;
    }

    public void RegisterEnemy(EnemyArmyOnTheMap enemyArmyOnTheMap)
    {
        if(enemiesPointsDict.ContainsKey(enemyArmyOnTheMap) == false)
            enemiesPointsDict.Add(enemyArmyOnTheMap, enemyArmyOnTheMap.gameObject.transform.position);
    }

    public void ReloadArmies()
    {
        enterPointsDict = gmManager.GetEnterPoints();
        enemyManager.SetEnemiesPointsDict(enemiesPointsDict);
    }

    public void SetManager(EnemyManager manager) => enemyManager = manager;
}
