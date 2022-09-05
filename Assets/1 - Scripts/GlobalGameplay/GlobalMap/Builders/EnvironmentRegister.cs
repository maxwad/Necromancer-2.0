using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnvironmentRegister : MonoBehaviour
{
    private GlobalMapTileManager gmManager;

    public Tilemap environmentMap;

    public void Registration(GlobalMapTileManager manager)
    {
        if(gmManager == null) gmManager = manager;

        foreach(Transform child in environmentMap.transform)
        {
            if(child.GetComponent<ClickableObject>() != null)
            {
                manager.AddBuildingToAllOnTheMap(child.gameObject);
            }
            else
            {
                foreach(Transform underChild in child.transform)
                {
                    if(underChild.GetComponent<ClickableObject>() != null)
                    {
                        manager.AddBuildingToAllOnTheMap(underChild.gameObject);
                    }
                }
            }
        }
    }
}
