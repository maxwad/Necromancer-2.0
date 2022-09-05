using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CastleBuilder : MonoBehaviour
{
    private GlobalMapTileManager gmManager;

    public Tilemap castlesMap;
    private List<Vector3> castlesPoints = new List<Vector3>();
    public GameObject castlePrefab;

    public int castlesCount = 5;

    public void Build(GlobalMapTileManager manager)
    {
        if(gmManager == null) gmManager = manager;

        List<Vector3Int> tempPoints = new List<Vector3Int>();

        for(int x = 0; x < castlesMap.size.x; x++)
        {
            for(int y = 0; y < castlesMap.size.y; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);

                if(castlesMap.HasTile(position) == true)
                {
                    tempPoints.Add(position);
                    castlesMap.SetTile(position, null);
                }
            }
        }

        while(castlesPoints.Count < castlesCount)
        {
            int randomPosition = Random.Range(0, tempPoints.Count);
            castlesPoints.Add(castlesMap.CellToWorld(tempPoints[randomPosition]));
            tempPoints.RemoveAt(randomPosition);
        }

        for(int i = 0; i < tempPoints.Count; i++)
        {
            manager.AddPointToEmptyPoints(castlesMap.CellToWorld(tempPoints[i]));
        }

        for(int i = 0; i < castlesPoints.Count; i++)
        {
            GameObject castle = Instantiate(castlePrefab, castlesPoints[i], Quaternion.identity);
            castle.transform.SetParent(castlesMap.transform);

            manager.AddBuildingToAllOnTheMap(castle);
        }
    }
}
