using System;
using UnityEngine;

public class EnemyCastle : MonoBehaviour
{
    private AISystem aiSystem;

    [SerializeField] private int defeatRestDays = 10;
    [SerializeField] private int completeRestDays = 5;
    private int currentRest = 0;
    private bool isReady = true;
    private bool isCastleDestroyed = false;

    public Vassal vassal;
    private Vector3 enterPoint;
    private GameObject player;

    private void Start()
    {       
        aiSystem   = GlobalStorage.instance.aiSystem;
        player     = GlobalStorage.instance.globalPlayer.gameObject;
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

            if(currentRest == 0)
                isReady = true;
        }        
    }

    private bool IsPlayerHere()
    {
        return player.transform.position == enterPoint;
    }

    public void GiveMyABreak(bool deathMode = false)
    {
        isReady = false;
        currentRest = (deathMode == false) ? completeRestDays : defeatRestDays;
        //aiSystem.CrusadeComplete(this);
    }

    public Vector3 GetStartPosition() => enterPoint;

    public Vassal GetVassal() => vassal;

    public bool GetCastleStatus() => isReady;

    public bool GetRestStatus() => currentRest > 0;

    public void CastleDestroyed() => isCastleDestroyed = true;

    public bool IsCastleDestroyed() => isCastleDestroyed;

    //public void SetNewActionParameters()
    //{
    //    defeatRestDays--;
    //    completeRestDays--;

    //    vassal.SetNewActionParameters();
    //}

    #region SAVE/LOAD

    public VassalSD SaveData()
    {
        VassalSD vassalSD = new VassalSD();

        if(isCastleDestroyed == false)
            vassalSD = vassal.SaveData();

        vassalSD.isCastleDestroyed = isCastleDestroyed;
        vassalSD.isCastleReady = isReady;
        vassalSD.currentRest = currentRest;

        return vassalSD;
    }

    public void LoadData()
    {

    }
    #endregion

    private void OnEnable()
    {
        EventManager.NewMove += CheckStatus;
    }


    private void OnDisable()
    {
        EventManager.NewMove -= CheckStatus;
    }
}
