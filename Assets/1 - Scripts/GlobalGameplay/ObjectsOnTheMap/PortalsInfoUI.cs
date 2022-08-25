using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class PortalsInfoUI : MonoBehaviour
{
    private bool isStartingParametersCreated = false;

    private List<PortalsParts> portalsPartsList = new List<PortalsParts>();

    private bool isAccessAllowed = true;
    private float portalsKnowledge = 0;
    private PortalsManager portalsManager;

    private Dictionary<GameObject, Vector3> openedPortals = new Dictionary<GameObject, Vector3>();
    private List<Building> allPortals = new List<Building>();

    public GameObject buttonPlace;
    public GameObject buttonMap;
    public GameObject buttonPrefab;
    public GameObject buttonIconTeleportPrefab;

    public Button randomTeleport;
    public Button toCastleTeleport;
    public TMP_Text castleButtonText;

    private string toTheCastle = "To the Castle";
    private string comeBack = "Back to the Path";

    private Vector3Int mapSize;
    private Vector3 mapUISize;

    public void Initialize(bool mode)
    {
         if(isStartingParametersCreated == false) CreateStartParameters();

        isAccessAllowed = !mode;
        portalsKnowledge = GlobalStorage.instance.player.GetComponent<PlayerStats>().GetStartParameter(PlayersStats.Portal);

        openedPortals = portalsManager.CheckPortal(isAccessAllowed);


        for(int i = 0; i < portalsPartsList.Count; i++)
        {
            PortalsParts portal = portalsPartsList[i];

            bool isEnable = openedPortals.ContainsKey(portal.portal);
            portal.mainButtonGO.SetActive(isEnable);
            portal.buttonOnTheMapGO.SetActive(isEnable);

            if(isEnable == true)
            {
                if(portalsKnowledge < 2 || isAccessAllowed == false)
                {
                    portal.mainButton.interactable = false;
                    portal.buttonOnTheMap.interactable = false;
                    portal.mainButton.onClick.RemoveAllListeners();
                    portal.buttonOnTheMap.onClick.RemoveAllListeners();
                }
                else
                {
                    portal.mainButton.interactable = true;
                    portal.buttonOnTheMap.interactable = true;
                    portal.mainButton.onClick.AddListener(() => portalsManager.TeleportTo(portal.position));
                    portal.buttonOnTheMap.onClick.AddListener(() => portalsManager.TeleportTo(portal.position));
                }
            }
        }

        if(portalsKnowledge < 1 || isAccessAllowed == false)
        {
            randomTeleport.interactable = false;
        }
        else
        {
            randomTeleport.interactable = true;            
        }


        if(portalsKnowledge < 3)
        {
            toCastleTeleport.interactable = false;
        }
        else
        {
            toCastleTeleport.interactable = true;
            toCastleTeleport.onClick.RemoveAllListeners();

            if(portalsManager.IsThereBackPosition() == false)
            {
                toCastleTeleport.onClick.AddListener(portalsManager.TeleportToCastle);
                castleButtonText.text = toTheCastle;
            }
            else
            {
                toCastleTeleport.onClick.AddListener(portalsManager.BackToPath);
                castleButtonText.text = comeBack;
            }
        }
    }

    public class PortalsParts
    {
        public GameObject portal;

        public GameObject mainButtonGO;
        public GameObject buttonOnTheMapGO;

        public Button mainButton;
        public Button buttonOnTheMap;

        public Image mainButtonImage;
        public Image buttonOnTheMapImage;

        public Color color;
        public Vector3 position;
    }

    private void CreateStartParameters()
    {
        mapSize = GlobalStorage.instance.gmManager.mapBG.size;
        mapUISize.x = buttonMap.GetComponent<RectTransform>().rect.width;
        mapUISize.y = buttonMap.GetComponent<RectTransform>().rect.height;

        portalsManager = GlobalStorage.instance.portalsManager;
        allPortals = portalsManager.GetAllPortals();

        for(int i = 0; i < allPortals.Count; i++)
        {
            PortalsParts portalItem = CreatePortalData(allPortals[i]);
            portalsPartsList.Add(portalItem);
        }

        randomTeleport.onClick.RemoveAllListeners();
        randomTeleport.onClick.AddListener(portalsManager.TeleportToRandomPortal);

        isStartingParametersCreated = true;
    }

    private PortalsParts CreatePortalData(Building portal)
    {
        PortalsParts portalItem = new PortalsParts();
        portalItem.portal = portal.building;

        portalItem.mainButtonGO = Instantiate(buttonPrefab, buttonPlace.transform);
        portalItem.buttonOnTheMapGO = Instantiate(buttonIconTeleportPrefab, buttonMap.transform);

        portalItem.mainButton = portalItem.mainButtonGO.GetComponent<Button>();
        portalItem.buttonOnTheMap = portalItem.buttonOnTheMapGO.GetComponent<Button>();
        portalItem.buttonOnTheMap.GetComponent<RectTransform>().anchoredPosition = CalculateIconPosition(portal.position);

        portalItem.mainButtonImage = portalItem.mainButton.GetComponent<Image>();
        portalItem.buttonOnTheMapImage = portalItem.buttonOnTheMap.GetComponent<Image>();

        portalItem.color = portalItem.portal.GetComponent<SpriteRenderer>().color;
        portalItem.position = portal.position;

        portalItem.mainButtonImage.color = portalItem.color;
        portalItem.buttonOnTheMapImage.color = portalItem.color;

        portalItem.mainButtonGO.GetComponentInChildren<TMP_Text>().text = portal.building.name;

        return portalItem;
    }

    private Vector3 CalculateIconPosition(Vector3 portalPosition)
    {
        float rate = mapUISize.x / mapSize.x;
        Vector3Int posOnCell = GlobalStorage.instance.gmManager.mapBG.WorldToCell(portalPosition);

        return new Vector3(posOnCell.x, posOnCell.y, 0) * rate;
    }
}
