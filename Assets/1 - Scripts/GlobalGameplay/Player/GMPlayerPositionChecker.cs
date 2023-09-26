using UnityEngine;
using Zenject;
using static NameManager;

public class GMPlayerPositionChecker : MonoBehaviour
{
    private GlobalMapPathfinder gmPathFinder;
    private GMPlayerMovement gmMovement;
    private EnemyManager enemyManager;
    private AISystem aiSystem;

    [Inject]
    public void Construct(
        [Inject(Id = Constants.GLOBAL_MAP)] GameObject globalMap,
        GMPlayerMovement gmMovement,
        EnemyManager enemyManager,
        AISystem aiSystem
        )
    {
        this.gmMovement = gmMovement;
        this.enemyManager = enemyManager;
        this.aiSystem = aiSystem;

        this.gmPathFinder = globalMap.GetComponent<GlobalMapPathfinder>();
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
            foreach(var vassal in aiSystem.GetVassalsInfo())
            {
                if(Vector3.Distance(position, vassal.transform.position) < 0.01f)
                {
                    if(fightMode == true)
                    {
                        EnemyArmyOnTheMap vassalArmy = vassal.GetComponent<EnemyArmyOnTheMap>();
                        gmMovement.StopMoving();
                        vassalArmy.PrepairToTheBattle();
                    }
                    return true;
                }
            }

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
