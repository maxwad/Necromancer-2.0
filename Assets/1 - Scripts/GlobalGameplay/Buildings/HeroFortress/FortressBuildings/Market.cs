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

    [SerializeField] private TMP_Text rateUI;

    [SerializeField] private Toggle[] playersStorage;
    [SerializeField] private Toggle[] marketsStorage;
    [SerializeField] private ToggleGroup players;
    [SerializeField] private ToggleGroup markets;

    [SerializeField] private Toggle[] typeOfDeal;
    [SerializeField] private Slider playersSlider;
    [SerializeField] private Slider marketsSlider;
    [SerializeField] private TMP_Text playersSumm;
    [SerializeField] private TMP_Text marketsSumm;
    [SerializeField] private Image playersResource;
    [SerializeField] private Image marketsResource;
    [SerializeField] private Color errorColor;
    [SerializeField] private Color normalColor;


    private float exchangeRateBase = 100f;
    private float exchangeRate = 10f;
    private float currentRate = 0f;

    private float timeDiscountBase = 0.05f;
    private float timeDiscountCurrent = 0f;
    private float inflationPortion = 1000f;
    private float marketDiscount = 0f;

    private float inRate = 0f;
    private float outRate = 0f;

    public void Init()
    {
        if(fortress == null)
        {
            fortress = GlobalStorage.instance.heroFortress;
            allBuildings = GlobalStorage.instance.fortressBuildings;
            resourcesManager = GlobalStorage.instance.resourcesManager;
            resourcesIcons = resourcesManager.GetAllResourcesIcons();
        }

        gameObject.SetActive(true);
        ResetForm();

        marketDiscount = allBuildings.GetBonusAmount(CastleBuildings.Market);
        CalculateRate();
        rateUI.text = Mathf.Round(currentRate).ToString();
    }

    private void CalculateRate()
    {
        //float timeDiscount = fortress.GetMarketPause();
        outRate = exchangeRate - (exchangeRate * marketDiscount);
        currentRate = outRate;
        //Debug.Log(outRate);
        //if(currentRate == 0) currentRate = outRate;

        //int discountCout = fortress.GetMarketPause();
        //for(int i = 0; i < discountCout; i++)
        //{
        //    currentRate = currentRate + outRate * timeDiscountBase;
        //    if(currentRate >   outRate) 
        //    {
        //        currentRate = outRate;
        //        break;
        //    }
        //}
    }

    private void ResetForm()
    {
        foreach(var item in playersStorage)
            item.isOn = false;

        foreach(var item in marketsStorage)
            item.isOn = false;

        playersStorage[0].isOn = true;
        marketsStorage[1].isOn = true;

        marketsStorage[0].interactable = false;

        playersResource.sprite = resourcesIcons[ResourceType.Gold];
        marketsResource.sprite = resourcesIcons[ResourceType.Food];

        playersSumm.text = "0";
        marketsSumm.text = "0";

        playersSlider.value = 0;
        marketsSlider.value = 0;

        playersSlider.gameObject.SetActive(typeOfDeal[0].isOn);
        marketsSlider.gameObject.SetActive(!typeOfDeal[0].isOn);
    }
}
