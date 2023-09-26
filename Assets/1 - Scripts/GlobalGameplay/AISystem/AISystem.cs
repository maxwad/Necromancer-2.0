using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AISystem : MonoBehaviour
{
    private GameMaster gameMaster;
    private GMInterface gmInterface;
    private GlobalCamera gmCamera;
    private GameObject player;

    [SerializeField] private List<Color> castleColors;
    [SerializeField] private List<string> castleOwners;
    [SerializeField] private int countOfActiveVassals = 3; 

    private List<EnemyCastle> allCastles = new List<EnemyCastle>();
    private List<EnemyCastle> destroyedCastles = new List<EnemyCastle>();
    private List<EnemyCastle> activeCastles = new List<EnemyCastle>();
    private int countOfCastles = 0;
    private int currentMover = 0;

    private bool isCastlesSorted = false;
    private bool isPlayerDead = false;

    [Inject]
    public void Construct(
        GameMaster gameMaster,
        GMInterface gmInterface,
        GMPlayerMovement globalPlayer
        )
    {
        this.gameMaster  = gameMaster;
        this.gmInterface = gmInterface;
        this.player = globalPlayer.gameObject;

        gmCamera = Camera.main.GetComponent<GlobalCamera>();
    }

    public void RegisterCastle(GameObject castle)
    {
        EnemyCastle newCastle = castle.GetComponent<EnemyCastle>();
        if(newCastle != null)
        {
            allCastles.Add(newCastle);

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

        List<EnemyCastle> passiveCastles = new List<EnemyCastle>();
        int activeVassals = 0;

        foreach(var castle in allCastles)
        {
            if(castle.GetCastleStatus() == true)
            {
                passiveCastles.Add(castle);
            }
            else
                activeVassals++;
        }

        if(activeVassals < countOfActiveVassals && activeVassals < allCastles.Count)
        {
            if(passiveCastles.Count > 0)
            {
                //int index = Random.Range(0, passiveCastles.Count);
                int index = 0;
                if(activeCastles.Contains(passiveCastles[index]) == false)
                {
                    activeCastles.Add(passiveCastles[index]);
                }
            }
        }

        gmInterface.turnPart.ActivateTurnBlock(true);
        gmInterface.ShowInterfaceElements(false);
        currentMover = 0;

        //Debug.Log("We have castlse to move: " + activeCastles.Count);
        ActivateNextCastle();
    }

    //for testing
    private void SortCastles()
    {
        //Dictionary<EnemyCastle, bool> sortedCastles = new Dictionary<EnemyCastle, bool>();
        List<EnemyCastle> sortedCastles = new List<EnemyCastle>();
        List<float> distList = new List<float>();
        Dictionary<EnemyCastle, float> distCastles = new Dictionary<EnemyCastle, float>();

        foreach(var castle in allCastles)
        {
            float distance = Vector3.Distance(castle.gameObject.transform.position, player.transform.position);
            distCastles.Add(castle, distance);
            distList.Add(distance);
        }

        distList.Sort();

        for(int i = 0; i < distList.Count; i++)
        {
            foreach(var castle in distCastles)
            {
                if(castle.Value == distList[i])
                {
                    sortedCastles.Add(castle.Key);
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

    public void EndMoves()
    {
        foreach(var checkCastle in new List<EnemyCastle>(activeCastles))
        {
            if(checkCastle.GetCastleStatus() == false && checkCastle.GetRestStatus() == true)
                activeCastles.Remove(checkCastle);
        }

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

    //private void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.Backspace) == true)
    //    {
    //        CastleIsDestroyed();
    //    }
    //}

    public void CastleIsDestroyed(EnemyCastle castle, bool messageNeeded)
    {
        //messageNeeded - for loading = false
        //for destroying during game = true
        castle.CastleDestroyed();
        destroyedCastles.Add(castle);
        allCastles.Remove(castle);
        countOfCastles--;

        if(allCastles.Count == 0)
            Debug.Log("All Vassals are DEAD!");
        else
        {
            foreach(var castleItem in allCastles)
                castleItem.SetNewActionParameters();
        }
    }

    public List<Vassal> GetVassalsInfo()
    {
        List<Vassal> vassals = new List<Vassal>();
        foreach(var item in allCastles)
        {
            if(item.GetCastleStatus() == false)
                vassals.Add(item.GetVassal());
        }

        return vassals;
    }

    public void HandleBattleResult(bool isVassalWin, EnemyArmyOnTheMap enemyArmy)
    {
        bool isVassalFinded = false;

        foreach(var castle in allCastles)
        {
            if(castle.GetCastleStatus() == false)
            {
                if(castle.GetVassal().GetFightPauseStatus() == true)
                {
                    castle.GetVassal().ContinueTurn(isVassalWin);
                    isVassalFinded = true;
                    break;
                }
            }
        }

        if(isVassalFinded == false)
        {
            foreach(var castle in allCastles)
            {
                if(castle.GetVassal().GetArmy() == enemyArmy)
                {
                    castle.GetVassal().ContinueTurn(isVassalWin);
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

    #region SAVE/LOAD

    public AI_SD SaveData()
    {
        AI_SD aiSD = new AI_SD();

        foreach(var castle in allCastles)
            aiSD.vassalsList.Add(castle.SaveData());

        foreach(var castle in destroyedCastles)
            aiSD.vassalsList.Add(castle.SaveData());


        foreach(var castle in activeCastles)
            aiSD.activeCastleList.Add(castle.transform.position.ToVec3());

        return aiSD;
    }

    public void LoadData(AI_SD aiData)
    {
        foreach(var castle in aiData.vassalsList)
        {
            allCastles
                    .Where(o => o.transform.position == castle.castlePosition.ToVector3())
                    .First()
                    .LoadData(castle);
        }

        foreach(var castle in aiData.vassalsList)
        {
            if(castle.isCastleDestroyed == true)
            {
                EnemyCastle neededCastle = allCastles
                    .Where(o => o.transform.position == castle.castlePosition.ToVector3())
                    .First();

                if(neededCastle == null)
                    Debug.Log("ATTENTION! I CAN'T FIND DESTROYED CASTLE");
                else
                    CastleIsDestroyed(neededCastle, false);
            }
        }

        foreach(var castlePos in aiData.activeCastleList)
        {
             activeCastles.Add(allCastles
                 .Where(o => o.transform.position == castlePos.ToVector3())
                 .First()
                 );
        }
    }

    #endregion
}
