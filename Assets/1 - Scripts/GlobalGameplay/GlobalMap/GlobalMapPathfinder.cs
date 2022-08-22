using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class GlobalMapPathfinder : MonoBehaviour
{
    public Tilemap fogMap;
    public Tilemap roadMap;
    public Tilemap overlayMap;

    [HideInInspector] public GMHexCell[,] roads;

    public Tile roadTile;
    public Tile finishTile;

    private GMPlayerMovement player;

    List<Vector2> pathPoints = new List<Vector2>();
    private bool isGoalCellFinded = false;
    private GMHexCell targetCell;

    private float movementPointsMax = 0;
    private float currentMovementPoints = 0;

    private Vector3Int startPoint;
    private Vector3Int destinationPoint;

    public GameObject stepsCounterPrefab;
    private int stepsCounter = 0;
    private Dictionary<Vector2, GameObject> counterDict = new Dictionary<Vector2, GameObject>();

    public Dictionary<GameObject, Vector3> enterPointsDict = new Dictionary<GameObject, Vector3>();
    public GameObject focusObject = null;
    private bool shouldIClearPath = false;

    private void Awake()
    {
        player = GlobalStorage.instance.globalPlayer.GetComponent<GMPlayerMovement>();
    }

    private void GetParameters() 
    {
        float[] parameters = player.GetParametres();
        movementPointsMax = parameters[0];
        currentMovementPoints = parameters[1];
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0)) 
        {
            if(MenuManager.isGamePaused == false && MenuManager.isMiniPause == false && GlobalStorage.instance.isGlobalMode == true)
            {
                Click();
            }
        }                
    }
    public void Click() 
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
        
        if(hit.collider != null && hit.collider.GetComponent<ClickableObject>() != null)
        {            
            destinationPoint = roadMap.WorldToCell(enterPointsDict[hit.collider.gameObject]);
            focusObject = hit.collider.gameObject;
        }
        else
        {
            destinationPoint = roadMap.WorldToCell(mousePosition);
            focusObject = null;
        }

        HandleClick();
    }

    public void HandleClick()
    {
        GetParameters();        
        startPoint = roadMap.WorldToCell(player.transform.position);           

        if(destinationPoint != startPoint &&
            destinationPoint.x < roadMap.size.x && destinationPoint.x >= 0 &&
            destinationPoint.y < roadMap.size.y && destinationPoint.y >= 0)
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
                    player.StopMoving();
                    targetCell = roads[destinationPoint.x, destinationPoint.y];
                    StartCoroutine(PathFinding(startPoint, destinationPoint));
                }
            }
            else
            {
                shouldIClearPath = true;
                player.StopMoving();
                ClearSteps();
                overlayMap.ClearAllTiles();
            }
        }
    }

    private IEnumerator PathFinding(Vector3Int startCell, Vector3Int goalCell)
    {
        Dictionary<GMHexCell, NeighborData> QueueDict = new Dictionary<GMHexCell, NeighborData>();
        List<GMHexCell> neighborsQueue = new List<GMHexCell>();
        List<GMHexCell> roadBack = new List<GMHexCell>();

        if(player.IsMoving() == true)
        {
            while(player.IsMoving() == true)
            {
                yield return new WaitForSeconds(0.01f);
            }
            startCell = roadMap.WorldToCell(player.transform.position);
        }

        GMHexCell firstPathCell = roads[startCell.x, startCell.y];
        pathPoints.Clear();

        bool isSearching = true;
        bool isDeadEnd = false;

        ClearSteps();
        overlayMap.ClearAllTiles();

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

            for(int i = 0; i < roadBack.Count; i++){
                
                Tile currentTile = roadTile;

                if(currentMovementPoints == 0) 
                {
                    if(i != 0) currentTile = finishTile;
                    currentMovementPoints = movementPointsMax;
                } 

                if(i == roadBack.Count - 1) currentTile = finishTile;

                if(i > colorBound && currentTile != finishTile)
                    currentTile.color = Color.red;
                else
                    currentTile.color = Color.white;

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

    public void ClearRoadTile(Vector2 point)
    {
        overlayMap.SetTile(roadMap.WorldToCell(point), null);
    }

    public void CheckFog(float radius)
    {
        Vector3Int center = fogMap.WorldToCell(player.transform.position);

        for(float x = -radius; x < radius; x++)
        {
            for(float y = -radius; y <= radius+1; y++)
            {
                Vector3Int checkPosition = new Vector3Int((int)x, (int)y, 0) + center;
                if(Vector3Int.Distance(checkPosition, center) < radius)
                {
                    fogMap.SetTile(checkPosition, null);
                }
            }
        }
    }

    public void RefreshPath(Vector2 currentPosition, float remainingPoints)
    {
        if(shouldIClearPath == true) return;

        overlayMap.ClearAllTiles();
        ClearSteps();

        //foreach(var item in coordList)
        //{
        //    Destroy(item.gameObject);
        //}
        //coordList.Clear();


        float newPointsCount = remainingPoints;
        float countTurns = 0;

        int startIndex = 0;

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
                countTurns++;
            }

            if(newPointsCount == 0)
            {
                if(i != 0) currentTile = finishTile;
                newPointsCount = movementPointsMax;
            }

            if(currentTile != finishTile)
            {
                if(newPointsCount > 0 && countTurns == 0)
                    currentTile.color = Color.white;
                else
                    currentTile.color = Color.red;
            }
            else
            {
                stepsCounter++;
                AddStep(pathPoints[i], stepsCounter);
            }

            //TMP_Text text = Instantiate(coordinates, pathPoints[i], Quaternion.identity);
            //text.text = "" + overlayMap.WorldToCell(pathPoints[i]).x + " " + overlayMap.WorldToCell(pathPoints[i]).y;
            //coordList.Add(text);
            overlayMap.SetTile(overlayMap.WorldToCell(pathPoints[i]), currentTile);
            newPointsCount--;
        }
        ShowSteps();
    }

    private void AddStep(Vector2 position, int count)
    {
        string text = count > 99 ? "99+" : count.ToString();

        GameObject counterStep = Instantiate(stepsCounterPrefab, position, Quaternion.identity);
        counterStep.GetComponentInChildren<TMP_Text>().text = text;
        counterStep.SetActive(false);

        counterDict.Add(position, counterStep);
        counterStep.transform.SetParent(roadMap.transform);
    }

    private void ShowSteps()
    {

        if(counterDict.Count <= 1)
            return;
        else
        {
            foreach(var step in counterDict)
                step.Value.SetActive(true);
        }
    }

    public void RefreshSteps(Vector2 playerPosition)
    {
        if(counterDict.ContainsKey(playerPosition) == true)
        {
            Destroy(counterDict[playerPosition]);
            counterDict.Remove(playerPosition);

            GameObject newStep;
            int previousCount = 0;

            foreach(var step in counterDict)
            {
                previousCount++;
                newStep = step.Value;
                string text = previousCount > 99 ? "99+" : previousCount.ToString();
                newStep.GetComponentInChildren<TMP_Text>().text = text;
            }
        }
    }

    private void ClearSteps()
    {
        foreach(var step in counterDict)
            Destroy(step.Value);

        counterDict.Clear();
        stepsCounter = 0;
    }
}
