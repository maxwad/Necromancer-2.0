using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static NameManager;

public class PlayersArmyPart : MonoBehaviour
{
    private PlayersArmy playersArmy;

    //[SerializeField] private GameObject playerArmyUI;

    [Header("Army")]
    [SerializeField] private ArmySlot[] armySlots;

    [Header("Reserve")]
    [SerializeField] private ArmySlot[] reserveSlots;
    [SerializeField] private GameObject reserveVeil;

    [Header("Infirmary")]
    [SerializeField] private InfirmarySlot[] infirmarySlots;
    [SerializeField] private TMP_Text infirmaryCount;

    //[HideInInspector] public bool isWindowOpened = false;
    //[HideInInspector] public bool isInTheBattleWindow = false;
    //[HideInInspector] public bool isForBattleWindow = false;


    #region Schemes

    private void Awake()
    {
        playersArmy = GlobalStorage.instance.player.GetComponent<PlayersArmy>();
    }

    public void CreateReserveScheme(Unit[] army)
    {
        for(int i = 0; i < army.Length; i++)
        {
            reserveSlots[i].FillTheArmySlot(army[i]);
        }
    }

    public void CreateArmyScheme(Unit[] army)
    {
        for (int i = 0; i < army.Length; i++)
        {
            armySlots[i].FillTheArmySlot(army[i]);
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
        CreateReserveScheme(playersArmy.reserveArmy);
        CreateArmyScheme(playersArmy.playersArmy);
        CreateInfirmaryScheme();

        if(GlobalStorage.instance.isGlobalMode == true)
            reserveVeil.SetActive(false);
        else
            reserveVeil.SetActive(true);
    }

    //public void ResetSlots()
    //{
    //    for(int i = 0; i < armySlots.Length; i++)
    //        armySlots[i].ResetSelecting();
    //}

    #endregion

    #region Helpers



    #endregion       
}
