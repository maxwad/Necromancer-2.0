using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static NameManager;

public class VassalTargetSelector : MonoBehaviour
{
    private VassalPathfinder pathfinder;

    [SerializeField] private int actionRadius = 50;
    [SerializeField] private int searchPlayerRadius = 15;
    [SerializeField] private float movementPoints;

    //private Tilemap roadMap;
    //[SerializeField] private Tile testTile;

    private Vector3Int startPoint;


    public void HandleTarget(AITargetType target)
    {
        if(pathfinder == null)
        {
            //roadMap = GlobalStorage.instance.roadMap;
            pathfinder = GetComponent<VassalPathfinder>();
            movementPoints = pathfinder.GetMovementPoints();
            pathfinder.Init();
        }

        //startPoint = roadMap.WorldToCell(gameObject.transform.position);


        pathfinder.FindRandomCell();
        switch(target)
        {
            case AITargetType.Rest:
                return;

            case AITargetType.Walking:
                //pathfinder.FindRandomCell(startPoint);
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

    private void FindRandomCell()
    {
        List<Vector3Int> cells = new List<Vector3Int>();

    }
}
