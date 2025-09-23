using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AttackController : MonoBehaviour
{
    private BoostManager boostManager;
    private WeaponStorage weaponStorage;
    private Unit unit;
    private UnitController unitController;

    private float speedAttack;

    private float startAttackDelay = 1f;

    private Coroutine attack;

    [Inject]
    public void Construct(BoostManager boostManager, WeaponStorage weaponStorage)
    {
        this.boostManager = boostManager;
        this.weaponStorage = weaponStorage;

        unitController = GetComponent<UnitController>();
        unit = unitController.unit;
    }

    private void OnEnable()
    {
        startAttackDelay = Random.Range(startAttackDelay * 10 - startAttackDelay, startAttackDelay * 10 + startAttackDelay) / 10;
        if(attack != null) StopCoroutine(attack);
        attack = StartCoroutine(Attack());
    }

    public void ReloadAttack(Unit newUnit)
    {
        if(unit != null)
        {
            //Debug.Log("Reload was = " + unit.level + " and became " + newUnit.level);
        }
        if(gameObject.activeInHierarchy == false) return;

        unit = newUnit;
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

        while(unitController.unit == null)
        {
            //unitController.unit = GetComponent<UnitController>().unit;
            yield return null;
        }

        while (unitController.quantity != 0)
        {
            //Debug.Log("Attack level " + unit.level);
            speedAttack = unitController.unit.speedAttack + unitController.unit.speedAttack * boostManager.GetBoost(Enums.BoostType.CoolDown);
            weaponStorage.Attack(unitController.unit);

            yield return new WaitForSeconds(speedAttack);
        }
    }
}
