using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;
using System;

public class RuneDetailsItem : MonoBehaviour
{
    private ResourcesManager resourcesManager;
    private Dictionary<ResourceType, Sprite> resourcesIcons;

    [Header("Rune Details")]
    [SerializeField] private TMP_Text level;
    [SerializeField] private TMP_Text description;
    [SerializeField] private List<Image> costIconsList;
    [SerializeField] private List<TMP_Text> costAmountList;

    [Header("Action Elements")]
    [SerializeField] private GameObject createButton;
    [SerializeField] private GameObject confirmBlock;
    [SerializeField] private GameObject createWarning;
    [SerializeField] private GameObject maxAmountWarning;

    private Color normalColor = Color.white;
    [SerializeField] private Color warningColor;

    public void Init(RuneSO runeSO, int count, int maxCount, bool canICreate)
    {        
        if(resourcesManager == null)
        {
            resourcesManager = GlobalStorage.instance.resourcesManager;
            resourcesIcons = resourcesManager.GetAllResourcesIcons();
        }

        ResetForm();

        level.text = "Level " + runeSO.level;
        description.text = runeSO.positiveDescription.Replace("$", runeSO.value.ToString());

        if(count >= maxCount)
        {
            createButton.SetActive(false);
            maxAmountWarning.SetActive(true);
        }
        else
        {
            createButton.SetActive(true);
            maxAmountWarning.SetActive(false);

            foreach(var itemCost in costIconsList)
            {
                itemCost.transform.parent.gameObject.SetActive(false);
            }

            for(int i = 0; i < runeSO.cost.Count; i++)
            {
                costIconsList[i].transform.parent.gameObject.SetActive(true);
                costIconsList[i].sprite = resourcesIcons[runeSO.cost[i].type];
                costAmountList[i].text = runeSO.cost[i].amount.ToString();

                bool isResourceEnough = resourcesManager.CheckMinResource(runeSO.cost[i].type, runeSO.cost[i].amount);
                costAmountList[i].color = (isResourceEnough == true) ? normalColor : warningColor;
            }

            createButton.SetActive(canICreate);
            createWarning.SetActive(!canICreate);
        }
    }

    private void ResetForm()
    {
        createButton.SetActive(true);
        createWarning.SetActive(false);
        maxAmountWarning.SetActive(false);
    }
}
