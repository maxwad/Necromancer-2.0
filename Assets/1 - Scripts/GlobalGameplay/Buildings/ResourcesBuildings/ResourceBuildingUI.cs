using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

public class ResourceBuildingUI : MonoBehaviour
{
    private ResourceBuilding currentBuilding;
    private GMInterface gmInterface;
    private CanvasGroup canvas;
    private bool isHeroInside = false;

    [Header("UI")]
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private TMP_Text caption;
    [SerializeField] private TMP_Text upgradesLimit;
    [SerializeField] private Image resourceIcon;
    [SerializeField] private TMP_Text resourceAmount;
    [SerializeField] private TMP_Text siegeAmountCurrent;
    [SerializeField] private TMP_Text siegeAmountMax;
    [SerializeField] private Image siegeIcon;
    [SerializeField] private Sprite siegeOn;
    [SerializeField] private Sprite siegeOff;
    [SerializeField] private GameObject siegeWarning;
    [SerializeField] private GameObject garrisonBlock;
    private Garrison garrison;
    private GarrisonUI garrisonUI;
    [SerializeField] private Color bonusColor;

    [SerializeField] private List<RBUpgradeItemUI> upgradesUIList;

    [Inject]
    public void Construct(GMInterface gmInterface)
    {
        this.gmInterface = gmInterface;

        canvas = uiPanel.GetComponent<CanvasGroup>();
        garrisonUI = GetComponent<GarrisonUI>();
    }

    public void Open(bool openByClick, ResourceBuilding rBuilding)
    {
        // RETURN in build!
        if(openByClick == true && rBuilding.CheckOwner() == false)
        {
            InfotipManager.ShowWarning("You can't look into buildings that aren't yours.");
            return;
        }

        //if(gmInterface == null)
        //{
        //    //we can't do it in Start becouse some "doors" are disable before open (Castle)
        //    gmInterface = GlobalStorage.instance.gmInterface;
        //    canvas      = uiPanel.GetComponent<CanvasGroup>();
        //    garrisonUI  = GetComponent<GarrisonUI>();
        //}
        currentBuilding = rBuilding;

        MenuManager.instance.MiniPause(true);
        uiPanel.SetActive(true);

        isHeroInside = !openByClick;
        if(isHeroInside == true) rBuilding.Register();

        garrison = rBuilding.garrison;

        Init();

        Fading.instance.FadeWhilePause(true, canvas);
    }

    public void Close()
    {
        MenuManager.instance?.MiniPause(false);
        uiPanel.SetActive(false);
    }

    private void Init()
    {
        caption.text = currentBuilding.buildingName;
        resourceIcon.sprite = currentBuilding.resourceSprite;

        currentBuilding.ResetResourceAmount();
        resourceAmount.text = "+" + currentBuilding.GetAmount();

        int currentUpgrades = currentBuilding.GetCountOfActiveUpgrades();
        upgradesLimit.text = currentUpgrades + "/" + currentBuilding.maxCountUpgrades;

        bool isMax = currentUpgrades >= currentBuilding.maxCountUpgrades;

        foreach(var item in upgradesUIList)
            item.ItemOff(false);

        int index = 0;
        foreach(var item in currentBuilding.GetUpgradesStatuses())
        {
            if(item.Value.isHidden != true)
            {
                upgradesUIList[index].Init(this, item.Key, item.Value.isEnable);

                if(isMax == true)
                    upgradesUIList[index].ItemOff(item.Value.isEnable);
            }
            else
            {
                upgradesUIList[index].ItemOff(false);
            }

            index++;
        }

        garrisonBlock.SetActive(currentBuilding.GetGarrisonStatus());
        if(currentBuilding.GetGarrisonStatus() == true)
            garrisonUI.Init(isHeroInside, garrison);

        bool isSiege = currentBuilding.CheckSiegeStatus();
        siegeIcon.sprite = (isSiege == true) ? siegeOn : siegeOff;
        siegeWarning.SetActive(isSiege);
        UpdateGarrisonEffect(garrison);
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

    public void UpdateGarrisonEffect(Garrison garr)
    {
        if(garr != garrison) return;

        currentBuilding.ResetSiegeDays();

        siegeAmountCurrent.text = currentBuilding.currentSiegeDays.ToString();
        siegeAmountMax.text = currentBuilding.siegeDays.ToString();
        siegeAmountCurrent.color = (currentBuilding.currentSiegeDays > currentBuilding.siegeDays) ? bonusColor : Color.white;

        if(currentBuilding.GetGarrisonStatus() == true)
            garrisonUI.UpdateArmies();
    }

    private void OnEnable()
    {
        EventManager.UpdateSiegeTerm += UpdateGarrisonEffect;
    }

    private void OnDisable()
    {
        EventManager.UpdateSiegeTerm -= UpdateGarrisonEffect;
    }
}
