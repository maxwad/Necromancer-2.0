using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class EnemyCastle : MonoBehaviour
{
    private AISystem aiSystem;

    [SerializeField] private int defeatRestDays = 30;
    [SerializeField] private int completeRestDays = 5;
    private int currentRest = 0;
    private bool isReady = true;

    public Vassal vassal;
    private Vector3 enterPoint;
    private GameObject player;

    private void Start()
    {       
        aiSystem = GlobalStorage.instance.aiSystem;
        player = GlobalStorage.instance.globalPlayer.gameObject;
        enterPoint = GetComponent<ClickableObject>().GetEnterPoint();
    }

    public void Init(Color castleColor, string name)
    {
        GetComponent<SpriteRenderer>().color = castleColor;
        GetComponent<TooltipTrigger>().header = name + "'s Castle";
        gameObject.name = name + "'s Castle";

        vassal.Init(this, castleColor, name);
    }

    public void Activate()
    {
        if(IsPlayerHere() == true)
        {
            EndOfMove();
        }
        else
        {
            vassal.StartAction();
            isReady = false;
        }
    }

    public void EndOfMove()
    {
        aiSystem.CastleDoneMove();
    }

    private void CheckStatus()
    {
        if(currentRest > 0)
        {
            currentRest--;
            Debug.Log("Days to new goal " + currentRest);

            if(currentRest == 0)
                isReady = true;
        }        
    }

    private bool IsPlayerHere()
    {
        return player.transform.position == enterPoint;
    }

    public void GiveMyABreak()
    {
        isReady = false;
        currentRest = completeRestDays;
        aiSystem.CrusadeComplete(this);
    }

    public Vector3 GetStartPosition()
    {
        return enterPoint;
    }

    public Vassal GetVassal()
    {
        return vassal;
    }

    public bool GetCastleStatus()
    {
        return isReady;
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
