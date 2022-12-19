using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class GameStarter : MonoBehaviour
{
    private HeroFortress heroFortress;
    private ResourcesManager resourcesManager;
    //private UnitManager unitManager;

    [Header("Resources")]
    public List<Cost> startResources;

    [Header("Units")]
    public List<HiringAmount> startAmounts = new List<HiringAmount>();
    public List<HiringAmount> startGrowthAmounts;

    public void Init()
    {
        heroFortress = GlobalStorage.instance.heroFortress;
        resourcesManager = GlobalStorage.instance.resourcesManager;
        //unitManager = GlobalStorage.instance.unitManager;

        StartCoroutine(Launch());
    }

    private IEnumerator Launch()
    {
        yield return null;
        SetResources();

        yield return null;
        SetUnits();

        GlobalStorage.instance.LoadNextPart();
    }

    private void SetResources()
    {
        foreach(var resource in startResources)
        {
            resourcesManager.ChangeResource(resource.type, resource.amount);
        }
    }

    private void SetUnits()
    {
        foreach(var amount in startAmounts)
        {
            heroFortress.ChangePotentialUnitsAmount(amount.unitType, amount.amount);
        }

        heroFortress.SetStartGrowths(startGrowthAmounts);
    }
}
