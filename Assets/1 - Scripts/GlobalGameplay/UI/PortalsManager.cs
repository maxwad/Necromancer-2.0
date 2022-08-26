using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalsManager : MonoBehaviour
{
    public GameObject uiPanel;
    private ObjectsInfoUI commonInfo;
    private PortalsInfoUI portalsInfo;

    [HideInInspector] public Dictionary<GameObject, Vector3> portalsDict = new Dictionary<GameObject, Vector3>();

    [HideInInspector] public List<Building> portals = new List<Building>();

    [HideInInspector] public GameObject currentPortal;

    private Building castle;

    private Vector3 backPosition = Vector3.zero;

    [Header("Cost of Teleport")]
    public float toRandomTeleportCost = 10f;
    public float toCertainTeleportCost = 20f;
    public float toCastleTeleport = 30f;
    public float toBackTeleport = 0f;

    public void OpenWindow(bool mode, ClickableObject obj)   
    {
        GlobalStorage.instance.isModalWindowOpen = true;

        currentPortal = obj.gameObject;
        if(portals.Count == 0) CreatePortalsList();

        uiPanel.SetActive(true);

        if(commonInfo == null) commonInfo = uiPanel.GetComponent<ObjectsInfoUI>();
        if(portalsInfo == null) portalsInfo = uiPanel.GetComponent<PortalsInfoUI>();

        commonInfo.Initialize(mode, obj);
        portalsInfo.Initialize(mode);
    }


    public void AddPortal(GameObject building, Vector3 position)
    {
        portalsDict.Add(building, position);
    }


    public void SetCastle(GameObject building, Vector3 position)
    {
        castle = new Building(building, position);
    }


    public Dictionary<GameObject, Vector3> CheckPortal( bool mode)
    {
        Dictionary<GameObject, Vector3> visitedPortals = new Dictionary<GameObject, Vector3>();        

        if(mode == true)
        {
            for(int i = 0; i < portals.Count; i++)
            {
                if(portals[i].building == currentPortal)
                {
                    Building newPortal = portals[i];
                    newPortal.isVisited = true;
                    newPortal.status = "Portal is unlocked";
                    portals[i] = newPortal;

                    portals[i].building.GetComponent<TooltipTrigger>()?.SetStatus(true);
                }
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

    public void TeleportTo(Vector3 newPosition)
    {
        GlobalStorage.instance.globalPlayer.TeleportTo(newPosition, toCertainTeleportCost);
    }

    public void TeleportToRandomPortal()
    {
        int index = Random.Range(0, portals.Count);
        if(portals[index].building == currentPortal)
        {
            TeleportToRandomPortal();
        }
        else
        {
            Vector2 position = portals[index].position;
            GlobalStorage.instance.globalPlayer.TeleportTo(position, toRandomTeleportCost);
        }
    }

    public void TeleportToCastle()
    {
        backPosition = GlobalStorage.instance.globalPlayer.transform.position;
        GlobalStorage.instance.globalPlayer.TeleportTo(castle.position, toCastleTeleport);
    }

    public void BackToPath()
    {
        if(backPosition != Vector3.zero) GlobalStorage.instance.globalPlayer.TeleportTo(backPosition, toBackTeleport);
        backPosition = Vector3.zero;
    }

    public bool IsThereBackPosition()
    {
        return (backPosition == Vector3.zero) ? false : true;
    }

    public void CloseWindow()
    {
        GlobalStorage.instance.isModalWindowOpen = false;

        uiPanel.SetActive(false);
    }
}
