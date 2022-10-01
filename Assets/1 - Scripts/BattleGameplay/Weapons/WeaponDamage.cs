using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    private float physicAttack;
    private float magicAttack;
    [HideInInspector] public Unit unit;

    [HideInInspector] private List<GameObject> enemyList = new List<GameObject>();

    public void SetSettings(Unit unitSource)
    {
        unit = unitSource;
        physicAttack = unitSource.physicAttack;
        magicAttack = unitSource.magicAttack;
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
                if(unit.unitAbility == NameManager.UnitsAbilities.Garlic)
                {
                    collision.gameObject.GetComponent<EnemyController>().TakeDamage(physicAttack, magicAttack, Vector3.zero);

                    if(unit.level == 2) collision.GetComponent<EnemyController>().PushMe(transform.position, 5000f);
                    if(unit.level == 3) collision.GetComponent<EnemyMovement>().MakeMeFixed(true, true);

                    enemyList.Add(collision.gameObject);
                }

                else if(unit.unitAbility == NameManager.UnitsAbilities.Axe   ||
                        unit.unitAbility == NameManager.UnitsAbilities.Spear ||
                        unit.unitAbility == NameManager.UnitsAbilities.Bible ||
                        unit.unitAbility == NameManager.UnitsAbilities.Bow   ||
                        unit.unitAbility == NameManager.UnitsAbilities.Knife)
                {
                    collision.gameObject.GetComponent<EnemyController>().TakeDamage(physicAttack, magicAttack, transform.position);
                    enemyList.Add(collision.gameObject);
                }
                
                else if(unit.unitAbility == NameManager.UnitsAbilities.Bottle)
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
            if(unit.unitAbility != NameManager.UnitsAbilities.Bottle)
            collision.gameObject.GetComponent<HealthObjectStats>().TakeDamage(physicAttack, magicAttack);
        }
    }
}
