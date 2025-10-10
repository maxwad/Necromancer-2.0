using Enums;
using System.Collections;
using UnityEngine;
using Zenject;

public class ManningWeapon : EnemyWeapon
{
    protected override void Stop()
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        gameObject.SetActive(false);
    }

    protected override IEnumerator Attack()
    {
        while(currentShoot < countOfShoot)
        {
            yield return new WaitForSeconds(attackDelay - timeOffset);

            Shot();

            currentShoot++;
            currentTime += (attackDelay - timeOffset);
        }

        while(currentTime < attackPeriod)
        {
            yield return new WaitForSeconds(attackPeriod - currentTime);
            currentTime += (attackPeriod - currentTime);
        }

        gameObject.SetActive(false);
    }

    protected override void Shot()
    {
        resourcesManager.ChangeResource(ResourceType.Mana, -1);

        GameObject death = objectsPool.GetObject(ObjectPool.ManaDeath);
        death.transform.position = hero.transform.position;
        death.SetActive(true);

        GameObject bloodSpot = objectsPool.GetObject(ObjectPool.ManaSpot);
        bloodSpot.transform.position = hero.transform.position;
        bloodSpot.SetActive(true);
    }
}
