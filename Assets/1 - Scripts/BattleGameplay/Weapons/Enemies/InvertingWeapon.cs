using System.Collections;
using UnityEngine;
using Zenject;

public class InvertingWeapon : MonoBehaviour
{
    BattleArmyController battlePlayer;

    private EnemyWeaponParameters weaponParameters;
    private Coroutine coroutine;
    private float attackPeriod;
    private float attackDelay;
    private float timeOffset;
    private float currentTime = 0;
    private float countOfShoot = 0;
    private float currentShoot = 0;


    [Inject]
    public void Construct(BattleArmyController battlePlayer)
    {
        this.battlePlayer = battlePlayer;

        weaponParameters = GetComponent<EnemyWeaponParameters>();
        attackPeriod = weaponParameters.attackPeriod;
        attackDelay = weaponParameters.attackDelay;
        timeOffset = weaponParameters.timeOffset;
        countOfShoot = Mathf.Floor(attackPeriod / attackDelay);
    }

    private void OnEnable()
    {
        EventManager.EndOfBattle += Stop;

        currentTime = 0;
        currentShoot = 0;

        coroutine = StartCoroutine(Attact());
    }

    private void Stop()
    {
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
        battlePlayer.MovementInverting(mode);
    }

    private void OnDisable()
    {
        EventManager.EndOfBattle -= Stop;

        if(coroutine != null) 
        {
            StopCoroutine(coroutine);
            Shot(false);
        } 
    }
}
