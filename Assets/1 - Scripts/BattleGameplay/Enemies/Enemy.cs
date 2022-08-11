using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class Enemy
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

    public Enemy(EnemySO enemySO)
    {
        enemyName   = enemySO.enemyName;
        enemyGO     = enemySO.enemyGO;
        enemyIcon   = enemySO.enemyIcon;

        EnemiesType = enemySO.EnemiesType;

        health        = enemySO.health;
        magicAttack   = enemySO.magicAttack;
        physicAttack  = enemySO.physicAttack;
        magicDefence  = enemySO.magicDefence;
        physicDefence = enemySO.physicDefence;
        speedAttack   = enemySO.speedAttack;
        size          = enemySO.size;

        EnemyAbility  = enemySO.EnemyAbility;
        attackTool    = enemySO.attackTool;


        coinsPrice = enemySO.coinsPrice;
        foodPrice  = enemySO.foodPrice;
        woodPrice  = enemySO.woodPrice;
        ironPrice  = enemySO.ironPrice;
        stonePrice = enemySO.stonePrice;
        magicPrice = enemySO.magicPrice;

        exp        = enemySO.exp;
}
}
