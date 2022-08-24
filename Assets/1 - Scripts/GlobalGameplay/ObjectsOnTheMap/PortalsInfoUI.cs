using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class PortalsInfoUI : MonoBehaviour
{
    private bool isAccessDenied = false;
    private float portalsKnowledge = 0;
    private PortalsManager portalsManager;

    private Dictionary<GameObject, Vector3> openedPortals = new Dictionary<GameObject, Vector3>();
    private List<Building> allPortals = new List<Building>();

    private GameObject thisPortal;

    public GameObject buttonPlace;
    public GameObject buttonPrefab;
    private List<GameObject> buttonList = new List<GameObject>();

    public Button randomTeleport;

    private void OnEnable()
    {
        Initialize();
    }

    private void CheckAccess()
    {
        isAccessDenied = GetComponent<ObjectsInfoUI>().isOpenedByClick;
        portalsKnowledge = GlobalStorage.instance.player.GetComponent<PlayerStats>().GetStartParameter(PlayersStats.Portal);
    }

    private void Initialize()
    {
        CheckAccess();
        ResetWindow();

        portalsManager = GlobalStorage.instance.portalsManager;

        if(thisPortal == null) thisPortal = GetComponentInParent<ClickableObject>().gameObject;

        openedPortals = portalsManager.CheckPortal(thisPortal);
        if(allPortals.Count == 0) allPortals = portalsManager.GetAllPortals();

        foreach(var item in openedPortals)
        {
            GameObject button = Instantiate(buttonPrefab, buttonPlace.transform);
            button.GetComponentInChildren<TMP_Text>().text = item.Key.name;
            if(portalsKnowledge < 2 || isAccessDenied == true)
            {
                button.GetComponent<Button>().interactable = false;
            }
            else
            {
                Vector3 position = item.Value;
                button.GetComponent<Button>().onClick.AddListener(() => TeleportTo(position));
            }

            buttonList.Add(button);
        }

        if(portalsKnowledge < 1 || isAccessDenied == true)
        {
            randomTeleport.interactable = false;
        }
        else
        {
            randomTeleport.interactable = true;
            randomTeleport.onClick.RemoveAllListeners();
            randomTeleport.onClick.AddListener(TeleportToRandomPortal);
        }

    }

    public void TeleportTo(Vector3 newPosition)
    {

        Debug.Log("Click on " + newPosition);
    }

    public void TeleportToRandomPortal()
    {
        int index = Random.Range(0, allPortals.Count);
        if(allPortals[index].building == thisPortal)
        {
            TeleportToRandomPortal();
        }
        else
        {
            Vector2 position = allPortals[index].position;
            GlobalStorage.instance.globalPlayer.TeleportTo(position);
        }
    }


    public void ResetWindow()
    {
        foreach(var button in buttonList)
        {
            button.GetComponent<Button>().onClick.RemoveAllListeners();
            Destroy(button);
        }    
        buttonList.Clear();
    }


}
