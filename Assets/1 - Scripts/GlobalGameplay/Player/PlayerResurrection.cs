using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResurrection : MonoBehaviour
{
    private HeroController hero;
    private PortalsManager portalsManager;

    private void Start()
    {
        hero = GlobalStorage.instance.hero;
        portalsManager = GlobalStorage.instance.portalsManager;
    }

    public void StartResurrection()
    {
        StartCoroutine(Resurrection());
    }

    private IEnumerator Resurrection()
    {
        //yield return new WaitForSeconds(1f);
        CameraSwitcher switcher = Camera.main.GetComponent<CameraSwitcher>();
        while(switcher.isSwitching == true)
        {
            yield return null;
        }
        portalsManager.ResurrectionTeleport();
        hero.Resurrection();
    }
}
