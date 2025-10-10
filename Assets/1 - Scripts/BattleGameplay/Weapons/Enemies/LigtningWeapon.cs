using System.Collections;
using UnityEngine;
using Zenject;
using Enums;

public class LigtningWeapon : EnemyWeapon
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

            GameObject currentBullet = objectsPool.GetObject(bullet);
            currentBullet.SetActive(true);
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
}
