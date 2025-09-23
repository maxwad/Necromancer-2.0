using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using static Enums;
using Zenject;

public partial class GlobalMapTileManager : MonoBehaviour
{
    public Tile fogTile;
    public Tile testTile;
    private GMHexCell[,] roads;

    [Header("TileMaps")]
    public Tilemap mapBG;
    public Tilemap fogMap;
    public Tilemap roadMap;
    public Tilemap overlayMap;

    private List<Vector3> emptyPoints = new List<Vector3>();
    private Dictionary<GameObject, Vector3> enterPointsDict = new Dictionary<GameObject, Vector3>();

    [Header("Builders")]
    [HideInInspector] public ArenaBuilder arenaBuilder;
    [HideInInspector] public AltarBuilder altarBuilder;
    [HideInInspector] public CastleBuilder castleBuilder;
    [HideInInspector] public TombBuilder tombBuilder;
    [HideInInspector] public PortalBuilder portalBuilder;
    [HideInInspector] public ResourceBuilder resourceBuilder;
    [HideInInspector] public CampBuilder campBuilder;
    [HideInInspector] public MapBoxesManager boxesBuilder;
    [HideInInspector] public EnvironmentRegister environmentRegister;

    private List<GameObject> allBuildingsOnTheMap = new List<GameObject>();

    private float startRadiusWithoutFog = 15;
    private float constStep = 1.4f; // experimental const
    private List<Vector3Int> fogFreeCells = new List<Vector3Int>();

    private GameObject player;
    private GlobalMapPathfinder gmPathfinder;

    [Inject]
    public void Construct(GMPlayerMovement globalPlayer, MapBoxesManager boxesBuilder)
    {
        this.player = globalPlayer.gameObject;
        this.boxesBuilder = boxesBuilder;

        gmPathfinder        = GetComponent<GlobalMapPathfinder>();
        arenaBuilder        = GetComponent<ArenaBuilder>();
        altarBuilder        = GetComponent<AltarBuilder>();
        castleBuilder       = GetComponent<CastleBuilder>();
        tombBuilder         = GetComponent<TombBuilder>();
        portalBuilder       = GetComponent<PortalBuilder>();
        resourceBuilder     = GetComponent<ResourceBuilder>();
        campBuilder         = GetComponent<CampBuilder>();
        environmentRegister = GetComponent<EnvironmentRegister>();
    }

    public void Init(bool createMode)
    {
        roads = new GMHexCell[roadMap.size.x, roadMap.size.y];

        CreateRoadCells();
        SetHeighbors();

        if(createMode == true)
        {
            CreateWorld();
            CreateFogCells();
        }

        // end of loading initializing
        GlobalStorage.instance.LoadNextPart();
    }

    public void CreateWorld()
    {
        arenaBuilder.Build();
        altarBuilder.Build();
        castleBuilder.Build();
        tombBuilder.Build();
        portalBuilder.Build();
        resourceBuilder.Build();
        campBuilder.Build();
        boxesBuilder.Build();

        environmentRegister.Registration();
        CreateEnterPointsForAllBuildings();
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

        gmPathfinder.roads = roads;
    }

    public void CreateFogCells()
    {
        Vector3Int position;

        for(int x = 0; x < mapBG.size.x; x++)
        {
            for(int y = 0; y < mapBG.size.y; y++)
            {
                position = new Vector3Int(x, y, 0);
                fogMap.SetTile(position, fogTile);
            }
        }

        foreach(var cell in fogFreeCells)
            fogMap.SetTile(cell, null);

        CheckFog(true, startRadiusWithoutFog);
    }

    public void CheckFog(bool isFogNeeded, float radius)
    {
        // in release we need check FALSE
        if(isFogNeeded == true)
        {
            fogMap.gameObject.SetActive(false);
            return;
        }

        fogMap.gameObject.SetActive(true);

        Vector3Int center = fogMap.WorldToCell(player.transform.position);

        radius *= constStep;

        for(float x = -radius; x < radius; x++)
        {
            for(float y = -radius; y <= radius + 1; y++)
            {
                Vector3Int checkPosition = new Vector3Int((int)x, (int)y, 0) + center;
                if(Vector3Int.Distance(checkPosition, center) < radius)
                {
                    if(fogMap.GetTile(checkPosition) != null)
                        fogFreeCells.Add(checkPosition);

                    fogMap.SetTile(checkPosition, null);
                }
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

    public List<Vector3Int> GetTempPoints(Tilemap tilemap)
    {
        List<Vector3Int> tempPoints = new List<Vector3Int>();

        for(int x = 0; x < tilemap.size.x; x++)
        {
            for(int y = 0; y < tilemap.size.y; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);

                if(tilemap.HasTile(position) == true)
                {
                    tempPoints.Add(position);
                    tilemap.SetTile(position, null);
                }
            }
        }

        return tempPoints;
    }

    public void AddPointToEmptyPoints(Vector3 point)
    {
        if(emptyPoints.Contains(point) == false)
            emptyPoints.Add(point);
    }

    public void AddBuildingToAllOnTheMap(GameObject building)
    {
        if(allBuildingsOnTheMap.Contains(building) == false)
            allBuildingsOnTheMap.Add(building);
    }


    public List<Vector3> GetEmptyPoints()
    {
        return emptyPoints;
    }

    public Dictionary<GameObject, Vector3> GetEnterPoints()
    {
        return enterPointsDict;
    }

    public Vector3 GetEnterPoint(GameObject building)
    {
        return (enterPointsDict.ContainsKey(building) == true) ? enterPointsDict[building] : Vector3.zero;
    }

    public GMHexCell[,] GetRoads()
    {
        return roads;
    }


    private void CreateEnterPointsForAllBuildings()
    {
        Vector3 pos;
        for(int i = 0; i < allBuildingsOnTheMap.Count; i++)
        {
            Vector3Int enterPoint = SearchRealEmptyCellNearRoad(false, allBuildingsOnTheMap[i].transform.position);
            pos = roadMap.CellToWorld(enterPoint);
            //roadMap.SetTile(enterPoint, fogTile);
            if(enterPointsDict.ContainsKey(allBuildingsOnTheMap[i]) == false)
            {
                enterPointsDict.Add(allBuildingsOnTheMap[i], pos);

                ClickableObject building = allBuildingsOnTheMap[i].GetComponent<ClickableObject>();
                if(building != null)
                    building.SetEnterPoint(pos);
            }
        }

        gmPathfinder.SetEnterPoints(enterPointsDict);
    }

    public void CreateEnterPoint(GameObject building)
    {
        if(allBuildingsOnTheMap.Contains(building) == true) return;

        allBuildingsOnTheMap.Add(building);
        Vector3Int enterPoint = SearchRealEmptyCellNearRoad(false, building.transform.position);
        Vector3 position = roadMap.CellToWorld(enterPoint);
        enterPointsDict.Add(building, position);
    }

    public void DeleteEnterPoint(GameObject building)
    {
        allBuildingsOnTheMap.Remove(building);
        enterPointsDict.Remove(building);
    }

    public Vector3Int SearchRealEmptyCellNearRoad(bool mode, Vector3 point)
    {
        //mode = true - cell nearest road
        //mode = false - road

        Vector3Int pos = roadMap.WorldToCell(point);

        bool isFinded = false;
        int checkCount = 0;
        List<Vector3Int> neighborsQueue = new List<Vector3Int>();

        neighborsQueue.Add(pos);

        while(isFinded == false)
        {
            if(checkCount >= neighborsQueue.Count)
            {
                Debug.Log("SMTH WRONG");
                break;
            }

            for(int i = checkCount; i < neighborsQueue.Count; i++)
            {
                Vector3Int[] neighbors = GetNeihgborsArray(neighborsQueue[i]);
                pos = neighborsQueue[i];

                for(int l = 0; l < neighbors.Length; l++)
                {
                    if(roadMap.HasTile(neighbors[l]))
                    {
                        if(mode == false) pos = neighbors[l];
                        isFinded = true;
                        break;
                    }
                    else
                    {
                        if(neighborsQueue.Contains(neighbors[l]) == false)
                        {
                            neighborsQueue.Add(neighbors[l]);
                        }
                    }
                }
                checkCount++;
                if(isFinded == true) break;
            }
        }
        //mapBG.SetTile(pos, fogTile);

        Vector3Int[] GetNeihgborsArray(Vector3Int point)
        {
            Vector3Int[] neihgborsArray = new Vector3Int[6];

            neihgborsArray[0] = new Vector3Int(point.x + 1, point.y, point.z);
            neihgborsArray[1] = new Vector3Int(point.x - 1, point.y, point.z);

            if((point.y % 2) == 0)
            {
                neihgborsArray[2] = new Vector3Int(point.x, point.y + 1, point.z);
                neihgborsArray[3] = new Vector3Int(point.x, point.y - 1, point.z);
                neihgborsArray[4] = new Vector3Int(point.x - 1, point.y - 1, point.z);
                neihgborsArray[5] = new Vector3Int(point.x - 1, point.y + 1, point.z);
            }
            else
            {
                neihgborsArray[2] = new Vector3Int(point.x + 1, point.y + 1, point.z);
                neihgborsArray[3] = new Vector3Int(point.x + 1, point.y - 1, point.z);
                neihgborsArray[4] = new Vector3Int(point.x, point.y - 1, point.z);
                neihgborsArray[5] = new Vector3Int(point.x, point.y + 1, point.z);
            }

            return neihgborsArray;
        }

        return pos;
    }

    #endregion

    public bool CheckCellAsEnterPoint(Vector3 cell)
    {
        foreach(var enterPoint in enterPointsDict)
        {
            if(enterPoint.Value == cell) 
                return true;
        }

        return false;
    }

    public GMHexCell GetCell(Vector3 point)
    {
        Vector3Int cell = roadMap.WorldToCell(point);

        return roads[cell.x, cell.y];
    }

    public Vector3Int CellConverterToV3Int(Vector3 point)
    {
        return roadMap.WorldToCell(point);
    }

    public Vector3 CellConverterToV3(Vector3Int point)
    {
        return roadMap.CellToWorld(point);
    }
}
