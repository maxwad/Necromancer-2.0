using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class SaveTester : MonoBehaviour, ISaveable
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

    public float foodQuantity;
    public float goldQuantity;

    public void SetId(int id) => Id = id;

    public void Save(SaveLoadManager saveManager)
    {
        if(Id == -1)
            Id = Random.Range(0, int.MaxValue);

        Reward reward = new Reward(
            new List<ResourceType>() 
            { 
                ResourceType.Food, 
                ResourceType.Gold 
            },
            new List<float>() 
            {
                foodQuantity,
                goldQuantity 
            });

        saveManager.FillSaveData(Id, reward);
    }    

    public void Load(SaveLoadManager saveManager, Dictionary<int, object> state)
    {
        if(state.ContainsKey(Id) == true)
        {
            Reward reward = saveManager.ConvertToRequiredType<Reward>(state[Id]);

            foodQuantity = (int)reward.resourcesQuantity[0];
            goldQuantity = reward.resourcesQuantity[1];
        }
        else
        {
            Debug.Log("I don't have data for loading!");
        }

        saveManager.LoadDataComplete("Tester is loaded");
    }
}
