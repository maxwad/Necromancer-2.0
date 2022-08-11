using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

[CreateAssetMenu(fileName = "EnemyData", menuName = "EnemyItem")]

public class EnemySO : ScriptableObject
{
    public string enemyName;
    public EnemiesTypes EnemiesType;
    public GameObject enemyGO;
    public Sprite enemyIcon;

    //battle parameters
    public float health;
    public float physicAttack;
    public float physicDefence;
    public float magicAttack;
    public float magicDefence;
    public float speedAttack;
    public float size;

    public EnemyAbilities EnemyAbility;

    public GameObject attackTool;

    //cost parameters
    public int coinsPrice;
    public int foodPrice;
    public int woodPrice;
    public int ironPrice;
    public int stonePrice;
    public int magicPrice;

    //expirience parameters
    public int exp;
}
