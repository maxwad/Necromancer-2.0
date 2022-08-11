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

    private float radiusPlayerSearch = 20;
    private BattleArmyController player;
    private EnemyMovement movementScript;
    private SimpleAnimator animatorScript;

    private bool isSpelling = false;
    private BossSpells spell;
    private Coroutine spelling;

    public void Init()
    {
        movementScript = GetComponent<EnemyMovement>();
        animatorScript = GetComponent<SimpleAnimator>();
        player = GlobalStorage.instance.battlePlayer;

        spell = (BossSpells)UnityEngine.Random.Range(0, Enum.GetValues(typeof(BossSpells)).Length);
        //spell = (BossSpells)1;

        spelling = StartCoroutine(Spelling());

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
}
