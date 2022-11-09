using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    private BattleBoostManager boostManager;
    private WeaponStorage weaponStorage;
    private Unit unit;

    private float speedAttack;

    private float startAttackDelay = 1f;

    private Coroutine attack;


    private void OnEnable()
    {
        if(unit == null)
        {
            unit = GetComponent<UnitController>().unit;
            weaponStorage = GlobalStorage.instance.player.GetComponent<WeaponStorage>();
            boostManager = GlobalStorage.instance.boostManager;
        }

        startAttackDelay = Random.Range(startAttackDelay * 10 - startAttackDelay, startAttackDelay * 10 + startAttackDelay) / 10;
        if(attack != null) StopCoroutine(attack);
        attack = StartCoroutine(Attack());
    }

    private void OnDisable()
    {
        StopCoroutine(attack);
    }

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(startAttackDelay);

        while(unit == null)
        {
            unit = GetComponent<UnitController>().unit;
            yield return null;
        }

        while (unit.quantity != 0)
        {
            speedAttack = unit.speedAttack + unit.speedAttack * boostManager.GetBoost(NameManager.BoostType.CoolDown);
            weaponStorage.Attack(unit);

            yield return new WaitForSeconds(speedAttack);
        }
    }
}
