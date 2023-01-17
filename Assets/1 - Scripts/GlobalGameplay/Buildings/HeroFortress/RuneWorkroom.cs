using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;
using System;

public class RuneWorkroom : SpecialBuilding
{
    private RunesManager runesManager;
    private FortressBuildings allBuildings;
    private ObjectsPoolManager poolManager;
    private ResourcesManager resourcesManager;
    private Dictionary<ResourceType, Sprite> resourcesIcons;

    [Header("Common UI")]
    [SerializeField] private GameObject storeContainer;
    [SerializeField] private GameObject herosRunesContainer;
    [SerializeField] private GameObject detailsBlock;
    [SerializeField] private List<RuneDetailsItem> detailsItems;
    [SerializeField] private TMP_Text runeName;
    [SerializeField] private GameObject destroyBlock;
    [SerializeField] private GameObject destroyButton;
    [SerializeField] private GameObject confirmDestroyBlock;

    private List<RuneSO> runesInStore = new List<RuneSO>();
    [SerializeField] private List<RuneRWItemUI> runesInStoreUI = new List<RuneRWItemUI>();
    private List<RuneSO> createdRunes = new List<RuneSO>();
    private Dictionary<RuneSO, int> createdRunesDict = new Dictionary<RuneSO, int>();
    private List<RuneRWItemUI> createdRunesUI = new List<RuneRWItemUI>();



    //[Header("Rune Details")]
    //[SerializeField] private TMP_Text runeName;
    //[SerializeField] private TMP_Text level;
    //[SerializeField] private TMP_Text description;
    //[SerializeField] private List<Image> costIconsList;
    //[SerializeField] private List<TMP_Text> costAmountList;
    //[SerializeField] private TMP_Text destroyPrice;

    //private Color normalColor = Color.white;
    //[SerializeField] private Color warningColor;

    //[Header("Action Elements")]
    //[SerializeField] private GameObject createButton;
    //[SerializeField] private GameObject confirmCreateBlock;
    //[SerializeField] private GameObject createWarning;
    //[SerializeField] private GameObject createBlock;
    //[SerializeField] private GameObject maxAmountBlock;

    private int maxCount = 3;
    private bool isToExpensive = false;
    private RuneSO currentRune;
    private RuneRWItemUI currentRuneUI;

    private RunesType currentRuneType;
    private List<RuneSO> currentRuneFamily;

    public override GameObject Init(CastleBuildings building)
    {
        if(runesManager == null)
        {
            runesManager = GlobalStorage.instance.runesManager;
            allBuildings = GlobalStorage.instance.fortressBuildings;
            poolManager = GlobalStorage.instance.objectsPoolManager;
            resourcesManager = GlobalStorage.instance.resourcesManager;

            resourcesIcons = resourcesManager.GetAllResourcesIcons();
            runesInStore = runesManager.GetRunesForStorage();
        }

        ResetForm();

        detailsBlock.SetActive(false);
        destroyBlock.SetActive(false);

        gameObject.SetActive(true);
        return gameObject;
    }


    #region SETTINGS

    private void ResetForm()
    {
        isToExpensive = false;
        CloseConfirms();

        ResetSelections();
        ResetStore();
        ResetHeroRunes();
    }

    private void ResetStore()
    {
        foreach(var runeUI in runesInStoreUI)
        {
            runeUI.gameObject.SetActive(false);
        }

        for(int i = 0; i < runesInStore.Count; i++)
        {
            runesInStoreUI[i].gameObject.SetActive(true);
            runesInStoreUI[i].Init(this, runesInStore[i], true);
        }
    }

    private void ResetHeroRunes()
    {
        foreach(var runeUI in createdRunesUI)
        {
            runeUI.gameObject.SetActive(false);
        }

        createdRunesDict = runesManager.GetCreatedRunes();

        int index = 0;
        foreach(var itemRune in createdRunesDict)
        {
            if(index >= createdRunesUI.Count)
            {
                GameObject newRuneUI = poolManager.GetObject(ObjectPool.RuneUI);
                RuneRWItemUI ui = newRuneUI.GetComponent<RuneRWItemUI>();
                newRuneUI.transform.SetParent(herosRunesContainer.transform, false);

                createdRunesUI.Add(ui);
            }

            createdRunesUI[index].gameObject.SetActive(true);
            createdRunesUI[index].Init(this, itemRune.Key, false, itemRune.Value + "/" + maxCount);

            index++;
        }
    }

    public void ResetSelections()
    {
        for(int i = 0; i < runesInStore.Count; i++)
        {
            runesInStoreUI[i].ResetSelection();
        }

        for(int i = 0; i < createdRunesUI.Count; i++)
        {
            createdRunesUI[i].ResetSelection();
        }

    }

    #endregion

    public void SetRuneToCreate(RunesType rune, RuneRWItemUI runeUI)
    {
        currentRuneFamily = runesManager.GetRuneFamily(rune);

        destroyBlock.SetActive(false);
        detailsBlock.SetActive(true);

        currentRuneUI = runeUI;
        runeName.text = currentRuneFamily[0].runeName;

        for(int i = 0; i < detailsItems.Count; i++)
        {
            float buildingEffect = allBuildings.GetBonusAmount(CastleBuildingsBonuses.RuneLevel);
            bool isAllowed = buildingEffect >= currentRuneFamily[i].level;
            int runesAmount = runesManager.GetRunesAmount(currentRuneFamily[i]);

            detailsItems[i].Init(currentRuneFamily[i], runesAmount, maxCount, isAllowed);
        }

        //isToExpensive = false;
        //currentRune = rune;
        //currentRuneUI = runeUI;

        //if(isStoreRune == false)
        //{
        //    RuneSO nextRune = runesManager.GetRune(rune.rune, rune.level + 1);

        //    if(nextRune != null)
        //        currentRune = nextRune;
        //}

        //ShowDetails(isStoreRune);


    }

    //private void ShowDetails(bool isStoreRune)
    //{
    //    if(currentRune == null) return;

    //    detailsBlock.SetActive(true);

    //    runeName.text = currentRune.runeName;
    //    level.text = "Level " + currentRune.level;
    //    description.text = currentRune.positiveDescription.Replace("$", currentRune.value.ToString());

    //    destroyPrice.text = (currentRune.level - 1).ToString();
    //    destroyBlock.SetActive(!isStoreRune);


    //    int runesAmount = runesManager.GetRunesAmount(currentRune);

    //    if(runesAmount >= maxCount) 
    //    {
    //        createBlock.SetActive(false);
    //        maxAmountBlock.SetActive(true);
    //    }
    //    else
    //    {
    //        createBlock.SetActive(true);
    //        maxAmountBlock.SetActive(false);

    //        foreach(var itemCost in costIconsList)
    //        {
    //            itemCost.transform.parent.gameObject.SetActive(false);
    //        }

    //        for(int i = 0; i < currentRune.cost.Count; i++)
    //        {
    //            costIconsList[i].transform.parent.gameObject.SetActive(true);
    //            costIconsList[i].sprite = resourcesIcons[currentRune.cost[i].type];
    //            costAmountList[i].text = currentRune.cost[i].amount.ToString();

    //            bool isResourceEnough = resourcesManager.CheckMinResource(currentRune.cost[i].type, currentRune.cost[i].amount);
    //            costAmountList[i].color = (isResourceEnough == true) ? normalColor : warningColor;

    //            if(isResourceEnough == false)
    //                isToExpensive = true;
    //        }

    //        float buildingEffect = allBuildings.GetBonusAmount(CastleBuildingsBonuses.RuneLevel);
    //        bool isAllowed = buildingEffect >= currentRune.level;
    //        createButton.SetActive(isAllowed);
    //        createWarning.SetActive(!isAllowed);
    //    }
    //}

    public void SetRuneToDestroy(RunesType rune, RuneRWItemUI runeUI)
    {
        destroyBlock.SetActive(true);
        detailsBlock.SetActive(false);
    }

    #region Actions

    //Button
    public void TryToDestroy()
    {
        confirmDestroyBlock.SetActive(true);
    }

    //Button
    public void DestroyRune()
    {
        Pay(new List<Cost>()
        {
            new Cost()
            {
                type = ResourceType.Shards, 
                amount = -1 
            } 
        });
        CloseConfirms();
    }


    //Button
    public void TryToCreate()
    {
        if(isToExpensive == true)
        {
            InfotipManager.ShowWarning("You do not have enough Resources for this action.");
            return;
        }

        //confirmCreateBlock.SetActive(true);
    }

    //Button
    public void CreateRune()
    {
        Pay(currentRune.cost);
        runesManager.AddCreatedRune(currentRune);
        ResetForm();
        currentRuneUI.Select();
    }

    //Button
    public void CloseConfirms()
    {
        //confirmCreateBlock.SetActive(false);
        confirmDestroyBlock.SetActive(false);
    }

    private void Pay(List<Cost> costList)
    {
        foreach(var itemCost in costList)
        {
            resourcesManager.ChangeResource(itemCost.type, -itemCost.amount);
        }
    }
    #endregion
}
