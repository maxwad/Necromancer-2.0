using UnityEngine;
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
        viewRadius = playerStats.GetCurrentParameter(PlayersStats.RadiusView);
        scale = transform.localScale.x;

        scaleConstant = scale / viewRadius;
    }

    private void UpdateRadius(PlayersStats stats, float value)
    {
        if(stats == PlayersStats.RadiusView)
        {
            viewRadius = value;
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
