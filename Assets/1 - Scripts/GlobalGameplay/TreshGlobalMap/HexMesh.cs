using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]

public class HexMesh : MonoBehaviour
{
    private Mesh hexMesh;
    //private static List<Vector3> vertices = new List<Vector3>();
    //private static List<int> triangles = new List<int>();
    //private static List<Color> colors = new List<Color>();
    //private static List<Vector2> uvs = new List<Vector2>();

    [System.NonSerialized] private List<Vector3> vertices;
    [System.NonSerialized] private List<Color> colors;
    [System.NonSerialized] private List<int> triangles;
    [System.NonSerialized] private List<Vector2> uvs;

    private MeshCollider meshCollider;

    public bool useColors;
    public bool useUVCoordinates;

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        meshCollider = gameObject.AddComponent<MeshCollider>();

        hexMesh.name = "Hex Mesh";
    }

    public void Clear()
    {
        hexMesh.Clear();

        vertices = ListPool<Vector3>.Get();
        if(useColors == true) colors = ListPool<Color>.Get();
        triangles = ListPool<int>.Get();
        if(useUVCoordinates == true) uvs = ListPool<Vector2>.Get();

        //vertices.Clear();
        //triangles.Clear();
        //colors.Clear();
    }

    public void Apply()
    {
        hexMesh.SetVertices(vertices);
        ListPool<Vector3>.Add(vertices);

        hexMesh.SetTriangles(triangles, 0);
        ListPool<int>.Add(triangles);

        if(useColors == true)
        {
            hexMesh.SetColors(colors);
            ListPool<Color>.Add(colors);
        }

        if(useUVCoordinates == true)
        {
            hexMesh.SetUVs(0, uvs);
            ListPool<Vector2>.Add(uvs);
        }

        hexMesh.RecalculateNormals();

        meshCollider.sharedMesh = hexMesh;
    }

    public void AddTriangleColor(Color color)
    {
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }

    public void AddTriangle(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        int vertexIndex = vertices.Count;

        vertices.Add(p1);
        vertices.Add(p2);
        vertices.Add(p3);

        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    public void AddTriangleUV(Vector2 uv1, Vector2 uv2, Vector3 uv3)
    {
        uvs.Add(uv1);
        uvs.Add(uv2);
        uvs.Add(uv3);
    }

    public void AddQuadUV(Vector2 uv1, Vector2 uv2, Vector3 uv3, Vector3 uv4)
    {
        uvs.Add(uv1);
        uvs.Add(uv2);
        uvs.Add(uv3);
        uvs.Add(uv4);
    }

    public void AddQuadUV(float uMin, float uMax, float vMin, float vMax)
    {
        uvs.Add(new Vector2(uMin, vMin));
        uvs.Add(new Vector2(uMax, vMin));
        uvs.Add(new Vector2(uMin, vMax));
        uvs.Add(new Vector2(uMax, vMax));
    }

    public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        int vertexIndex = vertices.Count;

        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        vertices.Add(v4);

        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
    }

    public void AddQuadColor(Color c1, Color c2)
    {
        colors.Add(c1);
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c2);
    }
}
