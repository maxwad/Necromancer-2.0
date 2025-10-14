using System.Collections;
using UnityEngine;

public class LigtningWeapon : EnemyWeapon
{
    [SerializeField] protected MonoBehaviour ligtningPrefab;

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

            MonoBehaviour currentBullet = objectsPool.GetOrCreateElement(ligtningPrefab, this, effectsContainer.transform);
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
}
