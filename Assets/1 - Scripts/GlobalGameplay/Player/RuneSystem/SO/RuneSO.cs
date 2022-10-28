using System;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

[Serializable]
public class Cost
{
    public ResourceType type;
    public float amount;
}

[CreateAssetMenu(fileName = "RuneData", menuName = "RuneItem")]

public class RuneSO : ScriptableObject
{
    public string runeName;
    public string serieName;
    public RunesType rune;
    public Sprite activeIcon;
    public Sprite inActiveIcon;
    public int level;
    public float value;
    public StatBoostType valueType;
    public Cost[] cost;
    public List<Cost> realCost;
    public string positiveDescription;
    public string negativeDescription;
}
