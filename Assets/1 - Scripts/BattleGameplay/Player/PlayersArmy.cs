using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static NameManager;
using System;
using Zenject;

public class PlayersArmy : MonoBehaviour
{
    private UnitManager unitManager;
    private ObjectsPoolManager poolManager;
    private PlayersArmyPart playersArmyWindow;
    private InfirmaryManager infirmary;
    private PlayerStats playerStats;

    private List<UnitsTypes> unitsTypesList;
    public Dictionary<UnitsTypes, FullSquad> fullArmy = new Dictionary<UnitsTypes, FullSquad>();

    private bool isDeadRegistrating = false;
    private Dictionary<UnitsTypes, int> deadUnitsInCurrentBattle = new Dictionary<UnitsTypes, int>();
    private int possibleResurrections = 0;
    private List<UnitsTypes> unitsForResurrectionList = new List<UnitsTypes>();

    [HideInInspector] public Unit[] playersArmy = new Unit[4];   

    [Space]
    [SerializeField] private GameObject reserveArmyContainer;
    [SerializeField] private Vector2[] playersArmyPositions;

    [Space]
    [SerializeField] private BattleArmyController battleArmyController;

    private float damageArmyByEscape = 0.3f;

    [Inject]
    public void Construct
        (
        UnitManager unitManager,
        ObjectsPoolManager poolManager,
        PlayerPersonalWindow playerMilitaryWindow,
        InfirmaryManager infirmary,
        PlayerStats playerStats
        )
    {
        this.unitManager = unitManager;
        this.poolManager = poolManager;
        this.playersArmyWindow = playerMilitaryWindow.GetComponent<PlayersArmyPart>();
        this.infirmary = infirmary;
        this.playerStats = playerStats;
    }

    private void  InitializeArmy()
    {
        unitsTypesList = unitManager.GetUnitsTypesList();

        foreach(var squad in unitsTypesList)
        {
            FullSquad newSquad = new FullSquad();
            fullArmy.Add(squad, newSquad);

            newSquad.unit = unitManager.GetNewSquad(squad);

            newSquad.unitGO = poolManager.GetUnusualPrefab(newSquad.unit.unitGO);
            newSquad.unitGO.transform.SetParent(reserveArmyContainer.transform);

            newSquad.unitController = newSquad.unitGO.GetComponent<UnitController>();
            newSquad.unitController.Initilize(newSquad.unit);
            newSquad.unitController.AddQuantity(UnityEngine.Random.Range(5, 10));

            newSquad.unitGO.GetComponentInChildren<TMP_Text>().text = newSquad.unitController.quantity.ToString();

            GameObject uiSlot = poolManager.GetObject(ObjectPool.SquadSlot);
            newSquad.squadUI = uiSlot.GetComponent<ArmySlot>();
            newSquad.squadUI.Init(newSquad.unit);
            uiSlot.SetActive(true);

            newSquad.unitGO.SetActive(false);

        }

        playersArmyWindow.UpdateArmyWindow();

        //end of loading Units and Army
        GlobalStorage.instance.LoadNextPart();
    }

    public bool UpgradeSquad(UnitsTypes type, bool newLevelMode = true)
    {
        int level = (newLevelMode == true) ? (fullArmy[type].unit.level + 1) : 1;
        if(level == fullArmy[type].unit.level) return false;

        Unit upgradedUnit = unitManager.GetNewSquad(type, level);

        if(upgradedUnit == null) return false;

        fullArmy[type].unit = upgradedUnit;
        fullArmy[type].unitController.Initilize(fullArmy[type].unit);
        fullArmy[type].unitGO.GetComponentInChildren<TMP_Text>().text = fullArmy[type].unitController.quantity.ToString();
        fullArmy[type].squadUI.Init(fullArmy[type].unit);

        for(int i = 0; i < playersArmy.Length; i++)
        {
            if(playersArmy[i] != null && fullArmy[type].unit.unitType == playersArmy[i].unitType)
            {
                fullArmy[type].unit.status = UnitStatus.Army;
                playersArmy[i] = fullArmy[type].unit;
                break;
            }
        }

        return true;
    }

    public bool CheckNextUnitLevel(UnitsTypes type)
    {
        int level = fullArmy[type].unit.level + 1;
        Unit upgradedUnit = unitManager.GetNewSquad(type, level);

        return (upgradedUnit != null);
    }

    private void ResetArmy()
    {
        foreach(var squad in fullArmy)
            UpgradeSquad(squad.Value.unit.unitType, false);
    }

    public void UpdateArmy()
    {
        for(int i = 0; i < playersArmy.Length; i++)
            playersArmy[i] = null;

        foreach(var item in playersArmyWindow.armySlots)
        {
            if(item.squad != null)
                playersArmy[item.index] = fullArmy[item.squad.unitInSlot.unitType].unit;
        }
       
        if(GlobalStorage.instance.isGlobalMode == false)
        {
            ShowSquadsOnBattleField(false);
        }
    }

    public void ShowSquadsOnBattleField(bool mode)
    {
        foreach(var squad in fullArmy)
        {
            squad.Value.unitGO.transform.SetParent(reserveArmyContainer.transform);
            squad.Value.unitGO.SetActive(false);
        }

        if(mode == false)
        {
            for(int i = 0; i < playersArmy.Length; i++)
            {
                if(playersArmy[i] != null && playersArmy[i].isUnitActive == true)
                {
                    GameObject unit = fullArmy[playersArmy[i].unitType].unitGO;
                    unit.transform.SetParent(battleArmyController.gameObject.transform);
                    unit.transform.position = (Vector3)playersArmyPositions[i] + battleArmyController.gameObject.transform.position;
                    unit.SetActive(true);
                }
            }
        }
        else
        {
            ResetArmy();
        }
    }

    public void HiringUnits(UnitsTypes unitType, int quantity, bool setMode = false)
    {
        if(setMode == true)
        {
            fullArmy[unitType].unitController.quantity = quantity;
        }
        else
        {
            fullArmy[unitType].unitController.quantity += quantity;
        }

        if(fullArmy[unitType].unitController.quantity > 0)
        {
            fullArmy[unitType].unit.isUnitActive = true;
            fullArmy[unitType].squadUI.gameObject.SetActive(true);
        }
        else
        {
            fullArmy[unitType].unitController.quantity = 0;
            fullArmy[unitType].unit.isUnitActive = false;
            fullArmy[unitType].squadUI.gameObject.SetActive(false);            
        }

        ShowSquadsOnBattleField(false);
    }

    public void ResurrectionFewUnitInTheBattle(float quantity)
    {
        Dictionary<UnitsTypes, int> tempRegistrationDict = new Dictionary<UnitsTypes, int>();
        int counter = 0;

        for(int i = 0; i < quantity; i++)
        {
            if(unitsForResurrectionList.Count == 0) {                
                break;
            }
            else
            {
                UnitsTypes unitType = unitsForResurrectionList[UnityEngine.Random.Range(0, unitsForResurrectionList.Count)];

                if(fullArmy[unitType].unitController.quantity >= playerStats.GetCurrentParameter(PlayersStats.SquadMaxSize))
                    continue;

                ResurrectionUnit(unitType);
                counter++;

                if(tempRegistrationDict.ContainsKey(unitType) == true)
                    tempRegistrationDict[unitType]++;
                else
                    tempRegistrationDict.Add(unitType, 1);
            }
        }

        if(counter == 0)
            InfotipManager.ShowWarning("You have no units that you can resurrect.");
        else
        {
            foreach(var squad in tempRegistrationDict)
            {
                fullArmy[squad.Key].unitController.ShowEffectRessurection(squad.Value);
            }
        }
    }

    public void ResurrectionUnit(UnitsTypes unitType)
    {
        fullArmy[unitType].unitController.AddQuantity(1);

        if(fullArmy[unitType].unitController.quantity == 1)
        {
            UpgradeSquad(unitType, false);
            fullArmy[unitType].unit.isUnitActive = true;
            fullArmy[unitType].squadUI.gameObject.SetActive(true);
            ShowSquadsOnBattleField(false);
        }

        DeleteDeadUnit(unitType);
        infirmary.RemoveUnitFromInfirmary(unitType);
    }

    public void SquadLost(UnitsTypes unitType)
    {
        if(fullArmy[unitType].unitController.quantity <= 0) return;
        fullArmy[unitType].unitController.AddQuantity(-1);
        fullArmy[unitType].unitController.ShowEffectDeath();

        if(fullArmy[unitType].unitController.quantity == 0)
        {
            fullArmy[unitType].unit.isUnitActive = false;
            fullArmy[unitType].squadUI.gameObject.SetActive(false);
            fullArmy[unitType].unitController.Dead();
            ShowSquadsOnBattleField(false);
        }

        RegisterDeadUnit(unitType);
        infirmary.AddUnitToInfirmary(unitType);
    }

    private void RegisterDeadUnit(UnitsTypes unitType)
    {
        if(isDeadRegistrating == true)
        {
            if(deadUnitsInCurrentBattle.ContainsKey(unitType) == true)
                deadUnitsInCurrentBattle[unitType]++;
            else
            {
                deadUnitsInCurrentBattle.Add(unitType, 1);                
            }

            if(unitsForResurrectionList.Count < possibleResurrections)
            {
                unitsForResurrectionList.Add(unitType);
            }
        }
    }

    private void DeleteDeadUnit(UnitsTypes unitType)
    {
        if(isDeadRegistrating == true)
        {
            unitsForResurrectionList.Remove(unitType);
            if(deadUnitsInCurrentBattle.ContainsKey(unitType) == true)
            {
                deadUnitsInCurrentBattle[unitType]--;

                if(deadUnitsInCurrentBattle[unitType] == 0)
                {
                    deadUnitsInCurrentBattle.Remove(unitType);
                }                    
            }
        }
    }

    public void StopControlUnitDeath(bool mode)
    {
        isDeadRegistrating = !mode;
    }

    public void StopControlUnitDeathByEvent(bool mode)
    {
        if(mode == false)
        {
            isDeadRegistrating = true;
            possibleResurrections = infirmary.GetEmptySpaces();
        }
    }

    public void ClearDeadUnits()
    {
        deadUnitsInCurrentBattle.Clear();
        unitsForResurrectionList.Clear();
    }

    public Dictionary<UnitsTypes, int> GetDeadUnits() => deadUnitsInCurrentBattle;

    #region DAMAGE army after defeat/escape

    public void EscapeDamage(float count = -1)
    {
        int amount = 0;
        List<UnitsTypes> unitForKillingList = new List<UnitsTypes>();
        float countToKill;

        foreach(var squad in fullArmy)
        {
            if(squad.Value.unit.status == UnitStatus.Army && squad.Value.unit.isUnitActive == true)
            {
                if(squad.Value.unitController.quantity != 0)
                {
                    amount += squad.Value.unitController.quantity;
                    unitForKillingList.Add(squad.Value.unit.unitType);
                }
            }
        }

        if(amount == 0) return;

        if(count == -1)
            countToKill = Mathf.Ceil(amount * damageArmyByEscape);
        else if(count == -2)
            countToKill = amount;
        else
            countToKill = count;

        for(int i = 0; i < countToKill; i++)
        {
            KillUnit();
        }

        void KillUnit()
        {
            UnitsTypes unitType = unitForKillingList[UnityEngine.Random.Range(0, unitForKillingList.Count)];
            SquadLost(unitType);

            if(fullArmy[unitType].unitController.quantity == 0)
                unitForKillingList.Remove(unitType);            
        }
    }

    public void DefeatDamage()
    {
        EscapeDamage(-2);        
    }

    #endregion

    public Unit[] GetPlayersArmyForAutobattle() => playersArmy;

    #region SAVE/LOAD

    public PlayersArmySD Save()
    {
        PlayersArmySD saveData = new PlayersArmySD();

        foreach(var squad in fullArmy)
        {
            PlayersArmySquadInfoSD squadData = new PlayersArmySquadInfoSD();
            squadData.unit = squad.Key;
            squadData.status = squad.Value.unit.status;
            squadData.quantity = squad.Value.unitController.quantity;

            saveData.wholeArmy.Add(squadData);
        }

        for(int i = 0; i < playersArmy.Length; i++)
        {
            if(playersArmy[i] != null)
                saveData.activeArmy[i] = (int)playersArmy[i].unitType;
        }

        return saveData;
    }

    public void Load(PlayersArmySD saveData)
    {

        foreach(var squad in saveData.wholeArmy)
        {
            fullArmy[squad.unit].unit.status = squad.status;
            fullArmy[squad.unit].unitController.quantity = squad.quantity;
        }

        playersArmyWindow.CreateReserveScheme(fullArmy);

        for(int i = 0; i < saveData.activeArmy.Length; i++)
        {
            if(saveData.activeArmy[i] != -1)
                playersArmyWindow.LoadUnit(fullArmy[(UnitsTypes)saveData.activeArmy[i]], i);
        }
    }

    #endregion

    private void OnEnable()
    {
        EventManager.AllUnitsIsReady += InitializeArmy;
        EventManager.SwitchPlayer += ShowSquadsOnBattleField;
        EventManager.SwitchPlayer += StopControlUnitDeathByEvent;
    }

    private void OnDisable()
    {
        EventManager.AllUnitsIsReady -= InitializeArmy;
        EventManager.SwitchPlayer -= ShowSquadsOnBattleField;
        EventManager.SwitchPlayer -= StopControlUnitDeathByEvent;
    }
}
