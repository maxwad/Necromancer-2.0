using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static Enums;
using Zenject;

public class RuneWorkroom : SpecialBuilding
{
    private RunesManager runesManager;
    private FortressBuildings allBuildings;
    private ObjectsPoolManager poolManager;
    private ResourcesManager resourcesManager;

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
    private Dictionary<RuneSO, int> createdRunesDict = new Dictionary<RuneSO, int>();
    private List<RuneRWItemUI> createdRunesUI = new List<RuneRWItemUI>();

    private int maxCount = 3;
    private RuneSO currentRune;
    private RuneRWItemUI currentRuneUI;

    private List<RuneSO> currentRuneFamily;

    [Inject]
    public void Construct(
        RunesManager runesManager,
        FortressBuildings allBuildings,
        ObjectsPoolManager poolManager,
        ResourcesManager resourcesManager
        )
    {
        this.runesManager = runesManager;
        this.allBuildings = allBuildings;
        this.poolManager = poolManager;
        this.resourcesManager = resourcesManager;
    }

    public override GameObject Init(FBuilding building)
    {

        if(runesInStore.Count == 0)
            runesInStore = runesManager.GetRunesForStorage();

        detailsBlock.SetActive(false);
        destroyBlock.SetActive(false);
        gameObject.SetActive(true);


        ResetForm();
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
            runeUI.gameObject.SetActive(false);

        createdRunesUI.Clear();
        createdRunesDict = runesManager.GetCreatedRunes();

        int index = 0;
        foreach(var itemRune in createdRunesDict)
        {
            GameObject newRuneUI = poolManager.GetObject(ObjectPool.RuneUI);
            newRuneUI.SetActive(true);
            RuneRWItemUI ui = newRuneUI.GetComponent<RuneRWItemUI>();
            newRuneUI.transform.SetParent(herosRunesContainer.transform, false);

            createdRunesUI.Add(ui);

            createdRunesUI[index].gameObject.SetActive(true);
            createdRunesUI[index].Init(this, itemRune.Key, false, itemRune.Value + "/" + maxCount);

            index++;
        }
    }

    public void ResetSelections()
    {
        for(int i = 0; i < runesInStore.Count; i++)
            runesInStoreUI[i].ResetSelection();

        for(int i = 0; i < createdRunesUI.Count; i++)
            createdRunesUI[i].ResetSelection();

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

        if(runesManager.GetRunesAmount(currentRune) != 0)
            currentRuneUI.Select();
        else
            destroyBlock.SetActive(false);
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

    public override object Save()
    {
        return null;
    }

    public override void Load(List<object> saveData)
    {
        //we don't need any info for loading
        //all system is saving in SpellManager
    }
    #endregion
}
