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

    private PlayersArmy playersArmy;

    public void FillSlot(GameObject currentSquad)
    {
        squad = currentSquad.GetComponent<ArmySlot>();
        squad.index = index;
        squad.slotType = slotType;
        squad.unitInSlot.status = slotType;

        currentSquad.transform.SetParent(transform, false);
        currentSquad.transform.localPosition = Vector3.zero;

        if(playersArmy == null) playersArmy = GlobalStorage.instance.player.GetComponent<PlayersArmy>();
        playersArmy.UpdateSquads();
    }


    public void OnDrop(PointerEventData eventData)
    {
        ArmySlot slot = eventData.pointerDrag.GetComponent<ArmySlot>();

        if(GlobalStorage.instance.isGlobalMode == false)
        {
            if(slot.slotType == UnitStatus.Store)
            {
                InfotipManager.ShowWarning("You can not add any squads to your hero during the battle.");
                return;
            }

            if(slot.slotType == UnitStatus.Army && slotType == UnitStatus.Store)
            {
                InfotipManager.ShowWarning("You can not delete any squads from hero's army during the battle.");
                return;
            }
        }
        

        if(slot != null)
        {
            HandlingNewSlot(slot);

            //if(squad != null) 
            //    EventManager.OnSwitchSlotsEvent(slot.index, slot.unitInSlot.status, squad.gameObject);
            //else
            //    EventManager.OnSwitchSlotsEvent(slot.index, slot.unitInSlot.status, null);


            //FillSlot(eventData.pointerDrag);


            //eventData.pointerDrag.transform.SetParent(transform, false);
            //eventData.pointerDrag.transform.localPosition = Vector3.zero;
        }
        else
        {

            Debug.Log("THERE IS NO SLOT");
        }
    }

    public void HandlingNewSlot(ArmySlot slot)
    {
        if(squad != null)
            EventManager.OnSwitchSlotsEvent(slot.index, slot.unitInSlot.status, squad.gameObject);
        else
            EventManager.OnSwitchSlotsEvent(slot.index, slot.unitInSlot.status, null);

        FillSlot(slot.gameObject);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        //if(eventData.button == PointerEventData.InputButton.Right)
        //{
        //    //ClearSlot();

        //    Debug.Log("CLEAR");
        //}
    }

    public void ClearSlot()
    {
        if(squad != null)
        {
            //squad.unitInSlot.status = UnitStatus.Store;
            squad = null;
        }
    }

    private void SwitchSlot(int ind, UnitStatus place, GameObject slot)
    {
        if(index == ind && slotType == place)
        {
            if(slot != null)
            {
                FillSlot(slot);
            }
            else
            {
                squad = null;
            }
        }
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
