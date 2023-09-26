using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AltarBuilder : MonoBehaviour
{
    public GameObject altarsContainer;
    public Dictionary<Vector3, Altar> altars = new Dictionary<Vector3, Altar>();

    public void Build()
    {
        foreach(Transform child in altarsContainer.transform)
        {
            Altar altar = child.gameObject.GetComponent<Altar>();

            if(altar != null)
                altars.Add(child.position, altar);
        }            
    }

    public List<Vector3> GetPointsList()
    {
        List<Vector3> saveData = new List<Vector3>();
        foreach(var altar in altars)
        {
            if(altar.Value.GetVisitStatus() == true)
                saveData.Add(altar.Key);
        }

        return saveData;
    }

    public void LoadData(List<Vec3> saveData)
    {
        if(saveData != null)
        {
            foreach(var altar in saveData)
            {
                Vector3 position = altar.ToVector3();

                if(altars.ContainsKey(position) == true)
                    altars[position].VisitRegistration();
            }
        }
    }
}
