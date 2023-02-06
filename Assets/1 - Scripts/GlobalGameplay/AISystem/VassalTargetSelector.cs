using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static NameManager;

public class VassalTargetSelector : MonoBehaviour
{
    private Vassal mainAI;
    private VassalPathfinder pathfinder;
    private VassalMovement movement;
    private VassalAnimation animScript;

    [SerializeField] private int actionRadius = 50;
    [SerializeField] private int searchPlayerRadius = 15;

    AITargetType currentTarget = AITargetType.Rest;
    AIState currentState = AIState.Nothing;
    List<Vector3> currentPath = new List<Vector3>();
    Vector3Int finishCell = Vector3Int.zero;

    private bool shouldIContinueAction = false;
    private bool aggressiveMode = false;
    //private Tilemap roadMap;
    //[SerializeField] private Tile testTile;

    //private Vector3Int finishPoint;

    public void Init(Vassal vassal, VassalPathfinder pf, VassalMovement mv, VassalAnimation anim)
    {
        mainAI = vassal;
        pathfinder = pf;
        movement = mv;
        animScript = anim;
    }

    public void SelectRandomTarget()
    {
        //currentTarget = (AITargetType)UnityEngine.Random.Range(1, Enum.GetValues(typeof(AITargetType)).Length - 1);
        currentTarget = AITargetType.Walking;
        animScript.ShowAction(currentTarget.ToString());
    }

    public void SelectSpecialTarget(AITargetType target)
    {
        currentTarget = target;
        animScript.ShowAction(target.ToString());
    }

    public void SetState(AIState state)
    {
        currentState = state;
    }

    public void HandleTarget()
    {
        if(shouldIContinueAction == false)
        {
            shouldIContinueAction = true;

            switch(currentTarget)
            {
                case AITargetType.Rest:
                    return;

                case AITargetType.Walking:
                    aggressiveMode = true;
                    SetState(AIState.Moving);
                    FindPathToRandomCell();
                    //shouldIContinueAction = true;
                    //FindPathToRandomCell();

                    break;
                case AITargetType.CastleAttack:

                    break;
                case AITargetType.ResBuildingAttack:

                    break;
                case AITargetType.PlayerAttack:
                    aggressiveMode = true;

                    break;
                case AITargetType.ArmyDescent:
                    aggressiveMode = true;

                    break;
                case AITargetType.ToTheResource:

                    break;
                case AITargetType.ToTheOwnCastle:

                    break;
                default:
                    break;
            }
        }
        else
        {
            Debug.Log("Continue target");
            movement.Movement(currentPath);
        }
    }

    public void GetNextTarget()
    {
        //shouldIContinueAction = false;
        //HandleTarget();

        switch(currentState)
        {
            case AIState.Nothing:
                break;

            case AIState.Moving:
                if(currentTarget == AITargetType.Walking)
                {
                    SetState(AIState.ToTheOwnCastle);
                    animScript.ShowAction("Back to The Castle");
                    FindPathToTheCastle();
                }

                break;

            case AIState.Siege:
                break;

            case AIState.ToTheOwnCastle:
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

    public bool ShouldIAttack()
    {
        return aggressiveMode;
    }    

    #region TASKS

    private void FindPathToRandomCell()
    {
        finishCell = pathfinder.FindRandomCell();
        //finishCell = new Vector3(cell.x, cell.y, cell.z);
        currentPath = pathfinder.GetPath();

        if(finishCell != Vector3Int.zero && currentPath.Count != 0)
            movement.Movement(currentPath);
        else
            SelectRandomTarget();
    }

    private void FindPathToTheCastle()
    {
        finishCell = pathfinder.ConvertToV3Int(mainAI.GetCastlePoint());
        currentPath = pathfinder.CreatePath(finishCell);
        aggressiveMode = false;

        movement.Movement(currentPath);
    }

    private void PrepareToRest()
    {
        currentTarget = AITargetType.Rest;
        shouldIContinueAction = false;
        aggressiveMode = false;
        currentPath.Clear();
        mainAI.CrusadeIsOver();
    }


    #endregion
}
