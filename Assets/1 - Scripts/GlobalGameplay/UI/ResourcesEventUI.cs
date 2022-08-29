using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static NameManager;

public class ResourcesEventUI : MonoBehaviour
{
    public GameObject uiPanel;
    private GameObject currentResource;
    private bool isResurceEventWindowOpened = true;

    public TMP_Text caption;
    public Image resourceImage;
    public TMP_Text count;

    public void OpenWindow(ClickableObject obj)
    {
        GlobalStorage.instance.isModalWindowOpen = true;
        isResurceEventWindowOpened = true;

        currentResource = (obj == null) ? null : obj.gameObject;

        uiPanel.SetActive(true);

        Initialize();
    }

    private void Initialize()
    {
        ResourceObject obj = currentResource.GetComponent<ResourceObject>();
        if(obj != null)
        {
            caption.text = "Resource: " + obj.resourceType;
            if(GlobalStorage.instance.playerStats.GetCurrentParameter(PlayersStats.Curiosity) > 1)
                count.text = obj.quantity.ToString();
            else
                count.text = "Some";

            resourceImage.sprite = obj.GetComponent<SpriteRenderer>().sprite;
            resourceImage.color = obj.GetComponent<SpriteRenderer>().color;

        }
    }

    public void CloseWindow()
    {
        GlobalStorage.instance.isModalWindowOpen = false;
        isResurceEventWindowOpened = false;

        uiPanel.SetActive(false);
    }
}
