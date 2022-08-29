using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class ResourceObject : MonoBehaviour
{
    public ResourceType resourceType;

    public float quantity;

    public float maxQuantity = 100;

    private void Awake()
    {
        quantity = Mathf.Round(Random.Range(0, maxQuantity + 1));
    }
}
