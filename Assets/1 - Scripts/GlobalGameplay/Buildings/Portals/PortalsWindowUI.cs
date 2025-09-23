using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Enums;
using Zenject;

public class PortalsWindowUI : MonoBehaviour, IInputableKeys
{
    [SerializeField] private GameObject uiPanel;
    private CanvasGroup canvas;
    private InputSystem inputSystem;
    private PortalsManager portalsManager;
    private ResourcesManager resourcesManager;
    private GMPlayerMovement globalPlayer;
    private GlobalMapTileManager gmManager;

    private bool isWindowOpened = false;

    private List<PortalsParts> portalsPartsList = new List<PortalsParts>();
    private Dictionary<GameObject, bool> allPortals = new Dictionary<GameObject, bool>();
    private bool isPortablePortal = false;
    private float portalsKnowledge = 0;

    [SerializeField] private TMP_Text caption;   

    [Header("Buttons")]
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

    public GameObject playerPrefab;
    private RectTransform playerIcon;
    private PlayerStats playerStats;

    [Header("Mana Cost")]
    public TMP_Text textDirectlyCost;
    public TMP_Text textRandomCost;
    public TMP_Text textToTheCastleCost;

    [Header("Colors")]
    public Color normalColor;
    public Color deniedColor;
    public Color activeColor;
    public Color deactiveColor;

    [Inject]
    public void Construct(InputSystem inputSystem,
        PortalsManager portalsManager,
        ResourcesManager resourcesManager,
        PlayerStats playerStats,
        GMPlayerMovement globalPlayer,
        GlobalMapTileManager gmManager
        )
    {
        this.inputSystem      = inputSystem;
        this.portalsManager   = portalsManager;
        this.resourcesManager = resourcesManager;
        this.playerStats      = playerStats;
        this.globalPlayer     = globalPlayer;
        this.gmManager        = gmManager;

        canvas = uiPanel.GetComponent<CanvasGroup>();
    }
    private void Start()
    {
        CreateStartParameters();

        RegisterInputKeys();
    }

    public void RegisterInputKeys()
    {
        inputSystem.RegisterInputKeys(KeyActions.Teleport, this);
    }

    public void InputHandling(KeyActions keyAction)
    {
        if(GlobalStorage.instance.isGlobalMode == false) return;

        if(portalsManager.CanIUsePortals() == true)
        {
            if(isWindowOpened == false)
            {
                if(MenuManager.instance.IsTherePauseOrMiniPause() == false)
                    Open(true, null);
            }
            else
            {
                CloseWindow();
            }            
        }
        else
        {
            InfotipManager.ShowWarning("You don't know how to use portals yet");
            return;
        }        
    }

    public void Open(bool modeClick, GameObject portal)
    {
        MenuManager.instance.MiniPause(true);
        uiPanel.SetActive(true);
        isWindowOpened = true;

        if(portal == null) 
            isPortablePortal = true;
        else
            portalsManager.UnlockPortal(portal);

        portalsManager.SetCurrentPortal(portal);
        CreatePortalMap();

        Fading.instance.FadeWhilePause(true, canvas);
    }

    public void CreatePortalMap()
    {
        if(allPortals.Count == 0)
        {
            allPortals = portalsManager.GetAllPortals();

            foreach(var portal in allPortals)
            {
                PortalsParts portalItem = CreatePortalData(portal.Key);
                portalsPartsList.Add(portalItem);
            }
        }
        
        if(portalsManager.GetCurrentPortal() == null)
            caption.text = "Portable Portal";
        else
            caption.text = portalsManager.GetCurrentPortal().gameObject.name;

        portalsKnowledge = playerStats.GetCurrentParameter(PlayersStats.Portal);
        playerIcon.anchoredPosition = CalculateIconPosition(globalPlayer.transform.position);

        textDirectlyCost.text = portalsManager.toCertainTeleportCost.ToString();
        textDirectlyCost.color = normalColor;

        textRandomCost.text = portalsManager.toRandomTeleportCost.ToString();
        textRandomCost.color = normalColor;

        textToTheCastleCost.text = portalsManager.toCastleTeleport.ToString();
        textToTheCastleCost.color = normalColor;

        for(int i = 0; i < portalsPartsList.Count; i++)
        {
            PortalsParts portal = portalsPartsList[i];

            bool isEnable = allPortals[portal.portal];
            portal.mainButtonGO.SetActive(isEnable);
            portal.buttonOnTheMapGO.SetActive(isEnable);

            if(isEnable == true)
            {
                if(portalsKnowledge < 2 || isPortablePortal == true)
                {
                    portal.mainButton.interactable = false;
                    portal.buttonOnTheMap.interactable = false;

                    portal.mainButtonText.color = deactiveColor;

                    portal.mainButton.onClick.RemoveAllListeners();
                    portal.buttonOnTheMap.onClick.RemoveAllListeners();
                }
                else
                {
                    portal.mainButton.interactable = true;
                    portal.buttonOnTheMap.interactable = true;

                    portal.mainButtonText.color = activeColor;

                    portal.mainButton.onClick.RemoveAllListeners();
                    portal.buttonOnTheMap.onClick.RemoveAllListeners();

                    portal.mainButton.onClick.AddListener(() => portalsManager.TeleportTo(portal.portal));
                    portal.buttonOnTheMap.onClick.AddListener(() => portalsManager.TeleportTo(portal.portal));
                }

                if(portal.portal == portalsManager.GetCurrentPortal())
                {
                    portal.mainButton.interactable = false;
                    portal.buttonOnTheMap.interactable = false;

                    portal.mainButtonText.color = deactiveColor;
                }

                if(CheckCostOfTeleport(portalsManager.toCertainTeleportCost) == false)
                {
                    portal.mainButton.interactable = false;
                    portal.buttonOnTheMap.interactable = false;

                    portal.mainButtonText.color = deactiveColor;

                    textDirectlyCost.color = deniedColor;
                } 
            }
        }

        if(portalsKnowledge < 1 || isPortablePortal == true)
        {
            randomTeleport.interactable = false;
        }
        else
        {
            randomTeleport.interactable = true;            
        }

        if(CheckCostOfTeleport(portalsManager.toRandomTeleportCost) == false)
        {
            randomTeleport.interactable = false;
            textRandomCost.color = deniedColor;
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

                if(CheckCostOfTeleport(portalsManager.toCastleTeleport) == false)
                {
                    toCastleTeleport.interactable = false;
                    textToTheCastleCost.color = deniedColor;
                }
            }
            else
            {
                toCastleTeleport.onClick.AddListener(portalsManager.BackToPath);
                castleButtonText.text = comeBack;

                textToTheCastleCost.text = portalsManager.toBackTeleport.ToString();
                textToTheCastleCost.color = normalColor;
            }
        }
    }

    private void CreateStartParameters()
    {
        mapSize = gmManager.mapBG.size;
        mapUISize.x = buttonMap.GetComponent<RectTransform>().rect.width;
        mapUISize.y = buttonMap.GetComponent<RectTransform>().rect.height;        

        randomTeleport.onClick.RemoveAllListeners();
        randomTeleport.onClick.AddListener(portalsManager.TeleportToRandomPortal);

        playerIcon = Instantiate(playerPrefab, buttonMap.transform).GetComponent<RectTransform>();
    }

    private PortalsParts CreatePortalData(GameObject portal)
    {
        PortalsParts portalItem = new PortalsParts();
        portalItem.portal = portal;

        portalItem.mainButtonGO = Instantiate(buttonPrefab, buttonPlace.transform);
        portalItem.buttonOnTheMapGO = Instantiate(buttonIconTeleportPrefab, buttonMap.transform);

        portalItem.mainButton = portalItem.mainButtonGO.GetComponent<Button>();
        portalItem.buttonOnTheMap = portalItem.buttonOnTheMapGO.GetComponent<Button>();
        portalItem.buttonOnTheMap.GetComponent<RectTransform>().anchoredPosition = CalculateIconPosition(portal.transform.position);

        portalItem.mainButtonImage = portalItem.mainButton.GetComponent<Image>();
        portalItem.buttonOnTheMapImage = portalItem.buttonOnTheMap.GetComponent<Image>();

        portalItem.color = portalItem.portal.GetComponent<SpriteRenderer>().color;
        portalItem.position = portal.transform.position;

        portalItem.mainButtonImage.color = portalItem.color;
        portalItem.buttonOnTheMapImage.color = portalItem.color;

        portalItem.mainButtonText = portalItem.mainButtonGO.GetComponentInChildren<TMP_Text>();

        portalItem.mainButtonText.text = portal.name;

        return portalItem;
    }

    private Vector3 CalculateIconPosition(Vector3 portalPosition)
    {
        float rate = mapUISize.x / mapSize.x;
        Vector3Int posOnCell = gmManager.mapBG.WorldToCell(portalPosition);

        return new Vector3(posOnCell.x, posOnCell.y, 0) * rate;
    }

    private bool CheckCostOfTeleport(float cost)
    {
        float currentMana = resourcesManager.GetResource(ResourceType.Mana);

        return (currentMana >= cost);
    }

    public void CloseWindow()
    {
        MenuManager.instance.MiniPause(false);
        isWindowOpened = false;
        isPortablePortal = false;
        uiPanel.SetActive(false);
    }
}
