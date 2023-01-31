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

public class CastleDataForUI
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

[Serializable]
public class ResourceColors
{
    public ResourceType resourceType;
    public Color color;
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

public class RuneBoost
{
    public int level;
    public float boost;
    public int row;
    public int cell;

    public RuneBoost(int rowIndex, int cellIndex, RuneSO rune)
    {
        float value = rune.value;
        if(rowIndex == 1 && rune.isInvertedRune == false) value = -value;
        if(rowIndex != 1 && rune.isInvertedRune == true) value = -value;

        row = rowIndex;
        cell = cellIndex;
        boost = value;
        level = rune.level;
    }
}

#endregion


#region INFIRMARY

public class InjuredUnitData
{
    public int quantity;
    public float term;

    public InjuredUnitData(int amount, float days)
    {
        quantity = amount;
        term = days;
    }
}

#endregion


#region ALTARS, TOMBS, CAMPS

public class ResourceGiftData
{
    public int index;
    public ResourceType resourceType;
    public Sprite resourceIcon;
    public float amount = 0;
    public float amountInStore = 0;
    public float amountTotalTry = 0;
    public Color tryColor = Color.white;
    public Color amountColor = Color.white;
    public Color bgColor = Color.white;
    public bool isActiveBtn = false;
    public bool isDeficit = false;
    public AltarMiniGame miniGame = null;
}

public class TryData 
{
    public bool isComplete = false;
    public bool tempStatus = false;
    public ResourceType resourceType;
    public Sprite resourceIcon;
    public float amount = 0;
    public Color tryColor = Color.white;
}

public class TombInfo
{
    public Vector3 position;
    public bool isVisited = false;
    public SpellSO spell = null;
    public Reward reward = null;
}

public class CampGameParameters
{
    public int cellsAmount;
    public int rewardsAmount;
    public int attempts;
    public int helps;
    public int runeDrawnings;
    public List<CampBonus> combination = new List<CampBonus>();
}


[Serializable]
public class CampBonus
{
    public CampReward reward;
    public ResourceType resource;
    public Sprite icon;
    public string name;
    public int amount;
}
#endregion


#region ENEMY

public class Army
{
    public List<EnemiesTypes> squadList = new List<EnemiesTypes>();
    public List<int> quantityList = new List<int>();
    public bool isThisASiege = false;
    public TypeOfArmy typeOfArmy = TypeOfArmy.OnTheMap;
    public ArmyStrength strength = ArmyStrength.Low;
    public bool isAutobattlePosible = true;
    // more parameters
}

#endregion


#region REWARD

public class Reward
{
    public List<ResourceType> resourcesList = new List<ResourceType>();
    public List<float> resourcesQuantity = new List<float>();

    public Reward(List<ResourceType> resources, List<float> quantity)
    {
        resourcesList = resources;
        resourcesQuantity = quantity;
    }
}

#endregion


#region UNITS
public class Boost
{
    public BoostSender sender;
    public BoostEffect effect;
    public float value;

    public Boost(BoostSender boostSender, BoostEffect boostEffect, float boostValue)
    {
        sender = boostSender;
        effect = boostEffect;
        value = boostValue;
    }
}

#endregion


#region INPUT SYSTEM
public class InputAction
{
    public KeyActions keyAction;
    public IInputableKeys objectToActivate;

    public InputAction(KeyActions k, IInputableKeys o)
    {
        keyAction = k;
        objectToActivate = o;
    }
}

[Serializable]
public class KeyAction
{
    public KeyCode key;
    public KeyActions action;
}

public class AxiesData
{
    public float horValue;
    public float vertData;
    public float rotData;
    public float zoomData;

    public AxiesData(float hd, float vd, float rd, float zd)
    {
        horValue = hd;
        vertData = vd;
        rotData = rd;
        zoomData = zd;
    }
}

public class MouseData
{
    public Vector3 position;

    public bool mouseBtnLeft;
    public bool mouseBtnMiddle;
    public bool mouseBtnRight;

    public bool mouseBtnLeftDown;
    public bool mouseBtnMiddleDown;
    public bool mouseBtnRightDown;

    public MouseData(Vector3 p, bool bl, bool bm, bool br, bool bld, bool bmd, bool brd)
    {
        position = p;

        mouseBtnLeft   = bl;
        mouseBtnMiddle = bm;
        mouseBtnRight  = br;

        mouseBtnLeftDown   = bld;
        mouseBtnMiddleDown = bmd;
        mouseBtnRightDown  = brd;
    }
}

#endregion


#region CALENDAR

public class CalendarData
{
    public int daysLeft;

    public int currentDay;
    public int currentDecade;
    public int currentMonth;

    public CalendarData(int left, int day, int decade, int month)
    {
        daysLeft = left;
        currentDay = day;
        currentDecade = decade;
        currentMonth = month;
    }
}

#endregion

#region NEW


#endregion

#region NEW


#endregion
