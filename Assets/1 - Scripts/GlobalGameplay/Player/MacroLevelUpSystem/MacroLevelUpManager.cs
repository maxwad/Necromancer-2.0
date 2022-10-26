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
        playerStats = GlobalStorage.instance.playerStats;
        maxLevel = playerStats.GetCurrentParameter(PlayersStats.Level);
        newLevelUI = GlobalStorage.instance.gmInterface.gameObject.GetComponent<NewLevelUI>();
        managerUI = GlobalStorage.instance.playerMilitaryWindow.GetComponentInChildren<MacroLevelWindow>();
        gmInterface = GlobalStorage.instance.gmInterface;
        bonusSkillLevel = playerStats.GetCurrentParameter(PlayersStats.Learning);
        abilitiesStorage = GetComponentInChildren<AbilitiesStorage>();
        abilitiesStorage.Init();
        managerUI.UpdateAbilityBlock(abilityPoints);

        //TODO: delete on release
        //abilityPoints = abilitiesStorage.abilitiesCount;
        abilityPoints = 3;

        //for(int i = 0; i < 11; i++)
        //{
        //    UpgradeTempExpGoal();
        //}
        UpgradeTempExpGoal(false);

        gmInterface.UpgradeLevel(GetLevelData());
        gmInterface.UpgradeAbilityPoints(abilityPoints);

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

        gmInterface.UpgradeLevel(GetLevelData());
    }

    private void UpgradeTempExpGoal(bool levelUpMode)
    {
        currentExp = 0;
        currentExpGoal = Mathf.Pow(((currentLevel + 1) / standartExpRate), levelMultiplierRate);

        if(levelUpMode == true) NewLevel();
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

    public void ChangeAbilityPoints(int delta)
    {
        abilityPoints += delta;
        gmInterface.UpgradeAbilityPoints(abilityPoints);
    }

    public void OpenAbility(MacroAbilitySO newAbility)
    {
        ChangeAbilityPoints(-newAbility.cost);
        abilitiesStorage.ApplyAbility(newAbility);
        playerStats.UpdateMaxStat(newAbility.ability, newAbility.valueType, newAbility.value);
        managerUI.UpdateAbilityBlock(abilityPoints);
    }

    public float GetCurrentLevel()
    {
        return currentLevel;
    }

    public float GetMaxLevel()
    {
        return maxLevel;
    }

    public LevelData GetLevelData()
    {
        return new LevelData(currentLevel, currentExp, currentExpGoal, abilityPoints);
    }
    public LevelData GetFutureDataForBattleResult()
    {
        float futureExpGoal = Mathf.Pow(((currentLevel + 1) / standartExpRate), levelMultiplierRate);
        return new LevelData(currentLevel + 1, 0, futureExpGoal, abilityPoints);
    }

    public int GetAbilityPoints()
    {
        return abilityPoints;
    }

    private void UpgradeParameter(PlayersStats stat, float value)
    {
        if(stat == PlayersStats.Learning) bonusSkillLevel = value;
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
