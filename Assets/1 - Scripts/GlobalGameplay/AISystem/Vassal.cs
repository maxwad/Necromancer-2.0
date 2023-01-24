using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class Vassal : MonoBehaviour
{
    private int castleIndex;

    internal void Init(int index, Color castleColor)
    {
        castleIndex = index;
        GetComponent<SpriteRenderer>().color = castleColor;
    }
    public void StartAction()
    {
        
    }
}
