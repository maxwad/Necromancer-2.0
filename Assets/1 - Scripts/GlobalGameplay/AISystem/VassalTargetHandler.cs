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

    #endregion
}
