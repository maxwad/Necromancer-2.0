using System;
using UnityEngine;
using System.Collections.Generic;
using static NameManager;


#region CASTLE AND BUILDINGS

[Serializable]
public class HiringAmount
{
    public UnitsTypes unitType;
    public int amount;
}

public class ConstructionTime
{
    public int originalTerm;
    public int term;
    public int daysLeft;
}

public class ConstractionData
{
    public string constractionName;
    public Sprite icon;
    public int daysLeft;
}

public class castleDataForUI
{
    public int level;
    public bool canIBuild;
    public List<ConstractionData> constractions = new List<ConstractionData>();
}

public class BuildingsRequirements
{
    public bool isCostForCastle = true;
    public List<Cost> costs;
    public int fortressLevel;
    public bool canIBuild;
}

public class UpgradeStatus
{
    public bool isEnable = false;
    public bool isHidden = false;
}

[Serializable]
public class ResourceBuildingData
{
    public ResourceType resourceType;
    public ResourceBuildings resourceBuilding;
    public Sprite resourceSprite;
    public Sprite buildingSprite;
    public Color buildingColor;
    public float resourceBaseIncome;
}

#endregion


#region OBJECTPOOL

[Serializable]
public class ObjectPoolObjects
{
    public ObjectPool type;
    public GameObject obj;
}

[Serializable]
public class ObjectPoolWeapon
{
    public UnitsAbilities type;
    public GameObject weapon;
}

[Serializable]
public class ObjectPoolBossWeapon
{
    public BossWeapons type;
    public GameObject weapon;
}

#endregion

#region RUNES

[Serializable]
public class Cost
{
    public ResourceType type;
    public float amount;
}

#endregion


#region NEW


#endregion
