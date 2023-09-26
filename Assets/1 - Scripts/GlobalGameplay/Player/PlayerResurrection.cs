using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerResurrection : MonoBehaviour
{
    private HeroController hero;
    private PortalsManager portalsManager;


    [Inject]
    public void Construct(PortalsManager portalsManager, HeroController hero)
    {
        this.portalsManager = portalsManager;
        this.hero = hero;
    }

    public void StartResurrection()
    {
        StartCoroutine(Resurrection());
    }

    private IEnumerator Resurrection()
    {
        CameraSwitcher switcher = Camera.main.GetComponent<CameraSwitcher>();
        while(switcher.isSwitching == true)
        {
            yield return null;
        }
        portalsManager.ResurrectionTeleport();
        hero.Resurrection();
    }
}
