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
    public Tile testTile;
    private GMHexCell[,] roads;

    [Header("TileMaps")]
    public Tilemap mapBG;
    public Tilemap fogMap;
    public Tilemap roadMap;

    private List<Vector3> emptyPoints = new List<Vector3>();
    private Dictionary<GameObject, Vector3> enterPointsDict = new Dictionary<GameObject, Vector3>();

    [Header("Builders")]
    [HideInInspector] public ArenaBuilder arenaBuilder;
    [HideInInspector] public CastleBuilder castleBuilder;
    [HideInInspector] public TombBuilder tombBuilder;
    [HideInInspector] public ResourceBuilder resourceBuilder;
    [HideInInspector] public CampBuilder campBuilder;
    [HideInInspector] public EnvironmentRegister environmentRegister;

    private List<GameObject> allBuildingsOnTheMap = new List<GameObject>();

    GlobalMapPathfinder gmPathfinder;
    private float startRadiusWithoutFog = 30;

    public void Load()
    {
        roads = new GMHexCell[roadMap.size.x, roadMap.size.y];
        gmPathfinder = GetComponent<GlobalMapPathfinder>();

        arenaBuilder = GetComponent<ArenaBuilder>();
        castleBuilder = GetComponent<CastleBuilder>();
        tombBuilder = GetComponent<TombBuilder>();
        resourceBuilder = GetComponent<ResourceBuilder>();
        campBuilder = GetComponent<CampBuilder>();
        environmentRegister = GetComponent<EnvironmentRegister>();

        CreateRoadCells();
        CreateFogCells();
        SetHeighbors();

        arenaBuilder.Build(this);
        castleBuilder.Build(this);
        tombBuilder.Build(this);
        resourceBuilder.Build(this);
        campBuilder.Build(this);
        environmentRegister.Registration(this);

        CreateEnterPoints();
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

    public void AddPointToEmptyPoints(Vector3 point)
    {
        emptyPoints.Add(point);
    }


    public void AddBuildingToAllOnTheMap(GameObject building)
    {
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

    private void CreateEnterPoints()
    {
        Vector3 pos;

        for(int i = 0; i < allBuildingsOnTheMap.Count; i++)
        {
            Vector3Int enterPoint = SearchRealEmptyCellNearRoad(false, allBuildingsOnTheMap[i].transform.position);
            pos = roadMap.CellToWorld(enterPoint);
            enterPointsDict.Add(allBuildingsOnTheMap[i], pos);
            SortingBuildings(allBuildingsOnTheMap[i], pos);
        }
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


    private void SortingBuildings(GameObject building, Vector3 position)
    {
        ClickableObject obj = building.GetComponent<ClickableObject>();

        if(obj == null) return;

        switch(obj.objectType)
        {
            case TypeOfObjectOnTheMap.PlayersCastle:
                GlobalStorage.instance.portalsManager.SetCastle(building, position);
                break;

            case TypeOfObjectOnTheMap.NecromancerCastle:
                break;
            case TypeOfObjectOnTheMap.Castle:
                break;
            case TypeOfObjectOnTheMap.ResoursesFarm:
                break;
            case TypeOfObjectOnTheMap.ResoursesQuarry:
                break;
            case TypeOfObjectOnTheMap.ResoursesMine:
                break;
            case TypeOfObjectOnTheMap.ResoursesSawmill:
                break;
            case TypeOfObjectOnTheMap.Outpost:
                break;
            case TypeOfObjectOnTheMap.Camp:
                break;
            case TypeOfObjectOnTheMap.Altar:
                break;

            case TypeOfObjectOnTheMap.Portal:
                GlobalStorage.instance.portalsManager.AddPortal(building, position);
                break;

            case TypeOfObjectOnTheMap.RoadPointer:
                break;
            case TypeOfObjectOnTheMap.Arena:
                break;
            case TypeOfObjectOnTheMap.Tomb:
                break;
            default:
                break;
        }
    }
    #endregion


    private void SendDataToPathfinder()
    {
        gmPathfinder.roads = roads;
        gmPathfinder.CheckFog(startRadiusWithoutFog);
        gmPathfinder.enterPointsDict = enterPointsDict;
    }
}
