using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class Sanctuary : SpecialBuilding
{
    private HeroFortress fortress;
    private ResourcesManager resourcesManager;
    private Dictionary<ResourceType, Sprite> resourcesIcons;

    [SerializeField] private List<Cost> sealCost;
    private float commonAmountNeed = 0;
    private float commonAmountPaid = 0;

    [Header("Seals")]
    [SerializeField] private List<SoulSealItem> sealItems;

    [Header("Bowler")]
    [SerializeField] private TMP_Text commonProgressText;
    [SerializeField] private Image commonProgressImage;

    [Header("Resources")]
    [SerializeField] private List<SealResourceProgressItem> resourceBarsList;
    [SerializeField] private Dictionary<ResourceType, SealResourceProgressItem> progressItems = new Dictionary<ResourceType, SealResourceProgressItem>();
    [SerializeField] private List<ResourceColors> resourcesColors;

    [Header("Slider")]
    [SerializeField] private GameObject resultBlock;
    [SerializeField] private GameObject sliderBlock;
    [SerializeField] private Slider resourceSlider;
    [SerializeField] private Image sliderIcon;
    [SerializeField] private TMP_Text sliderAmount;
    private ResourceType currentResourceType;
    private float currentMaxAmount = 0;
    private float currentAmount = 0;


    public override GameObject Init(CastleBuildings building)
    {
        if(fortress == null)
        {
            fortress = GlobalStorage.instance.heroFortress;
            resourcesManager = GlobalStorage.instance.resourcesManager;
            resourcesIcons = resourcesManager.GetAllResourcesIcons();

            SetStartParameters();
        }

        gameObject.SetActive(true);

        ResetSeals();

        return gameObject;
    }

    private void SetStartParameters()
    {
        foreach(var item in sealCost)
            commonAmountNeed += item.amount;

        BowlerUpdate();

        for(int i = 0; i < sealCost.Count; i++)
        {
            if(progressItems.ContainsKey(sealCost[i].type) == false)
            {
                progressItems.Add(sealCost[i].type, resourceBarsList[i]);
                resourceBarsList[i].Init(this, i, sealCost[i], resourcesColors[i].color, resourcesIcons[sealCost[i].type]);

            }
        }

        resourceBarsList[0].ChangeResource();
    }

    private void ResetSeals()
    {
        int sealsAmount = fortress.GetSealsAmount();

        foreach(var seal in sealItems)
            seal.Activate(false);

        for(int i = 0; i < sealsAmount; i++)
            sealItems[i].Activate(true);
    }

    public void SelectSlider(int index, float leftAmount)
    {
        currentMaxAmount = resourcesManager.GetResource(sealCost[index].type);
        if(leftAmount < currentMaxAmount) 
            currentMaxAmount = leftAmount;

        currentResourceType = sealCost[index].type;

        resourceSlider.value = 0;
        SliderUpdate();
        sliderIcon.sprite = resourcesIcons[currentResourceType];
    }

    public void SelectAnoterSlider()
    {
        bool isInProcess = false;

        foreach(var resourceBar in resourceBarsList)
        {
            if(resourceBar.GetStatus() == false)
            {
                resourceBar.SetActive();
                isInProcess = true;
                break;
            }
        }

        if(isInProcess == false)
            ResearchComplete();
    }

    private void BowlerUpdate()
    {
        commonAmountPaid = 0;
        foreach(var resource in resourceBarsList)
            commonAmountPaid += resource.GetAmount();

        float progress = commonAmountPaid / commonAmountNeed;
        commonProgressImage.fillAmount = progress;
        commonProgressText.text = (Mathf.Round(progress * 100)) + "%";
    }

    //Slider
    public void SliderUpdate()
    {
        currentAmount = Mathf.Round(currentMaxAmount * resourceSlider.value);
        sliderAmount.text = currentAmount.ToString();
    }

    //Button
    public void AddResource()
    {
        resourcesManager.ChangeResource(currentResourceType, -currentAmount);
        progressItems[currentResourceType].AddResource(currentAmount);

        BowlerUpdate();
    }

    private void ResearchComplete()
    {
        sliderBlock.SetActive(false);
        resultBlock.SetActive(true);

        fortress.AddSeals();
        ResetSeals();
    }
}
