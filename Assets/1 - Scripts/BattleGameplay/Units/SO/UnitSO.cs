using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

[CreateAssetMenu(fileName = "UnitData", menuName = "UnitItem")]
public class UnitSO : ScriptableObject
{
    public string unitName;
    public GameObject unitGO;
    public Sprite unitIcon;

    //battle parameters
    public UnitsHouses UnitHome;
    public UnitsTypes UnitType;
    public float health;
    public float physicAttack;
    public float physicDefence;
    public float magicAttack;
    public float magicDefence;
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

}