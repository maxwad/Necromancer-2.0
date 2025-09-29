using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Enums;
using Zenject;

public class CostTip : MonoBehaviour
{
    [SerializeField] private RectTransform tipBlock;
    [SerializeField] private GameObject levelBlock;
    [SerializeField] private TMP_Text fortressLevel;

    [SerializeField] private GameObject costBlock;
    private List<Image> labels;
    private List<TMP_Text> amounts;

    [SerializeField] private Color allowColor;
    [SerializeField] private Color deniedColor;

    private ResourcesManager resourcesManager;
    public Dictionary<ResourceType, Sprite> resourcesIcons;

    [Inject]
    public void Construct(ResourcesManager resourcesManager)
    {
        this.resourcesManager = resourcesManager;
    }

    public void Init(BuildingsRequirements requirements)
    {
        if(resourcesIcons == null)
        {
            resourcesIcons = resourcesManager.GetAllResourcesIcons();

            labels = new List<Image>(costBlock.GetComponentsInChildren<Image>());
            amounts = new List<TMP_Text>(costBlock.GetComponentsInChildren<TMP_Text>());
        }

        foreach(var item in labels)
        {
            item.transform.parent.gameObject.SetActive(false);
        }

        levelBlock.SetActive(false);
        if(requirements.isCostForCastle == true)
        {
            levelBlock.SetActive(!requirements.canIBuild);
            fortressLevel.text = requirements.fortressLevel.ToString();
            fortressLevel.color = (requirements.canIBuild == true) ? allowColor : deniedColor;
        }
            
        List<Cost> cost = requirements.costs;

        if(labels.Count < cost.Count)
        {
            Debug.Log("Too much costs!");
            return;
        }

        for(int i = 0; i < cost.Count; i++)
        {
            labels[i].transform.parent.gameObject.SetActive(true);
            labels[i].sprite = resourcesIcons[cost[i].type];
            bool checkColor = resourcesManager.CheckMinResource(cost[i].type, cost[i].amount);
            amounts[i].color = (checkColor == true) ? allowColor : deniedColor;
            amounts[i].text = cost[i].amount.ToString();
        }
    }
}
