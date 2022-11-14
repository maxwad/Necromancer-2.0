using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class LigtningWeapon : MonoBehaviour
{
    private ObjectsPoolManager objectsPool;

    private EnemyWeaponParameters weaponParameters;
    private Coroutine coroutine;
    private float attackPeriod;
    private float attackDelay;
    private float timeOffset;
    private float currentTime = 0;
    private float countOfShoot = 0;
    private float currentShoot = 0;
    private ObjectPool bullet;


    private void OnEnable()
    {
        EventManager.EndOfBattle += Stop;
   
        if(objectsPool == null) 
        {
            objectsPool = GlobalStorage.instance.objectsPoolManager;
            weaponParameters = GetComponent<EnemyWeaponParameters>();

            bullet = weaponParameters.bullet;
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

    private void OnDisable()
    {
        EventManager.EndOfBattle -= Stop;

        if(coroutine != null) StopCoroutine(coroutine);
    }
}
