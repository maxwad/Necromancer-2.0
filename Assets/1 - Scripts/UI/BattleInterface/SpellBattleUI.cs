using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Enums;
using System;

public class SpellBattleUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text timer;
    [SerializeField] private TooltipTrigger tooltip;

    private Coroutine coroutine;
    private float timeStep = 0.1f;
    private WaitForSeconds delay;
    private float currentTime = 0;

    private BattleUISpellPart uiManager;
    [HideInInspector] public SpellSO spell;

    public void Init(BattleUISpellPart manager, SpellSO newSpell)
    {
        uiManager = manager;
        spell = newSpell;

        icon.sprite = newSpell.icon;
        string description = spell.description.Replace("$V", spell.value.ToString());
        description = description.Replace("$T", spell.actionTime.ToString());
        description = description.Replace("$R", spell.radius.ToString());
        tooltip.content = description;

        currentTime = newSpell.actionTime;
        delay = new WaitForSeconds(timeStep);
        coroutine = StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        while(currentTime > 0)
        {
            yield return delay;

            currentTime -= timeStep;
            timer.text = (Math.Round(currentTime, 1)).ToString();
            if(timer.text.Contains(",") == false)
                timer.text += ",0";
        }

        uiManager.DeleteUISpellEffect(spell);
        spell = null;
        gameObject.SetActive(false);
    }

    internal void AddActionTime(float actionTime)
    {
        currentTime += actionTime;
    }

    private void ClearEffect(bool mode)
    {
        if(coroutine != null) StopCoroutine(coroutine);
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        EventManager.SwitchPlayer += ClearEffect;
    }

    private void OnDisable()
    {
        EventManager.SwitchPlayer -= ClearEffect;
    }
}
