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

    private float speed = 30f; //50 for build
    private float defaultCountSteps = 1000;

    private bool cancelMovement = false;
    private bool iAmMoving = false;

    private void Start()
    {
        playerStats = GlobalStorage.instance.player.GetComponent<PlayerStats>();
        GetParameters();
        currentMovementPoints = movementPointsMax;
    }

    private void GetParameters()
    {
        movementPointsMax = playerStats.GetStartParameter(PlayersStats.MovementDistance);
        viewRadius = playerStats.GetStartParameter(PlayersStats.RadiusView);
    }

    private void UpgradeParameters(PlayersStats stats, float value)
    {
        if(stats == PlayersStats.MovementDistance) movementPointsMax = value;

        if(stats == PlayersStats.RadiusView) viewRadius = value;
    }

    public void MoveOnTheWay(Vector2[] pathPoints, GlobalTileMapManager map)
    {
        if(iAmMoving == false) StartCoroutine(Movement(pathPoints, map));
        else StopMoving();     
    }

    public void StopMoving()
    {
        if(iAmMoving == true) cancelMovement = true;
    }

    private IEnumerator Movement(Vector2[] pathPoints, GlobalTileMapManager map)
    {
        yield return new WaitForSeconds(0.2f);
        iAmMoving = true;

        for(int i = 1; i < pathPoints.Length; i++)
        {
            map.ClearRoadTile(pathPoints[i - 1]);
            map.CheckFog(pathPoints[i - 1], viewRadius);

            Vector2 distance = pathPoints[i] - (Vector2)transform.position;
            Vector2 step = distance / (defaultCountSteps / speed);

            for(float t = 0; t < defaultCountSteps / speed; t++)
            {
                transform.position += (Vector3)step;
                yield return new WaitForSeconds(0.01f);
            }
                        
            transform.position = pathPoints[i];          

            if(cancelMovement == true) break;
        }

        if(cancelMovement == false) map.ClearRoadTile(pathPoints[pathPoints.Length - 1]);

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
