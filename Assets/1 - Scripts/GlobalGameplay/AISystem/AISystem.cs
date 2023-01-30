using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class AISystem : MonoBehaviour
{
    private GameMaster gameMaster;

    [SerializeField] private List<Color> castleColors;
    [SerializeField] private List<string> castleOwners;
    [SerializeField] private int countOfActiveVassals = 8; 

    private Dictionary<EnemyCastle, bool> allCastles = new Dictionary<EnemyCastle, bool>();
    //private List<GameObject> allCastlesGO = new List<GameObject>();
    private List<EnemyCastle> activeCastles = new List<EnemyCastle>();
    private int countOfCastles = 0;
    private int currentMover = 0;

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
        if(allCastles.Count == 0) EndMoves();

        currentMover = 0;

        List<EnemyCastle> passiveCastles = new List<EnemyCastle>();
        int activeVassals = 0;

        foreach(var castle in allCastles)
        {
            if(castle.Value == false)
                passiveCastles.Add(castle.Key);
            else
                activeVassals++;
        }

        if(activeVassals < countOfActiveVassals && activeVassals < allCastles.Count)
        {
            int index = Random.Range(0, passiveCastles.Count);
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
        Debug.Log("currentMover = " + currentMover + "/" + activeCastles.Count);

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

        gameMaster.EndEnemyMoves();
    }

    public void CrusadeComplete(EnemyCastle castle)
    {
        allCastles[castle] = false;
        activeCastles.Remove(castle);
    }

    public void CastleIsDestroyed(EnemyCastle castle)
    {
        allCastles.Remove(castle);
        countOfCastles--;

        if(allCastles.Count == 0)
            Debug.Log("All Vassals are DEAD!");
    }

    //public List<GameObject> GetCastles()
    //{
    //    return allCastlesGO;
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

    //private void OnEnable()
    //{
    //    EventManager.NewMove += CheckStatus;
    //}

    //private void OnDisable()
    //{
    //    EventManager.NewMove -= CheckStatus;
    //}
}
