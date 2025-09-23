using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

[CreateAssetMenu(fileName = "SpellData", menuName = "SpellItem")]
public class SpellSO : ScriptableObject
{
    [Space]
    public Spells spell;
    public string spellName;
    public TypeOfSpell type;
    public int level;
    public bool hasPreSpell;

    public float manaCost;
    public float value;
    public float radius;
    public float actionTime;
    public float reloading;
    public Sprite icon;
    public List<Cost> cost;
    [TextArea(3, 10)]
    public string description;
}
