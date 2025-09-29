using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Enums;
using Zenject;

public class PlayersArmyPart : MonoBehaviour
{
    private PlayersArmy playersArmy;
    private InfirmaryManager infirmaryManager;
    private UnitManager unitManager;

    [SerializeField] private GameObject uiSlot;

    [Header("Army")]
    public SquadSlotPlacing[] armySlots;

    [Header("Reserve")]
    public SquadSlotPlacing[] reserveSlots;
    [SerializeField] private GameObject storeVeil;

    [Header("Infirmary")]
    [SerializeField] private InfirmarySlot[] infirmarySlots;
    [SerializeField] private TMP_Text infirmaryCount;

    [HideInInspector] public bool isStartInit = true;
    #region Schemes

    [Inject]
    public void Construct
        (
        PlayersArmy playersArmy,
        InfirmaryManager infirmaryManager,
        UnitManager unitManager
        )
    {
        this.playersArmy = playersArmy;
        this.infirmaryManager = infirmaryManager;
        this.unitManager = unitManager;
    }

    public void CreateReserveScheme(Dictionary<UnitsTypes, FullSquad> armyDict)
    {
        if(armyDict.Count != reserveSlots.Length) 
        {
            Debug.Log("Squads != slots!");
            return;
        }

        int i = 0;
        foreach(var squad in armyDict)
        {
            if(squad.Value.unit.status == UnitStatus.Store)
            {
                reserveSlots[i].FillSlot(squad.Value.squadUI.gameObject);
                squad.Value.squadUI.gameObject.SetActive(squad.Value.unitController.quantity != 0);
            }
            else
            {
                reserveSlots[i].FillSlot(null);
            }
            i++;
        }
    }

    public void LoadUnit(FullSquad squad, int index)
    {
        if(squad != null)
        {
            armySlots[index].FillSlot(squad.squadUI.gameObject);
            if(squad.unitController.quantity != 0)
                squad.squadUI.gameObject.SetActive(true);
        }
        else
        {
            armySlots[index].FillSlot(null);
        }
    }

    public void ForceClearCell(ArmySlot armySlot, bool directionMode = true)
    {
        if(directionMode == true)
        {
            foreach(var item in reserveSlots)
            {
                if(item.squad == null)
                {
                    item.HandlingNewSlot(armySlot);
                    break;
                }
            }
        }
        else
        {
            int i = reserveSlots.Length - 1;
            while(i >= 0)
            {
                if(reserveSlots[i].squad == null)
                {
                    reserveSlots[i].HandlingNewSlot(armySlot);
                    break;
                }
                i--;
            }
        }
    }

    public void UpdateInfirmaryScheme()
    {
        float infarmaryCapacity = infirmaryManager.GetCurrentCapacity();
        float currentInjuredCount = infirmaryManager.GetInjuredCount();
        infirmaryCount.text = "[" + currentInjuredCount + "/" + infarmaryCapacity + "]";

        for(int i = 0; i < infirmarySlots.Length; i++)
            infirmarySlots[i].ResetSlot();

        Dictionary<UnitsTypes, InjuredUnitData> injuredDict = infirmaryManager.GetCurrentInjuredDict();

        int slotIndex = 0;
        foreach(var unit in injuredDict)
        {
            Sprite icon = unitManager.GetUnitsIcon(unit.Key);
            infirmarySlots[slotIndex].FillTheInfarmarySlot(icon, unit.Value.quantity, unit.Value.term);
            slotIndex++;
        }
    }

    private void DisableAllSlots(Dictionary<UnitsTypes, FullSquad> armyDict)
    {
        foreach(var item in armyDict)
            item.Value.squadUI.gameObject.SetActive(false);
    }

    public void UpdateArmyWindow()
    {
        if(isStartInit == true)
        {
            DisableAllSlots(playersArmy.fullArmy);
            //CreateArmyScheme(playersArmy.fullArmy);
            CreateReserveScheme(playersArmy.fullArmy);
            isStartInit = false;
        }

        //DisableAllSlots(playersArmy.fullArmy);
        //CreateArmyScheme(playersArmy.fullArmy);
        //CreateReserveScheme(playersArmy.fullArmy);

        storeVeil.SetActive(!GlobalStorage.instance.isGlobalMode);

        if(GlobalStorage.instance.isGlobalMode == true)
            SortingUnits();

        UpdateInfirmaryScheme();
    }

    public void SortingUnits()
    {
        foreach(var item in playersArmy.fullArmy)
        {
            if(item.Value.unit.isUnitActive == false)
            {
                ForceClearCell(item.Value.squadUI, false);
            }
        }
    }


    #endregion      
}
