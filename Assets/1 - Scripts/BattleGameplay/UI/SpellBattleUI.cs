using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;
using System;

public class SpellBattleUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text timer;
    [SerializeField] private TooltipTrigger tooltip;

    private Coroutine coroutine;
    private float timeStep = 0.1f;
    private WaitForSeconds delay;
    
    public void Init(SpellSO spell)
    {
        icon.sprite = spell.icon;

        string description = spell.description.Replace("$", spell.value.ToString());
        description = description.Replace("&", spell.actionTime.ToString());
        tooltip.content = description;

        delay = new WaitForSeconds(timeStep);
        coroutine = StartCoroutine(Timer(spell.actionTime));
    }

    private IEnumerator Timer(float time)
    {
        double  startTime = Math.Round(time, 1);

        while(startTime > 0)
        {
            yield return delay;

            startTime -= timeStep;
            timer.text = (Math.Round(startTime, 1)).ToString();
        }

        gameObject.SetActive(false);
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
