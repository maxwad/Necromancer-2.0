using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public partial class GlobalMapTileManager : ISaveable
{
    private int _id = 0;

    public int Id
    {
        get
        {
            return _id;
        }
        set
        {
            _id = value;
        }
    }

    public void SetId(int id) 
    {
        if(Id == -1) Id = id;
    }


    public void Save(SaveLoadManager manager)
    {
        List<Vector3> arenaPoint = arenaBuilder.GetPointsList();
        List<Vector3> castlesPoints = castleBuilder.GetPointsList();
        List<Vector3> tombsPoints = tombBuilder.GetPointsList();
        List<Vector3> resourcesPoints = resourceBuilder.GetPointsList();






    }











    public void Load(SaveLoadManager manager, Dictionary<int, object> state)
    {
        throw new System.NotImplementedException();
    }
}
