using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class Market : MonoBehaviour
{
    private HeroFortress fortress;
    private FortressBuildings allBuildings;
    private ResourcesManager resourcesManager;
    public Dictionary<ResourceType, Sprite> resourcesIcons;
    public Dictionary<int, ResourceType> resources = new Dictionary<int, ResourceType>();

    [SerializeField] private TMP_Text rateUI;

    [SerializeField] private Toggle[] playersStorage;
    [SerializeField] private Toggle[] marketsStorage;

    [SerializeField] private Slider playersSlider;
    [SerializeField] private TMP_Text playersSum;
    [SerializeField] private TMP_Text marketsSum;
    [SerializeField] private Image playersResource;
    [SerializeField] private Image marketsResource;
    [SerializeField] private Color errorColor;
    [SerializeField] private Color normalColor;

    private ResourceType currentPlayersRes;
    private ResourceType currentMarketsRes;

    private float currentMaxAmount = 0;
    private float playerGivesAmount = 0;
    private float marketGivesAmount = 0;

    private float exchangeRateBase = 100f;
    private float exchangeRate = 10f;
    private float currentRate = 0f;
    private float minRate = 0.1f;

    private float inflationStep = 0.05f;
    private float inflationPortion = 1000f;
    private float currentInflation = 0;

    private float marketDiscount = 0f;

    public void Init()
    {
        if(fortress == null)
        {
            fortress = GlobalStorage.instance.heroFortress;
            allBuildings = GlobalStorage.instance.fortressBuildings;
            resourcesManager = GlobalStorage.instance.resourcesManager;

            resourcesIcons = resourcesManager.GetAllResourcesIcons();
            resources.Add(0, ResourceType.Gold);
            resources.Add(1, ResourceType.Food);
            resources.Add(2, ResourceType.Wood);
            resources.Add(3, ResourceType.Stone);
            resources.Add(4, ResourceType.Iron);
        }

        gameObject.SetActive(true);

        ResetForm();
    }

    private void ResetForm()
    {
        foreach(var slot in playersStorage)
            slot.isOn = false;

        foreach(var slot in marketsStorage)
            slot.isOn = false;

        playersStorage[0].isOn = true;
        currentPlayersRes = resources[0];

        marketsStorage[1].isOn = true;
        currentMarketsRes = resources[1];

        marketsStorage[0].interactable = false;

        playersResource.sprite = resourcesIcons[ResourceType.Gold];
        marketsResource.sprite = resourcesIcons[ResourceType.Food];

        playerGivesAmount = 0;
        marketGivesAmount = 0;
        playersSum.text = "0";
        marketsSum.text = "0";

        playersSlider.value = 0;

        CalculateRate();

        rateUI.color = (currentRate < exchangeRate) ? errorColor : normalColor;
        rateUI.text = currentRate.ToString();
    }

    private void CalculateRate()
    {
        marketDiscount = allBuildings.GetBonusAmount(CastleBuildingsBonuses.MarketRate);

        //"-" because we have negative parameter
        currentRate = exchangeRate - (exchangeRate * marketDiscount);

        float inflationCount = currentInflation / inflationPortion;
        int marketPause = fortress.GetMarketPause();
        float difference = inflationCount - marketPause;

        if(difference <= 0)
        {
            currentInflation = 0f;
        }
        else
        {
            for(int i = 0; i < difference; i++)
                currentRate -= exchangeRate * inflationStep;

            currentInflation -= inflationPortion * marketPause;
        }

        if(currentRate < minRate) currentRate = minRate;
    }

    //Toggle Buttons
    public void ChangePlayersResource(int index)
    {
        currentPlayersRes = resources[index];
        playersResource.sprite = resourcesIcons[resources[index]];
        currentMaxAmount = resourcesManager.GetResource(currentPlayersRes);
        playersSlider.value = 0;

        int checkIndex = 0;
        for(int i = 0; i < marketsStorage.Length; i++)
        {
            marketsStorage[i].interactable = true;
            if(marketsStorage[i].isOn == true) checkIndex = i;
        }      

        if(checkIndex == index)
        {
            checkIndex = (index == 1) ? 0 : 1;
            marketsStorage[checkIndex].isOn = true;
        }

        marketsStorage[index].interactable = false;
    }

    //Toggle Buttons
    public void ChangeMarketsResource(int index)
    {
        currentMarketsRes = resources[index];
        marketsResource.sprite = resourcesIcons[resources[index]];
    }

    //Slider
    public void ChangeAmount()
    {
        playerGivesAmount = Mathf.Ceil(playersSlider.value * currentMaxAmount);
        playersSum.text = playerGivesAmount.ToString();

        marketGivesAmount = Mathf.Ceil(playerGivesAmount * (currentRate / exchangeRateBase));
        marketsSum.text = marketGivesAmount.ToString();
    }

    //Button
    public void Deal()
    {
        if(playerGivesAmount != 0)
        {
            resourcesManager.ChangeResource(currentPlayersRes, -playerGivesAmount);
            resourcesManager.ChangeResource(currentMarketsRes, marketGivesAmount);

            currentInflation += playerGivesAmount;

            ResetForm();
        }
    }
}
