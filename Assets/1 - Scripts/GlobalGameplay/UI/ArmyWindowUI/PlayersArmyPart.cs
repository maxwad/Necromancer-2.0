using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static NameManager;

public class PlayersArmyPart : MonoBehaviour
{
    private PlayersArmy playersArmy;
    private ObjectsPoolManager poolManager;
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
        playersArmy = GlobalStorage.instance.player.GetComponent<PlayersArmy>();
        poolManager = GlobalStorage.instance.objectsPoolManager;
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

        foreach(var item in reserveSlots)
        {
            if(item.squad != null) Destroy(item.squad.gameObject);           
        }

        int i = 0;
        foreach(var squad in armyDict)
        {
            if(squad.Value.unit.status == UnitStatus.Store)
            {
                GameObject squadUI = Instantiate(uiSlot);
                squadUI.GetComponent<ArmySlot>().Init(squad.Value.unit);
                reserveSlots[i].FillSlot(squadUI);
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
        foreach(var item in armySlots)
        {
            if(item.squad != null) Destroy(item.squad.gameObject);
        }

        int i = 0;
        foreach(var squad in armyDict)
        {
            if(squad.Value.unit.status == UnitStatus.Army)
            {
                GameObject squadUI = Instantiate(uiSlot);
                squadUI.GetComponent<ArmySlot>().Init(squad.Value.unit);
                armySlots[i].FillSlot(squadUI);
                i++;
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

    public void UpdateArmyWindow()
    {
        //CreateReserveScheme(playersArmy.storeArmy);
        //CreateArmyScheme(playersArmy.playersArmy);
        //CreateReserveScheme(playersArmy.fullArmy);
        //CreateArmyScheme(playersArmy.fullArmy);

        //CreateInfirmaryScheme();
        if(isStartInit == true)
        {
            CreateArmyScheme(playersArmy.fullArmy);
            CreateReserveScheme(playersArmy.fullArmy);
            isStartInit = false;
        }

        if(GlobalStorage.instance.isGlobalMode == true)
            storeVeil.SetActive(false);
        else
            storeVeil.SetActive(true);
    }

    #endregion      
}
