using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static NameManager;

public class MapBoxesManager : MonoBehaviour
{
    private GlobalMapTileManager gmManager;
    private ObjectsPoolManager poolManager;

    public Tilemap boxesMap;
    private List<Vector3> allBoxesPoints = new List<Vector3>();
    private List<GameObject> allBoxes = new List<GameObject>();
    private Dictionary<GameObject, BoxObject> allBoxesComponents = new Dictionary<GameObject, BoxObject>();

    private List<Vector3> actualBoxesPoints = new List<Vector3>();
    private List<GameObject> actualBoxes = new List<GameObject>();
    public GameObject boxPrefabs;

    private bool isFirstBuilding = true;
    private float buildPropability = 75f;


    public void Build(GlobalMapTileManager manager)
    {
        if(poolManager == null)
        {
            gmManager = manager;
            poolManager = GlobalStorage.instance.objectsPoolManager;
        }

        FillBoxPoints();
        Generation();        
    }

    private IEnumerator Regeneration()
    {
        bool canIGenerate = false; 
        while(canIGenerate == false)
        {
            canIGenerate = true;
            foreach(Transform box in boxesMap.gameObject.transform)
            {
                if(box.gameObject.activeInHierarchy == true)
                {
                    canIGenerate = false;
                    break;
                }
            }

            yield return null;
        }
        
        Generation();
    }

    private void Generation()
    {
        for(int i = 0; i < allBoxes.Count; i++)
        {
            if(buildPropability >= Random.Range(0, 101))
            {
                actualBoxesPoints.Add(allBoxesPoints[i]);
                actualBoxes.Add(allBoxes[i]);
                allBoxes[i].SetActive(true);

                gmManager.AddBuildingToAllOnTheMap(allBoxes[i]);

                if(isFirstBuilding == false) gmManager.CreateEnterPoint(allBoxes[i]);
            }
        }
    }

    private void FillBoxPoints()
    {
        List<Vector3Int> tempPoints = gmManager.GetTempPoints(boxesMap);

        foreach(var point in tempPoints)
            CreateBox(boxesMap.CellToWorld(point));
    }

    private void CreateBox(Vector3 position)
    {
        GameObject boxOnTheMap = Instantiate(boxPrefabs, position, Quaternion.identity);
        boxOnTheMap.transform.SetParent(boxesMap.transform);
        BoxObject box = boxOnTheMap.GetComponent<BoxObject>();
        box.SetMapBoxManager(this);
        allBoxesComponents.Add(boxOnTheMap, box);

        allBoxes.Add(boxOnTheMap);
        allBoxesPoints.Add(position);

        boxOnTheMap.SetActive(false);
    }

    private void ClearActualPoints()
    {
        for(int i = 0; i < actualBoxes.Count; i++)
        {
            gmManager.DeleteEnterPoint(actualBoxes[i]);
            allBoxesComponents[actualBoxes[i]].Death();
        }

        actualBoxesPoints.Clear();
        actualBoxes.Clear();
    }

    private void ReGenerateBoxesOnTheMap()
    {
        ClearActualPoints();
        StartCoroutine(Regeneration());
    }

    private void OnEnable()
    {
        EventManager.NewMonth += ReGenerateBoxesOnTheMap;
    }

    private void OnDisable()
    {
        EventManager.NewMonth -= ReGenerateBoxesOnTheMap;
    }
}
