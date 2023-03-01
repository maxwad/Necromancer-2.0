using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CastleBuilder : MonoBehaviour
{
    private GlobalMapTileManager gmManager;
    private AISystem aiSystem;

    public Tilemap castlesMap;
    private List<Vector3> castlesPoints = new List<Vector3>();
    public GameObject castlePrefab;

    public int castlesCount = 9;

    public void Build(GlobalMapTileManager manager, List<Vector3> pointsToLoad)
    {
        if(gmManager == null)
        {
            gmManager = manager;
            aiSystem = GlobalStorage.instance.aiSystem;
        }

        List<Vector3Int> tempPoints = manager.GetTempPoints(castlesMap);

        if(pointsToLoad == null)
        {
            while(castlesPoints.Count < castlesCount)
            {
                int randomPosition = Random.Range(0, tempPoints.Count);
                castlesPoints.Add(castlesMap.CellToWorld(tempPoints[randomPosition]));
                tempPoints.RemoveAt(randomPosition);
            }
        }
        else
        {
            castlesPoints = pointsToLoad;
            foreach(var point in castlesPoints)
            {
                tempPoints.Remove(castlesMap.WorldToCell(point));
                Debug.Log("Castle point was deleted!");
            }
        }

        Debug.Log("There are " + tempPoints.Count + " Castle temp points left! Compare it later!");

        for(int i = 0; i < tempPoints.Count; i++)
        {
            manager.AddPointToEmptyPoints(castlesMap.CellToWorld(tempPoints[i]));
        }

        for(int i = 0; i < castlesPoints.Count; i++)
        {
            GameObject castle = Instantiate(castlePrefab, castlesPoints[i], Quaternion.identity);
            castle.transform.SetParent(castlesMap.transform);

            manager.AddBuildingToAllOnTheMap(castle);

            aiSystem.RegisterCastle(castle);
        }
    }

    public List<Vector3> GetPointsList()
    {
        return castlesPoints;
    }
}
