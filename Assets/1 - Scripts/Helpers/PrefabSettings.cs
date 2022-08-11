using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSettings : MonoBehaviour
{
    private SpriteRenderer sprite;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void SetSettings(
        float size = -1, 
        Color color = default(Color), 
        int sortingOrder = -1, 
        string sortingLayer = "",
        float animationSpeed = 0f
        )
    {
        if(size != -1) transform.localScale = new Vector3(size, size, size);

        if(color != Color.clear) sprite.color = color;

        if(sortingOrder != -1) sprite.sortingOrder = sortingOrder;

        if(sortingLayer != "") sprite.sortingLayerName = sortingLayer;

        if(animationSpeed != 0) GetComponent<SimpleAnimator>().SetSpeed(animationSpeed);
    }
}
