using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static NameManager;

public class RunesWindow : MonoBehaviour
{
    private RunesManager runesManager;
    private ObjectsPoolManager poolManager;
    private MacroLevelUpManager levelUpManager;

    [SerializeField] private RuneLevelWrapper levelRow;
    [SerializeField] private RunesRowWrapper firstRuneRow;
    [SerializeField] private RunesRowWrapper negativeRuneRow;
    [SerializeField] private RunesRowWrapper bonusRuneRow;
    [SerializeField] private GameObject runesContainer;
    private GridLayoutGroup grid;

    private List<RuneUIItem> freeRunes = new List<RuneUIItem>();

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
        grid = runesContainer.GetComponent<GridLayoutGroup>();
        levelUpManager = GlobalStorage.instance.macroLevelUpManager;

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

    public void UpdateWindow(float level = 0)
    {
        if(level == 0) level = levelUpManager.GetCurrentLevel();

        levelRow.Init(level);
        firstRuneRow.Init(level, false, false);
        negativeRuneRow.Init(level, true, false);
        bonusRuneRow.Init(level, false, true);
        FillRunesStorages();
    }

    public void FillRunesStorages()
    {
        //foreach(var item in freeRunes)
        //    Destroy(item.gameObject);

        foreach(var item in freeRunes)
            item.gameObject.SetActive(false);

        freeRunes.Clear();
        EnableGrid(true);

        List<RuneSO> availableRunes = runesManager.runesStorage.GetAvailableRunes();
        //we can't take all runes from pool because they change their order in Hierarchy
        //but not the position data
        //GameObject prefab = poolManager.GetObject(ObjectPool.Rune);

        for(int i = 0; i < availableRunes.Count; i++)
        {
            //GameObject runeGO = Instantiate(prefab);
            GameObject runeGO = poolManager.GetObject(ObjectPool.Rune);
            runeGO.transform.SetParent(runesContainer.transform, false);
            runeGO.transform.SetAsLastSibling();
            runeGO.SetActive(true);

            RuneUIItem runeUI = runeGO.GetComponent<RuneUIItem>();
            runeUI.Init(availableRunes[i]);

            freeRunes.Add(runeUI);
        }
    }

    public void CutRuneFromList(RuneUIItem rune)
    {
        freeRunes.Remove(rune);
    }

    public void EnableGrid(bool mode)
    {
        grid.enabled = mode;
    }

    private void UpgradeParameter(PlayersStats stat, float value)
    {
        if(stat == PlayersStats.NegativeCell && value > 0) UpdateWindow();
    }

    private void OnEnable()
    {
        EventManager.SetNewPlayerStat += UpgradeParameter;
        EventManager.UpgradeLevel += UpdateWindow;
    }

    private void OnDisable()
    {
        EventManager.SetNewPlayerStat -= UpgradeParameter;
        EventManager.UpgradeLevel -= UpdateWindow;
    }
}
