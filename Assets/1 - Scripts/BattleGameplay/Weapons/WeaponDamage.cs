using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class WeaponDamage : MonoBehaviour
{
    private BattleBoostManager boostManager;
    private PlayerStats playersStats;

    private float physicAttackBase;
    private float magicAttackBase;
    private float luck;

    private float physicAttack;
    private float magicAttack;
    private float criticalDamage;
    private float bossMultiplier;

    [HideInInspector] public Unit unit;

    [HideInInspector] private List<GameObject> enemyList = new List<GameObject>();

    public void SetSettings(Unit unitSource)
    {
        if(boostManager == null)
        {
            boostManager = GlobalStorage.instance.unitBoostManager;
            playersStats = GlobalStorage.instance.playerStats;
        }

        unit = unitSource;
        physicAttackBase = unitSource.physicAttack;
        magicAttackBase = unitSource.magicAttack;
        luck = playersStats.GetCurrentParameter(PlayersStats.Luck);

        physicAttack = physicAttackBase + physicAttackBase * boostManager.GetBoost(BoostType.PhysicAttack);
        magicAttack = magicAttackBase + magicAttackBase * boostManager.GetBoost(BoostType.MagicAttack);
        criticalDamage = luck + boostManager.GetBoost(BoostType.CriticalDamage);
        bossMultiplier = boostManager.GetBoost(BoostType.BossDamade);
    }

    public void ClearEnemyList()
    {
        enemyList.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(TagManager.T_ENEMY) == true)
        {
            //we need to check for re-touch. if we don't need this then add enemy in list
            if(enemyList.Contains(collision.gameObject) == false)
            {
                if(unit.unitAbility == UnitsAbilities.Garlic)
                {
                    collision.gameObject.GetComponent<EnemyController>().TakeDamage(physicAttack, magicAttack, Vector3.zero);

                    if(unit.level == 2) collision.GetComponent<EnemyController>().PushMe(transform.position, 5000f);
                    if(unit.level == 3) collision.GetComponent<EnemyMovement>().MakeMeFixed(true, true);

                    enemyList.Add(collision.gameObject);
                }

                else if(unit.unitAbility == UnitsAbilities.Axe   ||
                        unit.unitAbility == UnitsAbilities.Spear ||
                        unit.unitAbility == UnitsAbilities.Bible ||
                        unit.unitAbility == UnitsAbilities.Bow   ||
                        unit.unitAbility == UnitsAbilities.Knife)
                {
                    collision.gameObject.GetComponent<EnemyController>().TakeDamage(physicAttack, magicAttack, transform.position);
                    enemyList.Add(collision.gameObject);
                }
                
                else if(unit.unitAbility == UnitsAbilities.Bottle)
                {
                    //no action
                }

                else
                {
                    collision.gameObject.GetComponent<EnemyController>().TakeDamage(physicAttack, magicAttack, transform.position);
                }
            }           
            
        }

        if (collision.CompareTag(TagManager.T_OBSTACLE) == true)
        {
            if(unit.unitAbility != UnitsAbilities.Bottle)
            collision.gameObject.GetComponent<HealthObjectStats>().TakeDamage(physicAttack, magicAttack);
        }
    }

    private void UpgradeParameters(BoostType boost, float value)
    {
        if(boost == BoostType.PhysicAttack) physicAttack = physicAttackBase + physicAttackBase * value;
        if(boost == BoostType.MagicAttack) magicAttack = magicAttackBase + magicAttackBase * value;
        if(boost == BoostType.CriticalDamage) criticalDamage = luck + value;
        if(boost == BoostType.BossDamade) bossMultiplier = value;
    }

    private void OnEnable()
    {
        EventManager.SetBattleBoost += UpgradeParameters;
    }

    private void OnDisable()
    {
        EventManager.SetBattleBoost -= UpgradeParameters;
    }
}
