using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TombBuilder : MonoBehaviour
{
    private GlobalMapTileManager gmManager;

    public Tilemap tombsMap;
    private List<Vector3> tombsPoints = new List<Vector3>();
    public GameObject tombPrefab;

    public int tombsCount = 12;
    private TombsManager tombsManager;

    public void Build(GlobalMapTileManager manager, List<Vector3> pointsToLoad)
    {
        if(gmManager == null)
        {
            gmManager = manager;
            tombsManager = GlobalStorage.instance.tombsManager;
        }

        List<Vector3Int> tempPoints = manager.GetTempPoints(tombsMap);

        if(pointsToLoad == null)
        {
            while(tombsPoints.Count < tombsCount)
            {
                int randomPosition = Random.Range(0, tempPoints.Count);
                tombsPoints.Add(tombsMap.CellToWorld(tempPoints[randomPosition]));
                tempPoints.RemoveAt(randomPosition);
            }
        }
        else
        {
            tombsPoints = pointsToLoad;
            foreach(var point in tombsPoints)
                tempPoints.Remove(tombsMap.WorldToCell(point));
        }

        for(int i = 0; i < tempPoints.Count; i++)
        {
            manager.AddPointToEmptyPoints(tombsMap.CellToWorld(tempPoints[i]));
        }

        for(int i = 0; i < tombsPoints.Count; i++)
        {
            GameObject tomb = Instantiate(tombPrefab, tombsPoints[i], Quaternion.identity);
            tomb.transform.SetParent(tombsMap.transform);

            manager.AddBuildingToAllOnTheMap(tomb);

            tombsManager.Register(tomb);
        }
    }

    public List<Vector3> GetPointsList()
    {
        return tombsPoints;
    }
}
