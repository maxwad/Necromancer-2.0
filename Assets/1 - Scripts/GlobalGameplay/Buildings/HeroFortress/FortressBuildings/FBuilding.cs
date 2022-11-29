using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

[Serializable]
public class UpgradeCost
{
    public float coins = 0;
    public float food = 0;
    public float iron = 0;
    public float wood = 0;
    public float stone = 0;
}

public class FBuilding : MonoBehaviour
{
    [Header("Start Parameters")]
    public FortressBuildings building;
    public int level = 0;
    public int maxLevel = 3;
    public Sprite activeIcon;
    public Sprite inactiveIcon;

    public List<RuneSO> effects;
    public UpgradeCost cost;
    public string buildingDescription;

    [Header("UI Elements")]
    [SerializeField] private Image buildingsIcon;
    [SerializeField] private GameObject levelBlock;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Button buildButton;
    [SerializeField] private TooltipTrigger description;
    [SerializeField] private InfotipTrigger costDescription;


    private BoostManager boostManager;

    private void OnEnable()
    {
        if(boostManager == null)
        {
            boostManager = GlobalStorage.instance.boostManager;
        }

        Init();
    }

    private void Init()
    {
        buildButton.gameObject.SetActive(true);

        description.header = building.ToString();
        description.content = buildingDescription;

        if(level == 0)
        {
            buildingsIcon.sprite = inactiveIcon;
            levelBlock.SetActive(false);
        }
        else
        {
            buildingsIcon.sprite = activeIcon;
            levelBlock.SetActive(true);
            levelText.text = LevelConverter();
            if(level >= maxLevel) buildButton.gameObject.SetActive(false);
        }
    }

    public int GetLevel()
    {
        return level;
    }

    public UpgradeCost GetBuildingsCost()
    {
        float discount = boostManager.GetBoost(BoostType.BuildingsDiscount);

        UpgradeCost currentCost = new UpgradeCost();
        currentCost.coins = (cost.coins + cost.coins * discount) * (level + 1);
        currentCost.food = (cost.food + cost.food * discount) * (level + 1);
        currentCost.wood = (cost.wood + cost.wood * discount) * (level + 1);
        currentCost.iron = (cost.iron + cost.iron * discount) * (level + 1);
        currentCost.stone = (cost.stone + cost.stone * discount) * (level + 1);

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

    public void Upgrade()
    {
        if(level < maxLevel) level++;

        Init();
        Debug.Log("Now level is " + level);
    }
}
