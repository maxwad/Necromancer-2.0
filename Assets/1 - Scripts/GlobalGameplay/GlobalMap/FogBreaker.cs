using UnityEngine;
using System;
using static NameManager;

public class FogBreaker : MonoBehaviour
{
    private PlayerStats playerStats;

    private float scale;
    private float scaleConstant;
    private float viewRadius;

    private void Start()
    {
        playerStats = GlobalStorage.instance.playerStats;
        viewRadius = Mathf.Round(playerStats.GetCurrentParameter(PlayersStats.MovementDistance));
        scale = transform.localScale.x;

        scaleConstant = scale / viewRadius;
    }

    private void UpdateRadius(PlayersStats stats, float value)
    {
        if(stats == PlayersStats.MovementDistance)
        {
            viewRadius = (float)Math.Round(value, MidpointRounding.AwayFromZero);
            transform.localScale = new Vector3(scaleConstant * viewRadius, scaleConstant * viewRadius, 0);
        }        
    }

    private void OnEnable()
    {
        EventManager.SetNewPlayerStat += UpdateRadius;
    }

    private void OnDisable()
    {
        EventManager.SetNewPlayerStat -= UpdateRadius;
    }
}
