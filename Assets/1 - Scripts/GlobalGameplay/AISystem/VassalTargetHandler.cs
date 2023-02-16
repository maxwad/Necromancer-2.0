using UnityEngine;
using static NameManager;

public partial class VassalTargetSelector
{
    #region TASKS

    private void UpdatePath()
    {
        //if(finishCell == Vector3Int.zero)
        //{
        //    Debug.Log("THERE IS PROBLEM with the random finish cell");
        //    SelectTESTTarget();
        //    //CheckNextTarget();
        //}
        //else
        //    currentPath = pathfinder.CreatePath(finishCell);

        currentPath = pathfinder.CreatePath(finishCell);

    }

    private void FindPathToRandomCell()
    {
        finishCell = pathfinder.FindRandomCell();
        //UpdatePath();        
    }

    private void FindPathToTheOwnCastle()
    {
        finishCell = pathfinder.ConvertToV3Int(mainAI.GetCastlePoint());
        //UpdatePath();
    }
        
    private void FindPathToTheResBuilding()
    {
        finishCell = pathfinder.FindResBuildingCell();
        //UpdatePath();
    }

    private void FindPathToThePlayerCastle()
    {
        finishCell = pathfinder.FindPlayerCastleCell();
        //UpdatePath();
    }
    private void FindPathToThePlayer()
    {
        finishCell = pathfinder.ConvertToV3Int(player.transform.position);
        //UpdatePath();
    }

    public void PrepareToRest(bool deathMode = false)
    {
        currentTarget = AITargetType.Rest;
        shouldIContinueAction = false;
        currentPath.Clear();
        mainAI.CrusadeIsOver(deathMode);
    }

    public void TeleportingToTheCastle()
    {
        finishCell = pathfinder.ConvertToV3Int(mainAI.GetCastlePoint());
        movement.Teleportation(finishCell);
    }

    public void SplitArmy()
    {
        if(enemyManager.CheckPositionInEnemyPoints(transform.position) == true)
            return;

        if(pathfinder.CheckCellAsEnterPoint(transform.position) == true)
            return;

        vassalsArmy.Splitting();
    }


    private void Siege()
    {
        if(currentSiegeTarget == null)
            mainAI.EndOfMove();

        if(currentSiegeTarget.CheckOwner(TypeOfObjectsOwner.Enemy) == true)
        {
            if(siegeDaysLeft == 0)
            {
                Debug.Log("!!! Castle is down");
                GetNextAction();
            }
            else
            {
                Debug.Log("0 Siege is over");
                currentSiegeTarget = null;
                SelectSpecialTarget(AITargetType.ToTheOwnCastle);
                GetNextAction();
            }
        }
        else
        {
            if(currentSiegeTarget.CheckSiegeStatus() == false)
            {
                siegeDaysLeft = currentSiegeTarget.StartSiege();
                startSiegeAmountArmy = vassalsArmy.GetCommonAmountArmy();
                Debug.Log("1 Siege is started. There are enemies = " + startSiegeAmountArmy);
                mainAI.EndOfMove();
            }
            else
            {
                vassalsArmy.DecreaseSquads(GetBuildingsLevel());
                Debug.Log("2 Siege is continue. There are enemies = " + vassalsArmy.GetCommonAmountArmy());
                if(startSiegeAmountArmy / (float)vassalsArmy.GetCommonAmountArmy() > criticalArmySize)
                {
                    Debug.Log("!!! To much of deads");
                    currentSiegeTarget.StartSiege(false);
                    siegeDaysLeft = -1;
                    GetNextAction();
                }
                else
                {
                    if(pathfinder.CheckPlayerNearBy() == true)
                    {
                        Debug.Log("3 Siege aborted. There is player");
                        currentSiegeTarget.StartSiege(false);
                        siegeDaysLeft = -1;
                        GetNextAction();
                    }
                    else
                    {
                        Debug.Log("3 Siege is continue. There is no player");
                        siegeDaysLeft--;
                        mainAI.EndOfMove();
                    }
                }
            }
        }       
    }

    public void ForcedEndOfSiege()
    {
        if(currentSiegeTarget != null)
        {
            currentSiegeTarget.StartSiege(false);
            currentSiegeTarget = null;
            siegeDaysLeft = -1;
        }
    }

    private int GetBuildingsLevel()
    {
        return (currentSiegeTarget.GetBuildingsType() == ResourceBuildings.Castle) ?
                    fortress.GetFortressLevel() :
                    currentSiegeTarget.GetCountOfActiveUpgrades();        
    }
    #endregion
}
