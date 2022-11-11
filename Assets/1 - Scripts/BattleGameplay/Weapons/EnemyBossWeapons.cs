using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class EnemyBossWeapons : MonoBehaviour
{
    private GameObject currentWeapon;

    public void ActivateBossWeapon(BossSpells bossSpell, bool mode)
    {
        switch(bossSpell)
        {
            case BossSpells.InvertMovement:
                InvertMovement(mode);
                break;

            case BossSpells.Lightning:
                Lightning(mode);
                break;

            case BossSpells.ManningLess:
                ManningLess(mode);
                break;

            default:
                break;
        }
    }

    private void InvertMovement(bool mode)
    {
        GlobalStorage.instance.battlePlayer.MovementInverting(mode);
    }

    private void ManningLess(bool mode)
    {
        if(mode == true)
        {
            GlobalStorage.instance.resourcesManager.ChangeResource(ResourceType.Mana, -1);

            GameObject death = GlobalStorage.instance.objectsPoolManager.GetObject(ObjectPool.ManaDeath);
            death.transform.position = GlobalStorage.instance.hero.transform.position;
            death.SetActive(true);

            GameObject bloodSpot = GlobalStorage.instance.objectsPoolManager.GetObject(ObjectPool.ManaSpot);
            bloodSpot.transform.position = GlobalStorage.instance.hero.transform.position;
            bloodSpot.SetActive(true);
        }
    }
    private void Lightning(bool mode)
    {
        if(mode == true)
        {
            GameObject ray = GlobalStorage.instance.objectsPoolManager.GetObject(ObjectPool.EnemyLigthning);
            ray.transform.position = GlobalStorage.instance.hero.transform.position;
            ray.transform.SetParent(GlobalStorage.instance.effectsContainer.transform);
            currentWeapon = ray;
            currentWeapon.SetActive(true);
        }
    }
}