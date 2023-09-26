using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

public class CampBuilder : MonoBehaviour
{
    private GlobalMapTileManager gmManager;
    private CampManager campManager;
    private ObjectsPoolManager poolManager;

    public Tilemap bonfiresMap;
    public GameObject bonfirePrefab;

    private List<Vector3> emptyPoints = new List<Vector3>();

    [Inject]
    public void Construct(
        CampManager campManager,
        ObjectsPoolManager poolManager,
        GlobalMapTileManager manager
        )
    {
        this.campManager = campManager;
        this.poolManager = poolManager;
        this.gmManager = manager;
    }

    public void Build() 
    {
        emptyPoints = gmManager.GetEmptyPoints();
        foreach(Transform child in bonfiresMap.transform)
        {
            if(child.GetComponent<ClickableObject>() != null)
            {
                gmManager.AddBuildingToAllOnTheMap(child.gameObject);
                campManager.Register(child.gameObject);
            }
        }

        gmManager.GetTempPoints(bonfiresMap);        

        for(int i = 0; i < emptyPoints.Count; i++)
        {
            Vector3Int point = gmManager.SearchRealEmptyCellNearRoad(true, emptyPoints[i]);

            GameObject bonfire = poolManager.GetUnusualPrefab(bonfirePrefab);
            bonfire.transform.position = bonfiresMap.CellToWorld(point);
            bonfire.transform.SetParent(bonfiresMap.transform);
            bonfire.SetActive(true);

            gmManager.AddBuildingToAllOnTheMap(bonfire);
            campManager.Register(bonfire);
        }
    }

    public List<Vector3> GetPointsList()
    {
        return campManager.GetSaveData();
    }

    public void LoadData(List<Vector3> emptyCamps)
    {
         campManager.LoadCamps(emptyCamps);
    }
}