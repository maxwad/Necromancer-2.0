using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class PlayerManager : MonoBehaviour
{
    private PlayerResurrection playerResurrection;
    private ResourcesManager resourcesManager;
    private GameObject battlePlayer;
    private PlayerStats playerStats;

    private float luck;
    [HideInInspector] public float manaRegeneration = 0;
    private float luckManaBonus = 0;
    [HideInInspector] public float healthRegeneration = 0;
    private float luckHealthBonus = 0;

    private void Start()
    {
        resourcesManager = GlobalStorage.instance.resourcesManager;
        playerResurrection = GetComponent<PlayerResurrection>();
        battlePlayer = GlobalStorage.instance.battlePlayer.gameObject;
        playerStats = GlobalStorage.instance.playerStats;
        luck = playerStats.GetCurrentParameter(PlayersStats.Luck);
    }

    private void MovePlayerToTheGlobal(bool mode)
    {
        if (mode == false)
        {
             battlePlayer.SetActive(true);
        }
        else
        {
            battlePlayer.SetActive(false);
            //battleMap we never turn off, just clear in BattleMap script
        }
    }

    private void ResurrectPlayer()
    {
        playerResurrection.StartResurrection();
    }

    private void UpgradeParameters(PlayersStats stats, float value)
    {
        if(stats == PlayersStats.Luck) luck = value;  
        
        if(stats == PlayersStats.ManaRegeneration) manaRegeneration = value;
        if(stats == PlayersStats.ManaBigRegeneration) luckManaBonus = value;

        if(stats == PlayersStats.HealthRegeneration) healthRegeneration = value;
        if(stats == PlayersStats.HealthBigRegeneration) luckHealthBonus = value;
    }

    #region Hero's Regeneration

    private void NewTurn()
    {
        if(manaRegeneration != 0)
            resourcesManager.ChangeResource(ResourceType.Mana, manaRegeneration);

        if(luckManaBonus != 0)
        {
            if(Random.Range(0, 101) <= luck)
            {
                resourcesManager.ChangeResource(ResourceType.Mana, luckManaBonus);
                //TODO Effect!
            }
        }

        if(healthRegeneration != 0)
            resourcesManager.ChangeResource(ResourceType.Health, healthRegeneration);

        if(luckHealthBonus != 0)
        {
            if(Random.Range(0, 101) <= luck)
            {
                resourcesManager.ChangeResource(ResourceType.Health, luckHealthBonus);
                //TODO Effect!
            }
        }
    }

    #endregion

    private void OnEnable()
    {
        EventManager.SwitchPlayer += MovePlayerToTheGlobal;
        EventManager.Defeat += ResurrectPlayer;
        EventManager.SetNewPlayerStat += UpgradeParameters;
        EventManager.NewMove += NewTurn;
    }

    private void OnDisable()
    {
        EventManager.SwitchPlayer -= MovePlayerToTheGlobal;
        EventManager.Defeat -= ResurrectPlayer;
        EventManager.SetNewPlayerStat -= UpgradeParameters;
        EventManager.NewMove -= NewTurn;
    }

}
