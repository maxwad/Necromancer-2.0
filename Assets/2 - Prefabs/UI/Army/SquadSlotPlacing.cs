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
    public UISlotTypes slotTypes;
    public ArmySlot squad = null;
    public int index = 0;

    public void FillSlot(GameObject currentSquad)
    {
        squad = currentSquad.GetComponent<ArmySlot>();
        squad.index = index;

        currentSquad.transform.SetParent(transform, false);
        currentSquad.transform.localPosition = Vector3.zero;
    }


    public void OnDrop(PointerEventData eventData)
    {
        ArmySlot slot = eventData.pointerDrag.GetComponent<ArmySlot>();
        if(slot != null)
        {
            squad = slot;
            squad.unitInSlot.status = (slotTypes == UISlotTypes.Army) ? UnitStatus.Army : UnitStatus.Store;

            eventData.pointerDrag.transform.SetParent(transform, false);
            eventData.pointerDrag.transform.localPosition = Vector3.zero;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            ClearSlot();
        }
    }

    private void ClearSlot()
    {
        if(squad != null)
        {
            squad.unitInSlot.status = UnitStatus.Store;
            squad = null;
        }
    }
}
