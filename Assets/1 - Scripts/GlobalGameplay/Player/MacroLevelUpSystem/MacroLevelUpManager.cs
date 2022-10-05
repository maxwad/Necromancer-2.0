using UnityEngine;
using static NameManager;

public class MacroLevelUpManager : MonoBehaviour
{
    private PlayerStats playerStats;
    private NewMacroLevelUI newLevelUI;

    private float maxLevel;
    public float currentLevel = 1;
    private float currentExp;

    private float standartExpRate = 0.1f;
    private float levelMultiplierRate = 2f;
    private float currentExpGoal;

    [SerializeField] private float newLevelBonusAmount = 5f;
    [HideInInspector] public int abilityPoints = 0;

    private void Start()
    {
        playerStats = GlobalStorage.instance.playerStats;
        maxLevel = playerStats.GetCurrentParameter(PlayersStats.Level);
        newLevelUI = GlobalStorage.instance.gmInterface.gameObject.GetComponent<NewMacroLevelUI>();

        //for(int i = 0; i < 11; i++)
        //{
        //    UpgradeTempExpGoal();
        //}
        UpgradeTempExpGoal(false);
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
        Debug.Log("Now you need " + currentExpGoal + " exp for " + (currentLevel + 1) + " level.");

        if(levelUpMode == true) NewLevel();
    }

    private void NewLevel()
    {
        currentLevel++;
        ChangeAbilityPoints(true);

        PlayersStats stat = (currentLevel % 2 == 0) ? PlayersStats.Health : PlayersStats.Mana;
        playerStats.UpdateMaxStat(stat, newLevelBonusAmount);

        newLevelUI.Init(stat, currentLevel, this);
    }

    public void ChangeAbilityPoints(bool mode)
    {
        abilityPoints = (mode == true) ? abilityPoints++ : ( (abilityPoints <= 0) ? 0 : abilityPoints--);

        Debug.Log("You have " + abilityPoints + " points");
    }

    public void GetNewAbility(MacroAbilitySO newAbility)
    {

    }

    public float GetCurrentLevel()
    {
        return currentLevel;
    }
}
