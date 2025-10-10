using System.Collections;
using UnityEngine;
using Zenject;

public class InvertingWeapon : EnemyWeapon
{
    BattleArmyController battlePlayer;

    [Inject]
    public void Construct(BattleArmyController battlePlayer)
    {
        this.battlePlayer = battlePlayer;
    }

    protected override void Stop()
    {
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

        StopEffect();
        gameObject.SetActive(false);
    }

    protected override void Shot()
    {
        battlePlayer.MovementInverting(true);
    }

    protected override void SpecialDisableEffect()
    {
        StopEffect();
    }

    private void StopEffect()
    {
        battlePlayer.MovementInverting(false);
    }
}
