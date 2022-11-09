using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class BossController : MonoBehaviour
{
    private float delayAttack = 20f;
    private float timeAttack = 10f;
    private float timeStep = 1f;

    [HideInInspector] public float healthMax;
    [HideInInspector] public float currentHealth;
    [HideInInspector] public Sprite sprite;

    private float radiusPlayerSearch = 20;
    private BattleArmyController player;
    private EnemyMovement movementScript;
    private BattleUIManager battleUIManager;
    private SimpleAnimator animatorScript;

    private bool isSpelling = false;
    private BossSpells spell;
    private Coroutine spelling;

    [HideInInspector] public RuneSO rune;
    private RunesManager runesManager;
    private BattleBoostManager boostManager;

    public void Init(float maxHealth, Sprite pict)
    {
        battleUIManager = GlobalStorage.instance.battleIUManager;
        movementScript = GetComponent<EnemyMovement>();
        animatorScript = GetComponent<SimpleAnimator>();
        player = GlobalStorage.instance.battlePlayer;
        runesManager = GlobalStorage.instance.runesManager;
        boostManager = GlobalStorage.instance.boostManager;

        spell = (BossSpells)UnityEngine.Random.Range(0, Enum.GetValues(typeof(BossSpells)).Length);
        //spell = (BossSpells)1;

        spelling = StartCoroutine(Spelling());

        healthMax = maxHealth;
        sprite = pict;
        rune = runesManager.runesStorage.GetRuneForBoss();
        battleUIManager.RegisterBoss(healthMax, this);

        ApplyRune(false);
    }

    public void UpdateBossHealth(float currentHealth)
    {
        battleUIManager.UpdateBossHealth(currentHealth, this);
    }

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

    public void BossDeath()
    {
        DeleteRune();
        battleUIManager.UnRegisterBoss(this, true);
    }

    public void StopSpelling()
    {
        if(spelling != null) StopCoroutine(spelling);
        StopSpell();
    }

    private IEnumerator Spelling()
    {
        WaitForSeconds timeStepDelay = new WaitForSeconds(timeStep);

        while(true)
        {
            yield return new WaitForSeconds(delayAttack);
            float countTime = 0;

            while(Vector3.Distance(transform.position, player.transform.position) > radiusPlayerSearch)
            {
                yield return timeStepDelay;
            }

            movementScript.StopMoving(true);
            animatorScript.ChangeAnimation(Animations.Attack);

            while(countTime <= timeAttack)
            {
                yield return timeStepDelay;
                countTime += timeStep;

                if(countTime > 0 && isSpelling == false)
                {
                    Spell(timeAttack - countTime);
                    isSpelling = true;
                }
            }

            movementScript.StopMoving(false);
            animatorScript.ChangeAnimation(Animations.Walk);
            isSpelling = false;
        }        
    }

    private void Spell(float duration)
    {        
        GlobalStorage.instance.spellManager.GetComponent<SpellLibrary>().ActivateBossSpell(spell, true, duration);
    }

    private void StopSpell()
    {
        //we need this checking because when we turn off Editor, at this moment Unity had already destroyed most of objects and we receive error
        if(GlobalStorage.instance != null && GlobalStorage.instance.spellManager != null)
            GlobalStorage.instance.spellManager.GetComponent<SpellLibrary>().ActivateBossSpell(spell, false);
    }

    private void OnDestroy()
    {
        StopSpelling();
    }
}
