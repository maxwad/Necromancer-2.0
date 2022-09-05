using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CampBuilder : MonoBehaviour
{
    private GlobalMapTileManager gmManager;

    public Tilemap bonfiresMap;
    public GameObject bonfirePrefab;

    private List<Vector3> emptyPoints = new List<Vector3>();

    public void Build(GlobalMapTileManager manager) 
    {
        if(gmManager == null) gmManager = manager;

        emptyPoints = manager.GetEmptyPoints();

        foreach(Transform child in bonfiresMap.transform)
        {
            if(child.GetComponent<ClickableObject>() != null)
            {
                manager.AddBuildingToAllOnTheMap(child.gameObject);
            }
        }

        for(int x = 0; x < bonfiresMap.size.x; x++)
        {
            for(int y = 0; y < bonfiresMap.size.y; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                if(bonfiresMap.HasTile(position) == true)
                {
                    bonfiresMap.SetTile(position, null);
                }
            }
        }

        for(int i = 0; i < emptyPoints.Count; i++)
        {
            Vector3Int point = manager.SearchRealEmptyCellNearRoad(true, emptyPoints[i]);

            GameObject bonfire = Instantiate(bonfirePrefab, bonfiresMap.CellToWorld(point), Quaternion.identity);
            bonfire.transform.SetParent(bonfiresMap.transform);

            manager.AddBuildingToAllOnTheMap(bonfire);
        }

    }    
}