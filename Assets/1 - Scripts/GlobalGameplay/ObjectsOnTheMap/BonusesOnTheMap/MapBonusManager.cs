using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static NameManager;

public class MapBonusManager : MonoBehaviour
{
    private GlobalMapTileManager gmManager;
    private EnemyManager enemyManager;
    private ObjectsPoolManager poolManager;
    public GameObject heapsMap;
    public Tilemap roadMap;

    public int distanceBetweenHeaps = 15;
    public int countHeapsPerVertical = 5;
    public int heapsGap = 5;
    private int randomStartPointX;
    public Tile fogTile;

    private Dictionary<GameObject, Vector3> buildingsPointsDict = new Dictionary<GameObject, Vector3>();
    private Dictionary<EnemyArmyOnTheMap, Vector3> enemiesPointsDict = new Dictionary<EnemyArmyOnTheMap, Vector3>();
    private Dictionary<ResourceObject, Vector3> heapsPointsDict = new Dictionary<ResourceObject, Vector3>();

    public void InitializeHeaps(bool createMode)
    {
        enemyManager = GlobalStorage.instance.enemyManager;
        gmManager    = GlobalStorage.instance.gmManager;
        poolManager  = GlobalStorage.instance.objectsPoolManager;

        if(createMode == true)
            GenerateHeapsOnTheMap();

        GlobalStorage.instance.LoadNextPart();
    }

    public void GenerateHeapsOnTheMap()
    {
        buildingsPointsDict = gmManager.GetEnterPoints();
        enemiesPointsDict = enemyManager.GetEnemiesPointsDict();

        int width = roadMap.size.x;
        int height = roadMap.size.y;
        randomStartPointX = Random.Range(3, 15);

        List<Vector3> tempWorldPositions = new List<Vector3>();
        List<Vector3Int> tempCellPositions = new List<Vector3Int>();

        for(int currentX = randomStartPointX; currentX < width; currentX += distanceBetweenHeaps)
        {
            tempWorldPositions.Clear();
            tempCellPositions.Clear();

            for(int currentY = 0; currentY <= height; currentY++)
            {
                Vector3Int cellPosition = new Vector3Int(currentX, currentY, 0);
                if(roadMap.HasTile(cellPosition))
                {
                    Vector3 checkPosition = roadMap.CellToWorld(cellPosition);
                    tempCellPositions.Add(cellPosition);
                    tempWorldPositions.Add(checkPosition);
                }
            }

            int verticalEnemyCount = tempWorldPositions.Count / countHeapsPerVertical;
            verticalEnemyCount = (verticalEnemyCount == 0) ? 1 : verticalEnemyCount;

            for(int i = 0; i < tempCellPositions.Count; i += verticalEnemyCount)
            {
                //roadMap.SetTile(tempCellPositions[i], fogTile);                
                if(CheckPosition(tempWorldPositions[i]) == true)
                {
                    CreateHeap(tempWorldPositions[i]);
                }
            }
        }
    }

    private bool CheckPosition(Vector3 position)
    {
        bool isPositionFree = false;
        //int currentSearchIndex = 0;

        foreach(var point in enemiesPointsDict)
        {
            if(Vector3.Distance(point.Value, position) < heapsGap)
            {
                //roadMap.SetTile(roadMap.WorldToCell(position), fogTile);
                isPositionFree = false;
                return false;
            }
            else
            {
                isPositionFree = true;
                //currentSearchIndex++;
            }
        }

        foreach(var point in buildingsPointsDict)
        {
            if(Vector3.Distance(point.Value, position) < heapsGap)
            {
                //roadMap.SetTile(roadMap.WorldToCell(position), fogTile);
                isPositionFree = false;
                return false;
            }
            else
            {
                isPositionFree = true;
                //currentSearchIndex++;
            }
        }

        foreach(var point in heapsPointsDict)
        {
            if(Vector3.Distance(point.Value, position) < heapsGap / 2f)
            {
                //roadMap.SetTile(roadMap.WorldToCell(position), fogTile);
                isPositionFree = false;
                return false;
            }
            else
            {
                isPositionFree = true;
                //currentSearchIndex++;
            }
        }

        if(isPositionFree == true) isPositionFree = CheckPlayerPosition(position);

        //Debug.Log("Tries " + currentSearchIndex);
        return isPositionFree;
    }

    private bool CheckPlayerPosition(Vector3 position)
    {
        return !(GlobalStorage.instance.globalPlayer.transform.position == position);
    }

    private void CreateHeap(Vector3 position)
    {
        GameObject heapOnTheMap = poolManager.GetObject(ObjectPool.ResourceOnTheMap);
        heapOnTheMap.transform.SetParent(heapsMap.transform);
        heapOnTheMap.transform.position = position;
        heapOnTheMap.SetActive(true);

        ResourceObject heap = heapOnTheMap.GetComponent<ResourceObject>();
        heap.SetMapBonusManager(this);
        heapsPointsDict.Add(heap, position);
    }


    public void DeleteHeap(ResourceObject heap)
    {
        heapsPointsDict.Remove(heap);
    }

    public void ReGenerateHeapsOnTheMap()
    {
        StartCoroutine(ResetHeaps());
    }

    private IEnumerator ResetHeaps()
    {
        WaitForSeconds delay = new WaitForSeconds(0.1f);

        foreach(var enemy in new List<ResourceObject>(heapsPointsDict.Keys))
            enemy.Death();

        yield return delay;

        //wait till all enemies dissapear
        bool canIContinue = false;
        while(canIContinue == false)
        {
            canIContinue = true;
            foreach(var enemy in heapsPointsDict)
            {
                if(enemy.Key.gameObject.activeInHierarchy == true)
                {
                    canIContinue = false;
                    break;
                }
            }

            yield return delay;
        }

        heapsPointsDict.Clear();

        GenerateHeapsOnTheMap();
    }

    public bool IsHeapOnPosition(Vector3 pos)
    {
        foreach(var heap in heapsPointsDict)
        {
            if(heap.Value == pos)
                return true;
        }

        return false;
    }

    private void OnEnable()
    {
        EventManager.NewMonth += ReGenerateHeapsOnTheMap;
    }

    private void OnDisable()
    {
        EventManager.NewMonth -= ReGenerateHeapsOnTheMap;
    }

    ////FOR TESTING

    //private void Update()
    //{
    //    if(Input.GetMouseButtonDown(1) == true)
    //    {
    //        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //        pos = new Vector3(pos.x, pos.y, 0);
    //        Vector3Int pos3 = gmManager.CellConverterToV3Int(pos);
    //        pos = gmManager.CellConverterToV3(pos3);
    //        //Debug.Log("Click on " + pos + " player on " + GlobalStorage.instance.globalPlayer.gameObject.transform.position );
    //        CreateHeap(pos);
    //    }
    //}
}
