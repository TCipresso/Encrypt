using UnityEngine;

public class MapManager : MonoBehaviour
{
    public int chunkSize = 10;
    public int gridSizeX = 10;
    public int gridSizeY = 10;
    public int chunkElements = 10;
    public GameObject[] grid;

    void Start()
    {
        RegisterMap();
    }

    void RegisterMap()
    {
        grid = new GameObject[gridSizeX * gridSizeY * chunkElements];

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject chunk = transform.GetChild(i).gameObject;
            Vector3 chunkPosition = chunk.transform.position;
            int chunkX = Mathf.FloorToInt(chunkPosition.x / chunkSize);
            int chunkY = Mathf.FloorToInt(chunkPosition.z / chunkSize);
            int startingIndex = (chunkY * gridSizeX + chunkX) * chunkElements;

            for (int j = 0; j < chunkElements; j++)
            {
                grid[startingIndex + j] = chunk;
            }
        }

        PrintGrid();
    }

    void PrintGrid()
    {
        for (int i = 0; i < grid.Length; i++)
        {
            Debug.Log("Element " + i + ": " + grid[i].name);
        }
    }

    public bool IsWallAtPosition(int chunkIndex, int elementIndex)
    {
        int index = chunkIndex * chunkElements + elementIndex;
        if (index >= 0 && index < grid.Length)
        {
            return grid[index] != null && grid[index].CompareTag("wall");
        }
        return false;
    }
}
