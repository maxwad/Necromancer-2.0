using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class BattleUIInfirmaryPart : MonoBehaviour
{
    private BattleUIManager battleUIManager;
    private PlayerStats playerStats;

    [Header("Infirmary")]
    [SerializeField] private Image infirmaryScale;
    [SerializeField] private TMP_Text infirmaryInfo;
    private float currentMaxInfirmaryCount;
    private float currentInfirmaryCount;
    [SerializeField] private Color infirmaryUpColor;
    [SerializeField] private Color infirmaryDownColor;
    [SerializeField] private Color normalInfirmaryColor;

    public void Init(BattleUIManager manager)
    {
        battleUIManager = manager;
        playerStats = GlobalStorage.instance.playerStats;
    }

    public void FillInfirmary(float max = 0, float current = 0)
    {
        Color blinkColor = infirmaryUpColor;
        if(max == 0)
        {
            currentMaxInfirmaryCount = playerStats.GetCurrentParameter(PlayersStats.Infirmary);
            currentInfirmaryCount = GlobalStorage.instance.infirmaryManager.injuredList.Count;
        }
        else
        {
            if(currentInfirmaryCount > current) blinkColor = infirmaryDownColor;
            currentMaxInfirmaryCount = max;
            currentInfirmaryCount = current;
        }

        float widthInfirmary = currentInfirmaryCount / currentMaxInfirmaryCount;

        infirmaryScale.fillAmount = widthInfirmary;
        infirmaryInfo.text = currentInfirmaryCount.ToString() + "/" + currentMaxInfirmaryCount.ToString();
        battleUIManager.Blink(infirmaryScale, blinkColor, normalInfirmaryColor);
    }

    private void UpdateInfirmaryUI(float quantity, float capacity)
    {
        FillInfirmary(capacity, quantity);
    }

    private void OnEnable()
    {
        EventManager.UpdateInfirmaryUI += UpdateInfirmaryUI;
    }

    private void OnDisable()
    {
        EventManager.UpdateInfirmaryUI -= UpdateInfirmaryUI;
    }

}
