using System;
using UnityEngine;
using System.Collections.Generic;
using static NameManager;

[Serializable]
public class Vec3
{
    public float x = 0;
    public float y = 0;
    public float z = 0;

    public Vec3(Vector3 oldVector)
    {
        x = oldVector.x;
        y = oldVector.y;
        z = oldVector.z;
    }
}

[Serializable]
public class fogCell
{
    public float x = 0;
    public float y = 0;

    public fogCell(Vector3Int oldVector)
    {
        x = oldVector.x;
        y = oldVector.y;
    }
}

[Serializable]
public class GMTileManagerSD
{
    public List<Vec3> arenaPoint = new List<Vec3>();

    public List<Vec3> castlesPoints = new List<Vec3>();
    public AI_SD aiData = new AI_SD();

    public List<Vec3> altarsPoints = new List<Vec3>();

    public List<Vec3> tombsPoints = new List<Vec3>();
    public List<TombsSD> tombsData = new List<TombsSD>();

    public List<Vec3> resourcesPoints = new List<Vec3>();
    public List<ResBuildingSD> resBuildings = new List<ResBuildingSD>();

    public List<Vec3> campsPoints = new List<Vec3>();

    public List<Vec3> boxesPoints = new List<Vec3>();
    public List<Reward> boxesRewards = new List<Reward>();

    public List<fogCell> fogFreeCells = new List<fogCell>();
}


[Serializable]
public class AI_SD
{
    public List<VassalSD> vassalsList = new List<VassalSD>();
    public List<Vec3> activeCastleList = new List<Vec3>();
}


[Serializable]
public class VassalSD
{
    public Vec3 castlePosition;
    public bool isCastleDestroyed = false;
    public bool isCastleReady = false;
    public int currentRest;

    public EnemySD vassalArmy;

    public bool isVassalActive = false;
    public bool isFlipped = false;
    public Vec3 vassalPosition;

    public bool shouldIContinueAction = false;
    public bool aggressiveMode = false;

    public int currentTriesToGetTarget;

    public Vec3 finishCell = new Vec3(Vector3.zero);

    public AITargetType currentTarget = AITargetType.Rest;
    public AIActions currentAction = AIActions.End;
    public List<AIActions> currentActionsList = new List<AIActions>();

    public Vec3 currentSiegeTargetPosition = new Vec3(Vector3.zero);
}


[Serializable]
public class TombsSD
{
    public Vec3 position;
    public bool status;
    public Reward reward;
    public Spells spell;
    public EnemySD enemyGarrison;
}


[Serializable]
public class ResBuildingSD
{
    public TypeOfObjectsOwner owner = TypeOfObjectsOwner.Nobody;
    public bool isVisited = false;

    public ResourceBuildings buildingType;

    public bool isSiege = false;
    public Vec3 position;

    public List<string> upgrades = new List<string>();

    public int currentSiegeDays;
    public List<UnitsTypes> garrisonTypes = new List<UnitsTypes>();
    public List<int> garrisonAmounts = new List<int>();
}


[Serializable]
public class MapBonusManagerSD
{
    public List<Vec3> heapsPoints = new List<Vec3>();
    public List<Reward> heapsRewards = new List<Reward>();
}


[Serializable]
public class EnemySD
{
    public Vec3 position = new Vec3(Vector3.zero);
    public Army army;
    public TypeOfArmy typeOfArmy;
    public bool isEnemyGarrison = false;
    public Vec3 color = new Vec3(Vector3.zero);
}

[Serializable]
public class EnemySDWrapper
{
    public List<EnemySD> enemyList = new List<EnemySD>();
}

[Serializable]
public class CalendarSD
{
    public int day;
    public int week;
    public int month;
    public int year;

    public int daysPassed;
    public int daysLeft;
    public int weeksPassed;
    public int monthsPassed;
    public int yearsPassed;

    public List<string> decadeList = new List<string>();
    public int currentDecadeIndex;
}


[Serializable]
public class CameraSD
{
    public Vec3 position;
    public Vec3 rotation;
    public float rotationAngle;
    public float zoom;
    public float cameraSize;
}

[Serializable]
public class InfirmarySD
{
    public List<UnitsTypes> units = new List<UnitsTypes>();
    public List<InjuredUnitData> quantity = new List<InjuredUnitData>();
}