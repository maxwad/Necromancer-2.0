using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Enums;

public class InfotipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private Unit unit;
    private EnemySO enemy;
    private MacroAbilitySO skill;
    private RuneSO rune;
    private BuildingsRequirements requirments;
    private SpellSO spell;

    [SerializeField] private TipsType tipsType;

    private float timeDelay = 0.5f;
    private float currentWaitTime = 0;
    private bool isWaiting = false;
    private bool isTipOpen = false;

    private void Update()
    {
        if(isWaiting == true)
        {
            currentWaitTime += Time.unscaledDeltaTime;

            if(currentWaitTime >= timeDelay)
            {
                switch(tipsType)
                {
                    case TipsType.Unit:
                        InfotipManager.Show(unit);
                        break;

                    case TipsType.Enemy:
                        InfotipManager.Show(enemy);
                        break;

                    case TipsType.Hero:
                        InfotipManager.Show();
                        break;

                    case TipsType.Skill:
                        InfotipManager.Show(skill);
                        break;

                    case TipsType.Spell:
                        InfotipManager.Show(spell);
                        break;

                    case TipsType.Rune:
                        InfotipManager.Show(rune);
                        break;

                    case TipsType.Requirements:
                        InfotipManager.Show(requirments);
                        break;

                    default:
                        break;
                }

                isTipOpen = true;
                isWaiting = false;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isWaiting = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(isWaiting == false) InfotipManager.Hide(tipsType);

        isTipOpen = false;
        isWaiting = false;
        currentWaitTime = 0;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(isWaiting == false) InfotipManager.Hide(tipsType);

        isTipOpen = false;
        isWaiting = false;
        currentWaitTime = 0;
    }

    public void SetUnit(Unit squad)
    {
        unit = squad;
        tipsType = TipsType.Unit;
    }

    public void SetEnemy(EnemySO squad)
    {
        enemy = squad;
        tipsType = TipsType.Enemy;
    }  
    
    public void SetSkill(MacroAbilitySO skill)
    {
        this.skill = skill;
        tipsType = TipsType.Skill;
    }

    public void SetRune(RuneSO rune)
    {
        this.rune = rune;
        tipsType = TipsType.Rune;
    }

    public void SetSpell(SpellSO spell)
    {
        this.spell = spell;
        tipsType = TipsType.Spell;
    }

    public void SetCost(BuildingsRequirements requirments)
    {
        this.requirments = requirments;
        tipsType = TipsType.Requirements;
    }


    private void OnDisable()
    {
        currentWaitTime = 0;
        if(isTipOpen == true) 
        {
            isTipOpen = false;
            InfotipManager.Hide(tipsType);
        }
    }
}
