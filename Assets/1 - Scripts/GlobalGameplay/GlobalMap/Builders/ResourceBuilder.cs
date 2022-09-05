using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ResourceBuilder : MonoBehaviour
{
    private GlobalMapTileManager gmManager;

    public Tilemap resoursesMap;
    private List<Vector3> resoursesPoints = new List<Vector3>();
    public List<GameObject> resoursesPrefabs;


    public void Build(GlobalMapTileManager manager)
    {
        if(gmManager == null) gmManager = manager;

        for(int x = 0; x < resoursesMap.size.x; x++)
        {
            for(int y = 0; y < resoursesMap.size.y; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                if(resoursesMap.HasTile(position) == true)
                {
                    resoursesPoints.Add(resoursesMap.CellToWorld(position));
                    resoursesMap.SetTile(position, null);
                }
            }
        }

        for(int i = 0; i < resoursesPoints.Count; i++)
        {
            GameObject randomBuilding = resoursesPrefabs[Random.Range(0, resoursesPrefabs.Count)];
            GameObject resBuilding = Instantiate(randomBuilding, resoursesPoints[i], Quaternion.identity);
            resBuilding.transform.SetParent(resoursesMap.transform);

            manager.AddBuildingToAllOnTheMap(resBuilding);
        }
    }
}
