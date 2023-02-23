using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class SaveTester : MonoBehaviour, ISaveable
{
    public int Id { get; set; }

    public void Load(object state)
    {
        throw new System.NotImplementedException();
    }

    public object Save()
    {
        throw new System.NotImplementedException();
    }    
}
