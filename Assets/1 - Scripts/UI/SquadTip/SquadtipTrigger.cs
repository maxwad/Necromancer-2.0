using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SquadtipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool isEnemy = false;
    private Unit unit;
    private EnemyController enemy;

    private float timeDelay = 1f;
    private float currentWaitTime = 0;
    private bool isWaiting = false;
    private bool isSquadtipOpen = false;

    private void Update()
    {
        if(isWaiting == true)
        {
            currentWaitTime += Time.unscaledDeltaTime;

            if(currentWaitTime >= timeDelay)
            {
                if(isEnemy == true)
                    SquadtipManager.Show(enemy);
                else
                    SquadtipManager.Show(unit);

                isSquadtipOpen = true;
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
        if(isWaiting == false) SquadtipManager.Hide();

        isSquadtipOpen = false;
        isWaiting = false;
        currentWaitTime = 0;
    }

    public void SetUnit(Unit squad)
    {
        unit = squad;
        isEnemy = false;
    }

    public void SetEnemy(EnemyController squad)
    {
        enemy = squad;
        isEnemy = true;
    }

    private void OnDisable()
    {
        if(isSquadtipOpen == true) SquadtipManager.Hide();
    }
}
