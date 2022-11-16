using UnityEngine;
using static NameManager;

public class Unit
{
    public string unitName;
    public GameObject unitGO;
    public Sprite unitIcon;

    //battle parameters
    public UnitsHouses unitHome;
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

    //cost parameters
    public int coinsPrice;
    public int foodPrice;
    public int woodPrice;
    public int ironPrice;
    public int stonePrice;
    public int magicPrice;

    public float killToNextLevel;
    public bool isUnitActive = true;
    public UnitStatus status = UnitStatus.Store;
    //public int quantity;
    //public float currentHealth;

    public Unit(UnitSO unitSO)
    {
        unitName = unitSO.unitName;
        unitGO   = unitSO.unitGO;
        unitIcon = unitSO.unitIcon;

        unitHome = unitSO.unitHome;
        unitType = unitSO.unitType;

        health         = unitSO.health;
        magicAttack    = unitSO.magicAttack;
        physicAttack   = unitSO.physicAttack;
        magicDefence   = unitSO.magicDefence;
        physicDefence  = unitSO.physicDefence;
        speedAttack    = unitSO.speedAttack;
        size           = unitSO.size;
        level          = unitSO.level;

        unitAbility    = unitSO.unitAbility;
        attackTool     = unitSO.attackTool;
        abilityDescription = WeaponsDictionary.instance.GetAbilityDescription(unitAbility, level);


        coinsPrice = unitSO.coinsPrice;
        foodPrice  = unitSO.foodPrice;
        woodPrice  = unitSO.woodPrice;
        ironPrice  = unitSO.ironPrice;
        stonePrice = unitSO.stonePrice;
        magicPrice = unitSO.magicPrice;

        killToNextLevel = unitSO.killToNextLevel;


        //quantity = 1;
        //currentHealth = health;
    }


    public UnitController unitController;

    public void SetUnitController(UnitController unitControllerSource)
    {
        unitController = unitControllerSource;
    }

    public void SetQuantity(int count)
    {
        //quantity = count;
    }

    public void ActivateUnit(bool mode)
    {
        isUnitActive = mode;
    }
}
