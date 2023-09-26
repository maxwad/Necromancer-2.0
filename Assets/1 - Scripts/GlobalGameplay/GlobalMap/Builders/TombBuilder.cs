using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

public class TombBuilder : MonoBehaviour
{
    private GlobalMapTileManager gmManager;
    private ObjectsPoolManager poolManager;

    public Tilemap tombsMap;
    private List<Vector3> tombsPoints = new List<Vector3>();
    public GameObject tombPrefab;

    public int tombsCount = 12;
    private TombsManager tombsManager;

    [Inject]
    public void Construct(
        ObjectsPoolManager poolManager,
        TombsManager tombsManager,
        GlobalMapTileManager gmManager
        )
    {
        this.poolManager = poolManager;
        this.tombsManager = tombsManager;
        this.gmManager = gmManager;
    }

    public void Build(List<Vector3> pointsToLoad = null)
    {
        List<Vector3Int> tempPoints = gmManager.GetTempPoints(tombsMap);

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
            gmManager.AddPointToEmptyPoints(tombsMap.CellToWorld(tempPoints[i]));
        }

        for(int i = 0; i < tombsPoints.Count; i++)
        {
            GameObject tomb = poolManager.GetUnusualPrefab(tombPrefab);
            tomb.transform.position = tombsPoints[i];
            tomb.transform.SetParent(tombsMap.transform);
            tomb.SetActive(true);

            gmManager.AddBuildingToAllOnTheMap(tomb);

            tombsManager.Register(tomb);
        }

        tombsManager.HideSpells();
    }

    public List<Vector3> GetPoints()
    {
        return tombsPoints;
    }

    public List<TombsSD> GetSaveData()
    {
        return tombsManager.GetSaveData();
    }

    public void LoadData(List<TombsSD> tombsData)
    {
        tombsManager.LoadTomb(tombsData);        
    }
}
