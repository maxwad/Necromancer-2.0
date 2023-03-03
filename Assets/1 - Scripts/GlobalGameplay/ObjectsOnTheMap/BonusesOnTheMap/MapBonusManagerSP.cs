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

    public class MapBonusManagerSD
    {
        public List<Vec3> heapsPoints = new List<Vec3>();
        public List<Reward> heapsRewards = new List<Reward>();
    }

    public void Save(SaveLoadManager manager)
    {
        List<Reward> heapsRewards = new List<Reward>();

        foreach(var heap in heapsPointsDict)
            heapsRewards.Add(heap.Key.SaveReward());

        MapBonusManagerSD saveData = new MapBonusManagerSD();
        saveData.heapsRewards = heapsRewards;
        saveData.heapsPoints = TypesConverter.SplitDictionary(heapsPointsDict).Item2.ToVec3List();
        manager.FillSaveData(Id, saveData);
    }

    public void Load(SaveLoadManager manager, Dictionary<int, object> state)
    {
        throw new System.NotImplementedException();
    }
}
