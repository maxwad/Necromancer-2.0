using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static NameManager;

public class InfotipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private Unit unit;
    private EnemyController enemy;
    private MacroAbilitySO skill;

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
                        break;
                    case TipsType.Boost:
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

    public void SetEnemy(EnemyController squad)
    {
        enemy = squad;
        tipsType = TipsType.Enemy;
    }  
    
    public void SetSkill(MacroAbilitySO skill)
    {
        this.skill = skill;
        tipsType = TipsType.Skill;
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
