using Enums;
using System.Collections;
using UnityEngine;

public class ManningWeapon : EnemyWeapon
{
    [SerializeField] protected MonoBehaviour manaSpotPrefab;
    [SerializeField] protected MonoBehaviour manaDeathPrefab;

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

        objectsPool.DiscardAll(this);
        gameObject.SetActive(false);
    }

    protected override void Shot()
    {
        resourcesManager.ChangeResource(ResourceType.Mana, -1);

        MonoBehaviour death = objectsPool.GetOrCreateElement(manaDeathPrefab, this, effectsContainer.transform);
        death.transform.position = hero.transform.position;

        MonoBehaviour manaSpot = objectsPool.GetOrCreateElement(manaSpotPrefab, this, effectsContainer.transform);
        manaSpot.transform.position = hero.transform.position;
    }
}
