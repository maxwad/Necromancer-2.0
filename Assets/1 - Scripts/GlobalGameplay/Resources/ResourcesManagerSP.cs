using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public partial class ResourcesManager : ISaveable
{
    [SerializeField] private int _id = 101;

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
        if(Id >= 100) Id = id;
    }

    public int GetId() => Id;

    public class ResourceManagerSD
    {
        public List<float> resourcesList = new List<float>();
    }

    public void Save(SaveLoadManager manager)
    {
        ResourceManagerSD saveData = new ResourceManagerSD();

        saveData.resourcesList = new List<float>(resourcesDict.Values);

        manager.FillSaveData(Id, saveData);
    }

    public void Load(SaveLoadManager manager, Dictionary<int, object> state)
    {
        if(state.ContainsKey(Id) == false)
        {
            manager.LoadDataComplete("WARNING: no data for Calendar");
            return;
        }

        ResourceManagerSD saveData = TypesConverter.ConvertToRequiredType<ResourceManagerSD>(state[Id]);

        int counter = 0;
        foreach(var resource in new List<ResourceType>(resourcesDict.Keys))
        {
            LoadResource(resource, saveData.resourcesList[counter]);
            counter++;
        }

        manager.LoadDataComplete("Resources are loaded");
    }
}
