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
    private WaitForSeconds delay =  new WaitForSeconds(0.75f);

    private Color vassalColor;
    private string vassalName;

    private bool isTurnPaused = false;

    public void Init(EnemyCastle castle, Color color, string name)
    {
        myCastle = castle;
        vassalColor = color;
        vassalName = name;
        gameObject.name = vassalName;

        gmInterface = GlobalStorage.instance.gmInterface;

        GetComponent<TooltipTrigger>().content = vassalName;
        enemyArmy = GetComponent<EnemyArmyOnTheMap>();

        targetSelector = GetComponent<VassalTargetSelector>();
        pathfinder     = GetComponent<VassalPathfinder>();
        movement       = GetComponent<VassalMovement>();
        animScript     = GetComponent<VassalAnimation>();

        targetSelector.Init(this, pathfinder, movement, animScript);
        pathfinder.Init(targetSelector, movement);
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
            CreateArmy();
        }

        SetCameraOnVassal();
        gmInterface.turnPart.FillMessage(true, vassalName, vassalColor);

        movement.ResetMovementPoints();

        Action();
    }

    public void ContinueTurn(bool isVassalWin)
    {
        SetCameraOnVassal();
        targetSelector.EndOfSiege(isVassalWin, false);        

        Action(true);
    }

    public void Action(bool continueMode = false)
    {
        if(continueMode == true)
            targetSelector.GetNextAction();
        else
            targetSelector.SelectTarget();
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

    public void CrusadeIsOver(bool deathMode = false)
    {
        EndOfMove(true);
        myCastle.GiveMyABreak(deathMode);
    }

    public void StartFigth()
    {
        enemyArmy.PrepairToTheBattle(true);
    }

    #region GETTINGS & SETTINGS

    private void CreateArmy()
    {
        if(enemyArmy == null) 
            enemyArmy = GetComponent<EnemyArmyOnTheMap>();

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

    public void SetCameraOnVassal()
    {
        gmCamera.SetObserveObject(gameObject);
    }

    public EnemyArmyOnTheMap GetArmy()
    {
        return enemyArmy;
    }
    #endregion
}
