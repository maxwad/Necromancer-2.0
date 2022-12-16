using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class ResourceBuildingUI : MonoBehaviour
{
    private ResourceBuilding currentBuilding;
    private ResourcesManager resourcesManager;
    private GMInterface gmInterface;
    private CanvasGroup canvas;
    private bool isWindowOpen = false;
    private bool isHeroInside = false;

    [Header("UI")]
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private TMP_Text caption;
    [SerializeField] private TMP_Text upgradesLimit;
    [SerializeField] private Image resourceIcon;
    [SerializeField] private TMP_Text resourceAmount;
    [SerializeField] private GameObject garrisonBlock;
    [SerializeField] private BuildingGarrison garrison;


    [SerializeField] private List<RBUpgradeItemUI> upgradesUIList;

    private void Start()
    {
        gmInterface = GlobalStorage.instance.gmInterface;
        canvas = uiPanel.GetComponent<CanvasGroup>();
        resourcesManager = GlobalStorage.instance.resourcesManager;
    }

    public void Open(bool openByClick, ResourceBuilding rBuilding)
    {
        currentBuilding = rBuilding;
        gmInterface.ShowInterfaceElements(false);

        MenuManager.instance.MiniPause(true);
        GlobalStorage.instance.ModalWindowOpen(true);

        uiPanel.SetActive(true);
        isWindowOpen = true;

        isHeroInside = !openByClick;

        Init();

        Fading.instance.FadeWhilePause(true, canvas);
    }

    public void Close()
    {
        gmInterface.ShowInterfaceElements(true);

        MenuManager.instance?.MiniPause(false);
        GlobalStorage.instance.ModalWindowOpen(false);
        isWindowOpen = false;

        uiPanel.SetActive(false);
    }

    private void Init()
    {
        caption.text = currentBuilding.buildingName;
        resourceIcon.sprite = currentBuilding.resourceSprite;

        currentBuilding.ResetResourceAmount();
        resourceAmount.text = "+" + currentBuilding.resourceAmount;

        upgradesLimit.text = currentBuilding.GetCountOfActiveUpgrades() + "/" + currentBuilding.maxCountUpgrades;

        int index = 0;
        foreach(var item in currentBuilding.upgradesStatus)
        {
            upgradesUIList[index].Init(item.Key, item.Value);
            index++;
        }

        garrisonBlock.SetActive(currentBuilding.isGarrisonThere);
    }


}
