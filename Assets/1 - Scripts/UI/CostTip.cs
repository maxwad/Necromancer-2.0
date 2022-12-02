using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static NameManager;

public class CostTip : MonoBehaviour
{
    [SerializeField] private RectTransform tipBlock;
    private float minHeight = 60;
    private float itemHeight = 22;
    private int itemCount = 0;

    [SerializeField] private List<TMP_Text> labels;
    [SerializeField] private List<TMP_Text> amounts;

    [SerializeField] private Color allowColor;
    [SerializeField] private Color deniedColor;

    private ResourcesManager resourcesManager;

    public void Init(List<Cost> cost)
    {
        if(resourcesManager == null)
        {
            resourcesManager = GlobalStorage.instance.resourcesManager;
        }

        foreach(var item in labels)
        {
            item.transform.parent.gameObject.SetActive(false);
        }

        if(labels.Count < cost.Count)
        {
            Debug.Log("Too much costs!");
            return;
        }

        itemCount = 0;
        for(int i = 0; i < cost.Count; i++)
        {
            labels[i].transform.parent.gameObject.SetActive(true);
            labels[i].text = cost[i].type.ToString();
            bool checkColor = resourcesManager.CheckMinResource(cost[i].type, cost[i].amount);
            amounts[i].color = (checkColor == true) ? allowColor : deniedColor;
            amounts[i].text = cost[i].amount.ToString();
            itemCount++;
        }

        float blockHeight = minHeight + itemHeight * itemCount;
        tipBlock.SetSizeWithCurrentAnchors( RectTransform.Axis.Vertical, blockHeight);
    }
}
