using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static NameManager;

public class ConstructionTime
{
    public int originalTerm;
    public int term;
    public int daysLeft;
}

public class FortressBuildings : MonoBehaviour
{
    [SerializeField] private TMP_Text fortressLevelText;
    private int fortressLevel = 0;

    public int maxLevel = 3;
    public int destroyBuildings = 3;

    [SerializeField] private List<GameObject> buildingsGOList;
    private Dictionary<CastleBuildings, FBuilding> buildingsComponentDict = new Dictionary<CastleBuildings, FBuilding>();

    public Dictionary<CastleBuildings, int> buildingsLevels = new Dictionary<CastleBuildings, int>();
    public Dictionary<CastleBuildings, FBuilding> allReadyBuildings = new Dictionary<CastleBuildings, FBuilding>();

    public List<FortressUpgradeSO> upgradesList;
    public Dictionary<CastleBuildings, List<FortressUpgradeSO>> upgradesDict = new Dictionary<CastleBuildings, List<FortressUpgradeSO>>();

    public Dictionary<CastleBuildingsBonuses, float> buildingsBonuses = new Dictionary<CastleBuildingsBonuses, float>();

    private Dictionary<CastleBuildings, ConstructionTime> buildingsInProgress = new Dictionary<CastleBuildings, ConstructionTime>();
    private int maxCountOfProgress = 1;

    private void Awake()
    {
        foreach(var item in buildingsGOList)
        {
            FBuilding buildingComponent = item.GetComponentInChildren<FBuilding>(true);
            buildingsComponentDict.Add(buildingComponent.building, buildingComponent);
        }

        foreach(CastleBuildings item in Enum.GetValues(typeof(CastleBuildings)))
            buildingsLevels.Add(item, 0);

        foreach(CastleBuildingsBonuses item in Enum.GetValues(typeof(CastleBuildingsBonuses)))
            buildingsBonuses.Add(item, 0);

        foreach(CastleBuildings item in Enum.GetValues(typeof(CastleBuildings)))
        {
            List<FortressUpgradeSO> upgrades = new List<FortressUpgradeSO>();
            for(int i = 0; i < upgradesList.Count; i++)
            {
                if(upgradesList[i].building == item)
                {
                    upgrades.Add(upgradesList[i]);
                }
            }

            upgradesDict.Add(item, upgrades);
        }

        UpgradeFortressLevel();
    }

    public void Test()
    {
        foreach(var bonus in buildingsBonuses)
        {
            Debug.Log(bonus.Key + " = " + bonus.Value);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Delete))
        {
            DestroyBuildings();
        }
    }

    #region UPGRADE ACTIONS

    public void ShowAllBuildings(bool showMode)
    {
        foreach(var building in buildingsGOList)
        {
            building.SetActive(showMode);
        }
    }

    public void RegisterBuilding(CastleBuildings building, FBuilding buildingComponent)
    {
        if(allReadyBuildings.ContainsKey(building) == false)
            allReadyBuildings.Add(building, buildingComponent);
    }

    public void UpgradeBuilding(CastleBuildings building, int newLevel, CastleBuildingsBonuses buildingBonus)
    {
        if(newLevel <= maxLevel && newLevel >= 0)
            buildingsLevels[building] = newLevel;

        UpgradeFortressLevel();
        if(newLevel == 0) allReadyBuildings.Remove(building);

        buildingsInProgress.Remove(building);
        SetBonus(building, buildingBonus);
    }

    public void UpgradeFortressLevel()
    {
        int level = 0;
        foreach(var item in buildingsLevels)
            level += item.Value;

        fortressLevel = level;

        fortressLevelText.text = level.ToString();
    }

    public void DestroyBuildings()
    {
        for(int i = 0; i < destroyBuildings; i++)
        {
            List<CastleBuildings> buildings = new List<CastleBuildings>();

            foreach(var buildingItem in buildingsLevels)
            {
                if(buildingItem.Value > 0)
                {
                    buildings.Add(buildingItem.Key);
                }
            }

            if(buildings.Count == 0)
            {
                Debug.Log("GAMEOVER");
                break;
            }
            else
            {
                CastleBuildings randomBuilding = buildings[UnityEngine.Random.Range(0, buildings.Count)];
                FBuilding building = allReadyBuildings[randomBuilding];
                building.Downgrade();
            }
        }
    }

    #endregion

    #region GETTINGS

    public FortressUpgradeSO GetBuildingBonus(CastleBuildings building, int checkLevel = 0)
    {
        int level = (checkLevel == 0) ? 0 : checkLevel;
        FortressUpgradeSO bonus = upgradesDict[building][level];

        //foreach(var item in upgradesList)
        //{
        //    if(item.building == building && item.level == level)
        //    {
        //        bonus = item;
        //        break;
        //    }
        //}

        return bonus;
    }

    private void SetBonus(CastleBuildings building, CastleBuildingsBonuses buildingBonus)
    {
        float bonusAmount;

        FortressUpgradeSO bonus = upgradesDict[building][buildingsLevels[building] - 1];
        bonusAmount = (bonus.isInverted == true) ? -bonus.value : bonus.value;
        bonusAmount = (bonus.percentValueType == true) ? bonusAmount * 0.01f : bonusAmount;

        //if(buildingsLevels[building] != 0)
        //{
        //    FortressUpgradeSO bonus = upgradesDict[building][buildingsLevels[building]];
        //    bonusAmount = (bonus.isInverted == true) ? -bonus.value : bonus.value;
        //    bonusAmount = (bonus.percentValueType == true) ? bonusAmount * 0.01f : bonusAmount;
        //}
        //else
        //{
        //    bonusAmount = 0;
        //}

        buildingsBonuses[buildingBonus] = bonusAmount;

        Test();
    }

    public float GetBonusAmount(CastleBuildingsBonuses buildingBonus)
    {
        float bonus = buildingsBonuses[buildingBonus];

        //int level = buildingsLevels[building];

        //FortressUpgradeSO item = upgradesDict[building][level];

        //bonus = (item.isInverted == true) ? -item.value : item.value;
        //bonus = (item.percentValueType == true) ? bonus * 0.01f : bonus;

        //foreach(var item in upgradesList)
        //{
        //    if(item.building == building && item.level == level)
        //    {
        //        bonus = (item.isInverted == true) ? -item.value : item.value;
        //        bonus = (item.percentValueType == true) ? bonus * 0.01f : bonus;
        //        break;
        //    }
        //}



        return bonus;
    }

    public List<Cost> GetBuildingCost(CastleBuildings building)
    {
        float discount = GetBonusAmount(CastleBuildingsBonuses.BuildingDiscount);

        List<Cost> defaultCosts = new List<Cost>();

        int level = buildingsLevels[building];
        defaultCosts = upgradesDict[building][level].cost;

        //foreach(var item in upgradesList)
        //{
        //    if(item.building == building && item.level == level + 1)
        //    {
        //        defaultCosts = item.cost;
        //        break;
        //    }
        //}

        List<Cost> costs = new List<Cost>();
        for(int i = 0; i < defaultCosts.Count; i++)
        {
            Cost costItem = new Cost();

            costItem.type = defaultCosts[i].type;
            costItem.amount = Mathf.Round(defaultCosts[i].amount + defaultCosts[i].amount * discount) * (level + 1);
            costs.Add(costItem);
        }

        return costs;
    }

    public int GetMaxLevel()
    {
        return maxLevel;
    }

    public List<CastleBuildings> GetMilitary()
    {
        List<CastleBuildings> buildings = new List<CastleBuildings>();

        foreach(var building in allReadyBuildings)
        {
            if(building.Value.isMilitarySource == true)
            {
                if(buildingsLevels[building.Key] != 0)
                {
                    buildings.Add(building.Key);
                }
            }
        }
        return buildings;
    }

    //public int GetConstructionTime(CastleBuildings building) 
    //{
    //    int maxDays = 0;
    //    int level = buildingsLevels[building];

    //    maxDays = upgradesDict[building][buildingsLevels[building] + 1].constructuionTime;

    //    //foreach(var item in upgradesList)
    //    //{
    //    //    if(item.building == building && item.level == (buildingsLevels[building] + 1))
    //    //    {
    //    //        neededLevel = item.fortressLevel;
    //    //        break;
    //    //    }
    //    //}
    //}

    #endregion

    #region BUILDING PROCESS

    public bool CheckNeededLevel(CastleBuildings building)
    {
        int neededLevel = upgradesDict[building][buildingsLevels[building]].fortressLevel;

        Debug.Log("You need level: " + neededLevel);

        //foreach(var item in upgradesList)
        //{
        //    if(item.building == building && item.level == (buildingsLevels[building] + 1))
        //    {
        //        neededLevel = item.fortressLevel;
        //        break;
        //    }
        //}

        return fortressLevel >= neededLevel;
    }

    public bool CanIBuild()
    {
        return buildingsInProgress.Count < maxCountOfProgress;
    }

    public ConstructionTime StartBuildingBuilding(CastleBuildings building)
    {
        //if(buildingsInProgress.Count >= maxCountOfProgress) return null;

        ConstructionTime constructionTime = new ConstructionTime();

        float discountDays;
        int originalDays = upgradesDict[building][buildingsLevels[building]].constructuionTime;
        constructionTime.originalTerm = originalDays;

        //foreach(var item in upgradesList)
        //{
        //    if(item.building == building && item.level == (buildingsLevels[building] + 1))
        //    {
        //        originalDays = (int)item.constructuionTime;
        //        break;
        //    }
        //}

        if(buildingsLevels[CastleBuildings.Manufactory] == 0)
        {
            discountDays = 0f;
            maxCountOfProgress = 1;
        }
        else
        {
            discountDays = buildingsBonuses[CastleBuildingsBonuses.BuildingTime];

            //foreach(var item in upgradesList)
            //{
            //    if(item.building == CastleBuildings.Manufactory && item.level == buildingsLevels[CastleBuildings.Manufactory])
            //    {
            //        discountDays = (int)-item.value;
            //        break;
            //    }
            //}
            maxCountOfProgress = buildingsLevels[CastleBuildings.Manufactory];
        }
        constructionTime.term = originalDays + Mathf.FloorToInt(originalDays * discountDays);
        constructionTime.daysLeft = constructionTime.term;

        buildingsInProgress.Add(building, constructionTime);

        return constructionTime;
    }

    #endregion

    private void NewDay()
    {
        List<CastleBuildings> tempList = new List<CastleBuildings>(buildingsInProgress.Keys);
        foreach(var item in tempList)
        {
            buildingsInProgress[item].daysLeft--;
            buildingsComponentDict[item].UpdateBuildingProcess(buildingsInProgress[item]);
        }
    }

    private void OnEnable()
    {
        EventManager.NewMove += NewDay;
    }

    private void OnDisable()
    {
        EventManager.NewMove -= NewDay;
    }
}
