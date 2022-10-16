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

    private void Start()
    {
        macroLevelUpManager = GlobalStorage.instance.macroLevelUpManager;
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
        abilityPointsText.text = points.ToString();
    }
}
