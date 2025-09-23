using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

[CreateAssetMenu(fileName = "RBUpgradeData", menuName = "RBUpgradeItem")]

public class RBUpgradeSO : ScriptableObject
{
    public string upgradeName;
    public ResourceBuildingsUpgrades upgradeBonus;

    public Sprite activeIcon;
    public Sprite inActiveIcon;

    public float value;
    public bool percentValueType = false;
    public bool isInverted = false;
    public List<Cost> cost;

    public string description;
}
