using System;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

[CreateAssetMenu(fileName = "RuneData", menuName = "RuneItem")]

public class RuneSO : ScriptableObject
{
    public string runeName;
    public RunesType rune;
    public BoostSender source;
    public Sprite activeIcon;
    public Sprite inActiveIcon;
    public int level;
    public float value;
    public bool isInvertedRune = false;
    public List<Cost> cost;
    public List<Cost> realCost;
    public string positiveDescription;
    public string negativeDescription;
}
