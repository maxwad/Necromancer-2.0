using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class Military : SpecialBuilding
{
    private UnitManager unitManager;
    [HideInInspector] public HeroFortress fortress;
    private ResourcesManager resourcesManager;
    public Dictionary<ResourceType, Sprite> resourcesIcons;

    [SerializeField] private List<GameObject> unitsRows;

    private int maxUnitLevel;
    [HideInInspector] public CastleBuildings currentBuilding;


    public override GameObject Init(CastleBuildings building)
    {
        currentBuilding = building;
        if(fortress == null)
        {
            unitManager = GlobalStorage.instance.unitManager;
            fortress = GlobalStorage.instance.heroFortress;
            resourcesManager = GlobalStorage.instance.resourcesManager;

            resourcesIcons = resourcesManager.GetAllResourcesIcons();
        }

        ResetForm();
        gameObject.SetActive(true);
        maxUnitLevel = fortress.GetMaxLevel();

        List<UnitsTypes> units = unitManager.GetUnitsByBuildings(building);
        for(int i = 0; i < units.Count; i++)
        {
            GameObject unitsRow = unitsRows[i];
            unitsRow.SetActive(true);
            UnitInMarketUI[] unitsInBuilding = unitsRow.GetComponentsInChildren<UnitInMarketUI>(true);

            for(int j = 0; j < unitsInBuilding.Length; j++)
            {
                Unit unit = unitManager.GetUnitForTip(units[i], j + 1);
                if(unit != null)
                {
                    unitsInBuilding[j].Init(this, unit, j < maxUnitLevel - 1);
                }
            }
        }

        return gameObject;
    }

    private void ResetForm()
    {
        foreach(var row in unitsRows)
        {
            UnitInMarketUI[] unitsInBuilding = row.GetComponentsInChildren<UnitInMarketUI>();
            for(int j = 0; j < unitsInBuilding.Length; j++)
            {
                unitsInBuilding[j].gameObject.SetActive(false);
            }
            row.SetActive(false);
        }
    }

    public bool IsUnitOpen(UnitsTypes unitType, int level)
    {
        return unitManager.IsUnitOpen(unitType, level);
    }

    public void UpgradeUnitLevel(UnitsTypes unitType, int level)
    {
        unitManager.UpgradeUnitLevel(unitType, level);
    }

    public void CloseAllConfirms()
    {
        foreach(var row in unitsRows)
        {
            UnitInMarketUI[] unitsInBuilding = row.GetComponentsInChildren<UnitInMarketUI>();
            for(int j = 0; j < unitsInBuilding.Length; j++)
            {
                unitsInBuilding[j].CloseConfirm();
            }
        }
    }

    public override ISpecialSaveData Save()
    {
        return null;
    }

    public override void Load(List<ISpecialSaveData> saveData)
    {
        
    }
}
