using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public partial class MapBonusManager : ISaveable
{
    private int _id = 1;

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

    public int GetId() => Id;

    public void Save(SaveLoadManager manager)
    {
        List<Reward> heapsRewards = new List<Reward>();

        foreach(var heap in heapsPointsDict)
            heapsRewards.Add(heap.Key.SaveReward());

        Debug.Log(heapsRewards.Count + " heaps saved.");

        MapBonusManagerSD saveData = new MapBonusManagerSD();
        saveData.heapsRewards = heapsRewards;
        saveData.heapsPoints = TypesConverter.SplitDictionary(heapsPointsDict).Item2.ToVec3List();
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