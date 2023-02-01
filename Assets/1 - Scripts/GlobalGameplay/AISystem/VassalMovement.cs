using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class VassalMovement : MonoBehaviour
{
    private MapBonusManager heapManager;
    private EnemyManager enemyManager;
    private AISystem aiSystem;
    private GameObject player;

    private VassalAnimation animationScript;
    private VassalTargetSelector targetSelector;
    private VassalPathfinder pathfinder;

    [SerializeField] private int movementPoints = 20;
    private int currentMovementPoints;

    private float speed = 25f; //50 for build
    private float defaultCountSteps = 500;

    private bool shouldIStop = false;

    public void Init(VassalTargetSelector ts, VassalAnimation animation, VassalPathfinder pf)
    {
        targetSelector = ts;
        animationScript = animation;
        pathfinder = pf;

        heapManager = GlobalStorage.instance.mapBonusManager;
        enemyManager = GlobalStorage.instance.enemyManager;
        aiSystem = GlobalStorage.instance.aiSystem;
        player = GlobalStorage.instance.globalPlayer.gameObject;
    }

    public int GetMovementPointsAmoumt()
    {
        return movementPoints;
    }

    public void ResetMovementPoints()
    {
        currentMovementPoints = movementPoints;
    }

    public void Movement(List<Vector3> path)
    {
        ResetMovementPoints();

        StartCoroutine(Moving(path));
    }

    private IEnumerator Moving(List<Vector3> pathPoints)
    {
        Vector3 previousPosition = pathPoints[0];
        Vector3 currentPosition = pathPoints[0];
        bool shouldICancelPath = false;

        pathfinder.DrawThePath(pathPoints);

        for(int i = 1; i < pathPoints.Count; i++)
        {
            animationScript.FlipSprite(previousPosition.x - pathPoints[i].x < 0);

            if(currentMovementPoints == 0) break;

            if(CheckEnemy(pathPoints[i]) == true)
            {
                if(pathPoints.Count - i == 0)
                {
                    pathPoints.Clear();
                    Debug.Log("Path aborted.");
                    break;
                }

                if(currentMovementPoints == 1)
                {
                    currentMovementPoints = 0;
                    Debug.Log("To far to next cell. I'll wait next move.");
                    break;
                }
            }

            if(CheckPlayer(pathPoints[i]) == true)
            {
                if(targetSelector.ShouldIAttack() == true)
                {
                    // if target = attack - attack
                }
                else
                {
                    // otherwise - to the Castle
                }

            }

            Vector3 distance = pathPoints[i] - transform.position;
            Vector3 step = distance / (defaultCountSteps / speed);

            for(float t = 0; t < defaultCountSteps / speed; t++)
            {
                transform.position += step;
                yield return new WaitForSeconds(0.01f);
            }

            currentMovementPoints--;

            transform.position = pathPoints[i];
            previousPosition = pathPoints[i - 1];
            currentPosition = pathPoints[i];

            shouldICancelPath = CheckHeapNearBy(pathPoints[i]);
            if(shouldICancelPath == true)
                break;
        }

        int index = 0;
        while(transform.position != pathPoints[index])
        {
            pathPoints.Remove(pathPoints[index]);
        }

        if(currentMovementPoints == 0)
        {
            targetSelector.EndOfMove();
        }

        if(pathPoints.Count == 0)
        {
            targetSelector.GetNextTarget();
        }

        if(shouldICancelPath == true)
        {

        }
    }

    #region CHECKERS

    public bool CheckEnemy(Vector3 position)
    {
        foreach(var vassal in aiSystem.GetVassalsInfo())
        {
            if(position == vassal.transform.position)
                return true;
        }

        return enemyManager.CheckPositionInEnemyPoints(position) != null;
    }

    public bool CheckPlayer(Vector3 position)
    {
        return position == player.transform.position;
    }

    public bool CheckHeapNearBy(Vector3 position)
    {
        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(TagManager.T_Resource) == true)
        {
            ResourceObject heap = collision.GetComponent<ResourceObject>();
            heap.GetReward(false);
        }
    }

    #endregion
}
