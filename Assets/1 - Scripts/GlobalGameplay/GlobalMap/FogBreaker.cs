using UnityEngine;
using System;
using Enums;
using Zenject;

public class FogBreaker : MonoBehaviour
{
    private PlayerStats playerStats;

    private float scale;
    private float scaleConstant;
    private float viewRadius;

    [Inject]
    public void Construct(PlayerStats playerStats)
    {
        this.playerStats = playerStats;
    }

    private void Start()
    {
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
