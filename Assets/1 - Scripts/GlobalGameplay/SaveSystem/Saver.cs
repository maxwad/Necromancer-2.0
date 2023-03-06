using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public abstract class Saver : ISaveable
{
    public int _id = -1;

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

    public void SetId(int id) => Id = id;

    public int GetId() => Id;

    public abstract void Load(SaveLoadManager manager, Dictionary<int, object> state);

    public abstract void Save(SaveLoadManager manager);
}
