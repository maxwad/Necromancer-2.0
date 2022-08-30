using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class EnemyCardUI : MonoBehaviour
{
    public EnemyController enemy;
    public TMP_Text enemyName;
    public Image icon;
    public TMP_Text health;
    public TMP_Text pAttack;
    public TMP_Text mAttack;
    public TMP_Text pDefence;
    public TMP_Text mDefence;

    private string gag = "???";

    public void Initialize(EnemyController enemyData, bool showMode = false)
    {
        if(enemy == null) enemy = enemyData;

        enemyName.text = enemy.enemiesType.ToString();
        icon.sprite = enemy.icon;

        if(showMode == true)
        {
            health.text   = enemy.health.ToString();
            pAttack.text  = enemy.physicAttack.ToString();
            mAttack.text  = enemy.magicAttack.ToString();
            pDefence.text = enemy.physicDefence.ToString();
            mDefence.text = enemy.magicDefence.ToString();
        }
        else
        {
            health.text   = gag;
            pAttack.text  = gag;
            mAttack.text  = gag;
            pDefence.text = gag;
            mDefence.text = gag;
        }
        
        
    }
}
