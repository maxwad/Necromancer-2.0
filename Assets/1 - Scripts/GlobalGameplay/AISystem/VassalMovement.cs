using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class VassalMovement : MonoBehaviour
{
    private Vassal mainAI;
    private VassalAnimation animationScript;
    private VassalTargetSelector targetSelector;
    private VassalPathfinder pathfinder;

    [SerializeField] private int movementPoints = 20;
    private int extraPoints = 5;
    private int currentMovementPoints;

    private float speed = 300f; //50 for build
    private float defaultCountSteps = 500; //between cells

    private bool shouldIFigth = false;

    private Coroutine movementCoroutine;
    private WaitForSecondsRealtime decidingDelay = new WaitForSecondsRealtime(0.3f);
    private WaitForSecondsRealtime movingDelay = new WaitForSecondsRealtime(0.01f);

    public void Init(Vassal v, VassalTargetSelector ts, VassalAnimation animation, VassalPathfinder pf)
    {
        mainAI          = v;
        targetSelector  = ts;
        animationScript = animation;
        pathfinder      = pf;
    }

    public int GetMovementPointsAmoumt(bool currentMP = true)
    {
        return (currentMP == true) ? movementPoints : currentMovementPoints;
    }

    public void ResetMovementPoints()
    {
        currentMovementPoints = movementPoints;
    }

    public void AddExtraMovementPoints()
    {
        currentMovementPoints += extraPoints;
    }

    public void Movement(Queue<Vector3> path)
    {
        if(movementCoroutine != null)
            StopCoroutine(movementCoroutine);

        movementCoroutine = StartCoroutine(Moving(path));
    }

    private IEnumerator Moving(Queue<Vector3> pathPoints)
    {
        if(ShouldIAttackPlayer() == true)
        {
            targetSelector.SelectSpecialTarget(AITargetType.PlayerAttack);
            targetSelector.GetNextAction();
            yield break;
        }

        if(ShouldIContinuingTarget() == false)
        {
            targetSelector.SelectSpecialTarget(AITargetType.ToTheOwnCastle);
            targetSelector.GetNextAction();
            yield break;
        }

        if(pathPoints.Count == 0)
        {
            Debug.Log("Break moving");
            targetSelector.BreakMove();
            yield break;
        }

        pathfinder.DrawThePath(pathPoints);
        Vector3 previousPosition;
        Vector3 nextPoint = pathPoints.Dequeue();        

        yield return movingDelay;

        while(pathPoints.Count > 0)
        {
            if(currentMovementPoints == 0) break;

            previousPosition = nextPoint;
            nextPoint = pathPoints.Dequeue();

            Vector3Int heapCell = pathfinder.CheckHeapNearBy(nextPoint);
            if(heapCell != Vector3Int.zero)
            {
                pathPoints = pathfinder.AddHeapCellToThePath(heapCell);
                previousPosition = nextPoint;
                nextPoint = pathPoints.Dequeue();
            }

            if(pathfinder.CheckEnemy(nextPoint) == true)
            {
                if(currentMovementPoints == 1)
                {
                    currentMovementPoints = 0;
                    Debug.Log("To far to next cell. I'll wait next move.");
                    break;
                }
            }

            if(pathfinder.CheckPlayerInCell(nextPoint) == true)
            {
                mainAI.SetTurnStatus(true);
                shouldIFigth = true;
                break;
            }

            animationScript.SetFlipProperty(previousPosition.x - nextPoint.x < 0);
            Vector3 distance = nextPoint - transform.position;
            Vector3 step = distance / (defaultCountSteps / speed);

            for(float t = 0; t < defaultCountSteps / speed; t++)
            {
                transform.position += step;
                yield return movingDelay;
            }

            currentMovementPoints--;
            transform.position = nextPoint;
        }


        if(shouldIFigth == true)
        {
            yield return decidingDelay;
            Figth();
        }
        else
        {
            if(currentMovementPoints == 0)
            {               
                mainAI.EndOfMove();
                yield break;
            }                        

            if(pathPoints.Count <= 1)
            {
                targetSelector.GetNextAction();
                yield break;
            }
        }
    }

    private void Figth()
    {
        mainAI.StartFigth();
        shouldIFigth = false;
    }

    private bool ShouldIAttackPlayer()
    {
        if(targetSelector.GetAgressiveMode() == false)
            return false;

        if(targetSelector.GetCurrentTarget() != AITargetType.PlayerAttack &&
            pathfinder.CheckPlayerNearBy(true) == true)
            return true;

        return false;
    }

    private bool ShouldIContinuingTarget()
    {
        //checking for walking
        if(targetSelector.GetCurrentTarget() == AITargetType.Walking && 
            pathfinder.CheckPlayerNearBy(false) == true)
            return false;

        //checking player chase tries
        if((targetSelector.GetCurrentTarget() == AITargetType.ResBuildingAttack ||
            targetSelector.GetCurrentTarget() == AITargetType.CastleAttack) &&
            targetSelector.SpendTry() == false)
            return false;

        //cheking building on siege while moving to
        if((targetSelector.GetCurrentTarget() == AITargetType.ResBuildingAttack ||
            targetSelector.GetCurrentTarget() == AITargetType.CastleAttack) &&
            targetSelector.CheckSiegeStatus() == true)
            return false;

        return true;
    }

    public void Teleportation(Vector3Int newPosition)
    {
        Vector3 destination = pathfinder.ConvertToV3(newPosition);

        StartCoroutine(Teleport(destination));
    }

    private IEnumerator Teleport(Vector3 newPosition)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(2.5f);
        animationScript.Fading(true);

        yield return delay;

        gameObject.transform.position = newPosition;
        mainAI.SetCameraOnVassal();
        animationScript.Fading(false);

        yield return delay;        

        targetSelector.PrepareToRest(true); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(TagManager.T_Resource) == true)
        {
            ResourceObject heap = collision.GetComponent<ResourceObject>();
            heap.GetReward(false);
        }
    }
}
