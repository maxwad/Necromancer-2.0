using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static NameManager;
using System;

public class PlayerGMInterface : MonoBehaviour
{
    private PlayerStats playerStats;
    [SerializeField] private GameObject uiPanel;

    [Header("Mana")]
    [SerializeField] private TMP_Text manaInfo;
    private float currentMaxManaCount;
    private float currentManaCount;


    private void Start()
    {
        playerStats = GlobalStorage.instance.player.GetComponent<PlayerStats>();
        FillMana();
    }

    private void EnableUI(bool mode)
    {
        uiPanel.SetActive(mode);
    }

    private void FillMana(float max = 0, float current = 0)
    {
        if(max == 0)
        {
            currentMaxManaCount = playerStats.GetStartParameter(PlayersStats.Mana);
            currentManaCount = playerStats.GetCurrentParameter(PlayersStats.Mana);
        }
        else
        {
            currentMaxManaCount = max;
            currentManaCount = current;
        }

        manaInfo.text = currentManaCount.ToString();
    }

    private void UpdateManaUI(PlayersStats stat, float maxValue, float currentValue)
    {
        if(stat == PlayersStats.Mana) FillMana(maxValue, currentValue);        
    }

    private void OnEnable()
    {
        EventManager.UpgradeStatCurrentValue += UpdateManaUI;
        EventManager.ChangePlayer += EnableUI;
    }

    private void OnDisable()
    {
        EventManager.ChangePlayer -= EnableUI;
        EventManager.UpgradeStatCurrentValue -= UpdateManaUI;
    }
}
