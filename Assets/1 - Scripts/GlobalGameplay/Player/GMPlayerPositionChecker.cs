using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GMPlayerPositionChecker : MonoBehaviour
{
    private GlobalMapPathfinder gmPathFinder;
    private GMPlayerMovement gmMovement;

    private void Start()
    {
        gmPathFinder = GlobalStorage.instance.globalMap.GetComponent<GlobalMapPathfinder>();
        gmMovement = GlobalStorage.instance.globalPlayer.GetComponent<GMPlayerMovement>();
    }

    public void CheckPosition(Vector3 currentPosition)
    {
        if(gmPathFinder.focusObject != null)
        {
            if(gmPathFinder.enterPointsDict[gmPathFinder.focusObject] == currentPosition)
            {
                gmPathFinder.focusObject.GetComponent<ClickableObject>().ActivateUIWindow(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(TagManager.T_Resource) == true)
        {
            ResourceObject obj = collision.GetComponent<ResourceObject>();
            EventManager.OnResourcePickedUpEvent(obj.resourceType, obj.quantity);
            Destroy(collision.gameObject);
        }

        if(collision.CompareTag(TagManager.T_GM_ENEMY) == true)
        {
            gmMovement.StopMoving();
            gmPathFinder.DestroyPath();
            //collision.gameObject.GetComponent<EnemyArmyOnTheMap>().PrepairToTheBattle();
        }
    }

}
