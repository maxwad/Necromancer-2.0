using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static NameManager;

public class VassalPathfinder : MonoBehaviour
{
    private VassalMovement movement;
    private GlobalMapTileManager tileManager;

    private Tilemap roadMap;
    private Tilemap overlayMap;
    private GMHexCell[,] roads;

    private List<Vector3> currentPath = new List<Vector3>();

    //private Vector3Int startPoint;
    private Vector3Int finishPoint;
    private int movementPoints;


    [SerializeField] private Tile testTile;
    [SerializeField] private int actionRadius = 10;
    [SerializeField] private int searchPlayerRadius = 15;
    [SerializeField] private int maxMovesCount = 10;

    #region GETTINGS
    public void Init(VassalMovement mv)
    {
        movement = mv;
        movementPoints = movement.GetMovementPointsAmoumt();

        tileManager = GlobalStorage.instance.gmManager;
        roadMap = GlobalStorage.instance.roadMap;
        overlayMap = GlobalStorage.instance.overlayMap;

        roads = tileManager.GetRoads();
    }

    public List<Vector3> GetPath()
    {
        return currentPath;
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

        return GetFinishCell(cells);        
    }

    private Vector3Int GetFinishCell(List<Vector3Int> cells)
    {
        Vector3Int resultCell = Vector3Int.zero;

        int count = cells.Count;
        while(count > 0)
        {
            Vector3Int randomCellInt = cells[UnityEngine.Random.Range(0, cells.Count)];
            Vector3 randomCellV3 = roadMap.CellToWorld(randomCellInt);

            if(tileManager.CheckCellAsEnterPoint(randomCellV3) == true)
            {
                cells.Remove(randomCellInt);
                continue;
            }
            else
            {
                if(CheckMovesCount(randomCellInt) == true)
                {
                    resultCell = randomCellInt;
                    break;
                }
            }
        }

        return resultCell;
    }

    private bool CheckMovesCount(Vector3Int finishCell)
    {
        List<Vector3> path = CreatePath(finishCell);

        if(path.Count == 0) return false;

        if((path.Count / movementPoints) > maxMovesCount) return false;

        return true;
    }

    public List<Vector3> CreatePath(Vector3Int finishCell)
    {
        currentPath.Clear();

        Dictionary<GMHexCell, NeighborData> queueDict = new Dictionary<GMHexCell, NeighborData>();
        List<GMHexCell> neighborsQueue = new List<GMHexCell>();
        List<GMHexCell> roadBack = new List<GMHexCell>();

        Vector3Int startPoint = overlayMap.WorldToCell(gameObject.transform.position);
        GMHexCell firstPathCell = roads[startPoint.x, startPoint.y];

        if(finishCell == startPoint)
        {
            Debug.Log("Start point is finish point");
            return currentPath;
        }

        bool isSearching = true;
        bool isDeadEnd = false;

        queueDict.Add(firstPathCell, new NeighborData());
        neighborsQueue.Add(firstPathCell);
        int countCells = 0;

        while(isSearching == true)
        {
            if(countCells >= neighborsQueue.Count)
            {
                isDeadEnd = true;
                break;
            }

            for(int i = countCells; i < neighborsQueue.Count; i++)
            {
                GMHexCell[] currentNeighbors = neighborsQueue[i].neighbors;

                for(int j = 0; j < currentNeighbors.Length; j++)
                {
                    if(currentNeighbors[j] != null && queueDict.ContainsKey(currentNeighbors[j]) == false)
                    {
                        queueDict.Add(currentNeighbors[j], new NeighborData(queueDict[neighborsQueue[i]].cost + 1, neighborsQueue[i]));
                        neighborsQueue.Add(currentNeighbors[j]);

                        if(currentNeighbors[j].coordinates == new Vector3Int(finishCell.x, finishCell.y, 0))
                        {
                            roadBack.Add(currentNeighbors[j]);
                            isSearching = false;
                            break;
                        }
                    }
                }

                if(isSearching == false) break;

                countCells++;
            }
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
                currentPath.Add(roadMap.CellToWorld(roadBack[i].coordinates));
                overlayMap.SetTile(roadBack[i].coordinates, testTile);                
            }

            //return currentPath;
        }

        return currentPath;
    }

    public void DrawThePath(List<Vector3> path)
    {
        for(int i = 0; i < path.Count; i++)
        {
            overlayMap.SetTile(overlayMap.WorldToCell(path[i]), testTile);
        }
    }
}
