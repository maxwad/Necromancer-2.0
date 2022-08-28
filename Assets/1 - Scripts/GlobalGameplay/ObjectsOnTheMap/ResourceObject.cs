using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class ResourceObject : MonoBehaviour
{
    public ResourceType resourceType;

    public float quantity;

    public float maxQuantity = 10;

    private void Awake()
    {
        quantity = Random.Range(0, maxQuantity + 1);
    }


}
