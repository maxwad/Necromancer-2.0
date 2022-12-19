using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class ResourceBuildingUI : MonoBehaviour
{
    private ResourceBuilding currentBuilding;
    private ResourcesSources resourcesSources;
    private GMInterface gmInterface;
    private CanvasGroup canvas;
    private bool isHeroInside = false;

    [Header("UI")]
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private TMP_Text caption;
    [SerializeField] private TMP_Text upgradesLimit;
    [SerializeField] private Image resourceIcon;
    [SerializeField] private TMP_Text resourceAmount;
    [SerializeField] private TMP_Text siegeAmount;
    [SerializeField] private GameObject garrisonBlock;
    private Garrison garrison;
    private GarrisonUI garrisonUI;

    [SerializeField] private List<RBUpgradeItemUI> upgradesUIList;

    private void Start()
    {
        gmInterface = GlobalStorage.instance.gmInterface;
        canvas = uiPanel.GetComponent<CanvasGroup>();
        resourcesSources = GlobalStorage.instance.resourcesManager.GetComponent<ResourcesSources>();
        garrisonUI = GetComponent<GarrisonUI>();
    }

    public void Open(bool openByClick, ResourceBuilding rBuilding)
    {
        currentBuilding = rBuilding;

        gmInterface.ShowInterfaceElements(false);

        MenuManager.instance.MiniPause(true);
        GlobalStorage.instance.ModalWindowOpen(true);

        uiPanel.SetActive(true);

        isHeroInside = !openByClick;
        if(isHeroInside == true) rBuilding.Register();

        garrison = rBuilding.garrison;

        if(currentBuilding.isGarrisonThere == true)
            garrisonUI.Init(isHeroInside, garrison);

        Init();

        Fading.instance.FadeWhilePause(true, canvas);
    }

    public void Close()
    {
        gmInterface.ShowInterfaceElements(true);

        MenuManager.instance?.MiniPause(false);
        GlobalStorage.instance.ModalWindowOpen(false);

        uiPanel.SetActive(false);
    }

    private void Init()
    {
        caption.text = currentBuilding.buildingName;
        resourceIcon.sprite = currentBuilding.resourceSprite;

        currentBuilding.ResetResourceAmount();
        resourceAmount.text = "+" + currentBuilding.resourceAmount;

        currentBuilding.ResetSiegeDays();
        siegeAmount.text = currentBuilding.currentSiegeDays + "/" + currentBuilding.siegeDays.ToString();

        int currentUpgrades = currentBuilding.GetCountOfActiveUpgrades();
        upgradesLimit.text = currentUpgrades + "/" + currentBuilding.maxCountUpgrades;

        bool isMax = currentUpgrades >= currentBuilding.maxCountUpgrades;

        int index = 0;
        foreach(var item in currentBuilding.upgradesStatus)
        {
            upgradesUIList[index].Init(this, item.Key, item.Value);
            if(isMax == true) upgradesUIList[index].ItemOff(item.Value);
            index++;
        }

        garrisonBlock.SetActive(currentBuilding.isGarrisonThere);
    }

    public void Upgrade(RBUpgradeSO upgrade)
    {
        currentBuilding.ApplyUpgrade(upgrade);
        Init();
    }

    public bool CheckSiegeStatus()
    {
        return currentBuilding.CheckSiegeStatus();
    }
}
