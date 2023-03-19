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

    private FBuilding sourceBuilding;

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
    private bool researchIsComplete = false;
    private bool isDataLoading = false;


    public override GameObject Init(FBuilding building)
    {
        if(sourceBuilding == null)     
        {            
            sourceBuilding = building;
            SetStartParameters();
        }

        gameObject.SetActive(true);

        ResetSeals();

        return gameObject;
    }

    private void SetStartParameters()
    {
        if(fortress == null)
        {
            fortress = GlobalStorage.instance.heroFortress;
            resourcesManager = GlobalStorage.instance.resourcesManager;
            resourcesIcons = resourcesManager.GetAllResourcesIcons();
        }

        commonAmountNeed = 0;
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
    public void AddResource(bool loadMode = false)
    {
        if(loadMode == false)
            resourcesManager.ChangeResource(currentResourceType, -currentAmount);

        progressItems[currentResourceType].AddResource(currentAmount);

        BowlerUpdate();
    }

    private void ResearchComplete()
    {
        sliderBlock.SetActive(false);
        resultBlock.SetActive(true);

        if(researchIsComplete == false)
        {
            if(isDataLoading == false)
                fortress.AddSeals();

            researchIsComplete = true;
        }

        ResetSeals();
    }

    [System.Serializable]
    public class SanctuarySD
    {
        public bool isSanctContainer = false;
        public List<ResourceType> resources = new List<ResourceType>();
        public List<float> amounts = new List<float>();
    }

    public override object Save()
    {
        SanctuarySD saveData = new SanctuarySD();

        foreach(var item in progressItems)
        {
            saveData.resources.Add(item.Key);
            saveData.amounts.Add(item.Value.GetAmount());
        }

        saveData.isSanctContainer = true;

        return saveData;
    }

    public override void Load(List<object> saveData)
    {
        SanctuarySD loadData = null;

        foreach(var data in saveData)
        {
            if(data != null)
            {
                loadData = TypesConverter.ConvertToRequiredType<SanctuarySD>(data);

                if(loadData.isSanctContainer == true)
                    break;
            }
        }

        if(loadData != null)
        {
            SetStartParameters();
            isDataLoading = true;

            for(int i = 0; i < loadData.resources.Count; i++)
            {
                currentResourceType = loadData.resources[i];
                currentAmount = loadData.amounts[i];
                AddResource(true);
            }

            isDataLoading = false;
        }
        else
        {
            Debug.Log("There is no data for SANCTUARY!");
        }        
    }
}