using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class UnitCenter : MonoBehaviour
{
    private FortressBuildings allBuildings;
    private Garrison garrison;
    private UnitManager unitManager;
    private ResourcesManager resourcesManager;

    private Dictionary<ResourceType, Sprite> resourcesIcons;

    [SerializeField] private GameObject warningMessage;
    [SerializeField] private GameObject dealBlock;

    [SerializeField] private List<GameObject> slotList;
    [SerializeField] private List<GameObject> costs;

    [SerializeField] private TMP_Text unitsName;
    [SerializeField] private Image currentUnitIcon;
    [SerializeField] private Slider hiringSlider;
    [SerializeField] private TMP_Text unitsAmount;
    [SerializeField] private Button dealButton;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color warningColor;


    private Sprite emptyIcon;

    private List<Cost> currentCosts = new List<Cost>();
    private int maxAmount;
    private int currentAmount;
    private Unit currentUnit;
    private bool canIHire = true;


    public void Init()
    {
        if(allBuildings == null)
        {
            allBuildings = GlobalStorage.instance.fortressBuildings;
            garrison = allBuildings.GetComponent<Garrison>();
            unitManager = GlobalStorage.instance.unitManager;
            resourcesManager = GlobalStorage.instance.resourcesManager;
            resourcesIcons = resourcesManager.GetAllResourcesIcons();
            emptyIcon = currentUnitIcon.sprite;
        }

        gameObject.SetActive(true);

        ResetForm();

        FillSlots();
    }

    private void FillSlots()
    {
        List<CastleBuildings> openedMilitary = allBuildings.GetMilitary();

        if(openedMilitary.Count == 0)
        {
            warningMessage.SetActive(true);
            dealBlock.SetActive(false);
        }
        else
        {
            warningMessage.SetActive(false);
            dealBlock.SetActive(true);

            int slotIndex = 0;
            foreach(var building in openedMilitary)
            {
                List<UnitsTypes> units = unitManager.GetUnitsByBuildings(building);

                for(int i = 0; i < units.Count; i++)
                {
                    GameObject slot = slotList[slotIndex];
                    slot.SetActive(true);
                    UnitInCenterUI unitInSlot = slot.GetComponent<UnitInCenterUI>();

                    Unit unit = unitManager.GetUnitForTip(units[i]);
                    if(unit != null)
                    {
                        unitInSlot.Init(unit, this);
                    }

                    slotIndex++;
                }
            }
        }
    }

    public void SetUnitForHiring(Unit unit, int amount, List<Cost> costsList)
    {
        if(amount != 0)
        {
            currentUnit = unit;
            maxAmount = amount;
            currentCosts = costsList;

            currentAmount = 0;
            hiringSlider.value = 0;
            unitsAmount.text = "0";
            unitsName.text = currentUnit.unitName;
            currentUnitIcon.sprite = currentUnit.unitIcon;

            canIHire = true;
        }
        else
        {
            InfotipManager.ShowWarning("You don't have free units of that type for hiring.");
        }
    }

    //Slider
    public void ChangeUnitsAmount()
    {
        currentAmount = Mathf.CeilToInt(maxAmount * hiringSlider.value);
        unitsAmount.text = currentAmount.ToString();
        CalculateCosts();

        dealButton.interactable = (currentAmount > 0);
    }

    //Button
    public void SetMaxUnitsAmount()
    {
        if(maxAmount != 0)
        {
            bool maxIsReached = false;

            for(int i = 0; i <= maxAmount; i++)
            {
                foreach(var cost in currentCosts)
                {
                    if(resourcesManager.CheckMinResource(cost.type, cost.amount * i) == false)
                    {
                        maxIsReached = true;
                        break;
                    }
                }

                if(maxIsReached == true)
                    break;
                else
                    currentAmount = i;
            }

            hiringSlider.value = (float)currentAmount / (float)maxAmount;
        }
        else
        {
            InfotipManager.ShowWarning("Choose unit please.");
        }
    }

    //Button
    public void Deal()
    {
        if(currentUnit == null)
        {
            InfotipManager.ShowWarning("Choose unit please.");
            return;
        }

        if(canIHire == true)
        {
            foreach(var cost in currentCosts)
                resourcesManager.ChangeResource(cost.type, -cost.amount * currentAmount);

            garrison.HiringUnits(currentUnit.unitType, currentAmount);

            Init();
        }
        else
        {
            InfotipManager.ShowWarning("You don't have enough resources to hire that quantity of units.");
        }
    }

    private void CalculateCosts()
    {
        if(currentAmount != 0)
        {
            canIHire = true;

            for(int i = 0; i < costs.Count; i++)
            {
                costs[i].SetActive(!(i >= currentCosts.Count));
            }

            for(int i = 0; i < currentCosts.Count; i++)
            {
                ResourceType resourceType = currentCosts[i].type;
                TMP_Text costItem = costs[i].GetComponentInChildren<TMP_Text>();

                float resumeCost = Mathf.Round(currentCosts[i].amount * currentAmount);

                costs[i].GetComponentInChildren<Image>().sprite = resourcesIcons[resourceType];
                costItem.text = resumeCost.ToString();

                if(resourcesManager.CheckMinResource(resourceType, resumeCost) == true)
                {
                    costItem.color = normalColor;
                }
                else
                {
                    costItem.color = warningColor;
                    canIHire = false;
                }                
            }
        }
        else
        {
            for(int i = 0; i < costs.Count; i++)
            {
                costs[i].SetActive(false);
            }
        }
    }

    private void ResetForm()
    {
        foreach(var slot in slotList)
        {
            slot.SetActive(false);
        }

        foreach(var cost in costs)
        {
            cost.SetActive(false);
        }

        currentUnit = null;
        maxAmount = 0;
        currentAmount = 0;
        currentCosts.Clear();

        hiringSlider.value = 0;
        unitsAmount.text = "0";
        currentUnitIcon.sprite = emptyIcon;
        unitsName.text = "";

        dealButton.interactable = false;
        canIHire = true;
    }
}
