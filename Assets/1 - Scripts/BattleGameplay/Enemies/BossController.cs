using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using static NameManager;

public class BossController : MonoBehaviour
{
    [HideInInspector] public float healthMax;
    [HideInInspector] public float currentHealth;
    [HideInInspector] public Sprite sprite;

    private BattleArmyController player;
    private EnemyMovement movementScript;
    private BattleUIManager battleUIManager;
    private SimpleAnimator animatorScript;

    [Header("Attack")]
    private float delayAttack = 5f;
    private float attackPeriod = 3f;
    private float timeStep = 1f;
    private float radiusPlayerSearch = 15;

    private GameObject weapon;
    private BossWeapons weaponType;

    private Coroutine waitCoroutine;

    [Header("Runes")]
    [HideInInspector] public RuneSO rune;
    private RunesManager runesManager;
    private BoostManager boostManager;
    private ObjectsPoolManager poolManager;

    #region HELPERS

    public void Init(float maxHealth, Sprite pict)
    {
        battleUIManager = GlobalStorage.instance.battleIUManager;
        movementScript = GetComponent<EnemyMovement>();
        animatorScript = GetComponent<SimpleAnimator>();
        player = GlobalStorage.instance.battlePlayer;
        runesManager = GlobalStorage.instance.runesManager;
        boostManager = GlobalStorage.instance.boostManager;
        poolManager = GlobalStorage.instance.objectsPoolManager;

        healthMax = maxHealth;
        sprite = pict;
        rune = runesManager.runesStorage.GetRuneForBoss();
        battleUIManager.enemyPart.RegisterBoss(healthMax, this);

        ApplyRune(false);

        weaponType = (BossWeapons)UnityEngine.Random.Range(0, Enum.GetValues(typeof(BossWeapons)).Length);
        //weaponType = (BossWeapons)3;

        waitCoroutine = StartCoroutine(Waiting());
    }

    public void UpdateBossHealth(float currentHealth)
    {
        battleUIManager.enemyPart.UpdateBossHealth(currentHealth, this);
    }
   
    public void BossDeath(bool runeDeleting)
    {
        if(runeDeleting == true) DeleteRune();

        battleUIManager.enemyPart.UnRegisterBoss(this, true);

        if(waitCoroutine != null) StopCoroutine(waitCoroutine);
        if(weapon != null) weapon.SetActive(false);

    }

    #endregion

    #region RUNES
    public void ApplyRune(bool changeMode)
    {
        BoostType type = EnumConverter.instance.RuneToBoostType(rune.rune);
        float value;
        if(changeMode == false)
            value = (rune.isInvertedRune == true) ? -rune.value : rune.value;
        else
            value = (rune.isInvertedRune == true) ? rune.value : -rune.value;

        boostManager.SetBoost(type, BoostSender.EnemySystem, BoostEffect.EnemiesBattle, value);
    }

    public void DeleteRune()
    {
        BoostType type = EnumConverter.instance.RuneToBoostType(rune.rune);
        float value = (rune.isInvertedRune == true) ? -rune.value : rune.value;
        boostManager.DeleteBoost(type, BoostSender.EnemySystem, value);

        ApplyRune(true);
    }

    #endregion

    private IEnumerator Waiting()
    {
        while(true)
        {
            yield return new WaitForSeconds(delayAttack);

            while(Vector3.Distance(transform.position, player.transform.position) > radiusPlayerSearch)
            {
                yield return timeStep;
            }

            movementScript.StopMoving(true);
            animatorScript.ChangeAnimation(Animations.Attack);

            weapon = poolManager.GetBossWeapon(weaponType);
            weapon.transform.position = transform.position;
            attackPeriod = weapon.GetComponent<EnemyWeaponParameters>().attackPeriod;
            weapon.SetActive(true);
            yield return new WaitForSeconds(attackPeriod);

            movementScript.StopMoving(false);
            animatorScript.ChangeAnimation(Animations.Walk);
        }
    }
}
