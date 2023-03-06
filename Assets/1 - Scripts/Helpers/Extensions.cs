using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public static class Extensions
{
    public static Vec3 ToVec3(this Vector3 realVector)
    {
        return new Vec3(realVector);
    }

    public static List<Vec3> ToVec3List(this List<Vector3> list)
    {
        List<Vec3> newList = new List<Vec3>();

        foreach(var realVector in list)
            newList.Add(realVector.ToVec3());

        return newList;
    }


    public static Vector3 ToVector3(this Vec3 falseVector)
    {
        return new Vector3(falseVector.x, falseVector.y, falseVector.z);
    }

    public static List<Vector3> ToVector3List(this List<Vec3> list)
    {
        List<Vector3> newList = new List<Vector3>();

        foreach(var falseVector in list)
            newList.Add(falseVector.ToVector3());

        return newList;
    }

}
