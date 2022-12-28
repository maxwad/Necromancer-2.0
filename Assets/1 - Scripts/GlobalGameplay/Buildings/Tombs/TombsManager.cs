using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class TombsManager : MonoBehaviour
{
    private Dictionary<GameObject, Vector3> tombsDict = new Dictionary<GameObject, Vector3>();

    public void Register(GameObject building, Vector3 position)
    {
        tombsDict.Add(building, position);
    }

    public Dictionary<GameObject, Vector3> GetTombs()
    {
        return tombsDict;
    }
}
