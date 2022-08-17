using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
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

    private GlobalMapPathfinder globalMap;

    private SpriteRenderer sprite;
    private Vector2 previousPosition = Vector2.zero;
    private Vector2 currentPosition = Vector2.zero;

    private Coroutine newTurnCoroutine;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        playerStats = GlobalStorage.instance.player.GetComponent<PlayerStats>();
        SetParameters();
        currentMovementPoints = movementPointsMax;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return)) 
        {
            if(MenuManager.isGamePaused == false && MenuManager.isMiniPause == false && GlobalStorage.instance.isGlobalMode == true)
            {
                if(newTurnCoroutine == null) newTurnCoroutine = StartCoroutine(NewTurn());
            }
        }        
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

    public IEnumerator NewTurn()
    {
        StopMoving();

        while(iAmMoving == true)
        {
            yield return null;
        }

        currentMovementPoints = movementPointsMax;
        if(globalMap != null) globalMap.RefreshPath(currentPosition, currentMovementPoints);

        newTurnCoroutine = null;
    }

    private void UpgradeParameters(PlayersStats stats, float value)
    {
        if(stats == PlayersStats.MovementDistance) movementPointsMax = value;

        if(stats == PlayersStats.RadiusView) viewRadius = value;
    }

    public void MoveOnTheWay(Vector2[] pathPoints, GlobalMapPathfinder map)
    {
        globalMap = map;
        if(iAmMoving == false && pathPoints.Length != 0) StartCoroutine(Movement(pathPoints));
        else StopMoving();     
    }

    public void StopMoving()
    {
        if(iAmMoving == true) cancelMovement = true;
    }

    private IEnumerator Movement(Vector2[] pathPoints)
    {
        yield return new WaitForSeconds(0.2f);
        iAmMoving = true;
        currentPosition = pathPoints[0];

        for(int i = 1; i < pathPoints.Length; i++)
        {
            if(i == 1) previousPosition = pathPoints[0];
            sprite.flipX = previousPosition.x - pathPoints[i].x < 0 ? true : false;            

            if(currentMovementPoints == 0) break;
            currentMovementPoints--;

            globalMap.ClearRoadTile(pathPoints[i - 1]);
            globalMap.CheckFog(viewRadius);

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
            globalMap.RefreshSteps(pathPoints[i]);

            if(cancelMovement == true) break;            
        }

        if(cancelMovement == false && currentMovementPoints != 0) globalMap.ClearRoadTile(pathPoints[pathPoints.Length - 1]);

        iAmMoving = false;
        cancelMovement = false;
    }

    public bool IsMoving()
    {
        return iAmMoving;
    }

    private void OnEnable()
    {
        EventManager.NewBoostedStat += UpgradeParameters;
    }

    private void OnDisable()
    {
        EventManager.NewBoostedStat -= UpgradeParameters;
    }
}
