using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static NameManager;

public class SaveIdGenerator : MonoBehaviour
{
    private List<ISaveable> objectsToSave = new List<ISaveable>();

    [ContextMenu("Generate ids")]
    public void GenerateId()
    {
        objectsToSave = new List<ISaveable>(FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>());

        Debug.Log(objectsToSave.Count + " objects have numerated.");

        for(int i = 0; i < objectsToSave.Count; i++)
            objectsToSave[i].SetId(i + 1);
    }
}
