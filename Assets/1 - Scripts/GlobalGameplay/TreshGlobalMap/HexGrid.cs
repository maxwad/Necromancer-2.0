using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HexGrid : MonoBehaviour
{
    private int cellCountX;
    private int cellCountY;
    private int scaleMultiplier = 1;

    public int partCountX = 4;
    public int partCountY = 3;

    public HexCell cellPrefab;
    private HexCell[] cells;

    public TMP_Text cellLabel;

    public HexGridPart gridPartPrefab;

    private HexGridPart[] mapsParts;

    public Color defaultColor = Color.red;
    public Color touchColor = Color.green;

    private void Awake()
    {
        cellCountX = partCountX * HexMetrics.partSizeX;
        cellCountY = partCountY * HexMetrics.partSizeY;

        CreateMapsParts();
        CreateCells();       
    }

    private void CreateMapsParts()
    {
        mapsParts = new HexGridPart[partCountX * partCountY];

        for(int y = 0, i = 0; y < partCountY; y++)
        {
            for(int x = 0; x < partCountX; x++)
            {
                HexGridPart part = mapsParts[i++] = Instantiate(gridPartPrefab);
                part.transform.SetParent(transform);
            }
        }
    }

    private void CreateCells()
    {
        cells = new HexCell[cellCountX * cellCountY];

        for(int y = 0, i = 0; y < cellCountY; y++)
        {
            for(int x = 0; x < cellCountX; x++)
            {
                CreateOneCell(x, y, i++);
            }
        }
    }

    public HexCell GetCell(Vector3 point)
    {
        point = transform.InverseTransformPoint(point);

        HexCoordinates coordinates = HexCoordinates.FromPosition(point);
        //Debug.Log("Clicked at " + coordinates.ToString());

        int index = coordinates.X + coordinates.Y * cellCountX + coordinates.Y / 2;
        return cells[index];
    }

    public HexCell GetCell(HexCoordinates coordinates)
    {
        int y = coordinates.Y;
        if(y < 0 || y >= cellCountY) return null;

        int x = coordinates.X + y / 2;
        if(x < 0 || x >= cellCountX) return null;

        return cells[x + y * cellCountX];
    }

    private void CreateOneCell(int x, int y, int i)
    {
        Vector2 position;
        position.x = (x + y * 0.5f - y / 2) * (HexMetrics.innerRadius * 2f) * scaleMultiplier;
        position.y = y * (HexMetrics.outerRadius * 1.5f) * scaleMultiplier;

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, y);
        cell.Color = defaultColor;

        if(x > 0)
        {
            cell.SetNeighbor(HexDirection.W, cells[i - 1]);
        }
        if(y > 0)
        {
            if((y & 1) == 0)
            {
                cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]);
                if(x > 0)
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
                }
            }
            else
            {
                cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
                if(x < cellCountX - 1)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);
                }
            }
        }


        TMP_Text label = Instantiate<TMP_Text>(cellLabel);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.y);
        label.text = cell.coordinates.ToString();

        cell.uiRect = label.rectTransform;

        AddCellToPart(x, y, cell);
    }

    private void AddCellToPart(int x, int y, HexCell cell)
    {
        int partX = x / HexMetrics.partSizeX;
        int partY = y / HexMetrics.partSizeY;
        HexGridPart part = mapsParts[partX + partY * partCountX];

        int localX = x - partX * HexMetrics.partSizeX;
        int localY = y - partY * HexMetrics.partSizeY;
        part.AddCell(localX + (localY * HexMetrics.partSizeX), cell);

    }

    public void ShowUI(bool visible)
    {
        for(int i = 0; i < mapsParts.Length; i++)
        {
            mapsParts[i].ShowUI(visible);
        }
    }
}
