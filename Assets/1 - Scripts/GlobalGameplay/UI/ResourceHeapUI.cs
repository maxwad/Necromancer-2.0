using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static NameManager;

public class ResourceHeapUI : MonoBehaviour
{
    private ResourcesManager resourcesManager;
    public GameObject uiPanel;
    private GameObject currentResource;
    public GameObject bonusItem;
    public GameObject bonusContainer;
    private Dictionary<ResourceType, Sprite> resourcesIcons;

    public TMP_Text caption;
    public Image resourceImage;
    public TMP_Text count;

    private float curiosity = 0;

    public void OpenWindow(ClickableObject obj)
    {
        GlobalStorage.instance.isModalWindowOpen = true;

        //currentResource = (obj == null) ? null : obj.gameObject;

        uiPanel.SetActive(true);

        Initialize(obj);
    }

    private void Initialize(ClickableObject obj)
    {
        if(resourcesManager == null) 
        {
            resourcesManager = GlobalStorage.instance.resourcesManager;
            resourcesIcons = resourcesManager.GetAllResourcesIcons();
        }

        curiosity = GlobalStorage.instance.playerStats.GetCurrentParameter(PlayersStats.Curiosity);
        //ResourceObject obj = currentResource.GetComponent<ResourceObject>();
        Reward reward = null;

        if(obj.objectType == TypeOfObjectOnTheMap.Resource)
        {
            reward = obj.GetComponent<ResourceObject>().reward;           
        }

        if(obj.objectType == TypeOfObjectOnTheMap.BoxBonus)
        {
            reward = obj.GetComponent<BoxObject>().reward;
        }

        if(reward != null)
        {
            for(int i = 0; i < reward.resourcesList.Count; i++)
            {
                CreateResourceItem(reward.resourcesList[i], reward.resourcesQuantity[i]);
            }
        }
        //if(obj != null)
        //{
        //    caption.text = "Heap";
        //    if(GlobalStorage.instance.playerStats.GetCurrentParameter(PlayersStats.Curiosity) > 1)
        //        count.text = obj.resourceType.ToString() + ": " + obj.quantity.ToString();
        //    else
        //        count.text = obj.resourceType.ToString() + ": " + "Some";

        //    resourceImage.sprite = obj.GetComponent<SpriteRenderer>().sprite;
        //    resourceImage.color = obj.GetComponent<SpriteRenderer>().color;
        //}
    }

    private void CreateResourceItem(ResourceType resType, float quantity)
    {
        GameObject item = Instantiate(bonusItem);
        item.transform.SetParent(bonusContainer.transform, false);

        bonusItem.GetComponentInChildren<Image>().sprite = resourcesIcons[resType];
        TMP_Text text = bonusItem.GetComponentInChildren<TMP_Text>();
        text.text = resType.ToString() + ": ";
        if(curiosity > 1)
            text.text += quantity;
        else
            text.text += "Some";
    }

    private void ClearContainer()
    {
        foreach(Transform bonusItem in bonusContainer.transform)
        {
            bonusItem.gameObject.SetActive(false);
        }
    }

    public void CloseWindow()
    {
        GlobalStorage.instance.isModalWindowOpen = false;
        uiPanel.SetActive(false);
    }
}
