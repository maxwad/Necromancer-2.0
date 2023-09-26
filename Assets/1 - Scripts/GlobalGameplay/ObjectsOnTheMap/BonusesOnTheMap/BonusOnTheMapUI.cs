using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static NameManager;
using Zenject;

public class BonusOnTheMapUI : MonoBehaviour
{
    private ResourcesManager resourcesManager;
    private PlayerStats playerStats;
    private ObjectsPoolManager poolManager;
    public GameObject uiPanel;
    private CanvasGroup canvas;
    public GameObject bonusItemPrefab;
    public GameObject bonusContainer;
    private Dictionary<ResourceType, Sprite> resourcesIcons;

    public RectTransform blockUI;

    public float resourceBlockWidth = 300f;
    public float resourceBlockHeight = 300f;

    public float boxBlockWidth = 550f;
    public float boxBlockHeight = 300f;

    public TMP_Text caption;

    private List<GameObject> itemList = new List<GameObject>();
    private List<Image> resourceImagesList = new List<Image>();
    private List<TMP_Text> countList = new List<TMP_Text>();
    private List<TooltipTrigger> tooltipList = new List<TooltipTrigger>();

    private int countOfItems = 5;
    private float curiosity = 0;

    [Inject]
    public void Construct(
        ResourcesManager resourcesManager,
        PlayerStats playerStats,
        ObjectsPoolManager poolManager
        )
    {
        this.resourcesManager = resourcesManager;
        this.playerStats = playerStats;
        this.poolManager = poolManager;
    }

    public void OpenWindow(ClickableObject obj)
    {
        if(obj.objectType == TypeOfObjectOnTheMap.BoxBonus && obj.gameObject.GetComponent<BoxObject>().reward == null) return;

        GlobalStorage.instance.ModalWindowOpen(true);

        uiPanel.SetActive(true);
        if(canvas == null) 
            canvas = uiPanel.GetComponent<CanvasGroup>();

        Fading.instance.Fade(true, canvas);

        Initialize(obj);
    }

    private void Initialize(ClickableObject obj)
    {
        if(resourcesIcons == null) 
        {            
            resourcesIcons = resourcesManager.GetAllResourcesIcons();
            CreateAllItems();
        }

        curiosity = playerStats.GetCurrentParameter(PlayersStats.Curiosity);
        ClearContainer();

        Reward reward = null;

        if(obj.objectType == TypeOfObjectOnTheMap.Resource)
        {
            reward = obj.GetComponent<ResourceObject>().reward;
            caption.text = "Heap";
            ResizeContainer(0);
        }

        if(obj.objectType == TypeOfObjectOnTheMap.BoxBonus)
        {
            reward = obj.GetComponent<BoxObject>().reward;
            caption.text = "Box";
            ResizeContainer(1);
        }

        if(reward != null)
        {
            for(int i = 0; i < reward.resourcesList.Count; i++)
            {
                FillResourceItem(i, reward.resourcesList[i], reward.resourcesQuantity[i]);
            }
        }
    }

    private void FillResourceItem(int index, ResourceType resType, float quantity)
    {
        itemList[index].SetActive(true);

        resourceImagesList[index].sprite = resourcesIcons[resType];
        tooltipList[index].content = resType.ToString();

        if(curiosity > 0)
            countList[index].text = quantity.ToString();
        else
            countList[index].text = "Some";
    }

    private void CreateAllItems()
    {
        for(int i = 0; i < countOfItems; i++)
        {
            GameObject item = poolManager.GetUnusualPrefab(bonusItemPrefab);
            item.transform.SetParent(bonusContainer.transform, false);

            resourceImagesList.Add(item.GetComponentInChildren<Image>());
            countList.Add(item.GetComponentInChildren<TMP_Text>());
            tooltipList.Add(item.GetComponent<TooltipTrigger>());
            item.SetActive(true);

            itemList.Add(item);
        }

        itemList.ForEach(i => i.gameObject.SetActive(false));
    }

    private void ClearContainer()
    {
        foreach(Transform bonusItem in bonusContainer.transform)
        {
            bonusItem.gameObject.SetActive(false);
        }
    }

    private void ResizeContainer(int mode)
    {
        //0 - heap, 1 - box, 2 - battle
        float width = 0;
        float height = 0;

        if(mode == 0)
        {
            width = resourceBlockWidth;
            height = resourceBlockHeight;
        }

        if(mode == 1)
        {
            width = boxBlockWidth;
            height = boxBlockHeight;
        }

        blockUI.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        blockUI.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

    }

    public void CloseWindow()
    {
        GlobalStorage.instance.ModalWindowOpen(false);
        uiPanel.SetActive(false);
    }
}
