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

    public void Build(GlobalMapTileManager manager, List<Vector3> pointsToLoad)
    {
        if(gmManager == null) gmManager = manager;

        List<Vector3Int> tempPoints = manager.GetTempPoints(resourcesMap);
        if(pointsToLoad == null)
        {
            foreach(var point in tempPoints)
                resourcesPoints.Add(resourcesMap.CellToWorld(point));
        }
        else
        {
            resourcesPoints = pointsToLoad;
        }

        for(int i = 0; i < resourcesPoints.Count; i++)
        {
            GameObject resBuilding = Instantiate(resourcePrefab, resourcesPoints[i], Quaternion.identity);
            resBuilding.transform.SetParent(buildingsContainer.transform);

            manager.AddBuildingToAllOnTheMap(resBuilding);
        }
    }

    public List<Vector3> GetPointsList()
    {
        return resourcesPoints;
    }
}
