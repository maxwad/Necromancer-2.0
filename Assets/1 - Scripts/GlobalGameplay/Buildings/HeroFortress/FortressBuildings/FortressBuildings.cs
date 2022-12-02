using System;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class FortressBuildings : MonoBehaviour
{
    private HeroFortress fortress;

    public int maxLevel = 3;
    public Dictionary<CastleBuildings, int> buildingsLevels = new Dictionary<CastleBuildings, int>();
    public Dictionary<CastleBuildings, FBuilding> allBuildings = new Dictionary<CastleBuildings, FBuilding>();
    public List<FortressUpgradeSO> upgradesList;


    private void Awake()
    {
        fortress = GetComponent<HeroFortress>();

        foreach(CastleBuildings item in Enum.GetValues(typeof(CastleBuildings)))
            buildingsLevels.Add(item, 0);
    }

    public void RegisterBuilding(CastleBuildings building, FBuilding buildingComponent)
    {
        if(allBuildings.ContainsKey(building) == false)
            allBuildings.Add(building, buildingComponent);
    }


    public void UpgradeBuilding(CastleBuildings building, int newLevel)
    {
        if(newLevel <= maxLevel && newLevel >= 0)
            buildingsLevels[building] = newLevel;

        UpgradeFortressLevel();
        if(newLevel == 0) allBuildings.Remove(building);
    } 

    public FortressUpgradeSO GetBuildingBonus(CastleBuildings building, int checkLevel = 0)
    {
        FortressUpgradeSO bonus = null;
        int level = (checkLevel == 0) ? buildingsLevels[building] : checkLevel;
        foreach(var item in upgradesList)
        {
            if(item.building == building && item.level == level)
            {
                bonus = item;
                break;
            }
        }

        return bonus;
    }

    public float GetBonusAmount(CastleBuildings building)
    {
        float bonus = 0;
        int level = buildingsLevels[building];
        foreach(var item in upgradesList)
        {
            if(item.building == building && item.level == level)
            {
                bonus = (item.isInverted == true) ? -item.value : item.value;
                bonus = (item.percentValueType == true) ? bonus * 0.01f : bonus;
                break;
            }
        }

        return bonus;
    }

    public List<Cost> GetCost(CastleBuildings building)
    {
        float discount = GetBonusAmount(CastleBuildings.Manufactory);

        List<Cost> defaultCosts = new List<Cost>();

        int level = buildingsLevels[building];
        foreach(var item in upgradesList)
        {
            if(item.building == building && item.level == level + 1)
            {
                defaultCosts = item.cost;
                break;
            }
        }

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

    public void UpgradeFortressLevel()
    {
        int level = 0;
        foreach(var item in buildingsLevels)
            level += item.Value;

        fortress.UpgradeFortressLevel(level);
    }
}
