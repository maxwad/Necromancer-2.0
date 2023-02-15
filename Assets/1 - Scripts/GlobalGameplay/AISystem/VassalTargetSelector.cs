using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static NameManager;

public partial class VassalTargetSelector : MonoBehaviour
{
    private Vassal mainAI;
    private VassalPathfinder pathfinder;
    private VassalMovement movement;
    private VassalAnimation animScript;
    private EnemyArmyOnTheMap vassalsArmy;
    private EnemyManager enemyManager;
    private GameObject player;

    private AITargetType currentTarget = AITargetType.Rest;
    //private AIState currentState = AIState.Nothing;
    //private List<AIActions> currentActions = new List<AIActions>();
    private Queue<AIActions> currentActionsQ = new Queue<AIActions>();
    private AIActions currentAction = AIActions.End;

    private Queue<Vector3> currentPath = new Queue<Vector3>();
    private Vector3Int finishCell = Vector3Int.zero;
    private ResourceBuilding currentSiegeTarget = null;
    private int startSiegeAmountArmy;
    private float criticalArmySize = 5;

    private bool shouldIContinueAction = false;
    private bool aggressiveMode = false;
    private int tryToActMax = 3;
    private int currentTryToAct = 0;

    private WaitForSeconds delay = new WaitForSeconds(0.15f);


    public void Init(Vassal vassal, VassalPathfinder pf, VassalMovement mv, VassalAnimation anim)
    {
        mainAI = vassal;
        pathfinder = pf;
        movement = mv;
        animScript = anim;

        vassalsArmy = GetComponent<EnemyArmyOnTheMap>();
        enemyManager = GlobalStorage.instance.enemyManager;
        player = GlobalStorage.instance.globalPlayer.gameObject;
    }

    public void SelectRandomTarget()
    {
        if(shouldIContinueAction == false)
        {
            shouldIContinueAction = true;
            // change delta for skipping some target
            //currentTarget = (AITargetType)UnityEngine.Random.Range(1, Enum.GetValues(typeof(AITargetType)).Length - (delta = 3));
            currentTarget = AITargetType.CastleAttack;
            CreateActionsQueue();
            GetNextAction();
        }
        else
        {
            HandleAction();
        }
    }

    public void SelectSpecialTarget(AITargetType target)
    {
        currentTarget = target;
        CreateActionsQueue();
    }

    public void CheckNextTarget()
    {
        if(currentTarget == AITargetType.PlayerAttack && currentAction == AIActions.Moving)
        {
            SelectSpecialTarget(AITargetType.PlayerAttack);
            currentAction = currentActionsQ.Dequeue();
        }

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
                break;

            case AITargetType.Walking:
                currentActionsQ.Enqueue(AIActions.SearchSomePoint);
                currentActionsQ.Enqueue(AIActions.Moving);
                EnableAgressiveMode(false);
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
                currentActionsQ.Enqueue(AIActions.SearchPlayer);
                currentActionsQ.Enqueue(AIActions.Moving);
                isCamebackNeeded = false;
                break;

            case AITargetType.ArmyDescent:
                currentActionsQ.Enqueue(AIActions.SearchSomePoint);
                currentActionsQ.Enqueue(AIActions.Moving);
                currentActionsQ.Enqueue(AIActions.ArmyDescent);
                break;

            case AITargetType.Death:
                currentActionsQ.Enqueue(AIActions.SearchOwnCastle);
                currentActionsQ.Enqueue(AIActions.TeleportToCastle);
                isCamebackNeeded = false;
                EnableAgressiveMode(false);
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
        currentAction = currentActionsQ.Dequeue();
        HandleAction();
    }

    public void HandleAction()
    {
        StartCoroutine(HandleActionCoroutine());
    }

    private IEnumerator HandleActionCoroutine()
    {
        yield return delay;

        switch(currentAction)
        {
            case AIActions.SearchSomePoint:
                FindPathToRandomCell();
                GetNextAction();
                break;

            case AIActions.SearchOwnCastle:
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
                UpdatePath();
                movement.Movement(currentPath);
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

        animScript.ShowAction(currentTarget + ":\n " + currentAction);
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
}
