using UnityEngine;
using static NameManager;

public partial class VassalTargetSelector
{
    #region TASKS

    private void FindPathToRandomCell()
    {
        finishCell = pathfinder.FindRandomCell();
        currentPath = pathfinder.GetPath();

        if(finishCell == Vector3Int.zero)
            Debug.Log("THERE IS PROBLEM with the random finish cell");        
    }

    private void FindPathToTheCastle()
    {
        finishCell = pathfinder.ConvertToV3Int(mainAI.GetCastlePoint());
        currentPath = pathfinder.CreatePath(finishCell);

        if(finishCell == Vector3Int.zero)
            Debug.Log("THERE IS PROBLEM with the back path");
    }
        
    private void FindPathToTheResBuilding()
    {
        finishCell = pathfinder.FindResBuildingCell();
        currentPath = pathfinder.CreatePath(finishCell);

        if(finishCell == Vector3Int.zero)
            Debug.Log("THERE IS PROBLEM with the back path");
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
        currentPath = pathfinder.CreatePath(finishCell);
    }

    private void Siege()
    {
        if(currentSiegeTarget == null)
            CheckNextTarget();

        if(currentSiegeTarget.CheckOwner(TypeOfObjectsOwner.Enemy) == true)
        {
            Debug.Log("Check new owner");
            currentSiegeTarget = null;
            SelectSpecialTarget(AITargetType.ToTheOwnCastle);
            GetNextAction();
        }
        else
        {
            if(currentSiegeTarget.CheckSiegeStatus() == false)
            {
                Debug.Log("Siege is started");
                currentSiegeTarget.StartSiege();
                startSiegeAmountArmy = vassalsArmy.GetCommonAmountArmy();
                mainAI.EndOfMove();
            }
            else
            {
                vassalsArmy.DecreaseSquads(GetBuildingsLevel());

                if(startSiegeAmountArmy / (float)vassalsArmy.GetCommonAmountArmy() > criticalArmySize)
                {
                    Debug.Log("To much of deads");
                    currentSiegeTarget.StartSiege(false);
                    GetNextAction();
                }
                else
                {
                    Debug.Log("Usual siege day");
                    mainAI.EndOfMove();
                }
            }
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
