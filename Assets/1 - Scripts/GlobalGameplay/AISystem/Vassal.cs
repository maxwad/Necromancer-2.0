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

    //private Vector3 startPosition;
    //private AIState currentState = AIState.Rest;
    //private AITargetType currentTarget = AITargetType.Rest;
    //private List<AITargetType> stateList;

    private Color vassalColor;
    private string vassalName;

    private bool isTurnPaused = false;

    public void Init(EnemyCastle castle, Color color, string name)
    {
        myCastle = castle;
        vassalColor = color;
        vassalName = name;

        gmInterface = GlobalStorage.instance.gmInterface;

        GetComponent<TooltipTrigger>().content = vassalName;
        enemyArmy = GetComponent<EnemyArmyOnTheMap>();

        targetSelector = GetComponent<VassalTargetSelector>();
        pathfinder     = GetComponent<VassalPathfinder>();
        movement       = GetComponent<VassalMovement>();
        animScript     = GetComponent<VassalAnimation>();

        targetSelector.Init(this, pathfinder, movement, animScript);
        pathfinder.Init(movement);
        movement.Init(this, targetSelector, animScript, pathfinder);
        animScript.Init(vassalColor);

        gmCamera = Camera.main.GetComponent<GlobalCamera>();
    }
    
    public void StartAction()
    {
        if(targetSelector.GetCurrentTarget() == AITargetType.Rest)
        {
            transform.position = GetCastlePoint();
            animScript.Activate(true);
            GetArmy();
            targetSelector.SelectRandomTarget();
        }

        gmCamera.SetObserveObject(gameObject);
        gmInterface.turnPart.FillMessage(vassalName, vassalColor);

        StartCoroutine(Action());
    }

    public void ContinueTurn(bool isVassalWin)
    {
        //Add some conditions
        AIState state = (isVassalWin == true) ? AIState.Moving : AIState.Dead;
        targetSelector.SetState(state);

        StartCoroutine(Action(true));
    }

    public IEnumerator Action(bool continueMode = false)
    {
        yield return delay;

        if(continueMode == true)
            targetSelector.GetNextTarget();
        else
            targetSelector.HandleTarget();
    }

    public void EndOfMove(bool crusadeIsEnd = false)
    {
        StartCoroutine(Deactivation(crusadeIsEnd));
    }

    public IEnumerator Deactivation(bool crusadeIsEnd)
    {
        yield return delay;

        myCastle.EndOfMove();

        if(crusadeIsEnd == true)
        {
            transform.position = GetCastlePoint();
            animScript.Activate(false);
        }
    }

    public void CrusadeIsOver()
    {
        EndOfMove(true);

        myCastle.GiveMyABreak();
    }

    public void StartFigth()
    {
        enemyArmy.PrepairToTheBattle(true);
    }

    #region GETTINGS & SETTINGS

    private void GetArmy()
    {
        if(enemyArmy == null) enemyArmy = GetComponent<EnemyArmyOnTheMap>();

        enemyArmy.Birth();
    }

    public Vector3 GetCastlePoint()
    {
        return myCastle.GetStartPosition();
    }

    public bool GetFightPauseStatus()
    {
        return isTurnPaused;
    }

    public void SetTurnStatus(bool mode)
    {
        isTurnPaused = mode;
    }

    #endregion
}
