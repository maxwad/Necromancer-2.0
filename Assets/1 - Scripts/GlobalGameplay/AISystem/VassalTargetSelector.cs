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

    [SerializeField] private int actionRadius = 50;
    [SerializeField] private int searchPlayerRadius = 15;

    private AITargetType currentTarget = AITargetType.Rest;
    private AIState currentState = AIState.Nothing;
    //private List<AIActions> currentActions = new List<AIActions>();
    private Queue<AIActions> currentActionsQ = new Queue<AIActions>();
    private AIActions currentAction = AIActions.End;

    private List<Vector3> currentPath = new List<Vector3>();
    private Vector3Int finishCell = Vector3Int.zero;

    private bool shouldIContinueAction = false;
    private bool aggressiveMode = false;

    private WaitForSeconds delay = new WaitForSeconds(0.75f);


    public void Init(Vassal vassal, VassalPathfinder pf, VassalMovement mv, VassalAnimation anim)
    {
        mainAI = vassal;
        pathfinder = pf;
        movement = mv;
        animScript = anim;
    }

    public void SelectRandomTarget()
    {
        if(shouldIContinueAction == false)
        {
            shouldIContinueAction = true;
            //currentTarget = (AITargetType)UnityEngine.Random.Range(1, Enum.GetValues(typeof(AITargetType)).Length - 3);
            currentTarget = AITargetType.Walking;
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

    private void CreateActionsQueue()
    {
        aggressiveMode = true;
        bool isCamebackNeeded = true;
        currentActionsQ.Clear();

        switch(currentTarget)
        {
            case AITargetType.Rest:
                isCamebackNeeded = false;
                aggressiveMode = false;
                break;

            case AITargetType.Walking:
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
                currentActionsQ.Enqueue(AIActions.SearchPlayer);
                currentActionsQ.Enqueue(AIActions.Moving);
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
                aggressiveMode = false;
                break;

            case AITargetType.ToTheOwnCastle:
                aggressiveMode = false;
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

    public void SetState(AIState state)
    {
        currentState = state;
    }

    public void GetNextAction()
    {
        currentAction = currentActionsQ.Dequeue();
        Debug.Log(gameObject.name + " got action: " + currentAction);
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

            case AIActions.Moving:
                SetState(AIState.Moving);
                movement.Movement(currentPath, false);
                break;

            case AIActions.SearchOwnCastle:
                FindPathToTheCastle();
                GetNextAction();
                break;

            case AIActions.TeleportToCastle:
                TeleportingToTheCastle();
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
}
