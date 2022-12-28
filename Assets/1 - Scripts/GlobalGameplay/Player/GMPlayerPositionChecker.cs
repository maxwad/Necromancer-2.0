using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class GMPlayerPositionChecker : MonoBehaviour
{
    private GlobalMapPathfinder gmPathFinder;
    private GMPlayerMovement gmMovement;
    private EnemyManager enemyManager;

    private void Start()
    {
        gmPathFinder = GlobalStorage.instance.globalMap.GetComponent<GlobalMapPathfinder>();
        gmMovement = GlobalStorage.instance.globalPlayer.GetComponent<GMPlayerMovement>();
        enemyManager = GlobalStorage.instance.enemyManager;
    }

    public void CheckPosition(Vector3 position)
    {
        if(gmPathFinder.focusObject != null)
        {
            if(gmPathFinder.enterPointsDict[gmPathFinder.focusObject] == position)
            {
                ClickableObject obj = gmPathFinder.focusObject.GetComponent<ClickableObject>();
                if(obj.objectType == TypeOfObjectOnTheMap.BoxBonus)
                {
                    obj.gameObject.GetComponent<BoxObject>().GetReward();
                }
                else
                {
                    obj.ActivateUIWindow(false);
                }
            }
        }
    }

    public bool CheckEnemy(Vector3 position, bool fightMode = true)
    {
        EnemyArmyOnTheMap enemyArmy = enemyManager.CheckPositionInEnemyPoints(position);

        if(enemyArmy != null)
        {
            if(fightMode == true)
            {
                gmMovement.StopMoving();
                enemyArmy.PrepairToTheBattle();
            }            
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(TagManager.T_Resource) == true)
        {
            ResourceObject heap = collision.GetComponent<ResourceObject>();
            heap.GetReward();
        }
    }

}
