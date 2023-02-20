using UnityEngine;
using static NameManager;

public partial class VassalTargetSelector
{
    #region TASKS

    private void UpdatePath()
    {
        currentPath = pathfinder.CreatePath(finishCell);
    }

    private void FindPathToRandomCell()
    {
        finishCell = pathfinder.FindRandomCell(actionRadius);     
    }

    private void FindPathToTheOwnCastle()
    {
        finishCell = pathfinder.ConvertToV3Int(mainAI.GetCastlePoint());
    }
        
    private void FindPathToTheResBuilding()
    {
        finishCell = pathfinder.FindResBuildingCell();
    }

    private void FindPathToThePlayerCastle()
    {
        finishCell = pathfinder.FindPlayerCastleCell();
    }

    private void FindPathToThePlayer()
    {
        finishCell = pathfinder.ConvertToV3Int(player.transform.position);
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

        
        if(CheckSiegeStatus() == false)
        {
            currentSiegeTarget.StartSiege();
            startSiegeAmountArmy = vassalsArmy.GetCommonAmountArmy();
        }
        else
        {
            vassalsArmy.DecreaseSquads(GetBuildingsLevel());

            if(startSiegeAmountArmy / (float)vassalsArmy.GetCommonAmountArmy() > criticalArmyMultiplier ||
                pathfinder.CheckPlayerNearBy(false) == true ||
                currentSiegeTarget.UpdateSiege() == true
                )
            {
                EndOfSiege(true, true);
                return;
            }

        }
             
        mainAI.EndOfMove();            
    }

    public bool CheckSiegeStatus()
    {
        return currentSiegeTarget.CheckSiegeStatus();
    }

    public void EndOfSiege(bool isVassalAlive, bool nextAction)
    {
        if(currentSiegeTarget != null)
        {
            currentSiegeTarget.StartSiege(false);
            currentSiegeTarget = null;
        }

        AITargetType action = (isVassalAlive == true) ? AITargetType.ToTheOwnCastle : AITargetType.Death;
        SelectSpecialTarget(action);

        if(nextAction == true)
            GetNextAction();
    }

    private int GetBuildingsLevel()
    {
        return (currentSiegeTarget.GetBuildingsType() == ResourceBuildings.Castle) ?
                    fortress.GetFortressLevel() :
                    currentSiegeTarget.GetCountOfActiveUpgrades();        
    }

    #endregion
}
