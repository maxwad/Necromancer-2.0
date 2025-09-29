using System.Collections;
using UnityEngine;
using Zenject;
using Enums;

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

    [Inject]
    public void Construct(GMInterface gmInterface)
    {
        this.gmInterface = gmInterface;

        enemyArmy = GetComponent<EnemyArmyOnTheMap>();
        targetSelector = GetComponent<VassalTargetSelector>();
        pathfinder     = GetComponent<VassalPathfinder>();
        movement       = GetComponent<VassalMovement>();
        animScript     = GetComponent<VassalAnimation>();
        gmCamera = Camera.main.GetComponent<GlobalCamera>();
    }

    public void Init(EnemyCastle castle, Color color, string name)
    {
        myCastle = castle;
        vassalColor = color;
        vassalName = name;
        gameObject.name = vassalName;

        GetComponent<TooltipTrigger>().content = vassalName;

        targetSelector.Init(this, pathfinder, movement, animScript);
        pathfinder.Init(targetSelector);
        movement.Init(this, targetSelector, animScript, pathfinder);
        animScript.Init(vassalColor);
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
        SetTurnStatus(false);
        targetSelector.AddExtraMovementPoints();
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
        myCastle.GiveMyABreak(deathMode);
        EndOfMove(true);
    }

    public void StartFigth()
    {
        enemyArmy.PrepairToTheBattle(true);
    }

    #region GETTINGS & SETTINGS

    public void SetNewActionParameters()
    {
        targetSelector.IncreaseActionRadius();
    }

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



    #region SAVE/LOAD

    public VassalSD SaveData()
    {
        VassalSD vassalSD = targetSelector.SaveData();

        vassalSD.vassalArmy = enemyArmy.SaveEnemy();

        vassalSD.isFlipped = animScript.GetFlipProperty();
        vassalSD.isVassalActive = gameObject.activeInHierarchy;
        vassalSD.vassalPosition = gameObject.transform.position.ToVec3();

        return vassalSD;
    }

    public void LoadData(VassalSD vassalSD)
    {
        enemyArmy.LoadEnemy(vassalSD.vassalArmy);

        animScript.SetFlipProperty(vassalSD.isFlipped);
        gameObject.SetActive(vassalSD.isVassalActive);
        gameObject.transform.position = vassalSD.vassalPosition.ToVector3();

        targetSelector.LoadData(vassalSD);
    }

    #endregion
}
