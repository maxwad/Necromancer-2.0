using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    private BattleBoostManager boostManager;
    private WeaponStorage weaponStorage;
    private Unit unit;
    private UnitController unitController;

    private float speedAttack;

    private float startAttackDelay = 1f;

    private Coroutine attack;


    private void OnEnable()
    {
        if(weaponStorage == null)
        {
            unitController = GetComponent<UnitController>();
            unit = unitController.unit;
            weaponStorage = GlobalStorage.instance.player.GetComponent<WeaponStorage>();
            boostManager = GlobalStorage.instance.boostManager;
        }

        startAttackDelay = Random.Range(startAttackDelay * 10 - startAttackDelay, startAttackDelay * 10 + startAttackDelay) / 10;
        if(attack != null) StopCoroutine(attack);
        attack = StartCoroutine(Attack());
    }

    public void ReloadAttack(UnitController unitController)
    {
        unit = unitController.unit;
        startAttackDelay = Random.Range(startAttackDelay * 10 - startAttackDelay, startAttackDelay * 10 + startAttackDelay) / 10;
        if(attack != null) StopCoroutine(attack);
        attack = StartCoroutine(Attack());
    }

    private void OnDisable()
    {
        if(attack != null) StopCoroutine(attack);
    }

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(startAttackDelay);

        while(unit == null)
        {
            unit = GetComponent<UnitController>().unit;
            yield return null;
        }

        while (unitController.quantity != 0)
        {
            //Debug.Log("Attack level " + unit.level);
            speedAttack = unit.speedAttack + unit.speedAttack * boostManager.GetBoost(NameManager.BoostType.CoolDown);
            weaponStorage.Attack(unit);

            yield return new WaitForSeconds(speedAttack);
        }
    }
}
