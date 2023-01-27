using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static NameManager;

public class VassalPathfinder : MonoBehaviour
{
    private GlobalMapTileManager tileManager;

    private Tilemap roadMap;
    private Tilemap overlayMap;
    private GMHexCell[,] roads;

    private List<Vector3> currentPath = new List<Vector3>();

    private Vector3Int startPoint;
    private Vector3Int finishPoint;
    [SerializeField] private Tile testTile;
    [SerializeField] private int movementPoints = 20;
    [SerializeField] private int actionRadius = 50;
    [SerializeField] private int searchPlayerRadius = 15;
    [SerializeField] private int maxMovesCount = 10;


    public void Init()
    {
        tileManager = GlobalStorage.instance.gmManager;
        roadMap = GlobalStorage.instance.roadMap;
        overlayMap = GlobalStorage.instance.overlayMap;

        roads = tileManager.GetRoads();
    }

    public float GetMovementPoints()
    {
        return movementPoints;
    }

    public void FindRandomCell()
    {
        startPoint = overlayMap.WorldToCell(gameObject.transform.position);
        //Debug.Log(startPoint);

        int minX = ((startPoint.x - actionRadius) < 0) ? 0 : startPoint.x - actionRadius;
        int maxX = ((startPoint.x + actionRadius) >= roadMap.size.x) ? roadMap.size.x - 1 : startPoint.x + actionRadius;

        int minY = ((startPoint.y - actionRadius) < 0) ? 0 : startPoint.y - actionRadius;
        int maxY = ((startPoint.y + actionRadius) >= roadMap.size.y) ? roadMap.size.y - 1 : startPoint.y + actionRadius;


        //Debug.Log("minX = " + minX + " maxX = " + maxX + " minY = " + minY + " maxY = " + maxY);
        //Debug.Log("roadMap.size.x = " + roadMap.size.x + " roadMap.size.y = " + roadMap.size.y);
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


        //Debug.Log(cells.Count);
        for(int i = 0; i < cells.Count; i++)
        {
            overlayMap.SetTile(cells[i], testTile);
        }

        finishPoint = GetFinishCell(cells);

        if(finishPoint != Vector3Int.zero)
            Move();
    }

    private Vector3Int GetFinishCell(List<Vector3Int> cells)
    {
        Vector3Int result = Vector3Int.zero;

        int count = cells.Count;
        while(count > 0)
        {
            Vector3Int randomCellInt = cells[UnityEngine.Random.Range(0, cells.Count)];
            Vector3 randomCellV3 = roadMap.CellToWorld(randomCellInt);

            if(tileManager.CheckCellAsEnterPoint(randomCellV3) == true)
            {
                cells.Remove(randomCellInt);

                Debug.Log("It's enterPoint");
                continue;
            }
            else
            {
                if(CheckMovesCount(randomCellInt) == true)
                {
                    result = randomCellInt;
                    break;
                }
            }
        }

        return result;
    }

    private bool CheckMovesCount(Vector3Int finishCell)
    {
        List<Vector3> path = CreatePath(startPoint, finishCell);

        if(path == null) return false;

        if((path.Count / movementPoints) > maxMovesCount) return false;

        return true;
    }

    private List<Vector3> CreatePath(Vector3Int startCell, Vector3Int finishCell)
    {
        currentPath.Clear();

        Dictionary<GMHexCell, NeighborData> queueDict = new Dictionary<GMHexCell, NeighborData>();
        List<GMHexCell> neighborsQueue = new List<GMHexCell>();
        List<GMHexCell> roadBack = new List<GMHexCell>();

        GMHexCell firstPathCell = roads[startCell.x, startCell.y];

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

            return currentPath;
        }
        
        return null;
    }

    private void Move()
    {

    }
        


}
