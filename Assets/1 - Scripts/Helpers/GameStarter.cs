using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static NameManager;

public class GameStarter : MonoBehaviour
{
    private HeroFortress heroFortress;
    private ResourcesManager resourcesManager;

    [Header("Resources")]
    public List<Cost> startResources;

    [Header("Units")]
    public List<HiringAmount> startAmounts = new List<HiringAmount>();
    public List<HiringAmount> startGrowthAmounts;

    [Header("Castle Buildings")]
    public List<CastleBuildings> castleBuildings;
    public int buildingsLevel = 3;


    [Inject]
    public void Construct(HeroFortress heroFortress, ResourcesManager resourcesManager)
    {
        this.heroFortress = heroFortress;
        this.resourcesManager = resourcesManager;
    }

    public void Init(bool createMode)
    {
        if(createMode == true)
            StartCoroutine(Launch());
        else
            GlobalStorage.instance.LoadNextPart();
    }

    private IEnumerator Launch()
    {
        yield return null;
        SetResources();

        yield return null;
        SetUnits();     
        
        yield return null;
        SetFortressBuildings();

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

    private void SetFortressBuildings()
    {
        foreach(var building in castleBuildings)
        {
            heroFortress.BuildStartBuildings(building, buildingsLevel);
        }
    }
}
