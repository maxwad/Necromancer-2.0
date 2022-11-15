using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static NameManager;
using System;

public class PlayersArmy : MonoBehaviour
{
    private UnitManager unitManager;

    //TODO: we need to change start parameters army in future
    [HideInInspector] public Unit[] reserveArmy;
    private List<GameObject> unitsInReserveList = new List<GameObject>();
    //private Dictionary<UnitsTypes, GameObject> unitsInReserveDict = new Dictionary<UnitsTypes, GameObject>();

    [HideInInspector] public Unit[] playersArmy = new Unit[4];
    private GameObject[] unitsOnBattlefield = new GameObject[4];

    [Space]
    [SerializeField] private PlayersArmyPart playersArmyWindow;
    private int firstIndexForReplaceUnit = -1;
    private int secondIndexForReplaceUnit = -1;

    [Space]
    [SerializeField] private GameObject reserveArmyContainer;
    [SerializeField] private Vector2[] playersArmyPositions;

    [Space]
    [SerializeField] private BattleArmyController battleArmyController;

    private float damageArmyByEscape = 0.3f;

    private void  InitializeArmy()
    {
        unitManager = GlobalStorage.instance.unitManager;
        reserveArmy = unitManager.GetUnitsForPlayersArmy();

        //TODO: delete manual quantity
        foreach (var item in reserveArmy)
        {
            if (item != null)
            {
                item.SetQuantity(UnityEngine.Random.Range(5, 9));
                item.currentHealth = item.health;
            }
        }

        CreateRealUnitsInReserve();

        //end of loading Units and Army
        GlobalStorage.instance.LoadNextPart();
    }

    private void CreateRealUnitsInReserve()
    {
        for(int i = 0; i < reserveArmy.Length; i++)
        {
            if(reserveArmy[i] != null)
            {
                GameObject unit = Instantiate(reserveArmy[i].unitGO);
                unit.transform.SetParent(reserveArmyContainer.transform);
                unit.GetComponentInChildren<TMP_Text>().text = reserveArmy[i].quantity.ToString();

                unit.GetComponent<UnitController>().Initilize(reserveArmy[i]);
                unit.SetActive(false);
                unitsInReserveList.Add(unit);
            }
        }
    }

    public void SwitchUnit(bool mode, Unit unit)
    {
        ResetReplaceIndexes();
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
                if(sourceUnitArray[i] != null && sourceUnitArray[i].unitType == unit.unitType)
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
        for(int i = 0; i < unitsOnBattlefield.Length; i++)
        {
            if(unitsOnBattlefield[i] != null)
            {
                unitsOnBattlefield[i].transform.SetParent(reserveArmyContainer.transform);                

                unitsOnBattlefield[i].SetActive(false);
                unitsInReserveList.Add(unitsOnBattlefield[i]);

                unitsOnBattlefield[i] = null;
            }
        }


        //fill realArmy from realArmyReserve
        for(int i = 0; i < playersArmy.Length; i++)
        {
            if(playersArmy[i] != null)
            {
                for(int j = 0; j < unitsInReserveList.Count; j++)
                {
                    UnitController unit = unitsInReserveList[j].GetComponent<UnitController>();
                    if(playersArmy[i].unitType == unit.unit.unitType)
                    {
                        unitsOnBattlefield[i] = unitsInReserveList[j];
                        unitsOnBattlefield[i].SetActive(true);
                        unitsOnBattlefield[i].transform.position = (Vector3)playersArmyPositions[i] + battleArmyController.gameObject.transform.position;
                        unitsOnBattlefield[i].transform.SetParent(battleArmyController.gameObject.transform);

                        unitsInReserveList.Remove(unitsInReserveList[j]);
                        break;
                    }
                }
            }
        }

        playersArmyWindow.CreateReserveScheme(reserveArmy);
        playersArmyWindow.CreateArmyScheme(playersArmy);
    }

    public void UpgradeArmy(UnitsTypes unitType)
    {
        for (int i = 0; i < playersArmy.Length; i++)
        {
            if (playersArmy[i]?.unitType == unitType)
            {
                //playersArmy[i].quantity--;

                if (playersArmy[i].quantity == 0)
                {
                    playersArmy[i] = null;
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
            if(playersArmy[i]?.unitType == unitType)
            {
                playersArmy[i].quantity++;
                playersArmy[i].unitController.UpdateSquad(true);
                isSquadFinded = true;
                break;
            }
        }

        if(isSquadFinded == false)
        {
            TryResurrectUnitFromReserve(unitType);
        }
    }

    private void TryResurrectUnitFromReserve(UnitsTypes unitType)
    {
        bool isSquadFinded = false;

        for(int i = 0; i < reserveArmy.Length; i++)
        {
            if(reserveArmy[i]?.unitType == unitType)
            {
                reserveArmy[i].quantity++;
                reserveArmy[i].unitController.UpdateSquad(true);
                isSquadFinded = true;
                break;
            }
        }

        if(isSquadFinded == false)
        {
            Unit newSquad = unitManager.GetNewSquad(unitType);
            newSquad.quantity = 1;

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
            unitsOnBattlefield[findedIndex] = newUnitGO;

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
            newUnitGO.transform.SetParent(reserveArmyContainer.transform);

            newUnitGO.SetActive(false);
            unitsInReserveList.Add(newUnitGO);

            playersArmyWindow.CreateReserveScheme(reserveArmy);
        }
        else
        {
            Debug.Log("SOMRTHING WRONG WITH RESURRECTION!");
        }
    }

    #endregion

    #region DAMAGE army after defeat/escape

    public void EscapeDamage(float count = -1)
    {
        float commonCountUnits = 0;
        float countToKill;
        float tryToKill = 1000;

        for(int i = 0; i < playersArmy.Length; i++)
        {
            if(playersArmy[i] != null)
            {
                commonCountUnits += playersArmy[i].quantity;
            }
        }

        if(commonCountUnits == 0) return;

        if(count != -1 )
            countToKill = count;
        else
            countToKill = Mathf.Ceil(commonCountUnits * damageArmyByEscape);

        for(int i = 0; i < countToKill; i++)
        {
            KillUnit();
        }

        void KillUnit()
        {
            tryToKill--;
            if(tryToKill == 0)
            {
                Debug.Log("To much tries, sorry...");
                return;
            }

            int randomUnit = UnityEngine.Random.Range(0, playersArmy.Length);
            if(playersArmy[randomUnit] != null)
            {
                playersArmy[randomUnit].unitController.KillOneUnit();
            }
            else
            {
                KillUnit();
            }
        }
    }

    public void DefeatDamage()
    {
        for(int i = 0; i < playersArmy.Length; i++)
        {
            if(playersArmy[i] != null)
            {
                float countToKill = playersArmy[i].quantity;
                for(int j = 0; j < countToKill; j++)
                {
                    playersArmy[i].unitController.KillOneUnit();
                }
            }
        }
    }


    #endregion

    public Unit[] GetPlayersArmyForAutobattle()
    {
        return playersArmy;
    }

    private void OnEnable()
    {
        EventManager.AllUnitsIsReady += InitializeArmy;
        EventManager.WeLostOneUnit += UpgradeArmy;
        //EventManager.SwitchUnit += SwitchUnit;
        EventManager.ResurrectUnit += TryResurrectUnitFromArmy;
    }

    private void OnDisable()
    {
        EventManager.AllUnitsIsReady -= InitializeArmy;
        EventManager.WeLostOneUnit -= UpgradeArmy;
        //EventManager.SwitchUnit -= SwitchUnit; 
        EventManager.ResurrectUnit -= TryResurrectUnitFromArmy;
    }
}
