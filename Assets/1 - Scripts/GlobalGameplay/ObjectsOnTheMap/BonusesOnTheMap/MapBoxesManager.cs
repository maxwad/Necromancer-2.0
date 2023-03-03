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

    private float buildPropability = 75f;


    public void Build(GlobalMapTileManager manager, Dictionary<Vector3, Reward> boxesData)
    {
        if(poolManager == null)
        {
            gmManager = manager;
            poolManager = GlobalStorage.instance.objectsPoolManager;
        }

        FillBoxPoints();

        if(boxesData == null)
            Generation();
        else
            LoadBoxes(boxesData);
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
                gmManager.CreateEnterPoint(allBoxes[i]);
            }
        }
    }

    #region SAVE/LOAD

    private void LoadBoxes(Dictionary<Vector3, Reward> boxesData)
    {
        actualBoxesPoints.Clear();
        actualBoxes.Clear();

        int counter = 0;
        foreach(var box in allBoxes)
        {
            if(boxesData.ContainsKey(box.transform.position) == true)
            {
                actualBoxesPoints.Add(box.transform.position);
                actualBoxes.Add(box);
                box.SetActive(true);

                if(allBoxesComponents.ContainsKey(box) == true)
                    allBoxesComponents[box].Load(boxesData[box.transform.position]);

                gmManager.AddBuildingToAllOnTheMap(box);
                gmManager.CreateEnterPoint(box);
                counter++;
            }
        }

        Debug.Log("Boxes loaded: " + counter + "/" + boxesData.Count);
    }

    public Dictionary<Vector3, Reward> SaveBoxes()
    {
        Dictionary<Vector3, Reward> boxesData = new Dictionary<Vector3, Reward>();

        foreach(var box in actualBoxes)
            boxesData.Add(box.transform.position, allBoxesComponents[box].SaveReward());

        return boxesData;
    }

    #endregion

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
