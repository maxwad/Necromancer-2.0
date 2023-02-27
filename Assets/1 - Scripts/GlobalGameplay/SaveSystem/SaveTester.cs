using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class SaveTester : MonoBehaviour, ISaveable
{
    public int _id = -1;
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
    [SerializeField]
    public int goldQuantity;
    [SerializeField]
    public int foodQuantity;

    public int Id { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public void Save(SaveLoadManager saveManager)
    {
        if(_id == -1)
            _id = Random.Range(0, int.MaxValue);

        Reward reward = new Reward(new List<ResourceType>() { ResourceType.Food, ResourceType.Gold }, new List<float>() { foodQuantity, goldQuantity });

        saveManager.FillSaveData(_id, reward);
    }    

    public void Load(SaveLoadManager saveManager, Dictionary<int, object> state)
    {
        Debug.Log(_id + ": " + state.GetType());
        goldQuantity = 1234;
        foodQuantity = 4567;

        //if(state.ContainsKey(_id) == true)
        //{
        //    //Debug.Log(state[_id].GetType());
        //    Newtonsoft.Json.Linq.JObject test = (Newtonsoft.Json.Linq.JObject)state[_id];
        //    Reward reward = test.ToObject<Reward>();
        //    Debug.Log("LOADED for " + gameObject.name + " : " + reward.resourcesQuantity[0] + " & " + reward.resourcesQuantity[1]);
        //    goldQuantity = reward.resourcesQuantity[0];
        //    foodQuantity = reward.resourcesQuantity[1];
        //    Debug.Log("LOADED for " + gameObject.name + " : " + reward.resourcesQuantity[0] + " & " + reward.resourcesQuantity[1]);
        //}
        //else
        //{
        //    Debug.Log("I don't have data for loading!");
        //}

        //goldQuantity = 1234;
        //foodQuantity = 4567;

        saveManager.LoadDataComplete("Tester is loaded");
    }
}
