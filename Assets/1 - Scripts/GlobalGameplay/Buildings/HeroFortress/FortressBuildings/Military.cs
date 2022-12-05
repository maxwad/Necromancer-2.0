using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class Military : MonoBehaviour
{
    private UnitManager unitManager;
    [HideInInspector] public HeroFortress fortress;
    private FortressBuildings allBuildings;
    private ResourcesManager resourcesManager;
    public Dictionary<ResourceType, Sprite> resourcesIcons;

    [SerializeField] private List<GameObject> unitsRows;

    private int maxUnitLevel;



    public void Init(CastleBuildings building)
    {
        if(fortress == null)
        {
            unitManager = GlobalStorage.instance.unitManager;
            fortress = GlobalStorage.instance.heroFortress;
            allBuildings = GlobalStorage.instance.fortressBuildings;
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
            UnitInMarketUI[] unitsInBuilding = unitsRow.GetComponentsInChildren<UnitInMarketUI>();
            for(int j = 0; j < unitsInBuilding.Length; j++)
            {
                Unit unit = unitManager.GetUnitForTip(units[i], j + 1);                
                unitsInBuilding[j].Init(this, unit, j < maxUnitLevel);
            }
        }
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
}