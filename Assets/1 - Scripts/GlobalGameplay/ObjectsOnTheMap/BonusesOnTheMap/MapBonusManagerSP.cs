using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public partial class MapBonusManager : ISaveable
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

    public void Save(SaveLoadManager manager)
    {
        List<Reward> heapsRewards = new List<Reward>();

        foreach(var heap in heapsPointsDict)
            heapsRewards.Add(heap.Key.SaveReward());

        MapBonusManagerSD saveData = new MapBonusManagerSD();
        saveData.heapsRewards = heapsRewards;
        saveData.heapsPoints = TypesConverter.SplitDictionary(heapsPointsDict).Item2.ToVec3List();

        Debug.Log(heapsRewards.Count + " heaps saved.");

        manager.FillSaveData(Id, saveData);
    }

    public void Load(SaveLoadManager manager, Dictionary<int, object> state)
    {
        if(state.ContainsKey(Id) == false)
        {
            manager.LoadDataComplete("WARNING: no data about Heaps");
            return;
        }

        MapBonusManagerSD saveData = manager.ConvertToRequiredType<MapBonusManagerSD>(state[Id]);

        heapsPointsDict.Clear();
        List<Vector3> heapPoints = saveData.heapsPoints.ToVector3List();

        for(int i = 0; i < heapPoints.Count; i++)
        {
            ResourceObject heap = CreateHeap(heapPoints[i], false);
            heap.Load(saveData.heapsRewards[i]);
        }

        manager.LoadDataComplete("Heaps are loaded (" + heapPoints.Count + ")");
    }
}
