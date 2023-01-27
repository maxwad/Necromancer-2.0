using UnityEngine;
using static NameManager;

public class GMHexCell 
{
    public Vector3Int coordinates;

    public GMHexCell[] neighbors = new GMHexCell[6];

    public void SetNeighbor(NeighborsDirection direction, GMHexCell cell)
    {
        neighbors[(int)direction] = cell;
    }

    public int CountNeighbors()
    {
        int count = 0;

        for(int i = 0; i < neighbors.Length; i++)
            if(neighbors[i] != null) count++;

        return count;
    }
}
