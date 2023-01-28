using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class Vassal : MonoBehaviour
{
    private EnemyCastle myCastle;
    private EnemyArmyOnTheMap enemyArmy;

    private VassalAnimation animScript;
    private VassalTargetSelector targetSelector;
    private VassalPathfinder pathfinder;
    private VassalMovement movement;

    private Vector3 startPosition;
    private AIState currentState = AIState.Rest;
    private AITargetType currentTarget = AITargetType.Rest;
    private List<AITargetType> stateList;

    public void Init(EnemyCastle castle, Color castleColor, string name)
    {
        myCastle = castle;

        GetComponent<TooltipTrigger>().content = name;
        enemyArmy = GetComponent<EnemyArmyOnTheMap>();

        targetSelector = GetComponent<VassalTargetSelector>();
        pathfinder = GetComponent<VassalPathfinder>();
        movement = GetComponent<VassalMovement>();
        animScript = GetComponent<VassalAnimation>();

        targetSelector.Init(this, pathfinder, movement);
        pathfinder.Init(movement);
        movement.Init(animScript);
        animScript.Init(castleColor);
    }
    
    public void StartAction()
    {
        if(startPosition == Vector3.zero)
        {
            Debug.Log("StartAction");
            //targetSelector = GetComponent<VassalTargetSelector>();
            ////targetSelector.Init(animScript);

            //pathfinder = GetComponent<VassalPathfinder>();
            //pathfinder.Init();

            startPosition = myCastle.GetStartPosition();
        }

        transform.position = startPosition;

        animScript.Activate(true);
        GetArmy();
        SelectTarget();
    }

    private void GetArmy()
    {
        if(enemyArmy == null) enemyArmy = GetComponent<EnemyArmyOnTheMap>();

        enemyArmy.Birth();
    }

    public AIState GetVassalState()
    {
        return currentState;
    }

    public void SelectTarget()
    {
        currentTarget = (AITargetType)UnityEngine.Random.Range(1, Enum.GetValues(typeof(AITargetType)).Length);
        targetSelector.HandleTarget(this, currentTarget);
    }
}
