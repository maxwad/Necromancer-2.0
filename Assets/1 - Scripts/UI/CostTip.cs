using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class CostTip : MonoBehaviour
{
    [SerializeField] private RectTransform tipBlock;
    private float fullHeigth;
    private float differenceHeigth;
    private float levelBlockHeigth = 0;
    private float resumeHeigth = 0;

    [SerializeField] private GameObject levelBlock;
    [SerializeField] private TMP_Text fortressLevel;

    [SerializeField] private GameObject costBlock;
    private List<Image> labels;
    private List<TMP_Text> amounts;

    [SerializeField] private Color allowColor;
    [SerializeField] private Color deniedColor;

    private ResourcesManager resourcesManager;
    public Dictionary<ResourceType, Sprite> resourcesIcons;

    public void Init(BuildingsRequirements requirements)
    {
        if(resourcesManager == null)
        {
            resourcesManager = GlobalStorage.instance.resourcesManager;
            resourcesIcons = resourcesManager.GetAllResourcesIcons();
            levelBlockHeigth = levelBlock.GetComponent<RectTransform>().rect.height;
            fullHeigth = tipBlock.rect.height;

            labels = new List<Image>(costBlock.GetComponentsInChildren<Image>());
            amounts = new List<TMP_Text>(costBlock.GetComponentsInChildren<TMP_Text>());
        }

        foreach(var item in labels)
        {
            item.transform.parent.gameObject.SetActive(false);
        }

        levelBlock.SetActive(!requirements.canIBuild);
        fortressLevel.text = requirements.fortressLevel.ToString();
        fortressLevel.color = (requirements.canIBuild == true) ? allowColor : deniedColor;
        differenceHeigth = (requirements.canIBuild == true) ? levelBlockHeigth : 0;
            
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

        resumeHeigth = fullHeigth - differenceHeigth;
        tipBlock.SetSizeWithCurrentAnchors( RectTransform.Axis.Vertical, resumeHeigth);
    }
}
