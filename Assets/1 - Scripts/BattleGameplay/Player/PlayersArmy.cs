using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using static NameManager;
using System;

public class PlayersArmy : MonoBehaviour
{
    private UnitManager unitManager;

    //TODO: we need to change start parameters army in future
    [SerializeField] private UnitsTypes[] playersArmyEnums;
    [HideInInspector] public Unit[] reserveArmy;
    private List<GameObject> realUnitsInReserve = new List<GameObject>();
    private List<UnitController> realUnitsComponentsInReserve = new List<UnitController>(); 
    [HideInInspector] public Unit[] playersArmy = new Unit[4];

    [Space]
    [SerializeField] private PlayersArmyWindow playersArmyWindow;
    private int firstIndexForReplaceUnit = -1;
    private int secondIndexForReplaceUnit = -1;

    [Space]
    [SerializeField] private GameObject ReserveArmyContainer;
    [SerializeField] private Vector2[] playersArmyPositions;
    private GameObject[] realUnitsOnBattlefield = new GameObject[4];


    [Space]
    [SerializeField] private BattleArmyController battleArmyController;

    private void  InitializeArmy()
    {
        unitManager = GlobalStorage.instance.unitManager;
        reserveArmy = new Unit[Enum.GetValues(typeof(UnitsTypes)).Length];
        reserveArmy = unitManager.GetUnitsForPlayersArmy(playersArmyEnums);

        //TODO: delete manual quantity
        foreach (var item in reserveArmy)
        {
            if (item != null)
            {
                item.quantity = UnityEngine.Random.Range(4, 10);
                item.currentHealth = item.health;              
            }
        }

        CreateRealUnitsInReserve();
    }

    private void CreateRealUnitsInReserve()
    {
        for(int i = 0; i < reserveArmy.Length; i++)
        {
            if(reserveArmy[i] != null)
            {
                GameObject unit = Instantiate(reserveArmy[i].unitGO);
                unit.transform.SetParent(ReserveArmyContainer.transform);
                unit.GetComponentInChildren<TMP_Text>().text = reserveArmy[i].quantity.ToString();
                unit.GetComponent<UnitController>().Initilize(reserveArmy[i]);
                unit.SetActive(false);
                realUnitsInReserve.Add(unit);
                // we need this for acces to UnitController Script of disable objects
                realUnitsComponentsInReserve.Add(unit.GetComponent<UnitController>());
            }
        }
    }

    private void SwitchUnit(bool mode, Unit unit)
    {
        if(unit == null) return;

        Unit[] targetUnitArray = (mode == true) ? playersArmy : reserveArmy;
        Unit[] sourceUnitArray = (mode == true) ? reserveArmy : playersArmy;

        bool checkEmptySlotInArmy = false;
        int index = 0;

        for(int i = 0; i < targetUnitArray.Length; i++)
        {
            if(targetUnitArray[i] == null)
            {
                checkEmptySlotInArmy = true;
                index = i;
                break;
            }
        }

        if(checkEmptySlotInArmy == true)
        {
            for(int i = 0; i < sourceUnitArray.Length; i++)
            {
                if(sourceUnitArray[i] != null && sourceUnitArray[i].UnitType == unit.UnitType)
                {
                    sourceUnitArray[i] = null;
                    break;
                }
            }

            targetUnitArray[index] = unit;
        }
        else
        {
            return;
        }

        CreateArmyOnBattlefield();
    }

    public void UnitsReplacementUI(int index)
    {
        if(playersArmy.Length == 0) return;

        if (firstIndexForReplaceUnit == -1)
        {
            firstIndexForReplaceUnit = index;
        }
        else if (index == firstIndexForReplaceUnit)
        {
            ResetReplaceIndexes();
        }
        else
        {
            secondIndexForReplaceUnit = index;

            Unit oldUnit = playersArmy[firstIndexForReplaceUnit];
            playersArmy[firstIndexForReplaceUnit] = playersArmy[secondIndexForReplaceUnit];
            playersArmy[secondIndexForReplaceUnit] = oldUnit;

            CreateArmyOnBattlefield();

            ResetReplaceIndexes();
        }
    }

    public void ResetReplaceIndexes()
    {
        firstIndexForReplaceUnit = -1;
        secondIndexForReplaceUnit = -1;
    }

    private void CreateArmyOnBattlefield()
    {
        //move all realArmy units to realArmyReserve
        for(int i = 0; i < realUnitsOnBattlefield.Length; i++)
        {
            if(realUnitsOnBattlefield[i] != null)
            {
                realUnitsOnBattlefield[i].transform.SetParent(ReserveArmyContainer.transform);                

                realUnitsInReserve.Add(realUnitsOnBattlefield[i]);
                realUnitsComponentsInReserve.Add(realUnitsOnBattlefield[i].GetComponent<UnitController>());
                realUnitsOnBattlefield[i].SetActive(false);

                realUnitsOnBattlefield[i] = null;
            }
        }

        //fill realArmy from realArmyReserve
        for(int i = 0; i < playersArmy.Length; i++)
        {
            if(playersArmy[i] != null)
            {
                for(int j = 0; j < realUnitsInReserve.Count; j++)
                {
                    //read the name of list attentively!
                    if(realUnitsComponentsInReserve[j].GetComponent<UnitController>().unitType == playersArmy[i].UnitType)
                    {
                        realUnitsOnBattlefield[i] = realUnitsInReserve[j];
                        realUnitsOnBattlefield[i].SetActive(true);
                        realUnitsOnBattlefield[i].transform.position = (Vector3)playersArmyPositions[i] + battleArmyController.gameObject.transform.position;
                        realUnitsOnBattlefield[i].transform.SetParent(battleArmyController.gameObject.transform);

                        realUnitsInReserve.Remove(realUnitsInReserve[j]);
                        realUnitsComponentsInReserve.Remove(realUnitsComponentsInReserve[j]);
                        break;
                    }
                }
            }
        }

        playersArmyWindow.CreateReserveScheme(reserveArmy);
        playersArmyWindow.CreateArmyScheme(playersArmy);
    }


    public void UpgradeArmy(UnitsTypes unitType, int quantity)
    {
        for (int i = 0; i < playersArmy.Length; i++)
        {
            if (playersArmy[i]?.UnitType == unitType)
            {
                if (quantity == 0)
                {
                    playersArmy[i] = null;
                }
                else 
                {
                    playersArmy[i].quantity = quantity;
                }                
                
                break;
            }
        }
    }

    #region Resurrection

    private void TryResurrectUnitFromArmy(UnitsTypes unitType)
    {
        bool isSquadFinded = false;

        for(int i = 0; i < playersArmy.Length; i++)
        {
            if(playersArmy[i]?.UnitType == unitType)
            {
                playersArmy[i].quantity++;
                isSquadFinded = true;
                break;
            }
        }

        if(isSquadFinded == true)
        {
            for(int i = 0; i < realUnitsOnBattlefield.Length; i++)
            {
                UnitController unit = realUnitsOnBattlefield[i].GetComponent<UnitController>();
                if(unit.unitType == unitType)
                {
                    unit.quantity++;
                    unit.UpdateSquad(true);
                    break;
                }
            }
        }
        else
        {
            TryResurrectUnitFromReserve(unitType);
        }
    }

    private void TryResurrectUnitFromReserve(UnitsTypes unitType)
    {
        bool isSquadFinded = false;

        for(int i = 0; i < reserveArmy.Length; i++)
        {
            if(reserveArmy[i]?.UnitType == unitType)
            {
                reserveArmy[i].quantity++;
                isSquadFinded = true;
                break;
            }
        }

        if(isSquadFinded == true)
        {
            for(int i = 0; i < realUnitsComponentsInReserve.Count; i++)
            {
                UnitController unit = realUnitsComponentsInReserve[i];
                if(unit.unitType == unitType)
                {
                    unit.quantity++;
                    unit.UpdateSquad(true);
                    break;
                }
            }
        }
        else
        {
            Unit newSquad = unitManager.GetNewSquad(unitType);

            GameObject realUnit = Instantiate(newSquad.unitGO);
            realUnit.GetComponentInChildren<TMP_Text>().text = newSquad.quantity.ToString();
            realUnit.GetComponent<UnitController>().Initilize(newSquad);

            TryToCreateUnitInArmy(newSquad, realUnit);
        }
    }

    private void TryToCreateUnitInArmy(Unit newUnit, GameObject newUnitGO)
    {
        bool isFreePlaceFinded = false;
        int findedIndex = -1;

        for(int i = 0; i < playersArmy.Length; i++)
        {
            if(playersArmy[i] == null)
            {                
                isFreePlaceFinded = true;
                findedIndex = i;
                break;
            }
        }

        if(isFreePlaceFinded == true)
        {
            playersArmy[findedIndex] = newUnit;
            realUnitsOnBattlefield[findedIndex] = newUnitGO;

            newUnitGO.transform.position = (Vector3)playersArmyPositions[findedIndex] + battleArmyController.gameObject.transform.position;
            newUnitGO.transform.SetParent(battleArmyController.transform);

            playersArmyWindow.CreateArmyScheme(playersArmy);
        }
        else
        {
            TryToCreateUnitInReserve(newUnit, newUnitGO);
        }
    }

    private void TryToCreateUnitInReserve(Unit newUnit, GameObject newUnitGO) 
    {
        bool isFreePlaceFinded = false;
        int findedIndex = -1;

        for(int i = 0; i < reserveArmy.Length; i++)
        {
            if(reserveArmy[i] == null)
            {
                isFreePlaceFinded = true;
                findedIndex = i;
                break;
            }
        }

        if(isFreePlaceFinded == true)
        {
            reserveArmy[findedIndex] = newUnit;
            newUnitGO.transform.SetParent(ReserveArmyContainer.transform);

            newUnitGO.SetActive(false);
            realUnitsInReserve.Add(newUnitGO);
            realUnitsComponentsInReserve.Add(newUnitGO.GetComponent<UnitController>());

            playersArmyWindow.CreateReserveScheme(reserveArmy);
        }
        else
        {
            Debug.Log("SOMRTHING WRONG WITH RESURRECTION!");
        }
    }

    #endregion

    private void OnEnable()
    {
        EventManager.AllUnitsIsReady += InitializeArmy;
        EventManager.WeLostOneUnit += UpgradeArmy;
        EventManager.SwitchUnit += SwitchUnit;
        EventManager.ResurrectUnit += TryResurrectUnitFromArmy;
    }

    private void OnDisable()
    {
        EventManager.AllUnitsIsReady -= InitializeArmy;
        EventManager.WeLostOneUnit -= UpgradeArmy;
        EventManager.SwitchUnit -= SwitchUnit; 
        EventManager.ResurrectUnit -= TryResurrectUnitFromArmy;
    }
}
