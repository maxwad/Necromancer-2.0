using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                gmPathFinder.focusObject.GetComponent<ClickableObject>().ActivateUIWindow(false);
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
                gmPathFinder.DestroyPath();
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
            ResourceObject obj = collision.GetComponent<ResourceObject>();
            EventManager.OnResourcePickedUpEvent(obj.resourceType, obj.quantity);
            obj.Death();
        }

        //if(collision.CompareTag(TagManager.T_GM_ENEMY) == true)
        //{
        //    gmMovement.StopMoving();
        //    gmPathFinder.DestroyPath();
        //    collision.gameObject.GetComponent<EnemyArmyOnTheMap>().PrepairToTheBattle();
        //}
    }

}
