using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class Unit
{
    public string unitName;
    public GameObject unitGO;
    public Sprite unitIcon;

    //battle parameters
    public UnitsHouses UnitHome;
    public UnitsTypes UnitType;
    public float health;
    public float magicAttack;
    public float physicAttack;
    public float magicDefence;
    public float physicDefence;
    public float speedAttack;
    public float size;
    public int level;
    public UnitsAbilities UnitAbility;

    public GameObject attackTool;

    //cost parameters
    public int coinsPrice;
    public int foodPrice;
    public int woodPrice;
    public int ironPrice;
    public int stonePrice;
    public int magicPrice;

    public int quantity;
    public float currentHealth;

    public Unit(UnitSO unitSO)
    {
        unitName = unitSO.unitName;
        unitGO   = unitSO.unitGO;
        unitIcon = unitSO.unitIcon;

        UnitHome = unitSO.UnitHome;
        UnitType = unitSO.UnitType;

        health         = unitSO.health;
        magicAttack    = unitSO.magicAttack;
        physicAttack   = unitSO.physicAttack;
        magicDefence   = unitSO.magicDefence;
        physicDefence  = unitSO.physicDefence;
        speedAttack    = unitSO.speedAttack;
        size           = unitSO.size;
        level          = unitSO.level;

        UnitAbility    = unitSO.UnitAbility;
        attackTool     = unitSO.attackTool;


        coinsPrice = unitSO.coinsPrice;
        foodPrice  = unitSO.foodPrice;
        woodPrice  = unitSO.woodPrice;
        ironPrice  = unitSO.ironPrice;
        stonePrice = unitSO.stonePrice;
        magicPrice = unitSO.magicPrice;


        quantity = 1;
        currentHealth = health;
    }
}
