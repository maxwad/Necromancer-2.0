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

    private void PrepareToRest(bool deathMode = false)
    {
        currentTarget = AITargetType.Rest;
        shouldIContinueAction = false;
        currentPath.Clear();
        mainAI.CrusadeIsOver();
    }


    #endregion
}
