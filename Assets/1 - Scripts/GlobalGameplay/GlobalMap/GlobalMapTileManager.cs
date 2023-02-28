using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using static NameManager;

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
    [HideInInspector] public MapBoxesManager boxesBuilder;
    [HideInInspector] public EnvironmentRegister environmentRegister;

    private List<GameObject> allBuildingsOnTheMap = new List<GameObject>();

    GlobalMapPathfinder gmPathfinder;
    private float startRadiusWithoutFog = 15;


    public void Init(bool createMode)
    {
        gmPathfinder        = GetComponent<GlobalMapPathfinder>();
        arenaBuilder        = GetComponent<ArenaBuilder>();
        castleBuilder       = GetComponent<CastleBuilder>();
        tombBuilder         = GetComponent<TombBuilder>();
        resourceBuilder     = GetComponent<ResourceBuilder>();
        campBuilder         = GetComponent<CampBuilder>();
        boxesBuilder        = GlobalStorage.instance.mapBoxesManager;
        environmentRegister = GetComponent<EnvironmentRegister>();

        roads = new GMHexCell[roadMap.size.x, roadMap.size.y];

        CreateRoadCells();
        CreateFogCells();
        SetHeighbors();

        if(createMode == true)
            CreateWorld();

        // end of loading initializing
        GlobalStorage.instance.LoadNextPart();
    }

    public void CreateWorld()
    {
        arenaBuilder.Build(this);
        castleBuilder.Build(this);
        tombBuilder.Build(this);
        resourceBuilder.Build(this);
        campBuilder.Build(this);
        boxesBuilder.Build(this);
        environmentRegister.Registration(this);

        CreateEnterPointsForAllBuildings();
    }

    //public void Finalizing()
    //{
    //    CreateEnterPointsForAllBuildings();
    //    SendDataToPathfinder();
    //}

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
        for(int x = 0; x < mapBG.size.x; x++)
        {
            for(int y = 0; y < mapBG.size.y; y++)
            {
                fogMap.SetTile(new Vector3Int(x, y, 0), fogTile);
            }
        }

        gmPathfinder.CheckFog(true, startRadiusWithoutFog);
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
                SortingBuildings(allBuildingsOnTheMap[i], pos);

                ClickableObject building = allBuildingsOnTheMap[i].GetComponent<ClickableObject>();
                if(building != null)
                    building.SetEnterPoint(pos);
            }
        }

        gmPathfinder.SetEnterPoints(enterPointsDict);
    }

    public void CreateEnterPoint(GameObject building)
    {
        allBuildingsOnTheMap.Add(building);
        Vector3Int enterPoint = SearchRealEmptyCellNearRoad(false, building.transform.position);
        Vector3 position = roadMap.CellToWorld(enterPoint);
        enterPointsDict.Add(building, position);
        SortingBuildings(building, position);
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
            //case TypeOfObjectOnTheMap.ResourceBuilding:
            //    break;
            //case TypeOfObjectOnTheMap.Outpost:
            //    break;
            case TypeOfObjectOnTheMap.Camp:
                break;
            //case TypeOfObjectOnTheMap.Altar:
            //    break;

            case TypeOfObjectOnTheMap.Portal:
                GlobalStorage.instance.portalsManager.AddPortal(building, position);
                break;

            case TypeOfObjectOnTheMap.RoadPointer:
                break;
            case TypeOfObjectOnTheMap.Arena:
                break;
            //case TypeOfObjectOnTheMap.Tomb:
            //    GlobalStorage.instance.tombsManager.Register(building, position);
            //    break;
            default:
                break;
        }
    }
    #endregion


    private void SendDataToPathfinder()
    {
        //gmPathfinder.roads = roads;
        //gmPathfinder.CheckFog(true, startRadiusWithoutFog);
        //gmPathfinder.SetEnterPoints(enterPointsDict);
    }

    public bool CheckCellAsEnterPoint(Vector3 cell)
    {
        foreach(var enterPoint in enterPointsDict)
        {
            if(enterPoint.Value == cell) return true;
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
