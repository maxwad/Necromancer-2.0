using System.Collections;
using System;
using UnityEngine;
using static NameManager;

public class GMPlayerMovement : MonoBehaviour
{
    private PlayerStats playerStats;
    private GMInterface gmInterface;
    private BattleManager battleManager;
    private GlobalMapPathfinder gmPathFinder;
    private GlobalMapTileManager mapTileManager;
    private GMPlayerPositionChecker positionChecker;
    private ResourcesManager resourcesManager;

    private float movementPointsMax = 0;
    private float currentMovementPoints = 0;
    private float extraMovementPoints = 0;
    private bool isExtraMovementWaisted = false;
    private float viewRadius = 0;
    private bool isFogNeeded = true;

    private float luck = 0;

    private float speed = 200f; //50 for build
    private float defaultCountSteps = 1000;

    private bool cancelMovement = false;

    private bool iAmMoving = false;

    private SpriteRenderer sprite;
    [SerializeField] private Color activeColor;
    [SerializeField] private Color inActiveColor;

    private Vector2 previousPosition = Vector2.zero;
    private Vector2 currentPosition = Vector2.zero;

    private void Awake()
    {
        sprite       = GetComponent<SpriteRenderer>();
        sprite.color = activeColor;

        playerStats      = GlobalStorage.instance.playerStats;
        gmInterface      = GlobalStorage.instance.gmInterface;
        mapTileManager   = GlobalStorage.instance.gmManager;
        gmPathFinder     = GlobalStorage.instance.globalMap.GetComponent<GlobalMapPathfinder>();
        resourcesManager = GlobalStorage.instance.resourcesManager;
        positionChecker  = GetComponent<GMPlayerPositionChecker>();
        battleManager    = GlobalStorage.instance.battleManager;
    }

    private void Start()
    {        
        SetParameters();

        currentPosition = gameObject.transform.position;
        ChangeMovementPoints(movementPointsMax);
    }

    //private void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.PageUp))
    //    {
    //        ChangeMovementPoints(3);
    //    }
    //}

    public Vector3 GetCurrentPosition() => currentPosition;

    private void SetParameters()
    {
        movementPointsMax = (float)Math.Round(playerStats.GetCurrentParameter(PlayersStats.MovementDistance), MidpointRounding.AwayFromZero);
        extraMovementPoints = playerStats.GetCurrentParameter(PlayersStats.ExtraMovementPoints);
        viewRadius = movementPointsMax;
        luck = playerStats.GetCurrentParameter(PlayersStats.Luck);
    }

    public float[] GetParametres()
    {
        return new float[] {movementPointsMax, currentMovementPoints};
    }

    private void NewTurn() 
    {
        StartCoroutine(StartNewTurn());

        IEnumerator StartNewTurn()
        {
            StopMoving();

            while(iAmMoving == true)
            {
                yield return null;
            }

            ChangeMovementPoints(movementPointsMax);
            isExtraMovementWaisted = false;
        }
    }    

    public void ChangeMovementPoints(float value, bool setValue = false)
    {
        if(setValue == true)
            currentMovementPoints = value;
        else
            currentMovementPoints += value;

        if(currentMovementPoints > movementPointsMax) currentMovementPoints = movementPointsMax;

        gmInterface.movesPart.UpdateCurrentMoves(currentMovementPoints);
        if(gmPathFinder != null && value > 0) gmPathFinder.RefreshPath(currentPosition, currentMovementPoints);

        ChangeActiveStatus(!(currentMovementPoints == 0));
    }

    public void ChangeActiveStatus(bool mode)
    {
        sprite.color = (mode == true) ? activeColor : inActiveColor;
    }

    private void UpgradeParameters(PlayersStats stats, float value)
    {
        if(stats == PlayersStats.MovementDistance)
        {
            movementPointsMax = (float)Math.Round(value, MidpointRounding.AwayFromZero);
            ChangeMovementPoints(movementPointsMax);

            viewRadius = movementPointsMax;
            mapTileManager.CheckFog(isFogNeeded, viewRadius);
        }

        if(stats == PlayersStats.ExtraMovementPoints) extraMovementPoints = value;

        if(stats == PlayersStats.Fog) 
        { 
            isFogNeeded = (value == 1) ? false : true;
            mapTileManager.CheckFog(isFogNeeded, viewRadius);
        }
        
        if(stats == PlayersStats.Luck) luck = value;
    }

    public void MoveOnTheWay(Vector2[] pathPoints, GlobalMapPathfinder map)
    {
        if(gmPathFinder == null) gmPathFinder = map;

        if(iAmMoving == false && pathPoints.Length != 0) StartCoroutine(Movement(pathPoints));
        else StopMoving();     
    }

    public void StopMoving()
    {
        if(iAmMoving == true) cancelMovement = true;
    }

    private IEnumerator Movement(Vector2[] pathPoints)
    {
        iAmMoving = true;
        currentPosition = pathPoints[0];

        for(int i = 1; i < pathPoints.Length; i++)
        {
            if(i == 1) 
            {
                previousPosition = pathPoints[0];
                if(CheckEnemy(pathPoints[i]) == true) break;
            }
            
            sprite.flipX = previousPosition.x - pathPoints[i].x < 0 ? true : false;

            if(currentMovementPoints == 0) 
            {
                CheckExtraMovement();
                break;             
            }


            gmPathFinder.ClearRoadTile(pathPoints[i - 1]);
            mapTileManager.CheckFog(isFogNeeded, viewRadius);

            Vector2 distance = pathPoints[i] - (Vector2)transform.position;
            Vector2 step = distance / (defaultCountSteps / speed);

            for(float t = 0; t < defaultCountSteps / speed; t++)
            {
                transform.position += (Vector3)step;
                yield return new WaitForSeconds(0.01f);
            }
            ChangeMovementPoints(-1);
                        
            transform.position = pathPoints[i];
            previousPosition = pathPoints[i - 1];
            currentPosition = pathPoints[i];
            gmPathFinder.RefreshSteps(pathPoints[i]);

            if(cancelMovement == true) break;

            if(i + 1 < pathPoints.Length)
            {
                if(CheckEnemy(pathPoints[i + 1]) == true) break;               
            }
        }

        if(cancelMovement == false && currentMovementPoints != 0) {
            if((Vector2)transform.position == pathPoints[pathPoints.Length - 1])
            {
                gmPathFinder.ClearRoadTile(pathPoints[pathPoints.Length - 1]);
            }
        }

        if(currentMovementPoints == 0) CheckExtraMovement();

        iAmMoving = false;
        cancelMovement = false;

        CheckPosition();
    }

    private void CheckExtraMovement()
    {
        if(extraMovementPoints == 0 || isExtraMovementWaisted == true) return;

        if(UnityEngine.Random.Range(0, 101) <= luck)
        {
            ChangeMovementPoints(extraMovementPoints);
            isExtraMovementWaisted = true;
            BonusTipUIManager.ShowVisualEffect(PlayersStats.ExtraMovementPoints, extraMovementPoints);
        }
    }

    public bool CheckEnemy(Vector2 position)
    {
        return positionChecker.CheckEnemy(position);        
    }

    public void CheckPosition()
    {
        positionChecker.CheckPosition(currentPosition);
    }

    public bool IsMoving()
    {
        return iAmMoving;
    }

    public void TeleportTo(Vector2 newPosition, float cost)
    {
        gmPathFinder.DestroyPath(true);
        resourcesManager.ChangeResource(ResourceType.Mana, -cost);
        StartCoroutine(Teleportation(newPosition));        
    }

    private IEnumerator Teleportation(Vector2 newPosition)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(0.01f);
        MenuManager.instance.isGamePaused = true;
        MenuManager.instance.canIOpenMenu = false;

        SpriteRenderer playerSprite = GetComponent<SpriteRenderer>();
        Color currentColor = playerSprite.color;
        float alfa = currentColor.a;

        while(alfa > 0)
        {
            alfa -= 0.02f;
            currentColor.a = alfa;
            playerSprite.color = currentColor;
            yield return delay;
        }

        Camera.main.transform.position = new Vector3(newPosition.x, newPosition.y, Camera.main.transform.position.z);        
        transform.position = newPosition;
        currentPosition = newPosition;
        mapTileManager.CheckFog(isFogNeeded, viewRadius);

        while(alfa < 1)
        {
            alfa += 0.02f;
            currentColor.a = alfa;
            playerSprite.color = currentColor;
            yield return delay;
        }

        MenuManager.instance.isGamePaused = false;
        MenuManager.instance.canIOpenMenu = true;

        yield return delay;

        //battleManager.TryToContinueEnemysTurn();
    }

    public void PathAfterBattle(int result)
    {
        //-1 - retreat; 0 - defeat; 1 - victory

        if(result == 1)
        {
            if(gmPathFinder != null) 
                gmPathFinder.RefreshPath(currentPosition, currentMovementPoints);
        }
        else
        {
            gmPathFinder.DestroyPath(true);
        }
    }


    public PlayersMovementSD Save()
    {
        PlayersMovementSD saveData = new PlayersMovementSD();
        saveData.flipHero = sprite.flipX;
        saveData.position = transform.position.ToVec3();
        saveData.movementPoints = currentMovementPoints;
        saveData.isExtraMovementWaisted = isExtraMovementWaisted;

        return saveData;
    }

    public void Load (PlayersMovementSD saveData)
    {
        sprite.flipX = saveData.flipHero;
        transform.position = saveData.position.ToVector3();
        currentPosition = transform.position;
        ChangeMovementPoints(saveData.movementPoints, true);
        isExtraMovementWaisted = saveData.isExtraMovementWaisted;
    }

    private void OnEnable()
    {
        EventManager.SetNewPlayerStat += UpgradeParameters;
        EventManager.NewMove += NewTurn;
    }

    private void OnDisable()
    {
        EventManager.SetNewPlayerStat -= UpgradeParameters;
        EventManager.NewMove -= NewTurn;
    }
}
