using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static NameManager;

public class ResourceBuilder : MonoBehaviour
{
    private GlobalMapTileManager gmManager;

    public Tilemap resourcesMap;
    [SerializeField] private GameObject buildingsContainer;
    private List<Vector3> resourcesPointsDynamic = new List<Vector3>();
    private List<ResourceBuilding> resBuildingsDynamic = new List<ResourceBuilding>();
    public GameObject resourcePrefab;

    public void Build(GlobalMapTileManager manager, List<Vector3> pointsToLoad)
    {
        if(gmManager == null) gmManager = manager;

        resBuildingsDynamic.Clear();

        List<Vector3Int> tempPoints = manager.GetTempPoints(resourcesMap);
        if(pointsToLoad == null)
        {
            foreach(var point in tempPoints)
                resourcesPointsDynamic.Add(resourcesMap.CellToWorld(point));
        }
        else
        {
            resourcesPointsDynamic = pointsToLoad;
        }

        for(int i = 0; i < resourcesPointsDynamic.Count; i++)
        {
            GameObject resBuilding = Instantiate(resourcePrefab, resourcesPointsDynamic[i], Quaternion.identity);
            resBuilding.transform.SetParent(buildingsContainer.transform);

            manager.AddBuildingToAllOnTheMap(resBuilding);

            resBuildingsDynamic.Add(resBuilding.GetComponent<ResourceBuilding>());
        }

        resBuildingsDynamic.Add(GlobalStorage.instance.heroFortress.GetCastleMint());

        CreateFullBuildingsList();
        foreach(var building in resBuildingsDynamic)
            building.Init(null);

    }

    public List<Vector3> GetPointsList()
    {
        return resourcesPointsDynamic;
    }

    private void  CreateFullBuildingsList()
    {
        foreach(Transform child in buildingsContainer.transform)
        {
            ResourceBuilding building = child.GetComponent<ResourceBuilding>();
            if(building != null)
            {
                if(resBuildingsDynamic.Contains(building) == false)
                    resBuildingsDynamic.Add(building);
            }
        }
    }

    public List<ResBuildingSD> GetSaveData()
    {
        List<ResBuildingSD> saveData = new List<ResBuildingSD>();

        foreach(var building in resBuildingsDynamic)
            saveData.Add(building.GetSaveData());

        return saveData;
    }

    public void LoadData(List<ResBuildingSD> buildingsData)
    {
        CreateFullBuildingsList();
        int counter = 0;
        foreach(var point in resBuildingsDynamic)
        {
            foreach(var building in buildingsData)
            {
                if(point.transform.position == building.position.ToVector3())
                {
                    point.LoadData(building);
                    counter++;
                    break;
                }
            }
        }
    }
}
