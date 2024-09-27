using System.Collections.Generic;
using UnityEngine;

public class MapMan : MonoBehaviour
{
    public int mapWidth = 16;
    public int mapHeight = 16;
    public Node[,] nodes; // Array to store node references

    // This list is just for visualizing nodes in the Inspector
    public List<Node> nodeList = new List<Node>();

    void Start()
    {
        InitializeMap();
    }

    void InitializeMap()
    {
        nodes = new Node[mapWidth, mapHeight];
        Node[] allNodes = GetComponentsInChildren<Node>();

        foreach (Node node in allNodes)
        {
            Vector3 position = node.transform.position;
            int x = Mathf.RoundToInt(position.x / 5);
            int y = Mathf.RoundToInt(position.z / 5);

            node.gridX = x;
            node.gridY = y;

            if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
            {
                nodes[x, y] = node;
                nodeList.Add(node); // Add to the list to view in the Inspector
            }
            node.name = $"Node ({x}, {y})";
        }
    }

    public Node GetNodeAtPosition(Vector2Int coordinates)
    {
        if (coordinates.x >= 0 && coordinates.x < mapWidth && coordinates.y >= 0 && coordinates.y < mapHeight)
        {
            return nodes[coordinates.x, coordinates.y];
        }
        return null;
    }

    public void UpdateNodeState(int x, int y, bool setWall)
    {
        if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
        {
            Node node = nodes[x, y];
            if (node != null)
            {
                node.SetAsWall(setWall);
            }
        }
    }
}
