using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

public class BattleUIExpPart : MonoBehaviour
{
    private BattleUIManager battleUIManager;
    private MacroLevelUpManager levelManager;

    [Header("Left Column Exp")]
    [SerializeField] private Image leftExpScale;
    [SerializeField] private Color levelUpColor;
    [SerializeField] private Color normalLevelColor;

    [Header("Rigth Column Exp")]
    [SerializeField] private GameObject tempLevelGO;
    [SerializeField] private RectTransform currentTempLevelWrapper;
    [SerializeField] private Image rightExpScale;
    private float heigthOneLevel;
    private float currentMaxLevel;
    [SerializeField] private Color activeTempLevelColor;
    [SerializeField] private Color inactiveTempLevelColor;
    private List<Image> levelList = new List<Image>();

    [Inject]
    public void Construct(MacroLevelUpManager levelManager)
    {
        this.levelManager = levelManager;
    }

    public void Init(BattleUIManager manager)
    {
        battleUIManager = manager;
    }

    public void FillRigthTempLevelScale()
    {
        levelList.Clear();

        foreach(Transform child in currentTempLevelWrapper.transform)
        {
            if(child.transform.localScale.z == -1) Destroy(child.gameObject);
        }

        leftExpScale.fillAmount = 0;
        rightExpScale.fillAmount = 0;
        currentMaxLevel = levelManager.GetCurrentLevel();

        heigthOneLevel = currentTempLevelWrapper.rect.height / currentMaxLevel;

        for(int i = 0; i < currentMaxLevel; i++)
        {
            GameObject levelPart = Instantiate(tempLevelGO);
            RectTransform rectLevel = levelPart.GetComponent<RectTransform>();
            levelPart.transform.SetParent(currentTempLevelWrapper.transform, false);

            rectLevel.anchoredPosition = new Vector2(0, heigthOneLevel * (i + 1));

            levelPart.GetComponent<Image>().color = inactiveTempLevelColor;
            levelPart.GetComponentInChildren<TMP_Text>().text = (i + 1).ToString();

            levelList.Add(levelPart.GetComponent<Image>());
        }
    }

    public void TempLevelUp(float oldLevel)
    {
        levelList[(int)oldLevel].color = activeTempLevelColor;

        if(oldLevel + 1 < levelList.Count)
            leftExpScale.fillAmount = 0;

        rightExpScale.fillAmount = heigthOneLevel * (oldLevel + 1) / currentTempLevelWrapper.rect.height;

        battleUIManager.Blink(rightExpScale, levelUpColor, normalLevelColor, 100);
    }

    public void UpgradeScale(float scale, float value)
    {
        leftExpScale.fillAmount = value / scale;
        battleUIManager.Blink(leftExpScale, levelUpColor, normalLevelColor);
    }
}
