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

    [SerializeField] private GameObject storeContainer;
    [SerializeField] private GameObject herosRunesContainer;
    private List<RuneSO> runesInStore = new List<RuneSO>();
    [SerializeField] private List<RuneRWItemUI> runesInStoreUI = new List<RuneRWItemUI>();
    private List<RuneSO> createdRunes = new List<RuneSO>();
    private Dictionary<RuneSO, int> createdRunesDict = new Dictionary<RuneSO, int>();
    private List<RuneRWItemUI> createdRunesUI = new List<RuneRWItemUI>();

    [Header("Rune Details")]
    [SerializeField] private TMP_Text runeName;
    [SerializeField] private TMP_Text level;
    [SerializeField] private TMP_Text description;
    [SerializeField] private List<Image> costIconsList;
    [SerializeField] private List<TMP_Text> costAmountList;

    private Color normalColor = Color.white;
    [SerializeField] private Color warningColor;

    [Header("Action Elements")]
    [SerializeField] private GameObject createButton;
    [SerializeField] private GameObject confirmCreateBlock;
    [SerializeField] private GameObject createWarning;

    [SerializeField] private GameObject destroyButton;
    [SerializeField] private GameObject confirmDestroyBlock;
    [SerializeField] private GameObject destroyWarning;


    private int maxCount = 3;

    public override GameObject Init(CastleBuildings building)
    {
        if(runesManager == null)
        {
            runesManager = GlobalStorage.instance.runesManager;
            allBuildings = GlobalStorage.instance.fortressBuildings;
            poolManager = GlobalStorage.instance.objectsPoolManager;

            runesInStore = runesManager.GetRunesForStorage();
        }

        ResetForm();

        gameObject.SetActive(true);
        return gameObject;
    }


    #region SETTINGS

    private void ResetForm()
    {
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

        for(int i = 0; i < createdRunes.Count; i++)
        {
            
        }
    }

    #endregion
}
