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
    private float timeAttack = 12f;
    private float attackPeriod = 3f;
    private float timeStep = 1f;
    private float radiusPlayerSearch = 20;

    private BossSpells spell;

    private EnemyBossWeapons bossWeapon;
    private Coroutine waitCoroutine;

    [Header("Runes")]
    [HideInInspector] public RuneSO rune;
    private RunesManager runesManager;
    private BattleBoostManager boostManager;
    private EnemyManager enemyManager;

    #region HELPERS

    public void Init(float maxHealth, Sprite pict)
    {
        battleUIManager = GlobalStorage.instance.battleIUManager;
        movementScript = GetComponent<EnemyMovement>();
        animatorScript = GetComponent<SimpleAnimator>();
        player = GlobalStorage.instance.battlePlayer;
        runesManager = GlobalStorage.instance.runesManager;
        boostManager = GlobalStorage.instance.boostManager;
        enemyManager = GlobalStorage.instance.enemyManager;

        healthMax = maxHealth;
        sprite = pict;
        rune = runesManager.runesStorage.GetRuneForBoss();
        battleUIManager.enemyPart.RegisterBoss(healthMax, this);

        ApplyRune(false);

        bossWeapon = gameObject.AddComponent<EnemyBossWeapons>();

        BossSpell boosSpell = enemyManager.GetComponent<BossesArsenal>().GetBossSpell();

        spell = boosSpell.spell;
        attackPeriod = boosSpell.attackPeriod;
        //spell = (BossSpells)UnityEngine.Random.Range(0, Enum.GetValues(typeof(BossSpells)).Length);
        //spell = (BossSpells)1;

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

        bossWeapon.ActivateBossWeapon(spell, false, transform.position);
        if(waitCoroutine != null) StopCoroutine(waitCoroutine);

        Destroy(bossWeapon);
    }

    private void OnDestroy()
    {
        if(bossWeapon != null) bossWeapon.ActivateBossWeapon(spell, false, transform.position);
    }

    #endregion

    #region RUNES
    public void ApplyRune(bool changeMode)
    {
        BoostType type = BoostConverter.instance.RuneToBoostType(rune.rune);
        float value;
        if(changeMode == false)
            value = (rune.isInvertedRune == true) ? -rune.value : rune.value;
        else
            value = (rune.isInvertedRune == true) ? rune.value : -rune.value;

        boostManager.SetBoost(type, BoostSender.EnemySystem, BoostEffect.EnemiesBattle, value);
    }

    public void DeleteRune()
    {
        BoostType type = BoostConverter.instance.RuneToBoostType(rune.rune);
        float value = (rune.isInvertedRune == true) ? -rune.value : rune.value;
        boostManager.DeleteBoost(type, BoostSender.EnemySystem, value);

        ApplyRune(true);
    }

    #endregion

    private IEnumerator Waiting()
    {
        while(true)
        {
            float actionTime = 0;

            yield return new WaitForSeconds(delayAttack);

            while(Vector3.Distance(transform.position, player.transform.position) > radiusPlayerSearch)
            {
                yield return timeStep;
            }

            movementScript.StopMoving(true);
            animatorScript.ChangeAnimation(Animations.Attack);

            while(actionTime < timeAttack)
            {
                actionTime += attackPeriod;

                bossWeapon.ActivateBossWeapon(spell, true, transform.position);

                yield return new WaitForSeconds(attackPeriod);
            }

            bossWeapon.ActivateBossWeapon(spell, false, transform.position);

            movementScript.StopMoving(false);
            animatorScript.ChangeAnimation(Animations.Walk);
        }
    }
}
