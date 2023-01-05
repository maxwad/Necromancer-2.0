using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TombBuilder : MonoBehaviour
{
    private GlobalMapTileManager gmManager;

    public Tilemap tombsMap;
    private List<Vector3> tombsPoints = new List<Vector3>();
    public GameObject tombPrefab;

    public int tombsCount = 12;
    private EnemyArragement enemyArragement;
    private TombsManager tombsManager;

    public void Build(GlobalMapTileManager manager)
    {
        if(gmManager == null)
        {
            gmManager = manager;
            enemyArragement = GlobalStorage.instance.enemyManager.GetComponent<EnemyArragement>();
            tombsManager = GlobalStorage.instance.tombsManager;
        }

        List<Vector3Int> tempPoints = new List<Vector3Int>();

        for(int x = 0; x < tombsMap.size.x; x++)
        {
            for(int y = 0; y < tombsMap.size.y; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);

                if(tombsMap.HasTile(position) == true)
                {
                    tempPoints.Add(position);
                    tombsMap.SetTile(position, null);
                }
            }
        }

        while(tombsPoints.Count < tombsCount)
        {
            int randomPosition = Random.Range(0, tempPoints.Count);
            tombsPoints.Add(tombsMap.CellToWorld(tempPoints[randomPosition]));
            tempPoints.RemoveAt(randomPosition);
        }

        for(int i = 0; i < tempPoints.Count; i++)
        {
            manager.AddPointToEmptyPoints(tombsMap.CellToWorld(tempPoints[i]));
        }

        for(int i = 0; i < tombsPoints.Count; i++)
        {
            GameObject tomb = Instantiate(tombPrefab, tombsPoints[i], Quaternion.identity);
            tomb.transform.SetParent(tombsMap.transform);

            manager.AddBuildingToAllOnTheMap(tomb);

            tombsManager.Register(tomb);
        }
    }
}
