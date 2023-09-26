using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

public class EnvironmentRegister : MonoBehaviour
{
    private GlobalMapTileManager gmManager;

    public Tilemap environmentMap;


    [Inject]
    public void Construct(GlobalMapTileManager gmManager)
    {
        this.gmManager = gmManager;
    }

    public void Registration()
    {
        foreach(Transform child in environmentMap.transform)
        {
            if(child.GetComponent<ClickableObject>() != null)
            {
                gmManager.AddBuildingToAllOnTheMap(child.gameObject);
            }
            else
            {
                foreach(Transform underChild in child.transform)
                {
                    if(underChild.GetComponent<ClickableObject>() != null)
                    {
                        gmManager.AddBuildingToAllOnTheMap(underChild.gameObject);
                    }
                }
            }
        }
    }
}
