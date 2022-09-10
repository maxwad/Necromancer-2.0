using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class GMPlayerMovement : MonoBehaviour
{
    private PlayerStats playerStats;
    private float movementPointsMax = 0;
    private float currentMovementPoints = 0;
    private float viewRadius = 0;

    private float speed = 50f; //50 for build
    private float defaultCountSteps = 1000;

    private bool cancelMovement = false;
    private bool iAmMoving = false;

    private GlobalMapPathfinder gmPathFinder;
    private GMPlayerPositionChecker positionChecker;

    private SpriteRenderer sprite;
    private Vector2 previousPosition = Vector2.zero;
    private Vector2 currentPosition = Vector2.zero;

    private Coroutine newTurnCoroutine;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        playerStats = GlobalStorage.instance.playerStats;
        gmPathFinder = GlobalStorage.instance.globalMap.GetComponent<GlobalMapPathfinder>();
        positionChecker = GetComponent<GMPlayerPositionChecker>();
        SetParameters();
        currentMovementPoints = movementPointsMax;
        currentPosition = gameObject.transform.position;
        EventManager.OnUpgradeStatCurrentValueEvent(PlayersStats.MovementDistance, movementPointsMax, currentMovementPoints);
    }

    public Vector3 GetCurrentPosition()
    {
        return currentPosition;
    }

    private void SetParameters()
    {
        movementPointsMax = playerStats.GetStartParameter(PlayersStats.MovementDistance);
        viewRadius = playerStats.GetStartParameter(PlayersStats.RadiusView);
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

            currentMovementPoints = movementPointsMax;
            EventManager.OnUpgradeStatCurrentValueEvent(PlayersStats.MovementDistance, movementPointsMax, currentMovementPoints);
            if(gmPathFinder != null) gmPathFinder.RefreshPath(currentPosition, currentMovementPoints);

            newTurnCoroutine = null;
        }
    }    

    private void UpgradeParameters(PlayersStats stats, float value)
    {
        if(stats == PlayersStats.MovementDistance) movementPointsMax = value;

        if(stats == PlayersStats.RadiusView) viewRadius = value;
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

            if(currentMovementPoints == 0) break;
            currentMovementPoints--;
            EventManager.OnUpgradeStatCurrentValueEvent(PlayersStats.MovementDistance, movementPointsMax, currentMovementPoints);

            gmPathFinder.ClearRoadTile(pathPoints[i - 1]);
            gmPathFinder.CheckFog(viewRadius);

            Vector2 distance = pathPoints[i] - (Vector2)transform.position;
            Vector2 step = distance / (defaultCountSteps / speed);

            for(float t = 0; t < defaultCountSteps / speed; t++)
            {
                transform.position += (Vector3)step;
                yield return new WaitForSeconds(0.01f);
            }
                        
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
            gmPathFinder.ClearRoadTile(pathPoints[pathPoints.Length - 1]);
        } 

        iAmMoving = false;
        cancelMovement = false;

        CheckPosition();
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
        gmPathFinder.DestroyPath();
        GlobalStorage.instance.hero.SpendMana(cost);

        StartCoroutine(Telepartation(newPosition));        
    }

    private IEnumerator Telepartation(Vector2 newPosition)
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
        gmPathFinder.CheckFog(viewRadius);

        while(alfa < 1)
        {
            alfa += 0.02f;
            currentColor.a = alfa;
            playerSprite.color = currentColor;
            yield return delay;
        }

        MenuManager.instance.isGamePaused = false;
        MenuManager.instance.canIOpenMenu = true;
    }

    private void OnEnable()
    {
        EventManager.NewBoostedStat += UpgradeParameters;
        EventManager.NewMove += NewTurn;
    }

    private void OnDisable()
    {
        EventManager.NewBoostedStat -= UpgradeParameters;
        EventManager.NewMove -= NewTurn;
    }
}
