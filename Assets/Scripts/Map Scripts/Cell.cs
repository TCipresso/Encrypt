using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool isWalkable = true; // Can this cell be walked on?
    public Renderer cellRenderer;  // Renderer component to visually indicate walkability (optional)

    void Start()
    {
        UpdateCellVisual();
    }

    // Toggle whether this cell is walkable or a wall
    public void SetWalkable(bool walkable)
    {
        isWalkable = walkable;
        UpdateCellVisual();
    }

    // Optional: Update the cell's color to indicate whether it's a wall or walkable
    void UpdateCellVisual()
    {
        if (cellRenderer != null)
        {
            if (isWalkable)
            {
                cellRenderer.material.color = Color.green; // Walkable
            }
            else
            {
                cellRenderer.material.color = Color.red; // Wall
            }
        }
    }
}
