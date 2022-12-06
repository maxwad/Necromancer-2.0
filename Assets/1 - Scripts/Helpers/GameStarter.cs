using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class GameStarter : MonoBehaviour
{
    private HeroFortress heroFortress;
    private ResourcesManager resourcesManager;

    [Header("Resources")]
    public List<Cost> startResources;

    public void Init()
    {
        heroFortress = GlobalStorage.instance.heroFortress;
        resourcesManager = GlobalStorage.instance.resourcesManager;

        StartCoroutine(Launch());
    }

    private IEnumerator Launch()
    {
        yield return null;
        SetResources();

        yield return null;

        GlobalStorage.instance.LoadNextPart();
    }

    private void SetResources()
    {
        foreach(var resource in startResources)
        {
            resourcesManager.ChangeResource(resource.type, resource.amount);
        }
    }
}
