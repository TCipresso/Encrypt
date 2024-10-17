using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public Transform player; // Player target
    public Transform monster; // Monster that will follow the path

    public float speed = 3f; // Speed of the monster
    private Grid grid; // Reference to the grid
    private List<Node> path = new List<Node>(); // List of nodes for the monster to follow

    void Awake()
    {
        grid = GetComponent<Grid>(); // Get the Grid component
    }

    void Update()
    {
        FindPath(monster.position, player.position); // Find the path every frame
        FollowPath(); // Move the monster along the path
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (Node neighbor in grid.GetNeighbors(currentNode))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                {
                    continue;
                }

                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        path.Clear(); // Clear the previous path
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode); // Add each node to the path list
            currentNode = currentNode.parent;
        }
        path.Reverse(); // Reverse the list to make the first node the start

        // Debug: Print out the path
        Debug.Log("Path found: ");
        foreach (Node node in path)
        {
            Debug.Log(node.worldPosition);
        }
    }

    void FollowPath()
    {
        if (path == null || path.Count == 0) return; // Ensure there's a valid path

        // Get the current target node
        Node targetNode = path[0];

        // Debug: Check which node the monster is moving towards
        Debug.Log("Moving towards node: " + targetNode.worldPosition);

        // Move the monster towards the current target node
        Vector3 targetPosition = new Vector3(targetNode.worldPosition.x, monster.position.y, targetNode.worldPosition.z);
        monster.position = Vector3.MoveTowards(monster.position, targetPosition, speed * Time.deltaTime);

        // If the monster reaches the node, remove it from the path
        if (Vector3.Distance(monster.position, targetPosition) < 0.1f)
        {
            Debug.Log("Reached node, moving to next node.");
            path.RemoveAt(0);
        }
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        return 14 * distX + 10 * (distY - distX);
    }

    void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = 0; i < path.Count; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i].worldPosition, Vector3.one * (grid.nodeRadius * 0.5f));

                if (i > 0)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(path[i - 1].worldPosition, path[i].worldPosition);
                }
            }
        }
    }
}
