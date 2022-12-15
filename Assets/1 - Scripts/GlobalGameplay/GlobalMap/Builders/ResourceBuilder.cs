using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ResourceBuilder : MonoBehaviour
{
    private GlobalMapTileManager gmManager;

    public Tilemap resourcesMap;
    [SerializeField] private GameObject buildingsContainer;
    private List<Vector3> resourcesPoints = new List<Vector3>();
    public GameObject resourcePrefab;

    public void Build(GlobalMapTileManager manager)
    {
        if(gmManager == null) gmManager = manager;

        for(int x = 0; x < resourcesMap.size.x; x++)
        {
            for(int y = 0; y < resourcesMap.size.y; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                if(resourcesMap.HasTile(position) == true)
                {
                    resourcesPoints.Add(resourcesMap.CellToWorld(position));
                    resourcesMap.SetTile(position, null);
                }
            }
        }

        for(int i = 0; i < resourcesPoints.Count; i++)
        {
            GameObject resBuilding = Instantiate(resourcePrefab, resourcesPoints[i], Quaternion.identity);
            resBuilding.transform.SetParent(buildingsContainer.transform);

            manager.AddBuildingToAllOnTheMap(resBuilding);
        }
    }
}
