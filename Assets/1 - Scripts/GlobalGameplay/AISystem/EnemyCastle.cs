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

    private void Start()
    {       
        aiSystem = GlobalStorage.instance.aiSystem;
        player = GlobalStorage.instance.player;
        enterPoint = GetComponent<ClickableObject>().GetEnterPoint();
    }

    public void Init(Color castleColor, string name)
    {
        GetComponent<SpriteRenderer>().color = castleColor;
        GetComponent<TooltipTrigger>().header = name + "'s Castle";
        gameObject.name = name + "'s Castle";

        vassal.Init(this, castleColor, name);
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

    public Vector3 GetStartPosition()
    {
        return enterPoint;
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
