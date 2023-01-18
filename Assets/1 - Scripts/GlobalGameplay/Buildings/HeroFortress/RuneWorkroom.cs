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

    [SerializeField] private TMP_Text destroyCost;
    [SerializeField] private GameObject destroyBlock;
    [SerializeField] private GameObject destroyButton;
    [SerializeField] private GameObject confirmDestroyBlock;

    private List<RuneSO> runesInStore = new List<RuneSO>();
    [SerializeField] private List<RuneRWItemUI> runesInStoreUI = new List<RuneRWItemUI>();
    private List<RuneSO> createdRunes = new List<RuneSO>();
    private Dictionary<RuneSO, int> createdRunesDict = new Dictionary<RuneSO, int>();
    private List<RuneRWItemUI> createdRunesUI = new List<RuneRWItemUI>();

    private int maxCount = 3;
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

            detailsItems[i].Init(this, currentRuneFamily[i], runesAmount, maxCount, isAllowed);
        }        
    }

    public void SetRuneToDestroy(RuneSO rune, RuneRWItemUI runeUI)
    {
        destroyBlock.SetActive(true);
        detailsBlock.SetActive(false);

        currentRuneUI = runeUI;
        currentRune = rune;
        destroyCost.text = rune.level.ToString();
    }

    #region Actions
    //Button

    public void CreateRune(RuneSO rune)
    {
        Pay(rune.cost);
        runesManager.AddCreatedRune(rune);
        ResetForm();
        currentRuneUI.Select();
    }

    //Button
    public void TryToDestroy()
    {
        bool canIDestroy = runesManager.CanIDestroyRune(currentRune);

        if(canIDestroy == false)
        {
            InfotipManager.ShowWarning("You cannot destroy this Rune as it is in use.");
            return;
        }

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
                amount = -currentRune.level
            }
        });

        runesManager.DestroyRune(currentRune);

        ResetForm();
        currentRuneUI.Select();
    }

    //Button
    public void CloseConfirms()
    {
        foreach(var detailItem in detailsItems)
            detailItem.CloseConfirm();

        confirmDestroyBlock.SetActive(false);
    }

    private void Pay(List<Cost> costList)
    {
        foreach(var itemCost in costList)
            resourcesManager.ChangeResource(itemCost.type, -itemCost.amount);
    }
    #endregion
}
