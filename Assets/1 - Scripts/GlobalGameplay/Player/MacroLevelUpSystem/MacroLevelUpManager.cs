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

    private float maxLevel;
    public float currentLevel = 1;
    private float currentExp;

    private float standartExpRate = 0.1f;
    private float levelMultiplierRate = 2f;
    private float currentExpGoal;

    private bool haveIReplaceCardsSkill = true;
    private bool canIReplaceCurrentCard = true;

    [SerializeField] private float newLevelBonusAmount = 5;
    [SerializeField] private float bonusSkillLevel;

    private int abilityPoints = 50;
    private int countOfAbilitiesVariants = 3;

    [SerializeField] private List<MacroAbilitySO> abilitiesList = new List<MacroAbilitySO>();

    public void Init()
    {
        playerStats = GlobalStorage.instance.playerStats;
        maxLevel = playerStats.GetCurrentParameter(PlayersStats.Level);
        newLevelUI = GlobalStorage.instance.gmInterface.gameObject.GetComponent<NewLevelUI>();
        managerUI = GlobalStorage.instance.playerMilitaryWindow.GetComponentInChildren<MacroLevelWindow>();
        bonusSkillLevel = playerStats.GetCurrentParameter(PlayersStats.Learning);
        abilitiesStorage = GetComponentInChildren<AbilitiesStorage>();
        abilitiesStorage.Init();
        managerUI.UpdateAbilityBlock(abilityPoints);

        //TODO: delete on release
        //abilityPoints = abilitiesStorage.abilitiesCount;
        abilityPoints = 1;

        //for(int i = 0; i < 11; i++)
        //{
        //    UpgradeTempExpGoal();
        //}
        UpgradeTempExpGoal(false);

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

        ChangeAbilityPoints(true);
        if(currentLevel % bonusSkillLevel == 0) 
        {
            ChangeAbilityPoints(true);
            points++;
        }

        if(abilitiesList.Count == 0) 
            abilitiesList = abilitiesStorage.GetAbilitiesForNewLevel(countOfAbilitiesVariants);

        PlayersStats stat = (currentLevel % 2 == 0) ? PlayersStats.Health : PlayersStats.Mana;
        playerStats.UpdateMaxStat(stat, StatBoostType.Value, newLevelBonusAmount);

        newLevelUI.Init(stat, newLevelBonusAmount, currentLevel, points);
    }

    public void ChangeAbilityPoints(bool mode)
    {
        if(mode == true)
            abilityPoints++;
        else
            abilityPoints--;
    }

    public List<MacroAbilitySO> GetCurrentAbilities()
    {
        if(abilitiesList.Count == 0)
            abilitiesList = abilitiesStorage.GetAbilitiesForNewLevel(countOfAbilitiesVariants);

        return abilitiesList;
    }

    public void OpenAbility(MacroAbilitySO newAbility)
    {
        if(haveIReplaceCardsSkill == true) canIReplaceCurrentCard = true;
        abilitiesList.Clear();

        ChangeAbilityPoints(false);
        abilitiesStorage.ApplyAbility(newAbility);
        playerStats.UpdateMaxStat(newAbility.ability, newAbility.valueType, newAbility.value);
        managerUI.UpdateAbilityBlock(abilityPoints);
    }

    public float GetCurrentLevel()
    {
        return currentLevel;
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

    public void CardsReplaced()
    {
        canIReplaceCurrentCard = false;
    }

    public bool CanIReplaceCards()
    {
        return canIReplaceCurrentCard;
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
