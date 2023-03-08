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
public class GMTileManagerSD
{
    public List<Vec3> arenaPoint = new List<Vec3>();
    public List<Vec3> castlesPoints = new List<Vec3>();

    public List<Vec3> tombsPoints = new List<Vec3>();
    public List<TombsSD> tombsData = new List<TombsSD>();

    public List<Vec3> resourcesPoints = new List<Vec3>();
    public List<ResBuildingSD> resBuildings = new List<ResBuildingSD>();

    public List<Vec3> campsPoints = new List<Vec3>();

    public List<Vec3> boxesPoints = new List<Vec3>();
    public List<Reward> boxesRewards = new List<Reward>();
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

//public class CampsSD
//{
//    public List<Vec3> campsPoints = new List<Vec3>();
//}

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