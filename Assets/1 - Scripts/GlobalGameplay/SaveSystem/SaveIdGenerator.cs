using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static Enums;

public class SaveIdGenerator : MonoBehaviour
{
    private List<ISaveable> objectsToSave = new List<ISaveable>();
    private int parallelIdFlag = 100;

    [ContextMenu("Generate ids")]
    public void GenerateId()
    {
        objectsToSave = new List<ISaveable>(FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>());

        Debug.Log(objectsToSave.Count + " objects have numerated.");

        for(int i = 0; i < objectsToSave.Count; i++)
        {
            objectsToSave[i].SetId(i + parallelIdFlag);
            Debug.Log(i + parallelIdFlag + " - " + objectsToSave[i].GetType());
        }
    }
}
