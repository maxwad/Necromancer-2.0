using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using UnityEngine.EventSystems;

public class GlobalMapPathfinder : MonoBehaviour
{
    public Tilemap fogMap;
    public Tilemap roadMap;
    public Tilemap overlayMap;

    [HideInInspector] public GMHexCell[,] roads;

    public Tile roadTile;
    public Tile finishTile;

    private GMPlayerMovement player;
    private GMPlayerPositionChecker positionChecker;

    List<Vector2> pathPoints = new List<Vector2>();
    private bool isGoalCellFinded = false;
    private GMHexCell targetCell;

    private float movementPointsMax = 0;
    private float currentMovementPoints = 0;
    private float constStep = 1.4f; // experimental const

    private Vector3Int startPoint;
    private Vector3Int destinationPoint;

    public GameObject stepsCounterPrefab;
    private int stepsCounter = 0;
    private Dictionary<Vector2, GameObject> counterStepDict = new Dictionary<Vector2, GameObject>();

    public Dictionary<GameObject, Vector3> enterPointsDict = new Dictionary<GameObject, Vector3>();
    public GameObject focusObject = null;
    private bool shouldIClearPath = false;

    private void Awake()
    {
        player = GlobalStorage.instance.globalPlayer;
        positionChecker = player.GetComponent<GMPlayerPositionChecker>();
    }

    public void SetEnterPoints(Dictionary<GameObject, Vector3> points)
    {
        enterPointsDict = points;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0)) 
        {
            if(MenuManager.instance.IsTherePauseOrMiniPause() == false && GlobalStorage.instance.isGlobalMode == true)
            {
                if(EventSystem.current.IsPointerOverGameObject()) return;

                if(GlobalStorage.instance.isModalWindowOpen == true) return;

                LClick();
            }
        }

        if(Input.GetMouseButtonDown(1))
        {
            if(MenuManager.instance.IsTherePauseOrMiniPause() == false && GlobalStorage.instance.isGlobalMode == true)
            {
                RClick();
            }
        }
    }

    public void LClick() 
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
        ClickableObject checkingObject;
        if(hit.collider != null && (checkingObject = hit.collider.GetComponent<ClickableObject>()) != null)
        {
            if(checkingObject.isNormalUIWindow == true)
            {
                if(enterPointsDict.ContainsKey(hit.collider.gameObject) == true)
                {
                    destinationPoint = roadMap.WorldToCell(enterPointsDict[hit.collider.gameObject]);
                    focusObject = hit.collider.gameObject;
                }
                else
                {
                    return;
                }
            }
            else
            {
                destinationPoint = roadMap.WorldToCell(mousePosition);
                focusObject = null;
            }            
        }
        else
        {
            destinationPoint = roadMap.WorldToCell(mousePosition);
            focusObject = null;
        }
        //roadMap.SetTile(destinationPoint, finishTile);
        StartCoroutine(HandleClick());
    }

    public void RClick()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
        if(hit.collider != null && hit.collider.GetComponent<ClickableObject>() != null)
        {
            hit.collider.GetComponent<ClickableObject>().ActivateUIWindow(true);
        }
    }

    public IEnumerator HandleClick()
    {
        if(player.IsMoving() == true)
        {
            player.StopMoving();
            while(player.IsMoving() == true)
            {
                yield return null;
            }
        }

        GetParameters();
        startPoint = roadMap.WorldToCell(player.GetCurrentPosition());
        if(destinationPoint != startPoint && CheckBounds(destinationPoint) == true)
        {
            GMHexCell checkCell = roads[destinationPoint.x, destinationPoint.y];

            if(checkCell != null)
            {
                shouldIClearPath = false;

                if(isGoalCellFinded == true && targetCell == checkCell)
                {
                    player.MoveOnTheWay(pathPoints.ToArray(), this);
                    isGoalCellFinded = false;
                }
                else
                {
                    targetCell = roads[destinationPoint.x, destinationPoint.y];
                    PathFinding(startPoint, destinationPoint);
                }
            }
            else
            {
                shouldIClearPath = true;
                targetCell = null;
                ClearSteps();                
            }
        }

        if(destinationPoint == startPoint) player.CheckPosition();

        bool CheckBounds(Vector3Int cell)
        {
            return cell.x < roadMap.size.x && cell.x >= 0 && cell.y < roadMap.size.y && cell.y >= 0;
        }
    }

    private void PathFinding(Vector3Int startCell, Vector3Int goalCell)
    {
        Dictionary<GMHexCell, NeighborData> QueueDict = new Dictionary<GMHexCell, NeighborData>();
        List<GMHexCell> neighborsQueue = new List<GMHexCell>();
        List<GMHexCell> roadBack = new List<GMHexCell>();

        GMHexCell firstPathCell = roads[startCell.x, startCell.y];
        pathPoints.Clear();

        bool isSearching = true;
        bool isDeadEnd = false;

        ClearSteps();        

        QueueDict.Add(firstPathCell, new NeighborData());

        neighborsQueue.Add(firstPathCell);
        int countCells = 0;

        while(isSearching == true)
        {
            if(countCells >= neighborsQueue.Count)
            {
                isDeadEnd = true;
                break;
            }

            for(int i = countCells; i < neighborsQueue.Count; i++)
            {                
                GMHexCell[] currentNeighbors = neighborsQueue[i].neighbors;

                for(int j = 0; j < currentNeighbors.Length; j++)
                {
                    if(currentNeighbors[j] != null && QueueDict.ContainsKey(currentNeighbors[j]) == false)
                    {
                        QueueDict.Add(currentNeighbors[j], new NeighborData(QueueDict[neighborsQueue[i]].cost + 1, neighborsQueue[i]));
                        neighborsQueue.Add(currentNeighbors[j]);

                        if(currentNeighbors[j].coordinates == new Vector3Int(goalCell.x, goalCell.y, 0))
                        {
                            roadBack.Add(currentNeighbors[j]);
                            isSearching = false;
                            break;
                        }
                    }
                }

                if(isSearching == false) break;

                countCells++;                
            }
        }

        if(isDeadEnd == false)
        {
            GMHexCell currentBackCell = roadBack[0];
            while(currentBackCell != firstPathCell)
            {
                roadBack.Add(QueueDict[currentBackCell].source);
                currentBackCell = QueueDict[currentBackCell].source;
            }
            roadBack.Reverse();

            float colorBound = currentMovementPoints;
            bool enemyOnTheRoad = false;

            for(int i = 0; i < roadBack.Count; i++){
                
                Tile currentTile = roadTile;

                if(i == roadBack.Count - 1) currentTile = finishTile;

                if(currentMovementPoints == 0)
                {
                    if(i != 0) currentTile = finishTile;
                    currentMovementPoints = movementPointsMax;
                }

                currentTile.color = Color.white;

                if(i > colorBound) currentTile.color = Color.red;

                Vector3 position = overlayMap.CellToWorld(roadBack[i].coordinates);
                if(positionChecker.CheckEnemy(position, false) == true) enemyOnTheRoad = true;
                if(enemyOnTheRoad == true) currentTile.color = Color.red;

                overlayMap.SetTile(roadBack[i].coordinates, currentTile);
                pathPoints.Add(roadMap.CellToWorld(roadBack[i].coordinates));

                if(currentTile == finishTile)
                {
                    stepsCounter++;
                    AddStep(pathPoints[pathPoints.Count - 1], stepsCounter);
                }
                currentMovementPoints--;
            }

            isGoalCellFinded = true;
            ShowSteps();
        }
        else
        {
            isGoalCellFinded = false;
            Debug.Log("Dead End");
        }        
    }

    public void RefreshPath(Vector2 currentPosition, float remainingPoints)
    {
        if(shouldIClearPath == true) return;
                
        ClearSteps();
        GetParameters();

        float newPointsCount = remainingPoints;

        float colorBound = newPointsCount;
        float renderedTiles = 0;
        int startIndex = 0;
        bool enemyOnTheRoad = false;

        for(int i = 0; i < pathPoints.Count; i++)
        {
            if(pathPoints[i] == currentPosition)
            {
                startIndex = i;
                break;
            }
        }

        for(int i = startIndex; i < pathPoints.Count; i++)
        {
            Tile currentTile = roadTile;
            if(i == pathPoints.Count - 1) currentTile = finishTile;

            if(newPointsCount == 0)
            {
                if(i != 0) currentTile = finishTile;
                newPointsCount = movementPointsMax;
            }

            currentTile.color = Color.white;

            if(renderedTiles > colorBound) currentTile.color = Color.red;

            if(positionChecker.CheckEnemy(pathPoints[i], false) == true) enemyOnTheRoad = true;
            if(enemyOnTheRoad == true) currentTile.color = Color.red;                

            if(currentTile == finishTile)
            {
                stepsCounter++;
                AddStep(pathPoints[i], stepsCounter);
            }

            overlayMap.SetTile(overlayMap.WorldToCell(pathPoints[i]), currentTile);
            newPointsCount--;
            renderedTiles++;
        }
        ShowSteps();
    }

    private void AddStep(Vector2 position, int count)
    {
        string text = count > 99 ? "99+" : count.ToString();

        GameObject counterStep = Instantiate(stepsCounterPrefab, position, Quaternion.identity);
        counterStep.GetComponentInChildren<TMP_Text>().text = text;
        counterStep.SetActive(false);

        counterStepDict.Add(position, counterStep);
        counterStep.transform.SetParent(roadMap.transform);
    }

    private void ShowSteps()
    {

        if(counterStepDict.Count <= 1)
            return;
        else
        {
            foreach(var step in counterStepDict)
                step.Value.SetActive(true);
        }
    }

    public void RefreshSteps(Vector2 playerPosition)
    {
        if(counterStepDict.ContainsKey(playerPosition) == true)
        {
            Destroy(counterStepDict[playerPosition]);
            counterStepDict.Remove(playerPosition);

            GameObject newStep;
            int previousCount = 0;

            foreach(var step in counterStepDict)
            {
                previousCount++;
                newStep = step.Value;
                string text = previousCount > 99 ? "99+" : previousCount.ToString();
                newStep.GetComponentInChildren<TMP_Text>().text = text;
            }
        }
    }


    public void ClearRoadTile(Vector2 point)
    {
        overlayMap.SetTile(roadMap.WorldToCell(point), null);
    }

    public void CheckFog(bool isFogNeeded, float radius)
    {
        // in release we need check FALSE
        if(isFogNeeded == true)
        {
            fogMap.gameObject.SetActive(false);
            return;
        }

        fogMap.gameObject.SetActive(true);

        Vector3Int center = fogMap.WorldToCell(player.transform.position);

        radius *= constStep;

        for(float x = -radius; x < radius; x++)
        {
            for(float y = -radius; y <= radius + 1; y++)
            {
                Vector3Int checkPosition = new Vector3Int((int)x, (int)y, 0) + center;
                if(Vector3Int.Distance(checkPosition, center) < radius)
                {
                    fogMap.SetTile(checkPosition, null);
                }
            }
        }
    }

    private void GetParameters()
    {
        float[] parameters = player.GetParametres();
        movementPointsMax = parameters[0];
        currentMovementPoints = parameters[1];
    }

    public void ClearSteps()
    {
        foreach(var step in counterStepDict)
            Destroy(step.Value);

        counterStepDict.Clear();
        stepsCounter = 0;
        overlayMap.ClearAllTiles();
    }

    public void DestroyPath(bool destroyMode)
    {
        shouldIClearPath = destroyMode;

        targetCell = null;
        ClearSteps();
    }
}
