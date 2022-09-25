using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static NameManager;

public class SpellStat : MonoBehaviour
{
    //TODO: change to false for real game!
    public bool isFinded = true;
    public bool isUnlocked = false;

    [Space]
    public Spells spell;
    public TypeOfSpell type;

    public float manaCost;
    public float value;
    public float actionTime;
    public float reloading;
    public Sprite icon;
    [TextArea(3, 10)]
    public string description;
}
