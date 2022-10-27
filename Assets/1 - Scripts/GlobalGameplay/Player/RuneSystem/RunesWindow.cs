using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class RunesWindow : MonoBehaviour
{
    private RunesManager runesManager;
    private ObjectsPoolManager poolManager;
    private MacroLevelUpManager levelUpManager;

    [Header("Rows")]
    [SerializeField] private RuneLevelWrapper levelRow;
    [SerializeField] private RunesRowWrapper firstRuneRow;
    [SerializeField] private RunesRowWrapper negativeRuneRow;
    [SerializeField] private RunesRowWrapper bonusRuneRow;
    private RunesRowWrapper[] runeUIRows = new RunesRowWrapper[3];
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

    private Dictionary<RunesType, TMP_Text> boostDict;

    public Color positiveEffect;
    public Color negativeEffect;

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

        runeUIRows[0] = firstRuneRow;
        runeUIRows[1] = negativeRuneRow;
        runeUIRows[2] = bonusRuneRow;

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
            [RunesType.Exp]              = exp
        };

        foreach(var boost in boostDict)
            boost.Value.text = "0";
    }

    public void UpdateWindow(float level = 0)
    {
        if(runesContainer.activeInHierarchy == false) return;

        if(level == 0) level = levelUpManager.GetCurrentLevel();
        levelRow.Init(level);
        firstRuneRow.Init(level, false, false);
        negativeRuneRow.Init(level, true, false);
        bonusRuneRow.Init(level, false, true);
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

            RuneUIItem runeUI = runeGO.GetComponent<RuneUIItem>();
            runeUI.Init(availableRunes[i]);

            freeRunes.Add(runeUI);
        }
    }

    public void UpdateParameters(RunesType type, float value)
    {
        Color currentColor = Color.white;
        string mark = "";
        string end = "";

        if(value > 0) { 
            currentColor = (type == RunesType.CoolDown) ? negativeEffect : positiveEffect;
            mark = "+";
            end = "%";
        }

        if(value < 0)
        {
            currentColor = (type == RunesType.CoolDown) ? positiveEffect : negativeEffect;
            end = "%";
        }

        boostDict[type].color = currentColor;
        boostDict[type].text = mark + value.ToString() + end;
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
        if(row == 1) return;

        RunesRowWrapper runesRow = firstRuneRow;

        if(row == 2) runesRow = bonusRuneRow;

        runesRow.ForceRuneClearing(cell);
    }

    private void UpgradeParameter(PlayersStats stat, float value)
    {
        if(stat == PlayersStats.NegativeCell && value > 0) negativeRuneRow.Init(levelUpManager.GetCurrentLevel(), true, false);
    }

    private void OnEnable()
    {
        EventManager.SetNewPlayerStat += UpgradeParameter;
    }

    private void OnDisable()
    {
        EventManager.SetNewPlayerStat -= UpgradeParameter;
    }
}
