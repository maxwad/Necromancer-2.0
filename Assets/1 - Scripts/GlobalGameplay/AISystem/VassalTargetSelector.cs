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
    List<Vector3> currentPath = new List<Vector3>();

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

    public void HandleTarget(AITargetType target)
    {
        if(currentTarget == AITargetType.Rest)
        {
            currentTarget = target;
            Debug.Log("New target");

            FindPathToRandomCell();
            animScript.ShowAction(target.ToString());

            switch(target)
            {
                case AITargetType.Rest:
                    return;

                case AITargetType.Walking:
                    //FindPathToRandomCell();

                    break;
                case AITargetType.CastleAttack:

                    break;
                case AITargetType.ResBuildingAttack:

                    break;
                case AITargetType.PlayerAttack:

                    break;
                case AITargetType.ArmyDescent:

                    break;
                case AITargetType.ToTheTeleport:

                    break;
                case AITargetType.ToTheResource:

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

    private void FindPathToRandomCell()
    {
        Vector3Int finishPoint = pathfinder.FindRandomCell();
        currentPath = pathfinder.GetPath();

        if(finishPoint != Vector3Int.zero && currentPath.Count != 0)
        {
            movement.Movement(currentPath);
        }
        else
        {
            mainAI.SelectTarget();        
        }
    }

    public void EndOfMove()
    {
        mainAI.EndOfMove();
    }
}
