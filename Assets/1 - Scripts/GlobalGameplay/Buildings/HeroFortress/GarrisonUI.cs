using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class GarrisonUI : MonoBehaviour, IGarrison
{
    private UnitManager unitManager;
    private PlayersArmy playersArmy;
    private PlayerStats playerStats;
    private Garrison garrison;

    public List<HiringAmount> growthAmounts;

    private Dictionary<UnitsTypes, int> currentAmounts;
    private int squadMaxAmount;
    Dictionary<UnitsTypes, FullSquad> fullPlayerArmy;

    private bool isHeroInside = false;
    [SerializeField] private GameObject heroIcon;

    [SerializeField] private List<CastleSquadSlot> castleArmy;
    [SerializeField] private List<CastleSquadSlot> heroArmy;
    [SerializeField] private Toggle takeWholeSquad;

    [SerializeField] private GameObject exchangeBlock;
    [SerializeField] private Slider exchangeSlider;
    [SerializeField] private TMP_Text heroAmount;
    [SerializeField] private TMP_Text castleAmount;
    [SerializeField] private Image unitIcon;
    private UnitsTypes currentUnitForExchange;
    private int heroAmountToSet = 0;
    private int castleAmountToSet = 0;

    public void Init(bool heroMode, Garrison gar)
    {
        if(playerStats == null)
        {
            unitManager = GlobalStorage.instance.unitManager;
            playersArmy = GlobalStorage.instance.playersArmy;
            playerStats = GlobalStorage.instance.playerStats;

            takeWholeSquad.isOn = true;
        }

        isHeroInside = heroMode;
        squadMaxAmount = Mathf.RoundToInt(playerStats.GetCurrentParameter(PlayersStats.SquadMaxSize));
        fullPlayerArmy = playersArmy.fullArmy;
        garrison = gar;

        UpdateArmies();
    }

    #region FILLING 
    public void UpdateArmies()
    {
        FillCastleArmy();

        FillHerosArmy(isHeroInside);
    }

    private void FillCastleArmy()
    {
        foreach(var squad in castleArmy)
            squad.Deactivate();

        currentAmounts = garrison.GetUnitsInGarrison();

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

        if(takeWholeSquad.isOn == true)
        {
            WholeExchange(isCastlesSquad, unitType);
        }
        else
        {
            PartExchange(unitType);
        }
    }

    private void WholeExchange(bool isCastlesSquad, UnitsTypes unitType)
    {
        if(isCastlesSquad == true)
        {
            int allowQuantity = CheckOverflow(unitType);

            if(allowQuantity != garrison.GetUnitAmount(unitType))
            {
                InfotipManager.ShowMessage("Attention! You've reached the maximum squad size.");
            }
            playersArmy.HiringUnits(unitType, allowQuantity);
            garrison.AddUnits(unitType, -allowQuantity);
        }
        else
        {
            garrison.AddUnits(unitType, fullPlayerArmy[unitType].unitController.quantity);
            playersArmy.HiringUnits(unitType, -fullPlayerArmy[unitType].unitController.quantity);
        }

        UpdateArmies();
    }

    private void PartExchange(UnitsTypes unitType)
    {
        exchangeBlock.SetActive(true);
        currentUnitForExchange = unitType;
        unitIcon.sprite = fullPlayerArmy[unitType].unit.unitIcon;

        castleAmount.text = garrison.GetUnitAmount(unitType).ToString();
        castleAmountToSet = garrison.GetUnitAmount(unitType);

        heroAmount.text = fullPlayerArmy[unitType].unitController.quantity.ToString();
        heroAmountToSet = fullPlayerArmy[unitType].unitController.quantity;

        exchangeSlider.value = exchangeSlider.maxValue - ((float)castleAmountToSet / (float)(castleAmountToSet + heroAmountToSet));
    }

    //Slider
    public void ChangeAmounts()
    {
        int comnonAmounts = garrison.GetUnitAmount(currentUnitForExchange) + fullPlayerArmy[currentUnitForExchange].unitController.quantity;

        castleAmountToSet = Mathf.RoundToInt((exchangeSlider.maxValue - exchangeSlider.value) * comnonAmounts);
        heroAmountToSet = comnonAmounts - castleAmountToSet;

        if(heroAmountToSet > squadMaxAmount)
        {
            InfotipManager.ShowMessage("Attention! You've reached the maximum squad size.");
            heroAmountToSet = squadMaxAmount;
            castleAmountToSet = comnonAmounts - heroAmountToSet;
            exchangeSlider.value = exchangeSlider.maxValue - (float)castleAmountToSet / (float)(castleAmountToSet + heroAmountToSet);
        }

        castleAmount.text = castleAmountToSet.ToString();
        heroAmount.text = heroAmountToSet.ToString();
    }

    //Button
    public void Cancel()
    {
        exchangeBlock.SetActive(false);
    }

    //Button
    public void Deal()
    {
        garrison.AddUnits(currentUnitForExchange, castleAmountToSet, false);
        playersArmy.HiringUnits(currentUnitForExchange, heroAmountToSet, true);

        UpdateArmies();
        Cancel();
    }

    private int CheckOverflow(UnitsTypes unitType)
    {
        if(fullPlayerArmy[unitType].unitController.quantity >= squadMaxAmount)
        {
            return 0;
        }
        else
        {
            int difference = squadMaxAmount - fullPlayerArmy[unitType].unitController.quantity;
            int amount = garrison.GetUnitAmount(unitType);
            return (amount > difference) ? difference : amount;
        }
    }

    #endregion
}
