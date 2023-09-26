using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;
using System;
using Zenject;

public class RunesWindow : MonoBehaviour
{
    private RunesSystem runesManager;
    private ObjectsPoolManager poolManager;
    private MacroLevelUpManager levelUpManager;

    [Header("Rows")]
    [SerializeField] private RuneLevelWrapper levelRow;

    [SerializeField] private RunesRowWrapper firstRuneRow;
    [SerializeField] private RunesRowWrapper negativeRuneRow;
    [SerializeField] private RunesRowWrapper bonusRuneRow;
    [SerializeField] private GameObject runesContainer;
    private GridLayoutGroup grid;

    private List<RuneUIItem> freeRunes = new List<RuneUIItem>();

    [Header("Parameters")]
    [SerializeField] private TMP_Text mAttack;
    [SerializeField] private TMP_Text pAttack;
    [SerializeField] private TMP_Text bossDamage;
    [SerializeField] private TMP_Text critDamage;
    [SerializeField] private TMP_Text mDefence;
    [SerializeField] private TMP_Text pDefence;
    [SerializeField] private TMP_Text coolDown;
    [SerializeField] private TMP_Text wSize;
    [SerializeField] private TMP_Text squadSpeed;
    [SerializeField] private TMP_Text bonusAmount;
    [SerializeField] private TMP_Text bonusOpp;
    [SerializeField] private TMP_Text bonusRadius;
    [SerializeField] private TMP_Text exp;    
    [SerializeField] private TMP_Text spellActionTime;   
    [SerializeField] private TMP_Text spellReloading;

    private Dictionary<RunesType, TMP_Text> boostDict;

    public Color positiveEffect;
    public Color negativeEffect;

    //private void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.Space))
    //    {
    //        UpdateWindow();
    //    }
    //}

    [Inject]
    public void Construct(
        RunesSystem runesManager,
        ObjectsPoolManager poolManager,
        MacroLevelUpManager levelUpManager
        )
    {
        this.runesManager = runesManager;
        this.poolManager = poolManager;
        this.levelUpManager = levelUpManager;

        grid = runesContainer.GetComponent<GridLayoutGroup>();
    }

    public void Init()
    {
        firstRuneRow.Init(this);
        negativeRuneRow.Init(this);
        bonusRuneRow.Init(this);

        boostDict = new Dictionary<RunesType, TMP_Text>()
        {
            [RunesType.MagicAttack]      = mAttack,
            [RunesType.PhysicAttack]     = pAttack,
            [RunesType.BossDamade]       = bossDamage,
            [RunesType.CriticalDamage]   = critDamage,
            [RunesType.MagicDefence]     = mDefence,
            [RunesType.PhysicDefence]    = pDefence,
            [RunesType.CoolDown]         = coolDown,
            [RunesType.WeaponSize]       = wSize,
            [RunesType.MovementSpeed]    = squadSpeed,
            [RunesType.BonusAmount]      = bonusAmount,
            [RunesType.BonusOpportunity] = bonusOpp,
            [RunesType.BonusRadius]      = bonusRadius,
            [RunesType.Exp]              = exp,
            [RunesType.SpellActionTime]  = spellActionTime,
            [RunesType.SpellReloading]   = spellReloading
        };

        foreach(var boost in boostDict)
            boost.Value.text = "0";
    }

    public void UpdateWindow(float level = 0)
    {
        if(runesContainer.activeInHierarchy == false) return;

        if(level == 0) level = levelUpManager.GetCurrentLevel();
        levelRow.Init(level);
        firstRuneRow.Init(this, level, false, false);
        negativeRuneRow.Init(this, level, true, false);
        bonusRuneRow.Init(this, level, false, true);
        FillRunesStorages();
    }

    public void FillRunesStorages()
    {
        foreach(var item in freeRunes)
            item.gameObject.SetActive(false);

        freeRunes.Clear();
        EnableGrid(true);

        List<RuneSO> availableRunes = runesManager.runesStorage.GetAvailableRunes();

        for(int i = 0; i < availableRunes.Count; i++)
        {
            GameObject runeGO = poolManager.GetObject(ObjectPool.Rune);
            runeGO.transform.SetParent(runesContainer.transform, false);
            runeGO.transform.SetAsLastSibling();
            runeGO.SetActive(true);

            CanvasGroup runeCanvas = runeGO.GetComponent<CanvasGroup>();
            runeCanvas.alpha = 1f;
            runeCanvas.blocksRaycasts = true;

            RuneUIItem runeUI = runeGO.GetComponent<RuneUIItem>();
            runeUI.Init(availableRunes[i]);

            freeRunes.Add(runeUI);
        }
    }

    public void UpdateParameters(RunesType currentType, float value)
    {
        bool isInverted = runesManager.runesStorage.GetRuneInvertion(currentType);
        Color currentColor = Color.white;
        string mark = "";
        string end = "";

        if(value > 0) { 
            currentColor = (isInverted == true) ? negativeEffect : positiveEffect;
            mark = "+";
            end = "%";
        }

        if(value < 0)
        {
            currentColor = (isInverted == true) ? positiveEffect : negativeEffect;
            end = "%";
        }

        boostDict[currentType].color = currentColor;
        boostDict[currentType].text = mark + value.ToString() + end;
    }

    public void CutRuneFromList(RuneUIItem rune)
    {
        freeRunes.Remove(rune);
    }

    public void PasteRuneToList(RuneUIItem rune)
    {
        freeRunes.Add(rune);
    }

    public void EnableGrid(bool mode)
    {
        grid.enabled = mode;
    }

    public int CheckNegativeCell(int index)
    {        
        return negativeRuneRow.CheckCell(index);
    }

    public void FindAndClearRune(int row, int cell)
    {
        RunesRowWrapper runesRow = firstRuneRow;

        if(row == 1) runesRow = negativeRuneRow;

        if(row == 2) runesRow = bonusRuneRow;

        runesRow.ForceRuneClearing(cell);
    }

    private void UpgradeParameter(PlayersStats stat, float value)
    {
        if(stat == PlayersStats.NegativeCell && value > 0) negativeRuneRow.Init(this, levelUpManager.GetCurrentLevel(), true, false);
    }

    private void OnEnable()
    {
        EventManager.SetNewPlayerStat += UpgradeParameter;
    }

    private void OnDisable()
    {
        EventManager.SetNewPlayerStat -= UpgradeParameter;
    }

    public RunesLists Save()
    {
        RunesLists runesLists = new RunesLists();

        runesLists.firstRowRunes    = firstRuneRow.GetRunes();
        runesLists.negativeRowRunes = negativeRuneRow.GetRunes();
        runesLists.bonusRowRunes    = bonusRuneRow.GetRunes();

        return runesLists;
    }

    public GameObject CreateRuneForLoading(RunesType type, int level)
    {
        GameObject runeGO = poolManager.GetObject(ObjectPool.Rune);
        runeGO.transform.SetParent(runesContainer.transform, false);
        runeGO.transform.SetAsLastSibling();
        runeGO.SetActive(true);

        CanvasGroup runeCanvas = runeGO.GetComponent<CanvasGroup>();
        runeCanvas.alpha = 1f;
        runeCanvas.blocksRaycasts = true;

        RuneUIItem runeUI = runeGO.GetComponent<RuneUIItem>();
        RuneSO rune = runesManager.runesStorage.GetRune(type, level);
        runeUI.Init(rune);

        return runeGO;
    }

    public void Load(RunesLists runesLists)
    {
        firstRuneRow.LoadRunes(runesLists.firstRowRunes);
        negativeRuneRow.LoadRunes(runesLists.negativeRowRunes);
        bonusRuneRow.LoadRunes(runesLists.bonusRowRunes);
    }
}
