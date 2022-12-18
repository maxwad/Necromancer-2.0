using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class RBGarrisonUI : MonoBehaviour, IGarrison
{
    private UnitManager unitManager;
    private PlayersArmy playersArmy;
    private PlayerStats playerStats;
    private ResourceBuilding resourceBuilding;

    private Dictionary<UnitsTypes, int> currentAmounts = new Dictionary<UnitsTypes, int>();
    private int squadMaxAmount;
    Dictionary<UnitsTypes, FullSquad> fullPlayerArmy;

    private bool isHeroInside = false;
    [SerializeField] private GameObject heroIcon;

    [SerializeField] private List<CastleSquadSlot> buildingArmy;
    [SerializeField] private List<CastleSquadSlot> heroArmy;
    [SerializeField] private Toggle takeWholeSquad;

    [SerializeField] private GameObject exchangeBlock;
    [SerializeField] private Slider exchangeSlider;
    [SerializeField] private TMP_Text heroAmount;
    [SerializeField] private TMP_Text buildingAmount;
    [SerializeField] private Image unitIcon;
    private UnitsTypes currentUnitForExchange;
    private int heroAmountToSet = 0;
    private int castleAmountToSet = 0;


    private void Awake()
    {
        foreach(UnitsTypes item in Enum.GetValues(typeof(UnitsTypes)))
            currentAmounts.Add(item, 0);
    }

    private void Start()
    {
        unitManager = GlobalStorage.instance.unitManager;
        playersArmy = GlobalStorage.instance.playersArmy;
        playerStats = GlobalStorage.instance.playerStats;
        resourceBuilding = GetComponent<ResourceBuilding>();

        takeWholeSquad.isOn = false;
    }

    private void OnEnable()
    {

    }

    public void Init(bool heroMode)
    {
        Debug.Log("Init RBG");
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
        foreach(var squad in buildingArmy)
            squad.Deactivate();

        int squadIndex = 0;
        foreach(var squad in currentAmounts)
        {
            if(squad.Value != 0)
            {
                Unit unit = unitManager.GetUnitForTip(squad.Key);
                buildingArmy[squadIndex].Init(this, unit, squad.Value);
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

        buildingAmount.text = currentAmounts[unitType].ToString();
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

        buildingAmount.text = castleAmountToSet.ToString();
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
}
