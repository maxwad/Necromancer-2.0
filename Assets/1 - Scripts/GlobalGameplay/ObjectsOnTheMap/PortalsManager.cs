using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalsManager : MonoBehaviour
{
    [HideInInspector] public Dictionary<GameObject, Vector3> portalsDict = new Dictionary<GameObject, Vector3>();

    [HideInInspector] public List<Building> portals = new List<Building>();

    public void AddPortal(GameObject building, Vector3 position)
    {
        portalsDict.Add(building, position);
    }

    public Dictionary<GameObject, Vector3> CheckPortal(GameObject portal)
    {
        Dictionary<GameObject, Vector3> visitedPortals = new Dictionary<GameObject, Vector3>();

        if(portals.Count == 0) CreatePortalsList();

        for(int i = 0; i < portals.Count; i++)
        {
            if(portals[i].building == portal)
            {
                Building newPortal = portals[i];
                newPortal.isVisited = true;
                newPortal.status = "Portal is unlocked";
                portals[i] = newPortal;

                portals[i].building.GetComponent<TooltipTrigger>()?.SetStatus(true);
            }
        }

        foreach(var item in portals)
        {
            if(item.isVisited == true)
            {
                visitedPortals.Add(item.building, item.position);
            }
        }

        return visitedPortals;
    }
    
    public List<Building> GetAllPortals()
    {
        return portals;
    }

    private void CreatePortalsList()
    {
        foreach(var portal in portalsDict)
        {
            portals.Add(new Building(portal.Key, portal.Value));
        }
    }
}
