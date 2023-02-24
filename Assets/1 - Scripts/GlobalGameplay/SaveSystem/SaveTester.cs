using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class SaveTester : MonoBehaviour, ISaveable
{
    private int _id = -1;
    //public string Id
    //{
    //    get
    //    {
    //        return _id; 
    //    }
    //    set
    //    {
    //        _id = value; 
    //    }
    //}



    public int Id { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public void Load(object state)
    {
        throw new System.NotImplementedException();
    }

    public void Save(SaveLoadManager saveManager)
    {
        if(_id == -1)
            _id = Random.Range(0, int.MaxValue);

        Reward reward = new Reward(new List<ResourceType>() { ResourceType.Food, ResourceType.Gold }, new List<float>() { 4.7f, 0.1f });

        saveManager.FillSaveData(_id, reward);
    }    
}
