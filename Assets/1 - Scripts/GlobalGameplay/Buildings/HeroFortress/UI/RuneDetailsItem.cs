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

    private bool isToExpensive = false;
    private RuneWorkroom workroom;
    private RuneSO rune;

    public void Init(RuneWorkroom room, RuneSO runeSO, int count, int maxCount, bool canICreate)
    {        
        if(resourcesManager == null)
        {
            resourcesManager = GlobalStorage.instance.resourcesManager;
            resourcesIcons = resourcesManager.GetAllResourcesIcons();
            workroom = room;
        }

        ResetForm();

        rune = runeSO;
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

                if(isResourceEnough == false)
                    isToExpensive = true;
            }

            createButton.SetActive(canICreate);
            createWarning.SetActive(!canICreate);
        }
    }

    private void ResetForm()
    {
        isToExpensive = false;

        createButton.SetActive(true);
        createWarning.SetActive(false);
        maxAmountWarning.SetActive(false);
        CloseConfirm();
    }

    //Button
    public void TryToCreate()
    {
        //bool isToExpensive = detailsItems[index].GetPriceState();

        if(isToExpensive == true)
        {
            InfotipManager.ShowWarning("You do not have enough Resources for this action.");
            return;
        }

        workroom.CloseConfirms();
        confirmBlock.SetActive(true);
    }

    //Button
    public void CreateRune()
    {
        workroom.CreateRune(rune);
        CloseConfirm();
    }

    //Button
    public void CloseConfirm()
    {
        confirmBlock.SetActive(false);
    }

}
