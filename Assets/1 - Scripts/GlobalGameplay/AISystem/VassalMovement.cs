using System;
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

    private Vassal mainAI;
    private VassalAnimation animationScript;
    private VassalTargetSelector targetSelector;
    private VassalPathfinder pathfinder;

    [SerializeField] private int movementPoints = 4;
    private int currentMovementPoints;

    private float speed = 50f; //50 for build
    private float defaultCountSteps = 500;

    private bool shouldIStop = false;
    private bool shouldIChangePath = false;
    private bool shouldIFigth = false;
    private Vector3Int heapCell = Vector3Int.zero;
    private Vector3 finishCell = Vector3.zero;

    private List<Vector3> currentPath = new List<Vector3>();

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

    public void Movement(List<Vector3> path, bool resetMovePointsMode = true)
    {
        currentPath = path;

        //if(resetMovePointsMode == true)
        //    ResetMovementPoints();

        if(movementCoroutine != null)
            StopCoroutine(movementCoroutine);

        Debug.Log("StarT MOVING " + path.Count);

        movementCoroutine = StartCoroutine(Moving(path));
    }

    private IEnumerator Moving(List<Vector3> pathPoints)
    {
        if(pathPoints.Count == 0)
        {
            targetSelector.GetNextAction();
            yield break;
        }

        Debug.Log("1 Check");
        Vector3 previousPosition = pathPoints[0];

        pathfinder.DrawThePath(pathPoints);

        Debug.Log("2 Check");
        yield return movingDelay;

        for(int i = 1; i < pathPoints.Count; i++)
        {
            animationScript.FlipSprite(previousPosition.x - pathPoints[i].x < 0);
            Debug.Log("3 Check");
            if(currentMovementPoints == 0) break;
            Debug.Log("4 Check");
            if(shouldIChangePath == false)
            {
                shouldIChangePath = CheckHeapNearBy(pathPoints[i]);

                if(shouldIChangePath == true)
                    break;
            }
            Debug.Log("5 Check");
            shouldIChangePath = false;


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
            Debug.Log("6 Check");
            if(CheckPlayer(pathPoints[i]) == true)
            {
                mainAI.SetTurnStatus(true);
                shouldIFigth = true;
                break;
            }
            Debug.Log("7 Check");
            Vector3 distance = pathPoints[i] - transform.position;
            Vector3 step = distance / (defaultCountSteps / speed);

            for(float t = 0; t < defaultCountSteps / speed; t++)
            {
                Debug.Log("8 Check");
                transform.position += step;
                yield return movingDelay;
            }

            currentMovementPoints--;
            Debug.Log("9 Check");
            transform.position = pathPoints[i];
            previousPosition = pathPoints[i - 1];

            if(shouldIChangePath == true)
                break;
        }

        Debug.Log("10 Check");
        int index = 0;
        while(transform.position != pathPoints[index])
        {
            pathPoints.Remove(pathPoints[index]);
        }

        Debug.Log("11 Check");
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

            if(pathPoints.Count == 1 && shouldIChangePath == false)
            {            
                if(transform.position == pathfinder.ConvertToV3(targetSelector.GetFinishCell()))
                {
                    Debug.Log("I need new action.");
                    targetSelector.GetNextAction();
                }
                else
                {
                    BackToMainTarget(pathPoints);                    
                }

                yield break;
            }
            Debug.Log("13 Check");
            if(shouldIChangePath == true)
                CreatePathAndMoveToHeap(pathPoints);
            Debug.Log("14 Check");
        }

        Debug.Log("12 Check");
    }

    private void CreatePathAndMoveToHeap(List<Vector3> pathPoints)
    {
        pathPoints = pathfinder.CreatePath(heapCell);
        Movement(pathPoints, false);
    }

    private void BackToMainTarget(List<Vector3> pathPoints)
    {
        pathPoints = pathfinder.CreatePath(targetSelector.GetFinishCell());
        Movement(pathPoints, false);
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
