using UnityEngine;

public class Node : MonoBehaviour
{
    public int gridX; // X coordinate of the node in the grid
    public int gridY; // Y coordinate of the node in the grid
    public bool isWall; // Determines if the node is currently a wall

    // Method to set the node as a wall or floor
    public void SetAsWall(bool wallState)
    {
        isWall = wallState;
        UpdateNodeAppearance();
    }

    // Optional: Change appearance based on the state (wall/floor)
    private void UpdateNodeAppearance()
    {
        if (isWall)
        {
            transform.position = new Vector3(transform.position.x, 10, transform.position.z); // Example: Raise the node to act as a wall
        }
        else
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z); // Example: Lower the node to act as a floor
        }
    }

    // Method to get the coordinates of the node
    public Vector2Int GetCoordinates()
    {
        return new Vector2Int(gridX, gridY);
    }
}
