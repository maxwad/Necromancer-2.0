using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static NameManager;

public class WeaponDamage : MonoBehaviour
{
    private BoostManager boostManager;
    private PlayerStats playersStats;

    private float physicAttackBase;
    private float magicAttackBase;
    private float luck;

    private float physicAttack;
    private float magicAttack;
    private float criticalDamage;

    [HideInInspector] public Unit unit;

    [HideInInspector] private List<GameObject> enemyList = new List<GameObject>();

    [Inject]
    public void Construct(BoostManager boostManager, PlayerStats playersStats)
    {
        this.boostManager = boostManager;
        this.playersStats = playersStats;
    }

    public void SetSettings(Unit unitSource)
    {
        unit = unitSource;
        physicAttackBase = unitSource.physicAttack;
        magicAttackBase = unitSource.magicAttack;
        luck = playersStats.GetCurrentParameter(PlayersStats.Luck);

        physicAttack = physicAttackBase + physicAttackBase * boostManager.GetBoost(BoostType.PhysicAttack);
        magicAttack = magicAttackBase + magicAttackBase * boostManager.GetBoost(BoostType.MagicAttack);
        criticalDamage = luck + boostManager.GetBoost(BoostType.CriticalDamage);
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
                EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();

                if(unit.unitAbility == UnitsAbilities.Garlic)
                {
                    Hit(enemy, transform.position);

                    if(unit.level == 2) enemy.PushMe(transform.position, 5000f);
                    if(unit.level == 3) collision.GetComponent<EnemyMovement>().MakeMeFixed(true, true);

                    enemyList.Add(collision.gameObject);
                }

                else if(unit.unitAbility == UnitsAbilities.Axe   ||
                        unit.unitAbility == UnitsAbilities.Spear ||
                        unit.unitAbility == UnitsAbilities.Bible ||
                        unit.unitAbility == UnitsAbilities.Bow   ||
                        unit.unitAbility == UnitsAbilities.Knife)
                {
                    Hit(enemy, transform.position);
                    enemyList.Add(collision.gameObject);
                }
                
                else if(unit.unitAbility == UnitsAbilities.Bottle)
                {
                    //no action
                }

                else
                {
                    Hit(enemy, transform.position);
                }
            }           
            
        }

        if (collision.CompareTag(TagManager.T_OBSTACLE) == true)
        {
            if(unit.unitAbility != UnitsAbilities.Bottle)
            collision.gameObject.GetComponent<HealthObjectStats>().TakeDamage(physicAttack, magicAttack);
        }
    }

    public void Hit(EnemyController enemy, Vector3 position)
    {
        bool isCriticalDamage = Random.Range(0, 100) < criticalDamage;
        enemy.TakeDamage(physicAttack, magicAttack, position, isCriticalDamage, unit.unitAbility);

        //Debug.Log("Ph attack = " + physicAttack);
    }

    private void UpgradeParameters(BoostType boost, float value)
    {
        if(boost == BoostType.PhysicAttack) physicAttack = physicAttackBase + physicAttackBase * value;
        if(boost == BoostType.MagicAttack) magicAttack = magicAttackBase + magicAttackBase * value;
        if(boost == BoostType.CriticalDamage) criticalDamage = luck + value;
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
