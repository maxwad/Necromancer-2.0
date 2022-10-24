using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] private GridLayoutGroup grid;

    private List<RuneUIItem> freeRunes = new List<RuneUIItem>();
    private List<GameObject> freeRunesGO = new List<GameObject>();

    private bool isNegativeRowUnlocked = false;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            FillRunesStorages();
        }
    }

    private void Start()
    {
        runesManager = GlobalStorage.instance.runesManager;
        poolManager = GlobalStorage.instance.objectsPoolManager;
        grid = runesContainer.GetComponent<GridLayoutGroup>();

        //levelRow.Init();
        //firstRuneRow.Init();
        //negativeRuneRow.Init();
        //bonusRuneRow.Init();
        //FillRunesStorages();
        UpdateWindow();
    }

    //public void Init()
    //{
    //    UpdateWindow();
    //}

    public void UpdateWindow()
    {
        levelRow.Init();
        firstRuneRow.Init(false, false);
        negativeRuneRow.Init(true, false);
        bonusRuneRow.Init(false, true);
        FillRunesStorages();
    }

    public void FillRunesStorages()
    {
        foreach(var item in freeRunes)
            Destroy(item.gameObject);

        freeRunes.Clear();
        EnableGrid(true);

        List<RuneSO> availableRunes = runesManager.runesStorage.GetAvailableRunes();
        //we can't take all runes from pool because they change their order in Hierarchy
        //but not the position data
        GameObject prefab = poolManager.GetObject(ObjectPool.Rune);

        for(int i = 0; i < availableRunes.Count; i++)
        {
            GameObject runeGO = Instantiate(prefab);
            runeGO.transform.SetParent(runesContainer.transform, false);
            runeGO.SetActive(true);

            RuneUIItem runeUI = runeGO.GetComponent<RuneUIItem>();
            runeUI.Init(availableRunes[i]);

            freeRunes.Add(runeUI);
        }
    }

    public void EnableGrid(bool mode)
    {
        grid.enabled = mode;
    }
}
