using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public struct LevelData
{
    public float level;
    public float currentExp;
    public float boundExp;
    public int abilitiesPoints;

    public LevelData(float lvl, float cExp, float bExp, int aPoints)
    {
        level = lvl;
        currentExp = cExp;
        boundExp = bExp;
        abilitiesPoints = aPoints;
    }
}

public class MacroLevelUpManager : MonoBehaviour
{
    private PlayerStats playerStats;
    private NewLevelUI newLevelUI;
    [HideInInspector] public AbilitiesStorage abilitiesStorage;
    [HideInInspector] public MacroLevelWindow managerUI;
    private GMInterface gmInterface;

    private float maxLevel;
    public float currentLevel = 7;
    private float currentExp;

    private float standartExpRate = 0.1f;
    private float levelMultiplierRate = 2f;
    private float currentExpGoal;

    [SerializeField] private float newLevelBonusAmount = 5;
    [SerializeField] private float bonusSkillLevel;

    private int abilityPoints = 50;

    public void Init()
    {
        playerStats      = GlobalStorage.instance.playerStats;
        maxLevel         = playerStats.GetCurrentParameter(PlayersStats.Level);
        newLevelUI       = GlobalStorage.instance.gmInterface.gameObject.GetComponent<NewLevelUI>();
        managerUI        = GlobalStorage.instance.playerMilitaryWindow.GetComponentInChildren<MacroLevelWindow>();
        gmInterface      = GlobalStorage.instance.gmInterface;
        bonusSkillLevel  = playerStats.GetCurrentParameter(PlayersStats.Learning);
        abilitiesStorage = GetComponentInChildren<AbilitiesStorage>();
        abilitiesStorage.Init();
        ChangeAbilityPoints(3, true);

        //TODO: delete on release
        //abilityPoints = abilitiesStorage.abilitiesCount;

        //for(int i = 0; i < 11; i++)
        //{
        //    UpgradeTempExpGoal();
        //}
        UpgradeTempExpGoal(false);

        gmInterface.heroPart.UpgradeLevel(GetLevelData());
        gmInterface.heroPart.UpgradeAbilityPoints(abilityPoints);

        GlobalStorage.instance.LoadNextPart();
    }

    public void AddExp(float value)
    {
        float restExp = currentExpGoal - currentExp;
        if(value >= restExp)
        {
            restExp = Mathf.Abs(currentExpGoal - (currentExp + value));
            UpgradeTempExpGoal(true);
            AddExp(restExp);
        }
        else
        {
            currentExp += value;
        }

        gmInterface.heroPart.UpgradeLevel(GetLevelData());
    }

    private void UpgradeTempExpGoal(bool levelUpMode)
    {
        if(levelUpMode == true) NewLevel();

        currentExp = 0;
        currentExpGoal = Mathf.Pow(((currentLevel + 1) / standartExpRate), levelMultiplierRate);
    }

    private void NewLevel()
    {
        currentLevel++;
        //Debug.Log("Now you need " + currentExpGoal + " exp for " + (currentLevel + 1) + " level.");

        float points = 1f;

        ChangeAbilityPoints(1);
        if(currentLevel % bonusSkillLevel == 0) 
        {
            ChangeAbilityPoints(1);
            points++;
        }

        PlayersStats stat = (currentLevel % 2 == 0) ? PlayersStats.Health : PlayersStats.Mana;
        playerStats.UpdateMaxStat(stat, StatBoostType.Value, newLevelBonusAmount);

        newLevelUI.Init(stat, newLevelBonusAmount, currentLevel, points);
        EventManager.OnUpgradeLevelEvent(currentLevel);
    }

    public void ChangeAbilityPoints(int delta, bool loadMode = false)
    {
        if(loadMode == true)
            abilityPoints = delta;
        else
            abilityPoints += delta;

        managerUI.UpdateAbilityBlock(abilityPoints);
        gmInterface.heroPart.UpgradeAbilityPoints(abilityPoints);
    }

    public void OpenAbility(MacroAbilitySO newAbility, bool loadMode = false)
    {
        if(loadMode == false)
            ChangeAbilityPoints(-newAbility.cost);

        abilitiesStorage.ApplyAbility(newAbility);
        playerStats.UpdateMaxStat(newAbility.ability, newAbility.valueType, newAbility.value);
        managerUI.UpdateAbilityBlock(abilityPoints);
    }

    public float GetCurrentLevel() => currentLevel;

    public float GetMaxLevel() => maxLevel;

    public LevelData GetLevelData()
    {
        return new LevelData(currentLevel, currentExp, currentExpGoal, abilityPoints);
    }
    public LevelData GetFutureDataForBattleResult()
    {
        float futureExpGoal = Mathf.Pow(((currentLevel + 1) / standartExpRate), levelMultiplierRate);
        return new LevelData(currentLevel + 1, 0, futureExpGoal, abilityPoints);
    }

    public int GetAbilityPoints() => abilityPoints;

    private void UpgradeParameter(PlayersStats stat, float value)
    {
        if(stat == PlayersStats.Learning) bonusSkillLevel = value;
    }


    #region SAVE/LOAD

    public PlayersLevelUpSD Save()
    {
        PlayersLevelUpSD saveData = new PlayersLevelUpSD();

        saveData.currentLevel = currentLevel;
        saveData.currentExp = currentExp;
        saveData.abilityPoints = abilityPoints;
        saveData.openedAbilities = abilitiesStorage.GetOpenedAbilities();

        return saveData;
    }

    public void Load(PlayersLevelUpSD saveData)
    {
        currentLevel = saveData.currentLevel;
        UpgradeTempExpGoal(false);
        AddExp(saveData.currentExp);

        ChangeAbilityPoints(saveData.abilityPoints, true);
        abilitiesStorage.LoadOpenedAbilities(saveData.openedAbilities);
    }

    #endregion


    private void OnEnable()
    {
        EventManager.SetNewPlayerStat += UpgradeParameter;
    }

    private void OnDisable()
    {
        EventManager.SetNewPlayerStat -= UpgradeParameter;
    }
}
