using UnityEngine;
using static NameManager;

public partial class VassalTargetSelector
{
    #region TASKS

    private void UpdatePath()
    {
        if(finishCell == Vector3Int.zero)
        {
            Debug.Log("THERE IS PROBLEM with the random finish cell");
            CheckNextTarget();
        }
        else
            currentPath = pathfinder.CreatePath(finishCell);
    }

    private void FindPathToRandomCell()
    {
        finishCell = pathfinder.FindRandomCell();
        UpdatePath();        
    }

    private void FindPathToTheOwnCastle()
    {
        finishCell = pathfinder.ConvertToV3Int(mainAI.GetCastlePoint());
        UpdatePath();
    }
        
    private void FindPathToTheResBuilding()
    {
        finishCell = pathfinder.FindResBuildingCell();
        UpdatePath();
    }

    private void FindPathToThePlayerCastle()
    {
        finishCell = pathfinder.FindPlayerCastleCell();
        Debug.Log("Castle is on v3Int" + finishCell);
        Debug.Log("Castle is on v3" + pathfinder.ConvertToV3(finishCell));
        UpdatePath();
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

    private void FindPathToThePlayer()
    {
        finishCell = pathfinder.ConvertToV3Int(player.transform.position);
        UpdatePath();
    }

    private void Siege()
    {
        Debug.Log("1 " + currentSiegeTarget);
        if(currentSiegeTarget == null)
            CheckNextTarget();

        if(currentSiegeTarget.CheckOwner(TypeOfObjectsOwner.Enemy) == true)
        {
            Debug.Log("2 ");
            currentSiegeTarget = null;
            SelectSpecialTarget(AITargetType.ToTheOwnCastle);
            GetNextAction();
        }
        else
        {
            if(currentSiegeTarget.CheckSiegeStatus() == false)
            {
                Debug.Log("3 ");
                currentSiegeTarget.StartSiege();
                startSiegeAmountArmy = vassalsArmy.GetCommonAmountArmy();
                mainAI.EndOfMove();
            }
            else
            {
                Debug.Log("4 ");
                vassalsArmy.DecreaseSquads(GetBuildingsLevel());

                if(startSiegeAmountArmy / (float)vassalsArmy.GetCommonAmountArmy() > criticalArmySize)
                {
                    Debug.Log("5 ");
                    Debug.Log("To much of deads");
                    currentSiegeTarget.StartSiege(false);
                    GetNextAction();
                }
                else
                {
                    Debug.Log("6 ");
                    if(pathfinder.CheckPlayerNearBy() == true)
                    {
                        Debug.Log("7 ");
                        currentSiegeTarget.StartSiege(false);
                        GetNextAction();
                    }
                    else
                    {
                        Debug.Log("8");
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
        }
    }

    private int GetBuildingsLevel()
    {
        return (currentSiegeTarget.GetBuildingsType() == ResourceBuildings.Castle) ?
                    currentSiegeTarget.GetComponent<FortressBuildings>().GetFortressLevel() :
                    currentSiegeTarget.GetCountOfActiveUpgrades();        
    }
    #endregion
}
