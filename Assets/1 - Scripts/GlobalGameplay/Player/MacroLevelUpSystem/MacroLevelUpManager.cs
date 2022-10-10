using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public struct LevelData
{
    public float level;
    public float currentExp;
    public float boundExp;

    public LevelData(float lvl, float cExp, float bExp)
    {
        level = lvl;
        currentExp = cExp;
        boundExp = bExp;
    }
}

public class MacroLevelUpManager : MonoBehaviour
{
    private PlayerStats playerStats;
    private NewLevelUI newLevelUI;
    private MacroLevelWindow macroLevelUI;
    private AbilitiesStorage abilitiesStorage;

    private float maxLevel;
    public float currentLevel = 1;
    private float currentExp;

    private float standartExpRate = 0.1f;
    private float levelMultiplierRate = 2f;
    private float currentExpGoal;

    private bool haveIReplaceCardsSkill = true;
    private bool canIReplaceCurrentCard = true;

    [SerializeField] private float newLevelBonusAmount = 5f;
    private int abilityPoints = 50;
    private int countOfAbilitiesVariants = 3;

    [SerializeField] private List<MacroAbilitySO> abilitiesList = new List<MacroAbilitySO>();

    public void Init()
    {
        playerStats = GlobalStorage.instance.playerStats;
        maxLevel = playerStats.GetCurrentParameter(PlayersStats.Level);
        newLevelUI = GlobalStorage.instance.gmInterface.gameObject.GetComponent<NewLevelUI>();
        macroLevelUI = GlobalStorage.instance.playerMilitaryWindow.GetComponent<MacroLevelWindow>();
        abilitiesStorage = GetComponentInChildren<AbilitiesStorage>();
        abilitiesStorage.Init();

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
        //Debug.Log("Now you need " + currentExpGoal + " exp for " + (currentLevel + 1) + " level.");

        if(levelUpMode == true) NewLevel();
    }

    private void NewLevel()
    {
        currentLevel++;
        float points = 1f;

        ChangeAbilityPoints(true);
        if(currentLevel % 5 == 0) 
        {
            ChangeAbilityPoints(true);
            points++;
        }

        if(abilitiesList.Count == 0) 
            abilitiesList = abilitiesStorage.GetAbilitiesForNewLevel(countOfAbilitiesVariants);

        PlayersStats stat = (currentLevel % 2 == 0) ? PlayersStats.Health : PlayersStats.Mana;
        playerStats.UpdateMaxStat(stat, newLevelBonusAmount);

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

        abilitiesStorage.ApplyAbility(newAbility);
    }

    public float GetCurrentLevel()
    {
        return currentLevel;
    }

    public LevelData GetLevelData()
    {
        return new LevelData(currentLevel, currentExp, currentExpGoal);
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
}
