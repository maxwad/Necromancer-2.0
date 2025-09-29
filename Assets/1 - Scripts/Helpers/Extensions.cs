using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public static class Extensions
{
    public static Vec3 ToVec3(this Vector3 realVector)
    {
        return new Vec3(realVector);
    }

    public static Vec3 ToVec3(this Vector3Int realVector)
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

    public static List<Vec3> ToVec3List(this List<Vector3Int> list)
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


    public static Vector3Int ToVector3Int(this Vec3 falseVector)
    {
        return new Vector3Int((int)falseVector.x, (int)falseVector.y, (int)falseVector.z);
    }

    public static List<Vector3Int> ToVector3IntList(this List<Vec3> list)
    {
        List<Vector3Int> newList = new List<Vector3Int>();

        foreach(var falseVector in list)
            newList.Add(falseVector.ToVector3Int());

        return newList;
    }

    public static Vector3Int ToVector3Int(this fogCell cell)
    {
        return new Vector3Int((int)cell.x, (int)cell.y, 0);
    }

    public static List<Vector3Int> ToVector3IntList(this List<fogCell> list)
    {
        List<Vector3Int> newList = new List<Vector3Int>();

        foreach(var cell in list)
            newList.Add(cell.ToVector3Int());

        return newList;
    }

    public static fogCell ToFogCell(this Vector3Int realVector)
    {
        return new fogCell(realVector);
    }

    public static List<fogCell> ToFogCellList(this List<Vector3Int> list)
    {
        List<fogCell> newList = new List<fogCell>();

        foreach(var realVector in list)
            newList.Add(realVector.ToFogCell());

        return newList;
    }
}
