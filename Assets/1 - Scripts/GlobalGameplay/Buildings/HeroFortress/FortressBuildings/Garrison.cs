using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
    private PlayerStats playerStats;

    public List<HiringAmount> growthAmounts;

    private Dictionary<UnitsTypes, int> potentialAmounts = new Dictionary<UnitsTypes, int>();
    private Dictionary<UnitsTypes, int> currentAmounts = new Dictionary<UnitsTypes, int>();
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
        playerStats = GlobalStorage.instance.playerStats;

        takeWholeSquad.isOn = false;
    }

    public void Init(bool heroMode)
    {
        isHeroInside = heroMode;
        squadMaxAmount = Mathf.RoundToInt(playerStats.GetCurrentParameter(PlayersStats.SquadMaxSize));
        fullPlayerArmy = playersArmy.fullArmy;

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

            if(allowQuantity != currentAmounts[unitType])
            {
                InfotipManager.ShowMessage("Attention! You've reached the maximum squad size.");
            }
            playersArmy.HiringUnits(unitType, allowQuantity);
            currentAmounts[unitType] -= allowQuantity;
        }
        else
        {
            currentAmounts[unitType] += fullPlayerArmy[unitType].unitController.quantity;
            playersArmy.HiringUnits(unitType, -fullPlayerArmy[unitType].unitController.quantity);
        }

        UpdateArmies();
    }

    private void PartExchange(UnitsTypes unitType)
    {
        exchangeBlock.SetActive(true);
        currentUnitForExchange = unitType;
        unitIcon.sprite = fullPlayerArmy[unitType].unit.unitIcon;

        castleAmount.text = currentAmounts[unitType].ToString();
        castleAmountToSet = currentAmounts[unitType];

        heroAmount.text = fullPlayerArmy[unitType].unitController.quantity.ToString();
        heroAmountToSet = fullPlayerArmy[unitType].unitController.quantity;

        exchangeSlider.value = exchangeSlider.maxValue - ((float)castleAmountToSet / (float)(castleAmountToSet + heroAmountToSet));
    }

    //Slider
    public void ChangeAmounts()
    {
        int comnonAmounts = currentAmounts[currentUnitForExchange] + fullPlayerArmy[currentUnitForExchange].unitController.quantity;

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
        currentAmounts[currentUnitForExchange] = castleAmountToSet;
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
            return (currentAmounts[unitType] > difference) ? difference : currentAmounts[unitType];
        }
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
