using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HexMapEditor : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    private bool isSettingsPanelEnabled = true;

    private enum OptionalToggle
    {
        Ignore, Yes, No
    }

    public Color[] colors;

    public HexGrid hexGrid;

    private Color activeColor;
    private bool applyColor = false;
    private int brushSize = 0;

    private OptionalToggle roadsMode = OptionalToggle.Yes;

    private bool isDrag = false;
    private HexDirection dragDirection;
    private HexCell previousCell;


    private void Awake()
    {
        SelectColor(0);
    }


    private void Update()
    {
        if(Input.GetMouseButton(0) == true && EventSystem.current.IsPointerOverGameObject() == false) HandleInput();
        else previousCell = null;

        if(Input.GetKeyDown(KeyCode.F1) == true) settingsPanel.SetActive(isSettingsPanelEnabled = !isSettingsPanelEnabled);
    }


    private void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(inputRay, out hit))
        {
            HexCell currentCell = hexGrid.GetCell(hit.point);

            if(previousCell != null && previousCell != currentCell) ValidateDrag(currentCell);
            else isDrag = false;

            EditFewCells(currentCell);
            previousCell = currentCell;
        }
        else 
        {
            previousCell = null;
        }
        
    }

    private void ValidateDrag(HexCell currentCell)
    {
        for(dragDirection = HexDirection.NE; dragDirection <= HexDirection.NW; dragDirection++)
        {
            if(previousCell.GetNeighbor(dragDirection) == currentCell)
            {
                isDrag = true;
                return;
            }
        }

        isDrag = false;
    }

    private void EditFewCells(HexCell centerCell)
    {
        int centerCellX = centerCell.coordinates.X;
        int centerCellY = centerCell.coordinates.Y;

        for(int r = 0, y = centerCellY - brushSize; y <= centerCellY; y++, r++)
        {
            for(int x = centerCellX - r; x <= centerCellX + brushSize; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, y)));
            }
        }

        for(int r = 0, y = centerCellY + brushSize; y > centerCellY; y--, r++)
        {
            for(int x = centerCellX - brushSize; x <= centerCellX + r; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, y)));
            }
        }
    }

    private void EditCell(HexCell cell)
    {
        if(cell == null) return;

        if(applyColor == true) cell.Color = activeColor;

        if(roadsMode == OptionalToggle.No) cell.RemoveRoads();

        if(isDrag == true)
        {
            HexCell otherCell = cell.GetNeighbor(dragDirection.Opposite());
            if(otherCell != null)
            {
                if(roadsMode == OptionalToggle.Yes) cell.AddRoad(dragDirection);
            }
        }
    }

    public void SelectColor(int index)
    {
        applyColor = index >= 0;

        if(applyColor == true) activeColor = colors[index];
    }

    public void SetBrushSize(float size)
    {
        brushSize = (int)size;
    }

    public void ShowUI(bool visible)
    {
        hexGrid.ShowUI(visible);
    }

    public void ShowRoads(int mode)
    {
        roadsMode = (OptionalToggle)mode;
    }
}
