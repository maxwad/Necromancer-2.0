using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using static NameManager;

[Serializable]
public class UpgradeCost
{
    public ResourceType resourceType;
    public float resoursAmount = 0;
}

public class FBuilding : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [Header("Parameters")]
    public FortressBuildings building;
    public bool isPassiveEffect = true;
    public int level = 0;
    public int maxLevel = 3;
    public Sprite activeIcon;
    public Sprite inactiveIcon;

    public List<RuneSO> effects;
    public List<UpgradeCost> cost;
    public string buildingDescription;

    [Header("UI Elements")]
    [SerializeField] private Button buildingButton;
    [SerializeField] private Image buildingsIcon;
    [SerializeField] private GameObject levelBlock;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TooltipTrigger description;
    [SerializeField] private InfotipTrigger costDescription;

    private BoostManager boostManager;
    private OpeningBuildingWindow door;

    private void Start()
    { 
        boostManager = GlobalStorage.instance.boostManager;
        door = GlobalStorage.instance.fortressBuildingDoor;
    }

    private void OnEnable()
    {
        Init();
    }

    private void Init()
    {
        upgradeButton.gameObject.SetActive(true);

        description.header = building.ToString();
        description.content = buildingDescription;

        if(level == 0)
        {
            buildingButton.interactable = false;
            buildingsIcon.sprite = inactiveIcon;
            levelBlock.SetActive(false);
        }
        else
        {
            buildingButton.interactable = true;
            buildingsIcon.sprite = activeIcon;
            levelBlock.SetActive(true);
            levelText.text = LevelConverter();
            if(level >= maxLevel) upgradeButton.gameObject.SetActive(false);
        }
    }

    public int GetLevel()
    {
        return level;
    }

    public List<UpgradeCost> GetCost()
    {
        float discount = boostManager.GetBoost(BoostType.BuildingsDiscount);

        List<UpgradeCost> currentCost = new List<UpgradeCost>();
        foreach(var item in cost)
        {
            UpgradeCost itemCost = new UpgradeCost();
            itemCost.resourceType = item.resourceType;
            itemCost.resoursAmount = Mathf.Round(item.resoursAmount + item.resoursAmount * discount) * (level + 1);
            currentCost.Add(itemCost);
        }

        return currentCost;
    }

    private string LevelConverter()
    {
        string levelText = "";

        if(level == 1) levelText = "I";
        if(level == 2) levelText = "II";
        if(level == 3) levelText = "III";

        return levelText;
    }
    
    //Button
    public void Upgrade()
    {
        if(level < maxLevel) level++;

        Init();
    }

    public void Downgrade()
    {
        if(level > 0) level--;

        Init();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(level != maxLevel) costDescription.SetCost(GetCost());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(level > 0)
        {
            door.Open(this);
        }
        else
        {
            InfotipManager.ShowMessage("This building has not yet been built.");
        }
    }
}
