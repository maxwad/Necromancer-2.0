using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class Reward
{
    //public float exp = 0;
    public List<ResourceType> resourcesList = new List<ResourceType>();
    public List<float> resourcesQuantity = new List<float>();
    //public float mana = 0;

    public Reward( List<ResourceType> resources, List<float> quantity)
    {
        //exp = expValue;
        resourcesList = resources;
        resourcesQuantity = quantity;
        //mana = manaValue;
    }
    
}
