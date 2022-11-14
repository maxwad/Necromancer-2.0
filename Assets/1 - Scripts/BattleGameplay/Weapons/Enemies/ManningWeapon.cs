using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class ManningWeapon : MonoBehaviour
{
    private EnemyWeaponParameters weaponParameters;
    private Coroutine coroutine;
    private float attackPeriod;
    private float attackDelay;
    private float timeOffset;
    private float currentTime = 0;
    private float countOfShoot = 0;
    private float currentShoot = 0;

    private void OnEnable()
    {
        EventManager.EndOfBattle += Stop;

        if(weaponParameters == null)
        {
            weaponParameters = GetComponent<EnemyWeaponParameters>();

            attackPeriod = weaponParameters.attackPeriod;
            attackDelay = weaponParameters.attackDelay;
            timeOffset = weaponParameters.timeOffset;
            countOfShoot = Mathf.Floor(attackPeriod / attackDelay);
        }

        currentTime = 0;
        currentShoot = 0;

        coroutine = StartCoroutine(Attact());
    }

    private void Stop()
    {
        if(coroutine != null) StopCoroutine(coroutine);
        gameObject.SetActive(false);
    }

    private IEnumerator Attact()
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

    private void Shot()
    {
        GlobalStorage.instance.resourcesManager.ChangeResource(ResourceType.Mana, -1);

        GameObject death = GlobalStorage.instance.objectsPoolManager.GetObject(ObjectPool.ManaDeath);
        death.transform.position = GlobalStorage.instance.hero.transform.position;
        death.SetActive(true);

        GameObject bloodSpot = GlobalStorage.instance.objectsPoolManager.GetObject(ObjectPool.ManaSpot);
        bloodSpot.transform.position = GlobalStorage.instance.hero.transform.position;
        bloodSpot.SetActive(true);
    }

    private void OnDisable()
    {
        EventManager.EndOfBattle -= Stop;

        if(coroutine != null) StopCoroutine(coroutine);
    }
}
