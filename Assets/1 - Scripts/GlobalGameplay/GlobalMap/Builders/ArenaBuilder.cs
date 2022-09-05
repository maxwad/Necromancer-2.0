using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ArenaBuilder : MonoBehaviour
{
    private GlobalMapTileManager gmManager;

    public Tilemap arenaMap;
    private Vector3 arenaPoint;
    public GameObject arenaPrefab;

    public void Build(GlobalMapTileManager manager)
    {
        if(gmManager == null) gmManager = manager;

        List<Vector3Int> tempPoints = new List<Vector3Int>();

        for(int x = 0; x < arenaMap.size.x; x++)
        {
            for(int y = 0; y < arenaMap.size.y; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);

                if(arenaMap.HasTile(position) == true) tempPoints.Add(position);
            }
        }

        int randomPosition = Random.Range(0, tempPoints.Count);

        for(int i = 0; i < tempPoints.Count; i++)
        {
            if(i == randomPosition)
                arenaPoint = arenaMap.CellToWorld(tempPoints[i]);
            else
                manager.AddPointToEmptyPoints(arenaMap.CellToWorld(tempPoints[i]));

            arenaMap.SetTile(tempPoints[i], null);
        }

        GameObject arena = Instantiate(arenaPrefab, arenaPoint, Quaternion.identity);
        arena.transform.SetParent(arenaMap.transform);

        manager.AddBuildingToAllOnTheMap(arena);
    }
}
