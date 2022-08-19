using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using static NameManager;

public struct NeighborData
{
    public GMHexCell source;
    public int cost;

    public NeighborData(int currentCost = 0, GMHexCell currentSource = null)
    {
        source = currentSource;
        cost = currentCost;
    }
}

public class GlobalMapTileManager : MonoBehaviour
{
    public Tile fogTile;
    private GMHexCell[,] roads;

    [Header("TileMaps")]
    public Tilemap mapBG;
    public Tilemap fogMap;
    public Tilemap roadMap;
    public Tilemap arenaMap;
    public Tilemap castlesMap;
    public Tilemap tombsMap;
    public Tilemap resoursesMap;
    public Tilemap bonfiresMap;

    [Header("Prefabs")]
    public GameObject arenaPrefab;
    public GameObject castlePrefab;
    public GameObject tombPrefab;
    public GameObject bonfirePrefab;
    public List<GameObject> resoursesPrefabs;

    [Header("Points")]
    public Vector3 arenaPoint;
    public List<Vector3> castlesPoints;
    public List<Vector3> tombsPoints;
    public List<Vector3> resoursesPoints;

    private List<Vector3> emptyPoints = new List<Vector3>();
    private List<Vector3> convertedEmptyPoints = new List<Vector3>();
    private List<Vector3Int> tempPoints = new List<Vector3Int>();

    [Header("BuildingsCounters")]
    public int castlesCount = 5;
    public int tombsCount = 12;

    [Header("Buildings")]
    public List<GameObject> allBuildingsOnTheMap = new List<GameObject>();

    GlobalMapPathfinder gmPathfinder;
    private CursorManager cursorManager;
    private float startRadiusWithoutFog = 30;

    public void Load()
    {
        cursorManager = GlobalStorage.instance.cursorManager;
        roads = new GMHexCell[roadMap.size.x, roadMap.size.y];
        gmPathfinder = GetComponent<GlobalMapPathfinder>();

        CreateRoadCells();
        CreateFogCells();
        SetHeighbors();

        BuildArena();
        BuildCastles();
        BuildTombs();
        BuildResourses();
        FillEmptyPoints();

        SendDataToPathfinder();

        // end of loading map
        GlobalStorage.instance.canILoadNextStep = true;
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


    #region Generating

    private void BuildArena()
    {
        tempPoints.Clear();

        for(int x = 0; x < arenaMap.size.x; x++)
        {
            for(int y = 0; y < arenaMap.size.y; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);

                if(arenaMap.HasTile(position) == true) tempPoints.Add(position);
            }
        }

        int randomPosition = Random.Range(0, tempPoints.Count);

        for(int i = 0; i < tempPoints.Count; i++)
        {
            if(i == randomPosition)
                arenaPoint = arenaMap.CellToWorld(tempPoints[i]);
            else
                emptyPoints.Add(arenaMap.CellToWorld(tempPoints[i]));
        }

        GameObject arena = Instantiate(arenaPrefab, arenaPoint, Quaternion.identity);
        arena.transform.SetParent(arenaMap.transform);

        allBuildingsOnTheMap.Add(arena);
    }


    private void BuildCastles()
    {
        tempPoints.Clear();

        for(int x = 0; x < castlesMap.size.x; x++)
        {
            for(int y = 0; y < castlesMap.size.y; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);

                if(castlesMap.HasTile(position) == true) tempPoints.Add(position);
            }
        }

        while(castlesPoints.Count < castlesCount)
        {
            int randomPosition = Random.Range(0, tempPoints.Count);
            castlesPoints.Add(castlesMap.CellToWorld(tempPoints[randomPosition]));
            tempPoints.RemoveAt(randomPosition);
        }

        for(int i = 0; i < tempPoints.Count; i++)
        {
            emptyPoints.Add(castlesMap.CellToWorld(tempPoints[i]));
        }

        for(int i = 0; i < castlesPoints.Count; i++)
        {
            GameObject castle = Instantiate(castlePrefab, castlesPoints[i], Quaternion.identity);
            castle.transform.SetParent(castlesMap.transform);

            allBuildingsOnTheMap.Add(castle);
        }
    }

    private void BuildTombs()
    {
        tempPoints.Clear();

        for(int x = 0; x < tombsMap.size.x; x++)
        {
            for(int y = 0; y < tombsMap.size.y; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);

                if(tombsMap.HasTile(position) == true) tempPoints.Add(position);
            }
        }

        while(tombsPoints.Count < tombsCount)
        {
            int randomPosition = Random.Range(0, tempPoints.Count);
            tombsPoints.Add(tombsMap.CellToWorld(tempPoints[randomPosition]));
            tempPoints.RemoveAt(randomPosition);
        }

        for(int i = 0; i < tempPoints.Count; i++)
        {
            emptyPoints.Add(tombsMap.CellToWorld(tempPoints[i]));
        }

        for(int i = 0; i < tombsPoints.Count; i++)
        {
            GameObject tomb = Instantiate(tombPrefab, tombsPoints[i], Quaternion.identity);
            tomb.transform.SetParent(tombsMap.transform);

            allBuildingsOnTheMap.Add(tomb);
        }
    }

    private void BuildResourses()
    {
        int countOfPoints = 0;

        for(int x = 0; x < resoursesMap.size.x; x++)
        {
            for(int y = 0; y < resoursesMap.size.y; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                if(resoursesMap.HasTile(position) == true)
                {
                    countOfPoints++;
                    resoursesPoints.Add(resoursesMap.CellToWorld(position));
                }
            }
        }

        for(int i = 0; i < resoursesPoints.Count; i++)
        {
            GameObject randomBuilding = resoursesPrefabs[Random.Range(0, resoursesPrefabs.Count)];
            GameObject resBuilding = Instantiate(randomBuilding, resoursesPoints[i], Quaternion.identity);
            resBuilding.transform.SetParent(resoursesMap.transform);

            allBuildingsOnTheMap.Add(resBuilding);
        }
    }

    private void FillEmptyPoints()
    {
        for(int i = 0; i < emptyPoints.Count; i++)
{
            Vector3 point = SearchRealEmptyCellNearRoad(emptyPoints[i]);
            GameObject bonfire = Instantiate(bonfirePrefab, point, Quaternion.identity);
            bonfire.transform.SetParent(bonfiresMap.transform);

            allBuildingsOnTheMap.Add(bonfire);
        }


        Debug.Log(allBuildingsOnTheMap.Count);
    }

    private Vector3 SearchRealEmptyCellNearRoad(Vector3 point)
    {
        Vector3Int pos = roadMap.WorldToCell(point);

        

        return point;
    }

    #endregion

    private void SendDataToPathfinder()
    {
        gmPathfinder.roads = roads;
        gmPathfinder.CheckFog(startRadiusWithoutFog);
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
