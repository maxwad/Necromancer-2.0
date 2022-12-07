using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static NameManager;

public class UnitCenter : MonoBehaviour
{
    private FortressBuildings allBuildings;
    private UnitManager unitManager;

    [SerializeField] private GameObject warningMessage;
    [SerializeField] private List<GameObject> slotList;

    private List<Cost> currentCosts = new List<Cost>();
    private Unit currentUnit;

    public void Init()
    {
        if(allBuildings == null)
        {
            allBuildings = GlobalStorage.instance.fortressBuildings;
            unitManager = GlobalStorage.instance.unitManager;
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
        }
        else
        {
            warningMessage.SetActive(false);

            int slotIndex = 0;
            foreach(var building in openedMilitary)
            {
                List<UnitsTypes> units = unitManager.GetUnitsByBuildings(building);

                Debug.Log("We havve units: " + units.Count);
                for(int i = 0; i < units.Count; i++)
                {
                    GameObject slot = slotList[slotIndex];
                    slot.SetActive(true);
                    UnitInCenterUI unitInSlot = slot.GetComponent<UnitInCenterUI>();

                    Unit unit = unitManager.GetUnitForTip(units[i]);
                    if(unit != null)
                    {
                        Debug.Log("!!!");
                        unitInSlot.Init(unit, this);
                    }

                    slotIndex++;
                }
            }
        }
    }

    public void SetUnitForHiring(Unit unit, List<Cost> costs)
    {
        currentUnit = unit;
        currentCosts = costs;
    }

    //Slider
    public void CheangeUnitsAmount()
    {

    }

    //Button
    public void SetMaxUnitsAmount()
    {

    }

    //Button
    public void Deal()
    {

    }

    private void ResetForm()
    {
        foreach(var slot in slotList)
        {
            slot.SetActive(false);
        }

        currentUnit = null;
        currentCosts.Clear();
    }
}
