using System;
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
    private float timeAttack = 10f;
    private float attackPeriod = 2f;
    private float timeStep = 1f;
    private float radiusPlayerSearch = 20;

    private BossSpells spell;

    private EnemyBossWeapons bossWeapon;
    private Coroutine waitCoroutine;

    [Header("Runes")]
    [HideInInspector] public RuneSO rune;
    private RunesManager runesManager;
    private BattleBoostManager boostManager;

    #region HELPERS

    public void Init(float maxHealth, Sprite pict)
    {
        battleUIManager = GlobalStorage.instance.battleIUManager;
        movementScript = GetComponent<EnemyMovement>();
        animatorScript = GetComponent<SimpleAnimator>();
        player = GlobalStorage.instance.battlePlayer;
        runesManager = GlobalStorage.instance.runesManager;
        boostManager = GlobalStorage.instance.boostManager;

        healthMax = maxHealth;
        sprite = pict;
        rune = runesManager.runesStorage.GetRuneForBoss();
        battleUIManager.RegisterBoss(healthMax, this);

        ApplyRune(false);

        bossWeapon = gameObject.AddComponent<EnemyBossWeapons>();
        spell = (BossSpells)UnityEngine.Random.Range(0, Enum.GetValues(typeof(BossSpells)).Length);
        spell = (BossSpells)1;

        waitCoroutine = StartCoroutine(Waiting());
    }

    public void UpdateBossHealth(float currentHealth)
    {
        battleUIManager.UpdateBossHealth(currentHealth, this);
    }
   
    public void BossDeath(bool runeDeleting)
    {
        if(runeDeleting == true) DeleteRune();

        battleUIManager.UnRegisterBoss(this, true);

        bossWeapon.ActivateBossWeapon(spell, false);
        if(waitCoroutine != null) StopCoroutine(waitCoroutine);

        Destroy(bossWeapon);
    }

    private void OnDestroy()
    {
        bossWeapon.ActivateBossWeapon(spell, false);
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

            while(actionTime <= timeAttack)
            {
                actionTime += attackPeriod;

                bossWeapon.ActivateBossWeapon(spell, true);

                yield return new WaitForSeconds(attackPeriod);
            }

            bossWeapon.ActivateBossWeapon(spell, false);

            movementScript.StopMoving(false);
            animatorScript.ChangeAnimation(Animations.Walk);
        }
    }
}
