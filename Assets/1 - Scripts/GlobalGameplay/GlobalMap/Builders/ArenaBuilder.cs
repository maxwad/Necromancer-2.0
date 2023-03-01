using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ArenaBuilder : MonoBehaviour
{
    private GlobalMapTileManager gmManager;

    public Tilemap arenaMap;
    private Vector3 arenaPoint;
    public GameObject arenaPrefab;

    private List<Vector3> pointToSave = new List<Vector3>();

    public void Build(GlobalMapTileManager manager, List<Vector3> pointsToLoad)
    {
        if(gmManager == null) gmManager = manager;

        List<Vector3Int> tempPoints = manager.GetTempPoints(arenaMap);

        if(pointsToLoad == null)
        {
            int randomPosition = Random.Range(0, tempPoints.Count);
            arenaPoint = tempPoints[randomPosition];
            pointToSave.Add(arenaPoint);            
        }
        else
        {
            arenaPoint = pointsToLoad[0];
        }

        for(int i = 0; i < tempPoints.Count; i++)
        {
            if(tempPoints[i] != arenaPoint)
                manager.AddPointToEmptyPoints(arenaMap.CellToWorld(tempPoints[i]));

            arenaMap.SetTile(tempPoints[i], null);
        }

        GameObject arena = Instantiate(arenaPrefab, arenaPoint, Quaternion.identity);
        arena.transform.SetParent(arenaMap.transform);

        manager.AddBuildingToAllOnTheMap(arena);
    }

    public List<Vector3> GetPointsList()
    {
        return pointToSave;
    }
}
