using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class RunesWindow : MonoBehaviour
{
    private RunesManager runesManager;
    private ObjectsPoolManager poolManager;

    [SerializeField] private RuneLevelWrapper levelRow;
    [SerializeField] private RunesRowWrapper firstRuneRow;
    [SerializeField] private RunesRowWrapper negativeRuneRow;
    [SerializeField] private RunesRowWrapper bonusRuneRow;
    [SerializeField] private GameObject runesContainer;

    private List<RuneUIItem> freeRunes = new List<RuneUIItem>();


    private bool isNegativeRowUnlocked = false;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            UpdateWindow();
        }
    }

    private void Start()
    {
        runesManager = GlobalStorage.instance.runesManager;
        poolManager = GlobalStorage.instance.objectsPoolManager;

        levelRow.Init();
        firstRuneRow.Init();
        negativeRuneRow.Init();
        bonusRuneRow.Init();
        FillRunesStorages();
    }

    //public void Init()
    //{
    //    UpdateWindow();
    //}

    public void UpdateWindow()
    {
        levelRow.Init();
        firstRuneRow.Init();
        negativeRuneRow.Init();
        bonusRuneRow.Init();
        FillRunesStorages();
    }

    private void FillRunesStorages()
    {
        foreach(var item in freeRunes)
            item.gameObject.SetActive(false);

        freeRunes.Clear();

        List<RuneSO> availableRunes = runesManager.runesStorage.GetAvailableRunes();

        for(int i = 0; i < availableRunes.Count; i++)
        {
            GameObject runeGO = poolManager.GetObject(ObjectPool.Rune);
            runeGO.transform.SetParent(runesContainer.transform, false);
            runeGO.SetActive(true);

            RuneUIItem runeUI = runeGO.GetComponent<RuneUIItem>();
            runeUI.Init(availableRunes[i]);
            freeRunes.Add(runeUI);
        }
    }
}
