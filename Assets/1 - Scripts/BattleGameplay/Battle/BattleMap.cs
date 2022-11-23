using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static NameManager;

public class BattleMap : MonoBehaviour
{
    [Header("Player")]
    private GameObject player;
    private Vector2 playerPosition;
    private int playerRadiusGap = 15; //always less than bounds size! 
    private float distanceToUpdateObstacles = 1f;
    private float radiusToCheckObstacles = 250f;
    private bool readyToCheckObstacles = false;


    [Header("Battle Map")]
    public Tilemap battleBGTilemap;
    private int sizeX;
    private int sizeY;
    private int widthOfBound = 20;
    private int spawnZOffset = 20;
    private int quantityUnsuccessTry = 10000;
    [HideInInspector] public bool[,] battleArray;

    public Tilemap battleBoundsTilemap;
    public List<Tile> mapBG;
    public List<Tile> boundsBG;

    [Header("DefenderTowers")]
    private int quantityOfTowers = 4;
    [SerializeField] private GameObject towerContainer;
    private List<GameObject> towersOnMap = new List<GameObject>();

    public GameObject towersPrefab;
    private BattleObjectStats towerStats;


    [Header("Obstacles")]
    [SerializeField] private GameObject obstaclesContainer;
    private List<GameObject> obstaclesOnMap = new List<GameObject>();

    public List<GameObject> obstaclesPrefabs;
    private List<BattleObjectStats> obstaclesStats = new List<BattleObjectStats>();   


    [Header("Torches")]
    private int torchQuotePerSomeCells = 1;
    private int someCells = 500;
    private int quantityOfTorches;
    private List<GameObject> torchesOnMap = new List<GameObject>();

    public GameObject torchPrefab;
    private BattleObjectStats torchStats;


    [Header("Enemy")]
    private EnemySpawner enemySpawner;
    private Army currentArmy;


    private void Start()
    {
        player = GlobalStorage.instance.battlePlayer.gameObject;

        towerStats = towersPrefab.GetComponent<BattleObjectStats>();
        torchStats = torchPrefab.GetComponent<BattleObjectStats>();

        foreach (var obstacle in obstaclesPrefabs)
            obstaclesStats.Add(obstacle.GetComponent<BattleObjectStats>());

        enemySpawner = GetComponent<EnemySpawner>();
    }

    private void Update()
    {
        if (readyToCheckObstacles == true)
        {
            //CheckObstaclesOnBattle();
        }
    }

    public void InitializeMap(bool mode)
    {
        if (mode == false)
        {
            GetBattleData();
            DrawTheBackgroundMap();

            if (currentArmy.isThisASiege == true) DrawObjects(towersPrefab, towerContainer, towerStats, towersOnMap, quantityOfTowers);
            DrawObstacles();
            DrawObjects(torchPrefab, null, torchStats, torchesOnMap, quantityOfTorches);
            //MarkFilledCells();

            enemySpawner.Initialize(currentArmy);
            enemySpawner.ReadyToSpawnEnemy();
        }
        else
        {
            enemySpawner.StopSpawnEnemy();
            ClearMap();
        }        
    }

    private void DrawTheBackgroundMap() 
    {
        int sizeXWithBound = sizeX + widthOfBound * 2;
        int sizeYWithBound = sizeY + widthOfBound * 2;

        for (int x = 0; x < sizeXWithBound; x++)
        {
            for (int y = 0; y < sizeYWithBound; y++)
            {
                //draw bounds
                if ((x < widthOfBound || x >= sizeXWithBound - widthOfBound) || (y < widthOfBound || y >= sizeYWithBound - widthOfBound))
                {                   
                    Tile currentBoundTile = boundsBG[0];

                    if (x == y && x < widthOfBound) 
                        currentBoundTile = boundsBG[1];
                    

                    if (x - (widthOfBound + sizeX) == y - (widthOfBound + sizeY) && y >= widthOfBound + sizeY) 
                        currentBoundTile = boundsBG[1];


                    if (-x - 1 + (sizeX + 2 * widthOfBound) == y && y < widthOfBound) 
                        currentBoundTile = boundsBG[1];


                    if (sizeYWithBound - x - 1 == y && y >= widthOfBound + sizeY) 
                        currentBoundTile = boundsBG[1];

                    battleBoundsTilemap.SetTile(new Vector3Int(x, y, -20), currentBoundTile);
                    
                    battleArray[x, y] = false;
                }
                else
                {
                    battleBGTilemap.SetTile(new Vector3Int(x, y, -20), mapBG[Random.Range(0, mapBG.Count)]);

                    if ((x == widthOfBound) || (x == sizeXWithBound - widthOfBound - 1) || (y == widthOfBound) || (y == sizeYWithBound - widthOfBound - 1))
                    {
                        battleArray[x, y] = false;
                    }
                    else
                    {
                        battleArray[x, y] = true;
                    }                    
                }               
            }
        }
    }

    private bool CheckPlayerPosition(int checkX, int checkY)
    {
        bool canICreateObjectHere = true;

        int playerPositionX = (int)player.transform.position.x;
        int playerPositionY = (int)player.transform.position.y;

        //check player position and desire position
        for (int x = playerPositionX - playerRadiusGap; x < playerPositionX + playerRadiusGap; x++)
        {
            for (int y = playerPositionY - playerRadiusGap; y < playerPositionY + playerRadiusGap; y++)
            {
                if (checkX == x && checkY == y)
                {
                    canICreateObjectHere = false;
                    break;
                }                
            }
        }

        return canICreateObjectHere;
    }

    private void DrawObstacles()
    {
        int battleMapSizeX = battleArray.GetLength(0);
        int battleMapSizeY = battleArray.GetLength(1);

        int randomIndex;
        GameObject currentObstacle;
        BattleObjectStats currentStats;
        float probability;
        int obstacleSizeX;
        int obstacleSizeY;
        int obstacleGap;

        void InitializeCurrentObstacle()
        {
            randomIndex     = Random.Range(0, obstaclesPrefabs.Count);

            currentObstacle = obstaclesPrefabs[randomIndex];
            currentStats    = obstaclesStats[randomIndex];
            obstacleSizeX   = currentStats.sizeX;
            obstacleSizeY   = currentStats.sizeY;
            probability     = currentStats.probability;
            obstacleGap     = currentStats.gap;
        }       

        InitializeCurrentObstacle();

        for (int x = 0; x < battleMapSizeX; x++)
        {
            for (int y = 0; y < battleMapSizeY; y++)
            {
                if (battleArray[x, y] == false) continue;

                //check out of bounds array
                if ((x + obstacleSizeX / 2 + obstacleGap >= battleMapSizeX) ||
                    (x - obstacleSizeX / 2 - obstacleGap < 0) ||
                    (y + obstacleSizeY / 2 + obstacleGap >= battleMapSizeY) ||
                    (y - obstacleSizeY / 2 - obstacleGap < 0))
                {
                    continue;
                }

                //check player area
                if (CheckPlayerPosition(x, y) == false) continue;


                //check free space for obstacle
                if (battleArray[x + obstacleSizeX / 2 + obstacleGap, y] == false ||
                    battleArray[x - obstacleSizeX / 2 - obstacleGap, y] == false ||
                    battleArray[x, y + obstacleSizeY / 2 + obstacleGap] == false ||
                    battleArray[x, y - obstacleSizeY / 2 - obstacleGap] == false)                    
                {
                    continue;
                }

                //check free space inside obstacle area
                bool newTry = false;
                for (int checkX = -obstacleSizeX; checkX < obstacleSizeX / 2; checkX++)
                {
                    for (int checkY = -obstacleSizeY; checkY < obstacleSizeY / 2; checkY++)
                    {
                        if (battleArray[x + checkX, y + checkY] == false)
                        {
                            newTry = true;
                            continue;
                        }
                    }
                }

                if (newTry == true) continue;

                
                if (probability >= Random.Range(0, 100))
                {
                    GameObject obstacle = Instantiate(currentObstacle, new Vector3(x, y, spawnZOffset), Quaternion.identity);
                    obstacle.transform.SetParent(obstaclesContainer.transform);
                    obstacle.SetActive(true);
                    obstaclesOnMap.Add(obstacle);

                    FillCells(x, y, obstacleSizeX, obstacleSizeY, obstacleGap, false);

                    InitializeCurrentObstacle();
                } 
            }
        }

        readyToCheckObstacles = true;        
    }

    private void DrawObjects(GameObject prefab, GameObject container, BattleObjectStats stats, List<GameObject> objectsOnMap, int quantity)
    {
        int battleMapSizeX = battleArray.GetLength(0);
        int battleMapSizeY = battleArray.GetLength(1);

        int objectSizeX = stats.sizeX;
        int objectSizeY = stats.sizeY;
        int objectGap = stats.gap;

        int currentQuantity = objectsOnMap.Count;

        int unsuccessTry = quantityUnsuccessTry;

        for (int countTry = currentQuantity; countTry < quantity; countTry++)
        {
            bool isBuilded = false;
            while (isBuilded == false)
            {
                //break the endless cycle
                unsuccessTry--;
                if (unsuccessTry == 0) return;

                int randomX = Random.Range(0 + widthOfBound + sizeX, battleMapSizeX - widthOfBound - sizeX);
                int randomY = Random.Range(0 + widthOfBound + sizeY, battleMapSizeY - widthOfBound - sizeY);

                if (battleArray[randomX, randomY] == false) continue;

                //check player area
                if (CheckPlayerPosition(randomX, randomY) == false) continue;

                //check free space for object
                if (battleArray[randomX + objectSizeX / 2 + objectGap, randomY] == false ||
                    battleArray[randomX - objectSizeX / 2 - objectGap, randomY] == false ||
                    battleArray[randomX, randomY + objectSizeY / 2 + objectGap] == false ||
                    battleArray[randomX, randomY - objectSizeY / 2 - objectGap] == false)
                {
                    continue;
                }

                //check free space inside object area
                bool newTry = false;
                for (int checkX = -objectSizeX; checkX < objectSizeX / 2; checkX++)
                {
                    for (int checkY = -objectSizeY; checkY < objectSizeY / 2; checkY++)
                    {
                        if (battleArray[randomX + checkX, randomY + checkY] == false)
                        {
                            newTry = true;
                            continue;
                        }
                    }
                }

                if (newTry == true) continue;

                GameObject obj;

                if (prefab.GetComponent<HealthObjectStats>().isFromPool == true)
                {
                    obj = GlobalStorage.instance.objectsPoolManager.GetObject(ObjectPool.Torch);
                    obj.transform.position = new Vector3(randomX, randomY, spawnZOffset);
                    obj.SetActive(true);
                }
                else
                {
                    obj = Instantiate(prefab, new Vector3(randomX, randomY, spawnZOffset), Quaternion.identity);
                    if (container != null) obj.transform.SetParent(container.transform);
                }
                
                objectsOnMap.Add(obj);

                FillCells(randomX, randomY, objectSizeX, objectSizeY, objectGap, false);                

                isBuilded = true;
            }

        }

    }

    private void MarkFilledCells()
    {
        //mark all false cells
        for (int i = 0; i < battleArray.GetLength(0); i++)
        {
            for (int j = 0; j < battleArray.GetLength(1); j++) 
            {
                Vector3Int poz = new Vector3Int(i, j, -20);
                battleBGTilemap.SetTileFlags(poz, TileFlags.None);

                if (battleArray[i, j] == false)
                    battleBGTilemap.SetColor(poz, Color.black);
                else
                    battleBGTilemap.SetColor(poz, Color.white);
            }                
        }
    }

    private void ClearMap()
    {
        battleBoundsTilemap.ClearAllTiles();
        battleBGTilemap.ClearAllTiles();

        //objects from pool have ObjectPoolManager as a parent!

        foreach (Transform child in obstaclesContainer.transform)
            Destroy(child.gameObject);

        foreach (Transform child in towerContainer.transform)
            Destroy(child.gameObject);

        for (int i = 0; i < battleArray.GetLength(0); i++)
        {
            for (int j = 0; j < battleArray.GetLength(1); j++)
                battleArray[i, j] = true;
        }

        readyToCheckObstacles = false;
        obstaclesOnMap.Clear();
        towersOnMap.Clear();
        torchesOnMap.Clear();

        EventManager.OnEndOfBattleEvent();
    }

    private void FillCells(int xPos, int yPos, int sizeX, int sizeY, int gap, bool value)
    {
        battleArray[xPos, yPos] = false;

        //mark filled coordinates as false
        for (int checkX = -gap -sizeX / 2; checkX < sizeX / 2 + gap; checkX++)
        {
            for (int checkY = -gap - sizeY / 2; checkY < sizeY / 2 + gap; checkY++)
            {
                if (battleArray[xPos + checkX, yPos + checkY] == !value)
                {
                    battleArray[xPos + checkX, yPos + checkY] = value;
                }
            }
        }
    }

    //Enable or disable obstacles on map in dependent of distance to player
    private void CheckObstaclesOnBattle() 
    {
        if (Vector2.Distance(player.transform.position, playerPosition) >= distanceToUpdateObstacles)
        {
            foreach (GameObject obstacle in obstaclesOnMap)
            {
                if (Vector2.Distance(obstacle.transform.position, player.transform.position) <= radiusToCheckObstacles)
                    obstacle.SetActive(true);
                else
                    obstacle.SetActive(false);
            }

            playerPosition = player.transform.position;
        }
    }

    public void GetBattleData()
    {
        Vector3Int size = GlobalStorage.instance.battleManager.GetBattleMapSize();
        sizeX = size.x;
        sizeY = size.y;

        battleArray = new bool[sizeX + 2 * widthOfBound, sizeY + 2 * widthOfBound];

        //player in the middle on the map
        player.transform.position = new Vector3 ((sizeX + 2 * widthOfBound) / 2, (sizeY + 2 * widthOfBound) / 2, spawnZOffset);

        quantityOfTorches = (sizeX * sizeY * torchQuotePerSomeCells) / someCells;
        currentArmy = GlobalStorage.instance.battleManager.GetArmy();
    }

    private void ClearSpaceUnderObject(GameObject obj)
    {
        BattleObjectStats objectSpaceStats = obj.GetComponent<BattleObjectStats>();

        FillCells(
            (int)obj.transform.position.x, (int)obj.transform.position.y, 
            objectSpaceStats.sizeX, objectSpaceStats.sizeY, 
            objectSpaceStats.gap, 
            true);

        HealthObjectStats objHealthStats = obj.GetComponent<HealthObjectStats>();

        if (objHealthStats.typeOfObject == ObstacleTypes.Torch) 
        {
            torchesOnMap.Remove(obj);
            obj.SetActive(false);

            DrawObjects(torchPrefab, null, objectSpaceStats, torchesOnMap, quantityOfTorches);
        }

        if (objHealthStats.typeOfObject == ObstacleTypes.Tower)
        {
            towersOnMap.Remove(obj);
            Destroy(obj);
        }

        //MarkFilledCells();
    }

    private void OnEnable()
    {
        EventManager.SwitchPlayer += InitializeMap;
        EventManager.ObstacleDestroyed += ClearSpaceUnderObject;
    }

    private void OnDisable()
    {
        EventManager.SwitchPlayer -= InitializeMap;
        EventManager.ObstacleDestroyed -= ClearSpaceUnderObject;
    }
}
