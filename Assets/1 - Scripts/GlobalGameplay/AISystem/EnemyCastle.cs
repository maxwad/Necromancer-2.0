using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class EnemyCastle : MonoBehaviour
{
    private AISystem aiSystem;

    [SerializeField] private int defeatRestDays = 30;
    [SerializeField] private int completeRestDays = 15;
    private int currentRest = 0;
    private bool isReady = true;

    [SerializeField] private Vassal vassal;
    private Vector3 enterPoint;
    private GameObject player;

    private int castleIndex;

    private void Start()
    {       
        aiSystem = GlobalStorage.instance.aiSystem;
        player = GlobalStorage.instance.player;
        enterPoint = GetComponent<ClickableObject>().GetEnterPoint();
    }

    internal void Init(int index, Color castleColor)
    {
        GetComponent<SpriteRenderer>().color = castleColor;
        castleIndex = index;

        vassal.Init(index, castleColor);
    }

    public bool Activate()
    {
        if(isReady == true)
        {
            if(IsPlayerHere() == true)
            {
                return false;
            }
            else
            {
                Debug.Log(gameObject.name + " is activated.");
                vassal.StartAction();
                isReady = false;
                return true;
            }
        }

        return false;
    }

    private void CheckStatus()
    {
        if(currentRest > 0)
        {
            currentRest--;

            if(currentRest == 0)
                isReady = true;
        }        
    }

    private bool IsPlayerHere()
    {
        return player.transform.position == enterPoint;
    }

    private void OnEnable()
    {
        EventManager.NewMove += CheckStatus;
    }


    private void OnDisable()
    {
        EventManager.NewMove -= CheckStatus;
    }
}
