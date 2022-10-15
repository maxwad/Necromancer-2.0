using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MacroLevelWindow : MonoBehaviour
{
    private MacroLevelUpManager macroLevelUpManager;

    [Header("Level")]
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text currentExpText;
    [SerializeField] private TMP_Text boundExpText;
    [SerializeField] private Image levelScale;

    [Header("Ability")]
    [SerializeField] private TMP_Text abilityPointsText;
    //[SerializeField] private Button newAbilityBtn;
    //private NewSkillUI newSkillUI;

    private void Start()
    {
        macroLevelUpManager = GlobalStorage.instance.macroLevelUpManager;
        //newSkillUI = GetComponent<NewSkillUI>();
    }

    public void Init()
    {
        UpdateLevel();
    }

    public void UpdateLevel()
    {
        LevelData levelData = macroLevelUpManager.GetLevelData();
        levelText.text = levelData.level.ToString();

        currentExpText.text = levelData.currentExp.ToString();
        boundExpText.text = levelData.boundExp.ToString();
        levelScale.fillAmount = levelData.currentExp / levelData.boundExp;

        abilityPointsText.text = levelData.abilitiesPoints.ToString();
    }

    public void UpdateAbilityBlock(int points)
    {
        //int abilityPoints = macroLevelUpManager.GetAbilityPoints();
        abilityPointsText.text = points.ToString();

        //bool mode = (abilityPoints > 0) ? true : false;
        //ActivationButton(mode);
    }

    //button
    //public void GetDelayedAbility()
    //{
    //    ActivationButton(false);
    //    newSkillUI.OpenWindow();
    //}

    //public void ActivationButton(bool mode)
    //{
    //    if(mode == true) mode = macroLevelUpManager.abilitiesStorage.CanIGetNewAbility();

    //    newAbilityBtn.interactable = mode;
    //}

    //public bool CheckOpenedMiniWindow()
    //{
    //    return newSkillUI.TryToHideWindow();
    //}
}
