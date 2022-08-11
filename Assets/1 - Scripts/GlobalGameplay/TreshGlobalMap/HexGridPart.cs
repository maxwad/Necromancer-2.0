using System;
using UnityEngine;
using UnityEngine.UI;

public class HexGridPart : MonoBehaviour
{
    private HexCell[] cells;

    public HexMesh terrain;
    public HexMesh roads;
    private Canvas gridCanvas;

    private void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();

        cells = new HexCell[HexMetrics.partSizeX * HexMetrics.partSizeY];

        ShowUI(false);
    }

    internal void AddCell(int index, HexCell cell)
    {
        cells[index] = cell;
        cell.part = this;
        cell.transform.SetParent(transform, false);
        cell.uiRect.SetParent(gridCanvas.transform, false);
    }

    public void Refresh()
    {
        enabled = true;        
    }

    private void LateUpdate()
    {
        Triangulate();
        enabled = false;
    }

    public void ShowUI(bool visible)
    {
        gridCanvas.gameObject.SetActive(visible);
    }

    private void Triangulate()
    {
        terrain.Clear();
        roads.Clear();

        for(int i = 0; i < cells.Length; i++)
            TriangulateCell(cells[i]);

        terrain.Apply();
        roads.Apply();
    }

    private void TriangulateCell(HexCell cell)
    {
        Vector2 center = (Vector2)cell.transform.localPosition;

        for(int i = 0; i < 6; i++)
        {
            terrain.AddTriangle(
                center,
                center + HexMetrics.corners[i],
                center + HexMetrics.corners[i + 1]
                );

            terrain.AddTriangleColor(cell.Color);
        }
    }

    private void TriangulateRoadSegment(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Vector3 v5, Vector3 v6)
    {
        roads.AddQuad(v1, v2, v4, v5);
        roads.AddQuad(v2, v3, v5, v6);

        roads.AddQuadUV(0f, 1f, 0f, 0f);
        roads.AddQuadUV(1f, 0f, 0f, 0f);
    }

    private void TriangulateEdgeStrip(EdgeVertices e1, Color c1, EdgeVertices e2, Color c2, bool hasRoad = false)
    {
        terrain.AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
        terrain.AddQuadColor(c1, c2);
        terrain.AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
        terrain.AddQuadColor(c1, c2);
        terrain.AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
        terrain.AddQuadColor(c1, c2);
        terrain.AddQuad(e1.v4, e1.v5, e2.v4, e2.v5);
        terrain.AddQuadColor(c1, c2);

        if(hasRoad) TriangulateRoadSegment(e1.v2, e1.v3, e1.v4, e2.v2, e2.v3, e2.v4);        
    }
}
