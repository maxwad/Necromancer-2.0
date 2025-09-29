using Enums;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    public string unitName;
    public GameObject unitGO;
    public Sprite unitIcon;

    //battle parameters
    public CastleBuildings unitHome;
    public UnitsTypes unitType;
    public float health;
    public float magicAttack;
    public float physicAttack;
    public float magicDefence;
    public float physicDefence;
    public float speedAttack;
    public float size;
    public int level;
    public UnitsAbilities unitAbility;
    public string abilityDescription;

    public GameObject attackTool;

    public List<Cost> costs = new List<Cost>();

    public float killToNextLevel;
    public bool isUnitActive = true;
    public UnitStatus status = UnitStatus.Store;

    public Unit(UnitSO unitSO)
    {
        unitName = unitSO.unitName;
        unitGO = unitSO.unitGO;
        unitIcon = unitSO.unitIcon;

        unitHome = unitSO.unitHome;
        unitType = unitSO.unitType;

        health = unitSO.health;
        magicAttack = unitSO.magicAttack;
        physicAttack = unitSO.physicAttack;
        magicDefence = unitSO.magicDefence;
        physicDefence = unitSO.physicDefence;
        speedAttack = unitSO.speedAttack;
        size = unitSO.size;
        level = unitSO.level;

        unitAbility = unitSO.unitAbility;
        attackTool = unitSO.attackTool;
        abilityDescription = WeaponsDictionary.instance.GetAbilityDescription(unitAbility, level);

        foreach(var cost in unitSO.costs)
            costs.Add(cost);

        killToNextLevel = unitSO.killToNextLevel;
    }


    public UnitController unitController;

    public void SetUnitController(UnitController unitControllerSource)
    {
        unitController = unitControllerSource;
    }
}
