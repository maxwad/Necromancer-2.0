using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;
using static Enums;

public partial class MapBonusManager : MonoBehaviour
{
    private GlobalMapTileManager gmManager;
    private EnemyManager enemyManager;
    private ObjectsPoolManager poolManager;
    private GMPlayerMovement globalPlayer;
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

    [Inject]
    public void Construct(
        GlobalMapTileManager gmManager,
        EnemyManager enemyManager,
        ObjectsPoolManager poolManager,
        GMPlayerMovement globalPlayer
        )
    {
        this.gmManager = gmManager;
        this.enemyManager = enemyManager;
        this.poolManager = poolManager;
        this.globalPlayer = globalPlayer;
    }

    public void InitializeHeaps(bool createMode)
    {
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
                    CreateHeap(tempWorldPositions[i], true);
                }
            }
        }
    }

    private bool CheckPosition(Vector3 position)
    {
        foreach(var point in enemiesPointsDict)
        {
            if(Vector3.Distance(point.Value, position) < heapsGap)
            {
                //roadMap.SetTile(roadMap.WorldToCell(position), fogTile);
                return false;
            }
        }

        foreach(var point in buildingsPointsDict)
        {
            if(Vector3.Distance(point.Value, position) < heapsGap)
            {
                //roadMap.SetTile(roadMap.WorldToCell(position), fogTile);
                return false;
            }
        }

        foreach(var point in heapsPointsDict)
        {
            if(Vector3.Distance(point.Value, position) < heapsGap / 2f)
            {
                //roadMap.SetTile(roadMap.WorldToCell(position), fogTile);
                return false;
            }
        }

        return CheckPlayerPosition(position);
    }

    private bool CheckPlayerPosition(Vector3 position)
    {
        return !(globalPlayer.transform.position == position);
    }

    private ResourceObject CreateHeap(Vector3 position, bool createMode)
    {
        GameObject heapOnTheMap = poolManager.GetObject(ObjectPool.ResourceOnTheMap);
        heapOnTheMap.transform.SetParent(heapsMap.transform);
        heapOnTheMap.transform.position = position;
        heapOnTheMap.SetActive(true);

        ResourceObject heap = heapOnTheMap.GetComponent<ResourceObject>();
        if(createMode == true)
            heap.Birth();

        heap.SetMapBonusManager(this);
        heapsPointsDict.Add(heap, position);

        return heap;
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
}
