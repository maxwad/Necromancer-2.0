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

    private GlobalTileMapManager globalMap;
    private int passedPoints = 0;

    private void Start()
    {
        playerStats = GlobalStorage.instance.player.GetComponent<PlayerStats>();
        SetParameters();
        currentMovementPoints = movementPointsMax;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return)) NewTurn();
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

    public void NewTurn()
    {
        StopMoving();
        currentMovementPoints = movementPointsMax;
        if(globalMap != null) globalMap.RefreshPath(passedPoints, currentMovementPoints);
    }

    private void UpgradeParameters(PlayersStats stats, float value)
    {
        if(stats == PlayersStats.MovementDistance) movementPointsMax = value;

        if(stats == PlayersStats.RadiusView) viewRadius = value;
    }

    public void MoveOnTheWay(Vector2[] pathPoints, GlobalTileMapManager map)
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
        passedPoints = 0;

        for(int i = 1; i < pathPoints.Length; i++)
        {
            if(currentMovementPoints == 0) break;
            currentMovementPoints--;
            passedPoints++;

            globalMap.ClearRoadTile(pathPoints[i - 1]);
            globalMap.CheckFog(pathPoints[i - 1], viewRadius);

            Vector2 distance = pathPoints[i] - (Vector2)transform.position;
            Vector2 step = distance / (defaultCountSteps / speed);

            for(float t = 0; t < defaultCountSteps / speed; t++)
            {
                transform.position += (Vector3)step;
                yield return new WaitForSeconds(0.01f);
            }
                        
            transform.position = pathPoints[i];

            if(cancelMovement == true) 
            {
                //if we break a movement we need new path, so we have to make counter = 0
                passedPoints = 0;
                break;
            }
            
        }

        if(cancelMovement == false && currentMovementPoints != 0) globalMap.ClearRoadTile(pathPoints[pathPoints.Length - 1]);

        iAmMoving = false;
        cancelMovement = false;
        //passedPoints = 0;
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
