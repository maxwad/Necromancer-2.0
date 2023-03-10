using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public partial class VassalTargetSelector : MonoBehaviour
{
    private Vassal mainAI;
    private VassalPathfinder pathfinder;
    private VassalMovement movement;
    private VassalAnimation animScript;
    private EnemyArmyOnTheMap vassalsArmy;
    private EnemyManager enemyManager;
    private FortressBuildings fortress;
    private GameObject player;

    private List<AITargetType> targetList = new List<AITargetType>();
    private AITargetType currentTarget = AITargetType.Rest;
    private AIActions currentAction = AIActions.End;
    private Queue<AIActions> currentActionsQ = new Queue<AIActions>();

    private Queue<Vector3> currentPath = new Queue<Vector3>();
    private Vector3Int finishCell = Vector3Int.zero;

    private ResourceBuilding currentSiegeTarget = null;
    private int startSiegeAmountArmy;
    private float criticalArmyMultiplier = 5;

    private bool shouldIContinueAction = false;
    private bool aggressiveMode = false;

    [SerializeField] private int actionRadius = 15;
    [SerializeField] private int tryToGetTarget = 15;
    private float increasePercent = 0.3f;
    private int currentTriesToGetTarget = 0;

    private WaitForSeconds delay = new WaitForSeconds(0.15f);

    public void Init(Vassal vassal, VassalPathfinder pf, VassalMovement mv, VassalAnimation anim)
    {
        mainAI     = vassal;
        pathfinder = pf;
        movement   = mv;
        animScript = anim;

        vassalsArmy  = GetComponent<EnemyArmyOnTheMap>();
        enemyManager = GlobalStorage.instance.enemyManager;
        fortress     = GlobalStorage.instance.fortressBuildings;
        player       = GlobalStorage.instance.globalPlayer.gameObject;

        currentTriesToGetTarget = tryToGetTarget;

        CreateTargetList();
    }

    public void CreateTargetList()
    {
        targetList.Add(AITargetType.Walking);
        //targetList.Add(AITargetType.ResBuildingAttack);
        //targetList.Add(AITargetType.ArmyDescent);
        //targetList.Add(AITargetType.PlayerAttack);
        targetList.Add(AITargetType.CastleAttack);
    }

    public void IncreaseActionRadius()
    {
        actionRadius = (int)(actionRadius * (1 + increasePercent));
        tryToGetTarget = (int)(tryToGetTarget * (1 + increasePercent));
    }

    //public void InsertAction(AITargetType action, bool insertMode)
    //{
    //    if(insertMode == true)
    //    {
    //        if(targetList.Contains(action) == false)
    //            targetList.Add(action);
    //    }
    //    else
    //    {
    //        if(targetList.Contains(action) == true)
    //            targetList.Remove(action);
    //    }
    //}

    public void SelectTESTTarget()
    {
        Debug.Log(currentTarget + " is impossible now.");
        shouldIContinueAction = true;
        // change delta for skipping some target
        currentTarget = targetList[UnityEngine.Random.Range(0, targetList.Count)];
        Debug.Log("New Target: " + currentTarget);
        CreateActionsQueue();
        GetNextAction();
    }

    public void SelectTarget()
    {
        if(shouldIContinueAction == false)
        {
            shouldIContinueAction = true;
            // change delta for skipping some target
            currentTarget = targetList[UnityEngine.Random.Range(0, targetList.Count)];
            //currentTarget = AITargetType.CastleAttack;
            CreateActionsQueue();
            GetNextAction();
        }
        else
        {
            if(currentTarget == AITargetType.PlayerAttack && currentAction == AIActions.Moving)
            {
                SelectSpecialTarget(AITargetType.PlayerAttack);
                currentAction = currentActionsQ.Dequeue();
            }

            HandleAction();
        }
    }

    public void SelectSpecialTarget(AITargetType target)
    {
        currentTarget = target;
        CreateActionsQueue();
    }

    public void BreakMove()
    {
        if(transform.position == mainAI.GetCastlePoint())
            GetNextAction();
        else
            mainAI.EndOfMove();
    }

    private void CreateActionsQueue()
    {
        EnableAgressiveMode(true);
        bool isCamebackNeeded = true;
        currentActionsQ.Clear();

        switch(currentTarget)
        {
            case AITargetType.Rest:
                isCamebackNeeded = false;
                EnableAgressiveMode(false);
                currentActionsQ.Enqueue(AIActions.End);
                break;

            case AITargetType.Walking:
                EnableAgressiveMode(false);
                currentActionsQ.Enqueue(AIActions.SearchSomePoint);
                currentActionsQ.Enqueue(AIActions.Moving);
                break;

            case AITargetType.CastleAttack:
                currentActionsQ.Enqueue(AIActions.SearchPlayerCastle);
                currentActionsQ.Enqueue(AIActions.Moving);
                currentActionsQ.Enqueue(AIActions.Siege);
                break;

            case AITargetType.ResBuildingAttack:
                currentActionsQ.Enqueue(AIActions.SearchResBuilding);
                currentActionsQ.Enqueue(AIActions.Moving);
                currentActionsQ.Enqueue(AIActions.Siege);
                break;

            case AITargetType.PlayerAttack:
                isCamebackNeeded = false;
                currentActionsQ.Enqueue(AIActions.SearchPlayer);
                currentActionsQ.Enqueue(AIActions.Moving);
                break;

            case AITargetType.ArmyDescent:
                currentActionsQ.Enqueue(AIActions.SearchSomePoint);
                currentActionsQ.Enqueue(AIActions.Moving);
                currentActionsQ.Enqueue(AIActions.ArmyDescent);
                break;

            case AITargetType.Death:
                isCamebackNeeded = false;
                EnableAgressiveMode(false);
                currentActionsQ.Enqueue(AIActions.SearchOwnCastle);
                currentActionsQ.Enqueue(AIActions.TeleportToCastle);
                break;

            case AITargetType.ToTheOwnCastle:
                EnableAgressiveMode(false);
                break;

            default:
                break;
        }

        if(isCamebackNeeded == true)
        {
            currentActionsQ.Enqueue(AIActions.SearchOwnCastle);
            currentActionsQ.Enqueue(AIActions.Moving);
            currentActionsQ.Enqueue(AIActions.End);
        }
    }





    public void GetNextAction()
    {
        if(currentActionsQ.Count == 0)
        {
            Debug.Log("Warning! Abort of Vassal's Turn");
            mainAI.EndOfMove();
        }
        else
        {
            currentAction = currentActionsQ.Dequeue();
            HandleAction();
        }
    }

    public void HandleAction()
    {
        StartCoroutine(HandleActionCoroutine());
    }

    private IEnumerator HandleActionCoroutine()
    {
        animScript.ShowAction(currentTarget + ":\n " + currentAction);
        yield return delay;

        switch(currentAction)
        {
            case AIActions.SearchSomePoint:
                FindPathToRandomCell();
                GetNextAction();
                break;

            case AIActions.SearchOwnCastle:
                EnableAgressiveMode(false);
                FindPathToTheOwnCastle();
                GetNextAction();
                break;

            case AIActions.SearchPlayer:
                FindPathToThePlayer();
                GetNextAction();
                break;
            
            case AIActions.SearchResBuilding:
                FindPathToTheResBuilding();
                GetNextAction();
                break;    
                
            case AIActions.SearchPlayerCastle:
                FindPathToThePlayerCastle();
                GetNextAction();
                break;

            case AIActions.Moving:
                if(finishCell != Vector3Int.zero)
                {
                    UpdatePath();
                    movement.Movement(currentPath);
                }
                else
                {
                    Debug.Log("Finish is null");
                    shouldIContinueAction = false;
                    SelectTarget();
                }

                break;

            case AIActions.ArmyDescent:
                SplitArmy();
                GetNextAction();
                break;

            case AIActions.TeleportToCastle:
                TeleportingToTheCastle();
                break;

            case AIActions.Siege:
                Siege();
                break;

            case AIActions.End:
                PrepareToRest();
                break;

            default:
                break;
        }
    }

    public AITargetType GetCurrentTarget()
    {
        return currentTarget;
    }

    public Vector3Int GetFinishCell()
    {
        return finishCell;
    }      

    public bool GetAgressiveMode()
    {
        return aggressiveMode;
    }

    public void EnableAgressiveMode(bool enableMode)
    {
        aggressiveMode = enableMode;
    }

    public void SetCurrentSiegeTarget(ResourceBuilding building)
    {
        currentSiegeTarget = building;
    }

    public bool SpendTry()
    {
        currentTriesToGetTarget--;
        Debug.Log(currentTriesToGetTarget + " moves left");

        if(currentTriesToGetTarget == 0)
        {
            currentTriesToGetTarget = tryToGetTarget;
            return false;
        }

        return true;
    }

    public void AddExtraMovementPoints()
    {
        movement.AddExtraMovementPoints();
    }

    #region SAVE/LOAD

    public VassalSD SaveData()
    {
        VassalSD vassalSD = new VassalSD();

        vassalSD.shouldIContinueAction = shouldIContinueAction;
        vassalSD.aggressiveMode = aggressiveMode;

        vassalSD.currentTriesToGetTarget = currentTriesToGetTarget;

        if(finishCell != Vector3Int.zero)
            vassalSD.finishCell = pathfinder.ConvertToV3(finishCell).ToVec3();

        vassalSD.currentTarget = currentTarget;
        vassalSD.currentAction = currentAction;
        vassalSD.currentActionsList = new List<AIActions>(currentActionsQ);

        if(currentSiegeTarget != null)
            vassalSD.currentSiegeTargetPosition = currentSiegeTarget.transform.position.ToVec3();

        return vassalSD;
    }

    public void LoadData(VassalSD vassalSD)
    {
        shouldIContinueAction = vassalSD.shouldIContinueAction;
        aggressiveMode = vassalSD.aggressiveMode;

        currentTriesToGetTarget = vassalSD.currentTriesToGetTarget;

        Vector3Int cell = pathfinder.ConvertToV3Int(vassalSD.finishCell.ToVector3());
        if(cell != Vector3Int.zero)
        {
            finishCell = cell;
            currentPath = pathfinder.CreatePath(finishCell);
        }

        currentTarget = vassalSD.currentTarget;
        currentAction = vassalSD.currentAction;
        currentActionsQ = new Queue<AIActions>(vassalSD.currentActionsList);

        Vector3 targetCell = vassalSD.currentSiegeTargetPosition.ToVector3();
        if(targetCell != Vector3.zero)
            currentSiegeTarget = pathfinder.GetSiegeTarget(targetCell);

        animScript.ShowAction(currentTarget + ":\n " + currentAction);

    }
    #endregion
}
