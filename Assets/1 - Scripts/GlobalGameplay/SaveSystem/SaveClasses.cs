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
    public List<Vec3> resourcesPoints = new List<Vec3>();

    public List<Vec3> boxesPoints = new List<Vec3>();
    public List<Reward> boxesRewards = new List<Reward>();
}

[Serializable]
public class GMTileManagerSDLarge
{
    public List<Vector3> arenaPoint = new List<Vector3>();
    public List<Vector3> castlesPoints = new List<Vector3>();
    public List<Vector3> tombsPoints = new List<Vector3>();
    public List<Vector3> resourcesPoints = new List<Vector3>();

    public List<Vector3> boxesPoints = new List<Vector3>();
    public List<Reward> boxesRewards = new List<Reward>();
}

//public class HeapSaveData
//{
//    public ResourceType resourceType;
//    public SpriteRenderer sprite;
//    public Reward reward;
//}