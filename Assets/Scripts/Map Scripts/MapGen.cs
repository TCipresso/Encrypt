using UnityEngine;

public class MapGen : MonoBehaviour
{
    public GameObject tilePrefab; // The prefab to instantiate in the grid
    public int gridWidth = 16; // Grid width
    public int gridHeight = 16; // Grid height
    public float tileSize = 5f; // Size of each tile (assuming tiles are 5x1x5)

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 spawnPosition = new Vector3(x * tileSize, 0, y * tileSize);
                GameObject tile = Instantiate(tilePrefab, spawnPosition, Quaternion.identity, transform);
                tile.name = $"Tile ({x}, {y})";
            }
        }
    }
}
