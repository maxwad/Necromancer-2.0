using System;
using UnityEngine;

public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;

    private Color color;

    public Color Color {

        get
        {
            return color;
        }

        set
        {
            if(Color == value) return;

            color = value;
            Refresh();
        }    
    }

    public RectTransform uiRect;

    public HexGridPart part;

    [SerializeField] private HexCell[] neighbors;

    [SerializeField] private bool[] roads;

    public bool HasRoads
    {
        get
        {
            for(int i = 0; i < roads.Length; i++)
                if(roads[i] == true) return true;

            return false;
        }
    }



    void Refresh()
    {
        if(part != null)
        {
            part.Refresh();
            for(int i = 0; i < neighbors.Length; i++)
            {
                HexCell neighbor = neighbors[i];
                if(neighbor != null && neighbor.part != part) neighbor.part.Refresh();
            }
        }
    }

    public void RefreshSelfOnly()
    {
        part.Refresh();
    }

    #region Neighbors

    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }

    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    #endregion


    #region Roads

    public bool HasRoadThisDirection(HexDirection direction)
    {
        return roads[(int)direction];
    }

    public void AddRoad(HexDirection direction)
    {
        if(roads[(int)direction] == false) SetRoad((int)direction, true);
    }

    public void RemoveRoads()
    {
        for(int i = 0; i < neighbors.Length; i++)
        {
            if(roads[i] == true) SetRoad(i, false);
        }
    }

    private void SetRoad(int index, bool state)
    {
        roads[index] = state;

        if(neighbors[index] != null)
        {
            neighbors[index].roads[(int)((HexDirection)index).Opposite()] = state;
            neighbors[index].RefreshSelfOnly();
        }        
        RefreshSelfOnly();
    }

    #endregion
}
