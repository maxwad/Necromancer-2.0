using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static NameManager;
using System;

public class GMInterface : MonoBehaviour
{
    private PlayerStats playerStats;
    [SerializeField] private GameObject uiPanel;
    private ResourcesManager resourcesManager;
    public Dictionary<ResourceType, float> resourcesDict = new Dictionary<ResourceType, float>();

    [Header("Mana")]
    [SerializeField] private TMP_Text manaCount;
    private float currentMaxManaCount;
    private float currentManaCount;

    [Header("Resources")]
    [SerializeField] private TMP_Text goldCount;
    [SerializeField] private TMP_Text foodCount;
    [SerializeField] private TMP_Text woodCount;
    [SerializeField] private TMP_Text stoneCount;
    [SerializeField] private TMP_Text ironCount;
    [SerializeField] private TMP_Text magicCount;


    private void Start()
    {
        playerStats = GlobalStorage.instance.playerStats;
        resourcesManager = GlobalStorage.instance.resourcesManager;

        FillMana();
        FillResources();
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

        manaCount.text = currentManaCount.ToString();
    }

    private void UpdateManaUI(PlayersStats stat, float maxValue, float currentValue)
    {
        if(stat == PlayersStats.Mana) FillMana(maxValue, currentValue);        
    }

    private void FillResources()
    {
        resourcesDict.Clear();
        resourcesDict = resourcesManager.GetAllResources();
        foreach(var resource in resourcesDict)
        {
            switch(resource.Key)
            {
                case ResourceType.Gold:
                    goldCount.text = resource.Value.ToString();
                    break;

                case ResourceType.Food:
                    foodCount.text = resource.Value.ToString();
                    break;

                case ResourceType.Stone:
                    stoneCount.text = resource.Value.ToString();
                    break;

                case ResourceType.Wood:
                    woodCount.text = resource.Value.ToString();
                    break;

                case ResourceType.Iron:
                    ironCount.text = resource.Value.ToString();
                    break;

                case ResourceType.Magic:
                    magicCount.text = resource.Value.ToString();
                    break;

                default:
                    break;
            }
        }
    }


    private void OnEnable()
    {
        EventManager.UpgradeStatCurrentValue += UpdateManaUI;
        EventManager.UpgradeResources += FillResources;
        EventManager.ChangePlayer += EnableUI;
    }

    private void OnDisable()
    {
        EventManager.ChangePlayer -= EnableUI;
        EventManager.UpgradeResources -= FillResources;
        EventManager.UpgradeStatCurrentValue -= UpdateManaUI;
    }
}
