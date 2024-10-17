using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRegister : MonoBehaviour
{
    public int gridWidth = 6;  // Width of the grid
    public int gridHeight = 6; // Height of the grid
    public GameObject[,] grid; // 2D array to hold the cells (map)

    public Transform monster;  // Reference to the monster
    public Transform player;   // Reference to the player

    public Vector2Int monsterStart = new Vector2Int(1, 1); // Starting coord for monster (indexing from 1,1)
    public Vector2Int playerStart = new Vector2Int(5, 5);  // Starting coord for player (indexing from 5,5)

    void Start()
    {
        RegisterMap();
        PlaceCharacters();
    }

    void RegisterMap()
    {
        grid = new GameObject[gridWidth, gridHeight];
        int index = 0;

        // Loop through the children of the current GameObject (the parent that holds all the cells)
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (index < transform.childCount)
                {
                    grid[x, y] = transform.GetChild(index).gameObject;
                    index++;
                }
                else
                {
                    Debug.LogWarning("Not enough children in the map! Ensure the gridWidth and gridHeight are correct.");
                    return;
                }
            }
        }
    }

    void PlaceCharacters()
    {
        // Subtract 1 from monsterStart and playerStart for proper 0-based indexing
        Vector2Int adjustedMonsterStart = monsterStart - new Vector2Int(1, 1);
        Vector2Int adjustedPlayerStart = playerStart - new Vector2Int(1, 1);

        // Set the monster's position based on the grid
        if (IsWithinBounds(adjustedMonsterStart))
        {
            monster.position = grid[adjustedMonsterStart.x, adjustedMonsterStart.y].transform.position;
        }
        else
        {
            Debug.LogWarning("Monster start position is out of bounds.");
        }

        // Set the player's position based on the grid
        if (IsWithinBounds(adjustedPlayerStart))
        {
            player.position = grid[adjustedPlayerStart.x, adjustedPlayerStart.y].transform.position;
        }
        else
        {
            Debug.LogWarning("Player start position is out of bounds.");
        }
    }

    // Helper function to check if coordinates are within bounds
    bool IsWithinBounds(Vector2Int position)
    {
        return position.x >= 0 && position.x < gridWidth && position.y >= 0 && position.y < gridHeight;
    }

    // Optional helper to get a cell at specific coordinates
    public Cell GetCell(int x, int y)
    {
        if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
        {
            return grid[x, y].GetComponent<Cell>();
        }
        return null;
    }
}
