using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class ObjectOwner : MonoBehaviour
{
    public TypeOfObjectsOwner owner;
    public bool isGuardNeeded = true;
    public float probabilityGuard = 100;
}
