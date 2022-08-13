using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using static NameManager;

public class GlobalTileMapManager : MonoBehaviour
{
    [HideInInspector] public Tilemap mapBG;
    [HideInInspector] public Tilemap fogMap;
    [HideInInspector] public Tilemap roadMap;
    [HideInInspector] public Tilemap overlayMap;

    private GMHexCell[,] roads;

    public Tile fogTile;
    public Tile roadTile;
    public Tile finishTile;
    //public Tile testTile;

    //public TMP_Text coordinates;
    //private List<TMP_Text> coordList = new List<TMP_Text>();

    private GMPlayerMovement player;
    private CursorManager cursorManager;

    List<Vector2> pathPoints = new List<Vector2>();
    private bool isGoalCellFinded = false;
    private GMHexCell targetCell;

    private float movementPointsMax = 0;
    private float currentMovementPoints = 0;

    private struct NeighborData
    {
        public GMHexCell source;
        public int cost;

        public NeighborData(int currentCost = 0, GMHexCell currentSource = null)
        {
            source = currentSource;
            cost = currentCost;
        }
    }

    private Vector3Int startPoint;

    private void Start()
    {
        player = GlobalStorage.instance.globalPlayer.GetComponent<GMPlayerMovement>();
        cursorManager = GlobalStorage.instance.cursorManager;
        Initialize();
    }

    private void GetParameters() 
    {
        float[] parameters = player.GetParametres();
        movementPointsMax = parameters[0];
        currentMovementPoints = parameters[1];
    }


    public void Initialize()
    {
        roads = new GMHexCell[roadMap.size.x, roadMap.size.y];

        CreateRoadCells();
        CreateFogCells();
        SetHeighbors();
    }

    public void CreateRoadCells()
    {
        for(int x = 0; x < roads.GetLength(0); x++)
        {
            for(int y = 0; y < roads.GetLength(1); y++)
            {
                if(roadMap.HasTile(new Vector3Int(x, y, 0)))
                {
                    GMHexCell cell = new GMHexCell();
                    cell.coordinates = new Vector3Int(x, y, 0);
                    roads[x, y] = cell;
                }
                else
                {
                    roads[x, y] = null;
                }
            }
        }
    }

    public void CreateFogCells()
    {
        for(int x = 0; x < mapBG.size.x; x++)
        {
            for(int y = 0; y < mapBG.size.y; y++)
            {
                fogMap.SetTile(new Vector3Int(x, y, 0), fogTile);
            }
        }

        CheckFog(player.transform.position, 30f);
    }

    private void SetHeighbors()
    {
        for(int x = 0; x < roads.GetLength(0); x++)
        {
            for(int y = 0; y < roads.GetLength(1); y++)
            {
                GMHexCell cell = roads[x, y];
                bool isEvenRow = (y % 2) == 0;

                if(cell != null)
                {
                    if(x + 1 < roadMap.size.x) cell.SetNeighbor(NeighborsDirection.E, roads[x + 1, y]);
                    else cell.SetNeighbor(NeighborsDirection.E, null);

                    if(x - 1 >= 0) cell.SetNeighbor(NeighborsDirection.W, roads[x - 1, y]);
                    else cell.SetNeighbor(NeighborsDirection.W, null);

                    if(isEvenRow == true)
                    {
                        if(y + 1 < roadMap.size.y) cell.SetNeighbor(NeighborsDirection.NE, roads[x, y + 1]);
                        else cell.SetNeighbor(NeighborsDirection.NE, null);

                        if(y - 1 >= 0) cell.SetNeighbor(NeighborsDirection.SE, roads[x, y - 1]);
                        else cell.SetNeighbor(NeighborsDirection.SE, null);

                        if(x - 1 >= 0 && y - 1 >= 0) cell.SetNeighbor(NeighborsDirection.SW, roads[x - 1, y - 1]);
                        else cell.SetNeighbor(NeighborsDirection.SW, null);

                        if(x - 1 >= 0 && y + 1 < roadMap.size.y) cell.SetNeighbor(NeighborsDirection.NW, roads[x - 1, y + 1]);
                        else cell.SetNeighbor(NeighborsDirection.NW, null);
                    }
                    else
                    {
                        if(x + 1 < roadMap.size.x && y + 1 < roadMap.size.y) cell.SetNeighbor(NeighborsDirection.NE, roads[x + 1, y + 1]);
                        else cell.SetNeighbor(NeighborsDirection.NE, null);

                        if(x + 1 < roadMap.size.x && y - 1 >= 0) cell.SetNeighbor(NeighborsDirection.SE, roads[x + 1, y - 1]);
                        else cell.SetNeighbor(NeighborsDirection.SE, null);

                        if(y - 1 >= 0) cell.SetNeighbor(NeighborsDirection.SW, roads[x, y - 1]);
                        else cell.SetNeighbor(NeighborsDirection.SW, null);

                        if(y + 1 < roadMap.size.y) cell.SetNeighbor(NeighborsDirection.NW, roads[x, y + 1]);
                        else cell.SetNeighbor(NeighborsDirection.NW, null);
                    }
                }
            }
        }
    }

    private void Update()
    {
        if(MenuManager.isGamePaused == false && MenuManager.isMiniPause == false && GlobalStorage.instance.isGlobalMode == true)
        {
            if(Input.GetMouseButtonDown(0)) HandleClick();
        }        
    }

    private void HandleClick()
    {
        GetParameters();
        startPoint = roadMap.WorldToCell(player.transform.position);

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int positionOnRoad = roadMap.WorldToCell(mousePosition);

        if(positionOnRoad != startPoint &&
            positionOnRoad.x < roadMap.size.x && positionOnRoad.x >= 0 &&
            positionOnRoad.y < roadMap.size.y && positionOnRoad.y >= 0)
        {
            GMHexCell checkCell = roads[positionOnRoad.x, positionOnRoad.y];

            if(checkCell != null)
            {
                if(isGoalCellFinded == true && targetCell == checkCell)
                {
                    player.MoveOnTheWay(pathPoints.ToArray(), this);
                    isGoalCellFinded = false;
                }
                else
                {
                    player.StopMoving();
                    targetCell = roads[positionOnRoad.x, positionOnRoad.y];
                    StartCoroutine(PathFinding(startPoint, positionOnRoad));
                }
            }
        }
    }

    private IEnumerator PathFinding(Vector3Int startCell, Vector3Int goalCell)
    {
        Dictionary<GMHexCell, NeighborData> QueueDict = new Dictionary<GMHexCell, NeighborData>();
        List<GMHexCell> neighborsQueue = new List<GMHexCell>();
        List<GMHexCell> roadBack = new List<GMHexCell>();

        //foreach(var item in coordList)
        //{
        //    Destroy(item.gameObject);
        //}
        //coordList.Clear();

        if(player.IsMoving() == true)
        {
            while(player.IsMoving() == true)
            {
                yield return new WaitForSeconds(0.01f);
            }
            startCell = roadMap.WorldToCell(player.transform.position);
            //movementPointsMax++;
        }

        GMHexCell firstPathCell = roads[startCell.x, startCell.y];
        pathPoints.Clear();

        bool isSearching = true;
        bool isDeadEnd = false;


        overlayMap.ClearAllTiles();

        QueueDict.Add(firstPathCell, new NeighborData());

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
                    if(currentNeighbors[j] != null && QueueDict.ContainsKey(currentNeighbors[j]) == false)
                    {
                        QueueDict.Add(currentNeighbors[j], new NeighborData(QueueDict[neighborsQueue[i]].cost + 1, neighborsQueue[i]));
                        neighborsQueue.Add(currentNeighbors[j]);

                        if(currentNeighbors[j].coordinates == new Vector3Int(goalCell.x, goalCell.y, 0))
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
                roadBack.Add(QueueDict[currentBackCell].source);
                currentBackCell = QueueDict[currentBackCell].source;

            }
            roadBack.Reverse();

            float colorBound = currentMovementPoints;

            for(int i = 0; i < roadBack.Count; i++){
                
                Tile currentTile = roadTile;

                if(currentMovementPoints == 0) 
                {
                    if(i != 0) currentTile = finishTile;
                    currentMovementPoints = movementPointsMax;
                } 

                if(i == roadBack.Count - 1) currentTile = finishTile;

                if(i > colorBound && currentTile != finishTile)
                    currentTile.color = UnityEngine.Color.red;
                else
                    currentTile.color = UnityEngine.Color.white;

                overlayMap.SetTile(roadBack[i].coordinates, currentTile);
                pathPoints.Add(roadMap.CellToWorld(roadBack[i].coordinates));

                //TMP_Text text = Instantiate(coordinates, pathPoints[i], Quaternion.identity);
                //text.text = "" + overlayMap.WorldToCell(pathPoints[i]).x + " " + overlayMap.WorldToCell(pathPoints[i]).y;
                //coordList.Add(text);

                currentMovementPoints--;
            }

            isGoalCellFinded = true;                      
        }
        else
        {
            isGoalCellFinded = false;
            Debug.Log("Dead End");
        }
        
    }

    public void ClearRoadTile(Vector2 point)
    {
        overlayMap.SetTile(roadMap.WorldToCell(point), null);
    }

    public void CheckFog(Vector2 position, float radius)
    {
        Vector3Int center = fogMap.WorldToCell(position);
        //Tile t = fogMap.SetColor();

        for(float x = -radius; x < radius; x++)
        {
            for(float y = -radius; y <= radius+1; y++)
            {
                Vector3Int checkPosition = new Vector3Int((int)x, (int)y, 0) + center;
                if(Vector3Int.Distance(checkPosition, center) < radius)
                {
                    fogMap.SetTile(checkPosition, null);
                }
            }
        }
    }

    public void RefreshPath(int passedPoints, float remainingPoints)
    {
        overlayMap.ClearAllTiles();
        //foreach(var item in coordList)
        //{
        //    Destroy(item.gameObject);
        //}
        //coordList.Clear();

        //Debug.Log("REFRESH пройдено = " + passedPoints + "; доступно = " + remainingPoints + " ; осталось = " + (pathPoints.Count - passedPoints));
        float newPointsCount = remainingPoints;
        float countTurns = 0;

        for(int i = passedPoints; i < pathPoints.Count; i++)
        {
            Tile currentTile = roadTile;                    

            if(i == pathPoints.Count - 1) currentTile = finishTile;

            if(newPointsCount == 0)
            {
                if(i != 0) currentTile = finishTile;
                newPointsCount = movementPointsMax;
                countTurns++;
            }

            if(newPointsCount == 0)
            {
                if(i != 0) currentTile = finishTile;
                newPointsCount = movementPointsMax;
            }

            if(currentTile != finishTile)
            {
                if(newPointsCount > 0 && countTurns == 0)
                    currentTile.color = UnityEngine.Color.white;
                else
                    currentTile.color = UnityEngine.Color.red;
            }

            //TMP_Text text = Instantiate(coordinates, pathPoints[i], Quaternion.identity);
            //text.text = "" + overlayMap.WorldToCell(pathPoints[i]).x + " " + overlayMap.WorldToCell(pathPoints[i]).y;
            //coordList.Add(text);
            overlayMap.SetTile(overlayMap.WorldToCell(pathPoints[i]), currentTile);
            newPointsCount--;
        }

    }

    private void LateUpdate()
    {
        CursorView cursorView = CursorView.Default;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int positionOnTileMap;

        positionOnTileMap = mapBG.WorldToCell(mousePosition);
        if(mapBG.HasTile(positionOnTileMap) == true) cursorView = CursorView.Default;

        positionOnTileMap = roadMap.WorldToCell(mousePosition);
        if(roadMap.HasTile(positionOnTileMap) == true) cursorView = CursorView.Movement;

        positionOnTileMap = fogMap.WorldToCell(mousePosition);
        if(fogMap.HasTile(positionOnTileMap) == true) cursorView = CursorView.Default;

        if(cursorManager != null) cursorManager.ChangeCursor(cursorView);
    }
}
