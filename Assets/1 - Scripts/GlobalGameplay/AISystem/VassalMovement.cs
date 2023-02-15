using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class VassalMovement : MonoBehaviour
{
    private MapBonusManager heapManager;
    //private EnemyManager enemyManager;
    //private AISystem aiSystem;
    //private GameObject player;

    private Vassal mainAI;
    private VassalAnimation animationScript;
    private VassalTargetSelector targetSelector;
    private VassalPathfinder pathfinder;

    [SerializeField] private int movementPoints = 4;
    private int currentMovementPoints;

    private float speed = 100f; //50 for build
    private float defaultCountSteps = 500;

    private bool shouldIStop = false;
    private bool shouldIChangePath = false;
    private bool shouldIFigth = false;
    private Vector3Int heapCell = Vector3Int.zero;
    private Vector3 finishCell = Vector3.zero;

    private Queue<Vector3> currentPath = new Queue<Vector3>();

    private Coroutine movementCoroutine;
    private WaitForSecondsRealtime decidingDelay = new WaitForSecondsRealtime(0.3f);
    private WaitForSecondsRealtime movingDelay = new WaitForSecondsRealtime(0.01f);

    public void Init(Vassal v, VassalTargetSelector ts, VassalAnimation animation, VassalPathfinder pf)
    {
        mainAI = v;
        targetSelector = ts;
        animationScript = animation;
        pathfinder = pf;

        heapManager = GlobalStorage.instance.mapBonusManager;
        //enemyManager = GlobalStorage.instance.enemyManager;
        //aiSystem = GlobalStorage.instance.aiSystem;
        //player = GlobalStorage.instance.globalPlayer.gameObject;
    }

    public int GetMovementPointsAmoumt(bool currentMP = true)
    {
        return (currentMP == true) ? movementPoints : currentMovementPoints;
    }

    public void ResetMovementPoints()
    {
        currentMovementPoints = movementPoints;
    }

    public void Movement(Queue<Vector3> path)
    {
        currentPath = path;

        if(movementCoroutine != null)
            StopCoroutine(movementCoroutine);

        movementCoroutine = StartCoroutine(Moving(path));
    }

    private IEnumerator Moving(Queue<Vector3> pathPoints)
    {
        if(pathPoints.Count == 0)
        {
            Debug.Log("Break moving");
            mainAI.EndOfMove();
            yield break;
        }
        //if(pathPoints.Count == 0)
        //{
        //    Debug.Log(transform.position + " === " + pathfinder.ConvertToV3(targetSelector.GetFinishCell()));
        //    if(transform.position == pathfinder.ConvertToV3(targetSelector.GetFinishCell()))
        //    {
        //        Debug.Log("I need new action.");
        //        targetSelector.GetNextAction();
        //    }
        //    else
        //    {
        //        Debug.Log("End Of Move.");
        //        mainAI.EndOfMove();
        //    }
        //    //targetSelector.GetNextAction();
        //    //mainAI.EndOfMove();
        //    yield break;
        //}


        pathfinder.DrawThePath(pathPoints);
        Vector3 previousPosition;
        Vector3 nextPoint = pathPoints.Dequeue();        

        yield return movingDelay;

        //for(int i = 1; i < pathPoints.Count; i++)
        while(pathPoints.Count > 0)
        {
            //Debug.Log(i + "/" + pathPoints.Count);
            if(currentMovementPoints == 0) break;

            previousPosition = nextPoint;
            nextPoint = pathPoints.Dequeue();

            animationScript.FlipSprite(previousPosition.x - nextPoint.x < 0);

            if(shouldIChangePath == false)
            {
                shouldIChangePath = CheckHeapNearBy(nextPoint);
                if(shouldIChangePath == true)
                    break;
            }

            shouldIChangePath = false;

            if(pathfinder.CheckEnemy(nextPoint) == true)
            {
                //if(pathPoints.Count <= 1)
                //{
                //    pathPoints.Clear();
                //    Debug.Log("Path aborted.");
                //    break;
                //}

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

            Vector3 distance = nextPoint - transform.position;
            Vector3 step = distance / (defaultCountSteps / speed);

            for(float t = 0; t < defaultCountSteps / speed; t++)
            {
                transform.position += step;
                yield return movingDelay;
            }

            currentMovementPoints--;

            transform.position = nextPoint;
            //Debug.Log(transform.position + " === " + pathPoints[pathPoints.Count - 1]  + "(" + pathPoints.Count + ")");
            //previousPosition = pathPoints[i - 1];

            if(shouldIChangePath == true)
                break;
        }
        
        //int index = 0;
        //while(transform.position != pathPoints[index])
        //{
        //    pathPoints.Remove(pathPoints[index]);
        //}

        if(shouldIFigth == true)
        {
            yield return decidingDelay;
            Figth();
        }
        else
        {
            if(pathfinder.CheckPlayerNearBy() == true && targetSelector.GetAgressiveMode() == false)
            {
                if(targetSelector.GetCurrentTarget() != AITargetType.ToTheOwnCastle)
                {
                    targetSelector.SelectSpecialTarget(AITargetType.ToTheOwnCastle);
                    targetSelector.GetNextAction();
                    yield break;
                }
            }

            if(currentMovementPoints == 0)
            {
                targetSelector.CheckNextTarget();
                //mainAI.EndOfMove();
                yield break;
            }

            if(shouldIChangePath == true)
            {
                CreatePathAndMoveToHeap();
                yield break;
            }

            //if(shouldIRun == true)
            //{
            //    targetSelector.SelectSpecialTarget(AITargetType.ToTheOwnCastle);
            //    targetSelector.GetNextAction();
            //    yield break;
            //}

            if(pathPoints.Count <= 1)
            {
                Debug.Log("End of path");
                if(pathfinder.ConvertToV3Int(transform.position) == targetSelector.GetFinishCell())
                {
                    Debug.Log("positions are equals");
                    //Debug.Log("I need new action.");
                    targetSelector.GetNextAction();
                }
                else
                {
                    Debug.Log(pathfinder.ConvertToV3Int(transform.position));
                    Debug.Log(targetSelector.GetFinishCell());
                    Debug.Log("positions are not equals");
                    BackToMainTarget();                    
                }

                yield break;
            }

        }
    }

    private void CreatePathAndMoveToHeap()
    {
        //Debug.Log("To the heap");
        Queue<Vector3> pathPoints = pathfinder.CreatePath(heapCell);
        Movement(pathPoints);
    }

    private void BackToMainTarget()
    {
        //Debug.Log("Back to the target");
        Queue<Vector3> pathPoints = pathfinder.CreatePath(targetSelector.GetFinishCell());
        Movement(pathPoints);
    }

    private void Figth()
    {
        mainAI.StartFigth();
        shouldIFigth = false;
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

    #region CHECKERS

    public bool CheckHeapNearBy(Vector3 nextPoint)
    {
        GMHexCell checkCell = pathfinder.GetCell(transform.position);
        GMHexCell[] neighbors = checkCell.neighbors;

        foreach(var pos in neighbors)
        {
            if(pos == null) continue;

            if(pos.coordinates == pathfinder.ConvertToV3Int(nextPoint))
                continue;

            Vector3 checkPos = pathfinder.ConvertToV3(pos.coordinates);
            if(heapManager.IsHeapOnPosition(checkPos) == true)
            {
                heapCell = pos.coordinates;
                return true;
            }
        }

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
