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
        //if(pathfinder == null)
        //{
        //    mainAI = vassal;
        //    //roadMap = GlobalStorage.instance.roadMap;
        //    pathfinder = GetComponent<VassalPathfinder>();
        //    movementPoints = pathfinder.GetMovementPoints();
        //    //pathfinder.Init();
        //}

        //startPoint = roadMap.WorldToCell(gameObject.transform.position);


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

    private void FindPathToRandomCell()
    {
        List<Vector3> path;

        Vector3Int finishPoint = pathfinder.FindRandomCell();
        path = pathfinder.GetPath();

        if(finishPoint != Vector3Int.zero && path.Count != 0)
        {
            movement.Movement(path);
        }
        else
        {
            mainAI.SelectTarget();        
        }
    }
}
