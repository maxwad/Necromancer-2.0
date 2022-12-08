using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static NameManager;

[Serializable]
public class HiringAmount
{
    public CastleBuildings building;
    public int amount;
}

public class Garrison : MonoBehaviour
{
    private UnitManager unitManager;
    private FortressBuildings fortressBuildings;
    private PlayersArmy playersArmy;

    public List<HiringAmount> growthAmounts;

    private Dictionary<UnitsTypes, int> potentialAmounts = new Dictionary<UnitsTypes, int>();
    private Dictionary<UnitsTypes, int> currentAmounts = new Dictionary<UnitsTypes, int>();

    private bool isHeroInside = false;
    [SerializeField] private GameObject heroIcon;

    [SerializeField] private List<CastleSquadSlot> castleArmy;
    [SerializeField] private List<CastleSquadSlot> heroArmy;
    [SerializeField] private Toggle takeWholeSquad;


    private void Awake()
    {
        foreach(UnitsTypes item in Enum.GetValues(typeof(UnitsTypes)))
            potentialAmounts.Add(item, 0);

        foreach(UnitsTypes item in Enum.GetValues(typeof(UnitsTypes)))
            currentAmounts.Add(item, 0);
    }

    private void Start()
    {
        unitManager = GlobalStorage.instance.unitManager;
        fortressBuildings = GlobalStorage.instance.fortressBuildings;
        playersArmy = GlobalStorage.instance.playersArmy;

        takeWholeSquad.isOn = false;
    }

    public void Init(bool heroMode)
    {
        isHeroInside = heroMode;

        UpdateArmies();
    }

    #region FILLING 
    private void UpdateArmies()
    {
        FillCastleArmy();

        FillHerosArmy(isHeroInside);
    }

    private void FillCastleArmy()
    {
        foreach(var squad in castleArmy)
            squad.Deactivate();

        int squadIndex = 0;
        foreach(var squad in currentAmounts)
        {
            if(squad.Value != 0)
            {
                Unit unit = unitManager.GetUnitForTip(squad.Key);
                castleArmy[squadIndex].Init(this, unit, squad.Value);
                squadIndex++;
            }
        }        
    }

    private void FillHerosArmy(bool heroMode)
    {
        foreach(var squad in heroArmy)
            squad.Deactivate();

        heroIcon.SetActive(heroMode);
        takeWholeSquad.gameObject.SetActive(heroMode);

        if(heroMode == true)
        {
            Dictionary<UnitsTypes, FullSquad> fullArmy = playersArmy.fullArmy;

            int squadIndex = 0;
            foreach(var squad in fullArmy)
            {
                if(squad.Value.unitController.quantity != 0)
                {
                    Unit unit = unitManager.GetUnitForTip(squad.Key);
                    heroArmy[squadIndex].Init(this, unit, squad.Value.unitController.quantity);
                    squadIndex++;
                }
            }
        }
    }

    #endregion

    #region EXCHANGE

    public void StartExchange(bool isCastlesSquad, UnitsTypes unitType)
    {
        if(isHeroInside == false) return;

        Dictionary<UnitsTypes, FullSquad> fullArmy = playersArmy.fullArmy;

        if(takeWholeSquad.isOn == true)
        {
            if(isCastlesSquad == true)
            {
                playersArmy.HiringUnits(unitType, currentAmounts[unitType]);
                currentAmounts[unitType] = 0;
            }
            else
            {
                playersArmy.HiringUnits(unitType, -fullArmy[unitType].unitController.quantity);
                currentAmounts[unitType] += fullArmy[unitType].unitController.quantity;
            }
        }

        UpdateArmies();
    }

    private void ToCastleArmy()
    {

    }

    private void ToHeroArmy()
    {

    }

    #endregion

    #region GETTINGS

    public int GetHiringGrowth(CastleBuildings building)
    {
        int amount = 0;
        for(int i = 0; i < growthAmounts.Count; i++)
        {
            if(growthAmounts[i].building == building)
            {
                amount = growthAmounts[i].amount;
                break;
            }
        }

        amount += (int)fortressBuildings.GetBonusAmount(CastleBuildings.RecruitmentCenter);

        return amount;
    }

    public int GetHiringAmount(UnitsTypes unitType)
    {
        return potentialAmounts[unitType];
    }

    public void ChangePotentialUnitsAmount(UnitsTypes unit, int amount)
    {
        potentialAmounts[unit] += amount;

        if(potentialAmounts[unit] < 0) potentialAmounts[unit] = 0;
    }

    public void HiringUnits(UnitsTypes unit, int amount)
    {
        if(potentialAmounts[unit] >= amount)
        {
            potentialAmounts[unit] -= amount;
            currentAmounts[unit] += amount;

            UpdateArmies();
        }
    }

    #endregion
}
