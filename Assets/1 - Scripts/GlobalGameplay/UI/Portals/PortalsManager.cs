using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class PortalsManager : MonoBehaviour
{
    private PlayerStats playerStats;

    public GameObject uiPanel;
    private CanvasGroup canvas;
    private PortalsInfoUI portalsInfo;
    private bool isPortalWindowOpened = false;

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

    private void Awake()
    {
        playerStats = GlobalStorage.instance.playerStats;
    }

    private bool CanIUsePortals()
    {
        return playerStats.GetCurrentParameter(PlayersStats.Portal) > 0;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T) && CanIUsePortals() == true)
        {
            if(MenuManager.instance.IsTherePauseOrMiniPause() == false && GlobalStorage.instance.isGlobalMode == true)
            {
                if(isPortalWindowOpened == false)
                    OpenWindow(true, null);
                else
                    CloseWindow();
            }
        }
    }

    public void OpenWindow(bool mode, ClickableObject obj)   
    {
        if(CanIUsePortals() == false)
        {
            InfotipManager.ShowWarning("You don't know how to use portals yet");
            return;
        }

        GlobalStorage.instance.ModalWindowOpen(true);
        isPortalWindowOpened = true;

        currentPortal = (obj == null) ? null : obj.gameObject;

        if(portals.Count == 0) CreatePortalsList();

        uiPanel.SetActive(true);
        if(canvas == null) canvas = uiPanel.GetComponent<CanvasGroup>();
        Fading.instance.Fade(true, canvas);

        if(portalsInfo == null) portalsInfo = uiPanel.GetComponent<PortalsInfoUI>();
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

    public Dictionary<GameObject, Vector3> CheckPortal(bool mode, GameObject portal = null)
    {
        Dictionary<GameObject, Vector3> visitedPortals = new Dictionary<GameObject, Vector3>();
        GameObject chekingPortal = (portal == null) ? currentPortal : portal;

        if(mode == true)
        {
            for(int i = 0; i < portals.Count; i++)
            {
                if(portals[i].building == chekingPortal)
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
        CloseWindow();
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

        CheckPortal(true, portals[index].building);
        CloseWindow();
    }

    public void TeleportToCastle()
    {
        backPosition = GlobalStorage.instance.globalPlayer.transform.position;
        GlobalStorage.instance.globalPlayer.TeleportTo(castle.position, toCastleTeleport);
        CloseWindow();
    }

    public void ResurrectionTeleport()
    {
        GlobalStorage.instance.globalPlayer.TeleportTo(castle.position, 0);
    }

    public void BackToPath()
    {
        if(backPosition != Vector3.zero) GlobalStorage.instance.globalPlayer.TeleportTo(backPosition, toBackTeleport);
        backPosition = Vector3.zero;
        CloseWindow();
    }

    public bool IsThereBackPosition()
    {
        return (backPosition == Vector3.zero) ? false : true;
    }

    public void CloseWindow()
    {
        GlobalStorage.instance.ModalWindowOpen(false);
        isPortalWindowOpened = false;
        currentPortal = null;
        uiPanel.SetActive(false);
    }
}
