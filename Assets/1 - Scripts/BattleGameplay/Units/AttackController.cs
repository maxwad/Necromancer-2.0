using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    private WeaponStorage weaponStorage;
    private UnitController unitController;

    private float startAttackDelay = 1f;

    private Coroutine attack;

    private void Awake()
    {
        unitController = GetComponent<UnitController>();
        weaponStorage = GetComponent<WeaponStorage>();
    }

    private void OnEnable()
    {
        startAttackDelay = Random.Range(startAttackDelay * 10 - startAttackDelay, startAttackDelay * 10 + startAttackDelay) / 10;
        attack = StartCoroutine(Attack());
    }

    private void OnDisable()
    {
        StopCoroutine(attack);
    }

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(startAttackDelay);

        while (gameObject != null)
        {
            weaponStorage.Attack(unitController);

            yield return new WaitForSeconds(unitController.speedAttack);
        }
    }
}
