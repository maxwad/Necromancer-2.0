using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

[CreateAssetMenu(fileName = "FUpgradeData", menuName = "FUpgradeItem")]

public class FortressUpgradeSO : ScriptableObject
{
    public CastleBuildings building;
    public string buildingName;
    public CastleBuildingsBonuses buildingBonus;

    public Sprite activeIcon;
    public Sprite inActiveIcon;

    public int level;
    public float value;
    public bool percentValueType = false;
    public bool isInverted = false;
    public int fortressLevel;
    public int constructuionTime;
    public List<Cost> cost;
    
    public string description;
    public string BuildingDescription;

}
