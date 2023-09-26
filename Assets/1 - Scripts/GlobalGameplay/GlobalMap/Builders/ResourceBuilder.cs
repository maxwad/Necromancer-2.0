using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;
using static NameManager;

public class ResourceBuilder : MonoBehaviour
{
    private GlobalMapTileManager gmManager;
    private ObjectsPoolManager poolManager;
    private HeroFortress heroFortress;

    public Tilemap resourcesMap;
    [SerializeField] private GameObject buildingsContainer;
    private List<Vector3> resourcesPointsDynamic = new List<Vector3>();
    private List<ResourceBuilding> resBuildingsDynamic = new List<ResourceBuilding>();
    public GameObject resourcePrefab;

    [Inject]
    public void Construct(
        ObjectsPoolManager poolManager,
        GlobalMapTileManager manager,
        HeroFortress heroFortress)
    {
        this.poolManager = poolManager;
        this.gmManager = manager;
        this.heroFortress = heroFortress;
    }

    public void Build(List<Vector3> pointsToLoad = null)
    {

        resBuildingsDynamic.Clear();

        List<Vector3Int> tempPoints = gmManager.GetTempPoints(resourcesMap);
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
            GameObject resBuilding = poolManager.GetUnusualPrefab(resourcePrefab);
            resBuilding.transform.position = resourcesPointsDynamic[i];
            resBuilding.transform.SetParent(buildingsContainer.transform);
            resBuilding.SetActive(true);

            gmManager.AddBuildingToAllOnTheMap(resBuilding);

            resBuildingsDynamic.Add(resBuilding.GetComponent<ResourceBuilding>());
        }

        resBuildingsDynamic.Add(heroFortress.GetCastleMint());

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
