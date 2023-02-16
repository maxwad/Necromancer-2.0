using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static NameManager;

public class VassalPathfinder : MonoBehaviour
{
    private VassalMovement movement;
    private VassalTargetSelector targetSelector;
    private GlobalMapTileManager tileManager;
    private EnemyManager enemyManager;
    private ResourcesSources resources;
    private GameObject player;
    private AISystem aiSystem;
    private ResourceBuilding playerCastle;
    private MapBonusManager heapManager;

    private Tilemap roadMap;
    private Tilemap overlayMap;
    private GMHexCell[,] roads;

    private Queue<Vector3> currentPath = new Queue<Vector3>();
    private int movementPoints;


    [SerializeField] private Tile testTile;
    [SerializeField] private int actionRadius = 10;
    [SerializeField] private int searchPlayerRadius = 25;
    [SerializeField] private int maxMovesCount = 10;

    #region GETTINGS
    public void Init(VassalTargetSelector ts, VassalMovement mv)
    {
        targetSelector = ts;

        movement = mv;
        movementPoints = movement.GetMovementPointsAmoumt();

        tileManager  = GlobalStorage.instance.gmManager;
        roadMap      = GlobalStorage.instance.roadMap;
        overlayMap   = GlobalStorage.instance.overlayMap;
        enemyManager = GlobalStorage.instance.enemyManager;
        resources    = GlobalStorage.instance.resourcesManager.GetComponent<ResourcesSources>();
        aiSystem     = GlobalStorage.instance.aiSystem;
        playerCastle = GlobalStorage.instance.fortressGO.GetComponent<ResourceBuilding>();
        heapManager  = GlobalStorage.instance.mapBonusManager;
        player       = GlobalStorage.instance.globalPlayer.gameObject;

        roads = tileManager.GetRoads();
    }

    public GMHexCell GetCell(Vector3 position)
    {
        return tileManager.GetCell(position);
    }

    public Vector3 ConvertToV3(Vector3Int pos)
    {
        return tileManager.CellConverterToV3(pos);
    }

    public Vector3Int ConvertToV3Int(Vector3 pos)
    {
        return tileManager.CellConverterToV3Int(pos);
    }

    #endregion


    #region FINDERS

    public Vector3Int FindRandomCell()
    {
        Vector3Int startPoint = overlayMap.WorldToCell(gameObject.transform.position);

        int minX = ((startPoint.x - actionRadius) < 0) ? 0 : startPoint.x - actionRadius;
        int maxX = ((startPoint.x + actionRadius) >= roadMap.size.x) ? roadMap.size.x - 1 : startPoint.x + actionRadius;

        int minY = ((startPoint.y - actionRadius) < 0) ? 0 : startPoint.y - actionRadius;
        int maxY = ((startPoint.y + actionRadius) >= roadMap.size.y) ? roadMap.size.y - 1 : startPoint.y + actionRadius;

        List<Vector3Int> cells = new List<Vector3Int>();

        for(int i = minY; i < maxY; i++)
        {
            if(roads[minX, i] != null)
                cells.Add(new Vector3Int(minX, i, 0));

            if(roads[maxX, i] != null)
                cells.Add(new Vector3Int(maxX, i, 0));
        }

        for(int i = minX; i < maxX; i++)
        {
            if(roads[i, minY] != null)
                cells.Add(new Vector3Int(i, minY, 0));

            if(roads[i, maxY] != null)
                cells.Add(new Vector3Int(i, maxY, 0));
        }

        Vector3Int resultCell = Vector3Int.zero;
        int count = cells.Count;
        while(count > 0)
        {
            Vector3Int randomCellInt = cells[Random.Range(0, cells.Count)];

            Vector3 randomCell = roadMap.CellToWorld(randomCellInt);

            if(CheckCellAsEnterPoint(randomCell) == true)
            {
                cells.Remove(randomCellInt);
                count--;
                continue;
            }

            if(enemyManager.CheckPositionInEnemyPoints(randomCell) == true)
            {
                cells.Remove(randomCellInt);
                count--;
                continue;
            }

            if(CheckMovesCount(randomCellInt) == true)
            {
                resultCell = randomCellInt;
                break;
            }
        }

        return resultCell;
    }

    public Vector3Int FindResBuildingCell()
    {
        List<ResourceBuilding> resList = resources.GetAllResBuildings();
        List<ResourceBuilding> filteredList = new List<ResourceBuilding>();

        for(int i = 0; i < resList.Count; i++)
        {
            if(resList[i].GetOwner().owner != TypeOfObjectsOwner.Enemy && resList[i].CheckSiegeStatus() == false)
                filteredList.Add(resList[i]);
        }

        Dictionary<ResourceBuilding, float> distDict = new Dictionary<ResourceBuilding, float>();
        Vector3Int findedPoint = Vector3Int.zero;

        if(filteredList.Count > 0)
        {
            foreach(var item in filteredList)
            {
                if(distDict.ContainsKey(item) == false)
                {
                    float distance = Vector3.Distance(transform.position, item.gameObject.transform.position);
                    distDict.Add(item, distance);
                }
            }

            var findedResBuilding = distDict.OrderBy(building => building.Value).First();

            targetSelector.SetCurrentSiegeTarget(findedResBuilding.Key);
            Vector3 position = tileManager.GetEnterPoint(findedResBuilding.Key.gameObject);

            if(position != Vector3.zero)
                findedPoint = tileManager.CellConverterToV3Int(position);
        }
        
        return findedPoint;
    }

    public Vector3Int FindPlayerCastleCell()
    {
        Vector3Int findedPoint = Vector3Int.zero;

        if(playerCastle.CheckSiegeStatus() == false)
        {
            targetSelector.SetCurrentSiegeTarget(playerCastle);
            Vector3 position = tileManager.GetEnterPoint(playerCastle.gameObject);

            if(position != Vector3.zero)
                findedPoint = tileManager.CellConverterToV3Int(position);
        }

        return findedPoint;
    }

    #endregion

    public Queue<Vector3> CreatePath(Vector3Int finishCell, Vector3Int startCell = default)
    {
        currentPath.Clear();

        Dictionary<GMHexCell, NeighborData> queueDict = new Dictionary<GMHexCell, NeighborData>();
        Queue<GMHexCell> neighborsQueue = new Queue<GMHexCell>();
        List<GMHexCell> roadBack = new List<GMHexCell>();

        Vector3Int startPoint = (startCell == default) ? overlayMap.WorldToCell(gameObject.transform.position) : startCell;
        GMHexCell firstPathCell = roads[startPoint.x, startPoint.y];

        if(finishCell == startPoint || finishCell == Vector3Int.zero)
        {
            Debug.Log("Start point is finish point");
            return currentPath;
        }

        bool isSearching = true;
        bool isDeadEnd = true;

        queueDict.Add(firstPathCell, new NeighborData());
        neighborsQueue.Enqueue(firstPathCell);

        while(neighborsQueue.Count > 0)
        {
            GMHexCell cell = neighborsQueue.Dequeue();
            Vector3 checkPosition = roadMap.CellToWorld(cell.coordinates);

            if(CheckPlayerInCell(checkPosition) == true && targetSelector.GetAgressiveMode() == false)
                continue;

            GMHexCell[] currentNeighbors = cell.neighbors;

            for(int j = 0; j < currentNeighbors.Length; j++)
            {
                if(currentNeighbors[j] != null && queueDict.ContainsKey(currentNeighbors[j]) == false)
                {
                    queueDict.Add(currentNeighbors[j], new NeighborData(queueDict[cell].cost + 1, cell));
                    neighborsQueue.Enqueue(currentNeighbors[j]);

                    if(currentNeighbors[j].coordinates == new Vector3Int(finishCell.x, finishCell.y, 0))
                    {
                        roadBack.Add(currentNeighbors[j]);
                        isSearching = false;
                        isDeadEnd = false;
                        break;
                    }
                }
            }

            if(isSearching == false) break;            
        }

        if(isDeadEnd == false)
        {
            GMHexCell currentBackCell = roadBack[0];
            while(currentBackCell != firstPathCell)
            {
                roadBack.Add(queueDict[currentBackCell].source);
                currentBackCell = queueDict[currentBackCell].source;
            }
            roadBack.Reverse();            

            for(int i = 0; i < roadBack.Count; i++)
            {               
                currentPath.Enqueue(roadMap.CellToWorld(roadBack[i].coordinates));
                overlayMap.SetTile(roadBack[i].coordinates, testTile);                
            }
        }

        return currentPath;
    }

    #region CHECKERS       

    public bool CheckPlayerInCell(Vector3 position)
    {
        return position == player.transform.position;
    }

    public bool CheckPlayerNearBy()
    {
        return Vector3.Distance(transform.position, player.transform.position) < searchPlayerRadius;
    }

    public bool CheckEnemy(Vector3 position)
    {
        foreach(var vassal in aiSystem.GetVassalsInfo())
        {
            if(position == vassal.transform.position)
                return true;
        }

        return enemyManager.CheckPositionInEnemyPoints(position) != null;
    }

    public Vector3Int CheckHeapNearBy(Vector3 nextPoint)
    {
        GMHexCell checkCell = GetCell(transform.position);
        GMHexCell[] neighbors = checkCell.neighbors;

        foreach(var pos in neighbors)
        {
            if(pos == null) continue;

            if(pos.coordinates == ConvertToV3Int(nextPoint))
                continue;

            Vector3 checkPos = ConvertToV3(pos.coordinates);
            if(heapManager.IsHeapOnPosition(checkPos) == true)
                return pos.coordinates;
        }

        return Vector3Int.zero;
    }

    public bool CheckCellAsEnterPoint(Vector3 cell)
    {
        return tileManager.CheckCellAsEnterPoint(cell);
    }

    private bool CheckMovesCount(Vector3Int finishCell)
    {
        Queue<Vector3> path = CreatePath(finishCell);

        if(path.Count == 0) return false;

        if((path.Count / movementPoints) > maxMovesCount) return false;

        return true;
    }

    #endregion

    public Queue<Vector3> AddHeapCellToThePath(Vector3Int heapCell)
    {
        return CreatePath(targetSelector.GetFinishCell(), heapCell); ;
    }

    public void DrawThePath(Queue<Vector3> path)
    {
        List<Vector3> pathList = new List<Vector3>(path);
        for(int i = 0; i < path.Count; i++)
        {
            overlayMap.SetTile(overlayMap.WorldToCell(pathList[i]), testTile);
        }
    }
}
