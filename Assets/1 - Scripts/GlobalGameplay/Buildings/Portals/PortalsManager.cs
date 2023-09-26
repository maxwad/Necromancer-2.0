using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;
using Zenject;

public class PortalsManager : MonoBehaviour
{
    private PlayerStats playerStats;
    private GMPlayerMovement globalPlayer;
    private GlobalMapTileManager gmManager;
    private PortalsWindowUI portalsDoor;
    private GameObject castle;

    private Dictionary<GameObject, bool> allPortals = new Dictionary<GameObject, bool>();

    private Vector3 backPosition = Vector3.zero;

    [Header("Cost of Teleport")]
    public float toRandomTeleportCost = 10f;
    public float toCertainTeleportCost = 20f;
    public float toCastleTeleport = 30f;
    public float toBackTeleport = 0f;
    private GameObject currentPortal;

    [Inject]
    public void Construct(
        [Inject(Id = Constants.FORTRESS)] GameObject castle,
        PlayerStats playerStats,
        GMPlayerMovement globalPlayer,
        GlobalMapTileManager gmManager,
        PortalsWindowUI portalsDoor
        )
    {
        this.castle = castle;
        this.playerStats = playerStats;
        this.globalPlayer = globalPlayer;
        this.gmManager = gmManager;
        this.portalsDoor = portalsDoor;
    }

    public bool CanIUsePortals()
    {
        return playerStats.GetCurrentParameter(PlayersStats.Portal) > 3;
    }

    public void SetCurrentPortal(GameObject portal) => currentPortal = portal;

    public GameObject GetCurrentPortal() => currentPortal;

    public void Register(GameObject building) => allPortals.Add(building, false);

    public void UnlockPortal(GameObject portal = null)
    {
        if(portal == null) return;

        allPortals[portal] = true;
        portal.GetComponent<TooltipTrigger>()?.SetStatus(true);       
    }
    
    public Dictionary<GameObject, bool> GetAllPortals() => allPortals;

    public void TeleportTo(GameObject destinationPortal)
    {
        globalPlayer.TeleportTo(gmManager.GetEnterPoint(destinationPortal), toCertainTeleportCost);
        portalsDoor.CloseWindow();
    }

    public void TeleportToRandomPortal()
    {
        List<GameObject> portals = new List<GameObject>(allPortals.Keys);
        int index = Random.Range(0, portals.Count);
        if(portals[index] == currentPortal)
        {
            TeleportToRandomPortal();
        }
        else
        {
            Vector2 position = gmManager.GetEnterPoint(portals[index]);
            globalPlayer.TeleportTo(position, toRandomTeleportCost);
        }

        UnlockPortal(portals[index]);
        portalsDoor.CloseWindow();
    }

    public void TeleportToCastle()
    {
        backPosition = globalPlayer.transform.position;
        globalPlayer.TeleportTo(gmManager.GetEnterPoint(castle), toCastleTeleport);
        portalsDoor.CloseWindow();
    }

    public void ResurrectionTeleport()
    {
        globalPlayer.TeleportTo(gmManager.GetEnterPoint(castle), 0);
    }

    public void BackToPath()
    {
        if(backPosition != Vector3.zero) 
            globalPlayer.TeleportTo(backPosition, toBackTeleport);

        backPosition = Vector3.zero;
        portalsDoor.CloseWindow();
    }

    public bool IsThereBackPosition()
    {
        return (backPosition == Vector3.zero) ? false : true;
    }

    #region SAVE/LOAD
    public PortalsSD Save()
    {
        PortalsSD saveData = new PortalsSD();
        foreach(var portal in allPortals)
        {
            if(portal.Value == true)
                saveData.unlockedPortals.Add(portal.Key.transform.position.ToVec3());
        }

        saveData.backPosition = backPosition.ToVec3();

        return saveData;
    }

    public void Load(PortalsSD saveData)
    {
        foreach(var portalItem in saveData.unlockedPortals)
        {
            GameObject portal = allPortals.Where(item => item.Key.transform.position == portalItem.ToVector3()).First().Key;
            UnlockPortal(portal);
        }

        backPosition = saveData.backPosition.ToVector3();
    }
    #endregion
}
