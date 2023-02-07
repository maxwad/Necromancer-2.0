using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class AISystem : MonoBehaviour
{
    private GameMaster gameMaster;
    private GMInterface gmInterface;

    [SerializeField] private List<Color> castleColors;
    [SerializeField] private List<string> castleOwners;
    [SerializeField] private int countOfActiveVassals = 8; 

    private Dictionary<EnemyCastle, bool> allCastles = new Dictionary<EnemyCastle, bool>();
    //private List<GameObject> allCastlesGO = new List<GameObject>();
    private List<EnemyCastle> activeCastles = new List<EnemyCastle>();
    private int countOfCastles = 0;
    private int currentMover = 0;

    private int currentCastle = 0;

    private void Start()
    {
        gmInterface = GlobalStorage.instance.gmInterface;
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
        else
        {
            //SortCastles();
        }

        gmInterface.turnPart.ActivateTurnBlock(true);

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

    public void EndMoves()
    {
        if(gameMaster == null)
            gameMaster = GlobalStorage.instance.gameMaster;

        gmInterface.turnPart.ActivateTurnBlock(false);
        gameMaster.EndEnemyMoves();
    }

    public void CrusadeComplete(EnemyCastle castle)
    {
        allCastles[castle] = false;
        activeCastles.Remove(castle);
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

    public void HandleBattleResult(bool isVassalWin)
    {
        foreach(var castle in allCastles)
        {
            if(castle.Value == true)
            {
                if(castle.Key.vassal.GetFightPauseStatus() == true)
                {
                    castle.Key.vassal.ContinueTurn(isVassalWin);
                    break;
                }
            }
        }
    }

}
