using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;
using UnityEngine.EventSystems;
using System;

public class SquadSlotPlacing : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    public UnitStatus slotType;
    public ArmySlot squad = null;
    public int index = 0;

    public void FillSlot(GameObject currentSquad)
    {
        squad = currentSquad.GetComponent<ArmySlot>();
        squad.index = index;
        squad.slotType = slotType;
        squad.unitInSlot.status = slotType;

        currentSquad.transform.SetParent(transform, false);
        currentSquad.transform.localPosition = Vector3.zero;
    }


    public void OnDrop(PointerEventData eventData)
    {
        ArmySlot slot = eventData.pointerDrag.GetComponent<ArmySlot>();
        if(slot != null)
        {
            if(squad != null) EventManager.OnSwitchSlotsEvent(slot.index, slot.unitInSlot.status, squad.gameObject);

            FillSlot(eventData.pointerDrag);
            //squad = slot;
            //squad.unitInSlot.status = (slotTypes == UISlotTypes.Army) ? UnitStatus.Army : UnitStatus.Store;

            //eventData.pointerDrag.transform.SetParent(transform, false);
            //eventData.pointerDrag.transform.localPosition = Vector3.zero;
        }
        else
        {

            Debug.Log("THERE IS NO SLOT");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        if(squad != null)
        {
            squad.unitInSlot.status = UnitStatus.Store;
            squad = null;
        }
    }

    private void SwitchSlot(int ind, UnitStatus place, GameObject slot)
    {
        if(index == ind && slotType == place) FillSlot(slot);
    }

    private void OnEnable()
    {
        EventManager.SwitchSlots += SwitchSlot;
    }

    private void OnDisable()
    {
        EventManager.SwitchSlots += SwitchSlot;
    }
}
