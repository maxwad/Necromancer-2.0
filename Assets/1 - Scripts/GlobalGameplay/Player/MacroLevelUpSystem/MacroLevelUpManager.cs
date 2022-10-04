using UnityEngine;
using static NameManager;

public class MacroLevelUpManager : MonoBehaviour
{
    private float maxLevel;
    private float currentLevel = 0;
    private float currentExp;

    private float standartExpRate = 0.1f;
    private float levelMultiplierRate = 2f;
    private float currentExpGoal;

    [HideInInspector] public int abilityPoints = 0;
    private NewMacroLevelUI newLevelUI;

    private void Start()
    {
        maxLevel = GlobalStorage.instance.playerStats.GetCurrentParameter(PlayersStats.Level);
        newLevelUI = GlobalStorage.instance.gmInterface.gameObject.GetComponent<NewMacroLevelUI>();

        //for(int i = 0; i < 11; i++)
        //{
        //    UpgradeTempExpGoal();
        //}
        UpgradeTempExpGoal();
    }

    public void AddExp(float value)
    {
        float restExp = currentExpGoal - currentExp;
        if(value >= restExp)
        {
            restExp = Mathf.Abs(currentExpGoal - (currentExp + value));
            UpgradeTempExpGoal();
            AddExp(restExp);
        }
        else
        {
            currentExp += value;
        }
    }

    private void UpgradeTempExpGoal()
    {
        currentLevel++;
        currentExp = 0;
        currentExpGoal = Mathf.Pow(((currentLevel) / standartExpRate), levelMultiplierRate);

        if(currentLevel != 1) newLevelUI.Init(true, currentLevel, this);

        Debug.Log("Now you need " + currentExpGoal + " exp for " + (currentLevel + 1) + " level.");
    }

    public void ChangeAbilityPoints(bool mode)
    {
        abilityPoints = (mode == true) ? abilityPoints++ : ( (abilityPoints <= 0) ? 0 : abilityPoints--);

        Debug.Log("You have " + abilityPoints + " points");
    }

    public void GetNewAbility(MacroAbilitySO newAbility)
    {

    }
}
