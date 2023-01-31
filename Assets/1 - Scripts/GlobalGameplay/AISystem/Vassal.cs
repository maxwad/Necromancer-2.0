using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class Vassal : MonoBehaviour
{
    private GMInterface gmInterface;
    private EnemyCastle myCastle;
    private EnemyArmyOnTheMap enemyArmy;

    private VassalAnimation animScript;
    private VassalTargetSelector targetSelector;
    private VassalPathfinder pathfinder;
    private VassalMovement movement;

    private GlobalCamera gmCamera;
    private WaitForSeconds delay =  new WaitForSeconds(0.5f);

    private Vector3 startPosition;
    private AIState currentState = AIState.Rest;
    private AITargetType currentTarget = AITargetType.Rest;
    private List<AITargetType> stateList;

    private Color vassalColor;
    private string vassalName;

    public void Init(EnemyCastle castle, Color color, string name)
    {
        myCastle = castle;
        vassalColor = color;
        vassalName = name;

        gmInterface = GlobalStorage.instance.gmInterface;

        GetComponent<TooltipTrigger>().content = vassalName;
        enemyArmy = GetComponent<EnemyArmyOnTheMap>();

        targetSelector = GetComponent<VassalTargetSelector>();
        pathfinder = GetComponent<VassalPathfinder>();
        movement = GetComponent<VassalMovement>();
        animScript = GetComponent<VassalAnimation>();

        targetSelector.Init(this, pathfinder, movement, animScript);
        pathfinder.Init(movement);
        movement.Init(targetSelector, animScript);
        animScript.Init(vassalColor);

        gmCamera = Camera.main.GetComponent<GlobalCamera>();
    }
    
    public void StartAction()
    {
        if(currentState == AIState.Rest)
        {
            startPosition = myCastle.GetStartPosition();
            transform.position = startPosition;
        }

        //transform.position = startPosition;
        animScript.Activate(true);
        gmCamera.SetObserveObject(gameObject);

        gmInterface.turnPart.FillMessage(vassalName, vassalColor);
        StartCoroutine(Action());
    }

    public IEnumerator Action()
    {
        yield return delay;

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
        targetSelector.HandleTarget(currentTarget);
    }

    public void EndOfMove()
    {
        StartCoroutine(Deactivation());
    }

    public IEnumerator Deactivation()
    {
        yield return delay;

        myCastle.EndOfMove();
    }
}
