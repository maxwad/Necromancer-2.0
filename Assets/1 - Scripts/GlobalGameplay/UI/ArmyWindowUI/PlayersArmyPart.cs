using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static NameManager;

public class PlayersArmyPart : MonoBehaviour
{
    private PlayersArmy playersArmy;
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

    private void Awake()
    {
        playersArmy = GlobalStorage.instance.playersArmy;
    }

    public void CreateReserveScheme(Unit[] army)
    {
        for(int i = 0; i < army.Length; i++)
        {
            //reserveSlots[i].FillTheArmySlot(army[i]);
        }
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
                squad.Value.squadUI.gameObject.SetActive(true);
                i++;
            }
        }
    }


    public void CreateArmyScheme(Unit[] army)
    {
        for (int i = 0; i < army.Length; i++)
        {
            //armySlots[i].FillTheArmySlot(army[i]);
        }
    }

    public void CreateArmyScheme(Dictionary<UnitsTypes, FullSquad> armyDict)
    {
        int i = 0;
        foreach(var squad in armyDict)
        {
            if(squad.Value.unit.status == UnitStatus.Army)
            {
                armySlots[i].FillSlot(squad.Value.squadUI.gameObject);
                squad.Value.squadUI.gameObject.SetActive(true);
                i++;
            }
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

    public void CreateInfirmaryScheme()
    {
        List<UnitsTypes> injuredList = GlobalStorage.instance.infirmaryManager.GetCurrentInjuredList();
        List<Unit> actualUnits = GlobalStorage.instance.unitManager.GetActualArmy();
        float infarmaryCapacity = GlobalStorage.instance.infirmaryManager.GetCurrentCapacity();
        float currentInjuredCount = injuredList.Count;

        infirmaryCount.text = "[" + currentInjuredCount + "/" + infarmaryCapacity + "]";

        for(int i = 0; i < infirmarySlots.Length; i++)
        {
            infirmarySlots[i].ResetSlot();
        }

        int slotIndex = 0;
        foreach(var unit in actualUnits)
        {
            int count = 0;
            foreach(var injuredUnit in injuredList)
            {
                if(injuredUnit == unit.unitType) count++;
            }

            if(count != 0)
            {
                infirmarySlots[slotIndex].FillTheInfarmarySlot(unit, count);
                slotIndex++;
            }
        }        
    }

    private void DisableAllSlots(Dictionary<UnitsTypes, FullSquad> armyDict)
    {
        foreach(var item in armyDict)
        {
            item.Value.squadUI.gameObject.SetActive(false);
        }
    }

    public void UpdateArmyWindow()
    {
        //CreateInfirmaryScheme();
        if(isStartInit == true)
        {
            DisableAllSlots(playersArmy.fullArmy);
            CreateArmyScheme(playersArmy.fullArmy);
            CreateReserveScheme(playersArmy.fullArmy);
            isStartInit = false;
        }

        storeVeil.SetActive(!GlobalStorage.instance.isGlobalMode);

        if(GlobalStorage.instance.isGlobalMode == true)
            SortingUnits();
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
