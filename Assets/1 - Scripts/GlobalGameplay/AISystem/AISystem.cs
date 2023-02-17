using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class AISystem : MonoBehaviour
{
    private GameMaster gameMaster;
    private GMInterface gmInterface;
    private GlobalCamera gmCamera;

    [SerializeField] private List<Color> castleColors;
    [SerializeField] private List<string> castleOwners;
    [SerializeField] private int countOfActiveVassals = 8; 

    private Dictionary<EnemyCastle, bool> allCastles = new Dictionary<EnemyCastle, bool>();
    private List<EnemyCastle> activeCastles = new List<EnemyCastle>();
    private int countOfCastles = 0;
    private int currentMover = 0;

    private int currentCastle = 0;
    private bool isCastlesSorted = false;
    private bool isPlayerDead = false;

    private void Start()
    {
        gmInterface = GlobalStorage.instance.gmInterface;
        gmCamera = Camera.main.GetComponent<GlobalCamera>();
    }

    public void RegisterCastle(GameObject castle)
    {
        EnemyCastle newCastle = castle.GetComponent<EnemyCastle>();
        if(newCastle != null)
        {
            allCastles.Add(newCastle, false);
            //allCastlesGO.Add(castle);

            Color castleColor = Color.black;
            string name = "";

            if(castleColors.Count > countOfCastles)
                castleColor = castleColors[countOfCastles];

            if(castleOwners.Count > countOfCastles)
                name = castleOwners[countOfCastles];

            newCastle.Init(castleColor, name);

            countOfCastles++;
        }
    }

    public void StartMoves()
    {
        if(allCastles.Count == 0) 
            EndMoves();

        if(isCastlesSorted == false)
            SortCastles();

        gmInterface.turnPart.ActivateTurnBlock(true);
        gmInterface.ShowInterfaceElements(false);

        currentMover = 0;

        List<EnemyCastle> passiveCastles = new List<EnemyCastle>();
        int activeVassals = 0;

        foreach(var castle in allCastles)
        {
            if(castle.Value == false && castle.Key.GetCastleStatus() == true)
                passiveCastles.Add(castle.Key);
            else
                activeVassals++;
        }

        if(activeVassals < countOfActiveVassals && activeVassals < allCastles.Count)
        {
            //int index = Random.Range(0, passiveCastles.Count);
            int index = 0;
            if(activeCastles.Contains(passiveCastles[index]) == false)
            {
                activeCastles.Add(passiveCastles[index]);
                allCastles[passiveCastles[index]] = true;
            }
        }

        ActivateNextCastle();
    }

    //for testing
    private void SortCastles()
    {
        Dictionary<EnemyCastle, bool> sortedCastles = new Dictionary<EnemyCastle, bool>();
        List<float> distList = new List<float>();
        Dictionary<EnemyCastle, float> distCastles = new Dictionary<EnemyCastle, float>();

        foreach(var castle in allCastles)
        {
            float distance = Vector3.Distance(castle.Key.gameObject.transform.position, GlobalStorage.instance.globalPlayer.gameObject.transform.position);
            distCastles.Add(castle.Key, distance);
            distList.Add(distance);
        }

        distList.Sort();

        for(int i = 0; i < distList.Count; i++)
        {
            foreach(var castle in distCastles)
            {
                if(castle.Value == distList[i])
                {
                    sortedCastles.Add(castle.Key, false);
                    break;
                }
            }
        }

        isCastlesSorted = true;
        allCastles = sortedCastles;
    }

    private void ActivateNextCastle()
    {
        if(currentMover >= activeCastles.Count)
        {
            EndMoves();
        }
        else
        {
            activeCastles[currentMover].Activate();
            currentMover++;
        }
    }

    public void CastleDoneMove()
    {
        ActivateNextCastle();
    }

    public void CrusadeComplete(EnemyCastle castle)
    {
        allCastles[castle] = false;
        activeCastles.Remove(castle);
    }

    public void EndMoves()
    {
        if(gameMaster == null)
            gameMaster = GlobalStorage.instance.gameMaster;

        StartCoroutine(FinishTurnes());
    }

    private IEnumerator FinishTurnes()
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(0.5f);

        gmInterface.turnPart.ActivateTurnBlock(false);
        yield return delay;

        gmCamera.SetObserveObject();
        gmInterface.ShowInterfaceElements(true);
        gmInterface.turnPart.ActivateTurnBlock(true);
        gmInterface.turnPart.FillMessage(false, "", Color.white);

        yield return delay;
        yield return delay;

        if(isPlayerDead == true)
        {
            EventManager.OnDefeatEvent();
            isPlayerDead = false;
        }
        gmInterface.turnPart.ActivateTurnBlock(false);
        gameMaster.EndEnemyMoves();
    }


    //public void CastleIsDestroyed(EnemyCastle castle)
    //{
    //    allCastles.Remove(castle);
    //    countOfCastles--;

    //    if(allCastles.Count == 0)
    //        Debug.Log("All Vassals are DEAD!");
    //}

    public List<Vassal> GetVassalsInfo()
    {
        List<Vassal> vassals = new List<Vassal>();
        foreach(var item in allCastles)
        {
            if(item.Value == true)
                vassals.Add(item.Key.GetVassal());
        }

        return vassals;
    }

    public void HandleBattleResult(bool isVassalWin, EnemyArmyOnTheMap enemyArmy)
    {
        bool isVassalFinded = false;

        foreach(var castle in allCastles)
        {
            if(castle.Value == true)
            {
                if(castle.Key.vassal.GetFightPauseStatus() == true)
                {
                    castle.Key.vassal.ContinueTurn(isVassalWin);
                    isVassalFinded = true;
                    break;
                }
            }
        }

        if(isVassalFinded == false)
        {
            foreach(var castle in allCastles)
            {
                if(castle.Key.vassal.GetArmy() == enemyArmy)
                {
                    castle.Key.vassal.ContinueTurn(isVassalWin);
                    break;
                }
            }
        }
    }

    public void SetPlayerDeath(bool deathMode)
    {
        isPlayerDead = deathMode;
    }

    public bool IsPlayerDead()
    {
        return isPlayerDead;
    }
}
