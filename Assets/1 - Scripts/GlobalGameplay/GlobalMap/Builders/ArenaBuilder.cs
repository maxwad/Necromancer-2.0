using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

public class ArenaBuilder : MonoBehaviour
{
    private GlobalMapTileManager gmManager;
    private ObjectsPoolManager poolManager;

    public Tilemap arenaMap;
    private Vector3 arenaPoint;
    public GameObject arenaPrefab;

    private List<Vector3> pointToSave = new List<Vector3>();

    [Inject]
    public void Construct(ObjectsPoolManager poolManager, GlobalMapTileManager gmManager)
    {
        this.poolManager = poolManager;
        this.gmManager = gmManager;
    }

    public void Build(List<Vector3> pointsToLoad = null)
    {
        List<Vector3Int> tempPoints = gmManager.GetTempPoints(arenaMap);

        if(pointsToLoad == null)
        {
            int randomPosition = Random.Range(0, tempPoints.Count);
            arenaPoint = arenaMap.CellToWorld(tempPoints[randomPosition]);
            pointToSave.Add(arenaPoint);            
        }
        else
        {
            arenaPoint = pointsToLoad[0];
            pointToSave.Add(arenaPoint);
        }

        for(int i = 0; i < tempPoints.Count; i++)
        {
            if(tempPoints[i] != arenaMap.WorldToCell(arenaPoint))
                gmManager.AddPointToEmptyPoints(arenaMap.CellToWorld(tempPoints[i]));

            arenaMap.SetTile(tempPoints[i], null);
        }

        GameObject arena = poolManager.GetUnusualPrefab(arenaPrefab);
        arena.transform.position = arenaPoint;
        arena.transform.SetParent(arenaMap.transform);
        arena.SetActive(true);

        gmManager.AddBuildingToAllOnTheMap(arena);
    }

    public List<Vector3> GetPointsList()
    {
        return pointToSave;
    }
}
