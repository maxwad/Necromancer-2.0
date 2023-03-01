using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CampBuilder : MonoBehaviour
{
    private GlobalMapTileManager gmManager;
    private CampManager campManager;

    public Tilemap bonfiresMap;
    public GameObject bonfirePrefab;

    private List<Vector3> emptyPoints = new List<Vector3>();

    public void Build(GlobalMapTileManager manager) 
    {
        if(gmManager == null)
        {
            gmManager = manager;
            campManager = GlobalStorage.instance.campManager;
        }

        emptyPoints = gmManager.GetEmptyPoints();
        foreach(Transform child in bonfiresMap.transform)
        {
            if(child.GetComponent<ClickableObject>() != null)
            {
                gmManager.AddBuildingToAllOnTheMap(child.gameObject);
                campManager.Register(child.gameObject);
            }
        }

        manager.GetTempPoints(bonfiresMap);        

        for(int i = 0; i < emptyPoints.Count; i++)
        {
            Vector3Int point = gmManager.SearchRealEmptyCellNearRoad(true, emptyPoints[i]);

            GameObject bonfire = Instantiate(bonfirePrefab, bonfiresMap.CellToWorld(point), Quaternion.identity);
            bonfire.transform.SetParent(bonfiresMap.transform);

            gmManager.AddBuildingToAllOnTheMap(bonfire);
            campManager.Register(bonfire);
        }
    }
}