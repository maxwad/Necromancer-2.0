using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class InvertingWeapon : MonoBehaviour
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
        Shot(false);
        gameObject.SetActive(false);
    }

    private IEnumerator Attact()
    {
        while(currentShoot < countOfShoot)
        {
            yield return new WaitForSeconds(attackDelay - timeOffset);

            Shot(true);

            currentShoot++;
            currentTime += (attackDelay - timeOffset);
        }

        while(currentTime < attackPeriod)
        {
            yield return new WaitForSeconds(attackPeriod - currentTime);
            currentTime += (attackPeriod - currentTime);
        }

        Shot(false);
        gameObject.SetActive(false);
    }

    private void Shot(bool mode)
    {
        GlobalStorage.instance.battlePlayer.MovementInverting(mode);
    }

    private void OnDisable()
    {
        EventManager.EndOfBattle -= Stop;

        if(coroutine != null) StopCoroutine(coroutine);
        Shot(false);
    }
}
