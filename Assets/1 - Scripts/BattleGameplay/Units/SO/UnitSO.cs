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
    public CastleBuildings unitHome;
    public UnitsTypes unitType;
    public float health;
    public float physicAttack;
    public float physicDefence;
    public float magicAttack;
    public float magicDefence;
    public float speedAttack;
    public float size;
    public int level;
    public float killToNextLevel;
    public UnitsAbilities unitAbility;

    public GameObject attackTool;

    public List<Cost> costs;
}