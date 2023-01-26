using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static NameManager;

public class VassalPathfinder : MonoBehaviour
{
    private GlobalMapTileManager tileManager;

    private Tilemap roadMap;
    private Tilemap overlayMap;
    private GMHexCell[,] roads;

    private Vector3Int startPoint;
    [SerializeField] private Tile testTile;
    [SerializeField] private int movementPoints = 20;
    [SerializeField] private int actionRadius = 50;
    [SerializeField] private int searchPlayerRadius = 15;


    public void Init()
    {
        tileManager = GlobalStorage.instance.gmManager;
        roadMap = GlobalStorage.instance.roadMap;
        overlayMap = GlobalStorage.instance.overlayMap;

        roads = tileManager.GetRoads();
    }

    public float GetMovementPoints()
    {
        return movementPoints;
    }

    public void FindRandomCell()
    {
        startPoint = overlayMap.WorldToCell(gameObject.transform.position);
        //Debug.Log(startPoint);

        int minX = ((startPoint.x - actionRadius) < 0) ? 0 : startPoint.x - actionRadius;
        int maxX = ((startPoint.x + actionRadius) >= roadMap.size.x) ? roadMap.size.x - 1 : startPoint.x + actionRadius;

        int minY = ((startPoint.y - actionRadius) < 0) ? 0 : startPoint.y - actionRadius;
        int maxY = ((startPoint.y + actionRadius) >= roadMap.size.y) ? roadMap.size.y - 1 : startPoint.y + actionRadius;


        //Debug.Log("minX = " + minX + " maxX = " + maxX + " minY = " + minY + " maxY = " + maxY);
        //Debug.Log("roadMap.size.x = " + roadMap.size.x + " roadMap.size.y = " + roadMap.size.y);
        List <Vector3Int> cells = new List<Vector3Int>();

        for(int i = minY; i < maxY; i++)
        {
            if(roads[minX, i] != null)
                cells.Add(new Vector3Int(minX, i, 0));

            if(roads[maxX, i] != null)
                cells.Add(new Vector3Int(maxX, i, 0));
        }

        for(int i = minX; i < maxX; i++)
        {
            if(roads[i, minY] != null)
                cells.Add(new Vector3Int(i, minY, 0));

            if(roads[i, maxY] != null)
                cells.Add(new Vector3Int(i, maxY, 0));
        }


        //Debug.Log(cells.Count);
        for(int i = 0; i < cells.Count; i++)
        {
            overlayMap.SetTile(cells[i], testTile);
        }
    }
}
