using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

public class CastleBuilder : MonoBehaviour
{
    private GlobalMapTileManager gmManager;
    private AISystem aiSystem;
    private ObjectsPoolManager poolManager;

    public Tilemap castlesMap;
    private List<Vector3> castlesPoints = new List<Vector3>();
    public GameObject castlePrefab;

    public int castlesCount = 9;

    [Inject]
    public void Construct(
        ObjectsPoolManager poolManager, 
        AISystem aiSystem,
        GlobalMapTileManager manager)
    {
        this.poolManager = poolManager;
        this.aiSystem = aiSystem;
        this.gmManager = manager;
    }

    public void Build(List<Vector3> pointsToLoad = null)
    {
        List<Vector3Int> tempPoints = gmManager.GetTempPoints(castlesMap);

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
                tempPoints.Remove(castlesMap.WorldToCell(point));
        }

        for(int i = 0; i < tempPoints.Count; i++)
        {
            gmManager.AddPointToEmptyPoints(castlesMap.CellToWorld(tempPoints[i]));
        }

        for(int i = 0; i < castlesPoints.Count; i++)
        {
            GameObject castle = poolManager.GetUnusualPrefab(castlePrefab);
            castle.transform.position = castlesPoints[i];
            castle.transform.SetParent(castlesMap.transform);
            castle.SetActive(true);

            gmManager.AddBuildingToAllOnTheMap(castle);

            aiSystem.RegisterCastle(castle);
        }
    }

    public List<Vector3> GetPointsList() => castlesPoints;

    public AI_SD GetSaveData() => aiSystem.SaveData();

    public void LoadData(AI_SD aiData) => aiSystem.LoadData(aiData);
}
