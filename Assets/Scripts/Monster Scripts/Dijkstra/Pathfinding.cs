using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public MapMan mapManager;
    public Transform monster;
    public Transform player;
    private List<Node> path = new List<Node>();
    private PlayerLocator playerLocator;
    private bool hasValidNode = false;

    void Start()
    {
        // Set the player's starting position directly on the tile at (9, 10)
        player.position = new Vector3(9 * 5, player.position.y, 10 * 5);

        // Set the monster's starting position directly on a specific tile (e.g., 0, 0)
        monster.position = new Vector3(18 * 5, monster.position.y, 4 * 5);

        // Cache the PlayerLocator component attached to the player
        playerLocator = player.GetComponent<PlayerLocator>();
        if (playerLocator == null)
        {
            Debug.LogError("PlayerLocator component is missing on the player object.");
            return;
        }
    }

    void Update()
    {
        if (playerLocator == null) return;

        Node currentPlayerNode = playerLocator.GetCurrentNode();
        if (currentPlayerNode == null)
        {
            hasValidNode = false;
            return;
        }
        else
        {
            hasValidNode = true;
        }

        if (hasValidNode)
        {
            Vector2Int playerPosition = currentPlayerNode.GetCoordinates();
            Vector2Int monsterPosition = GetMonsterNodePosition();

            if (monsterPosition != playerPosition)
            {
                path = FindPath(monsterPosition, playerPosition);
                if (path == null || path.Count == 0)
                {
                    Debug.LogWarning("No valid path found between the monster and the player.");
                    return;
                }
                FollowPath();
            }
        }
    }

    Vector2Int GetMonsterNodePosition()
    {
        Vector3 monsterPosition = monster.position;
        int x = Mathf.RoundToInt(monsterPosition.x / 5);
        int y = Mathf.RoundToInt(monsterPosition.z / 5);
        return new Vector2Int(x, y);
    }

    List<Node> FindPath(Vector2Int start, Vector2Int target)
    {
        Node startNode = mapManager.GetNodeAtPosition(start);
        Node targetNode = mapManager.GetNodeAtPosition(target);

        if (startNode == null || targetNode == null)
        {
            Debug.LogError("Start or target node is null.");
            return new List<Node>();
        }

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        Dictionary<Node, int> gScore = new Dictionary<Node, int> { { startNode, 0 } };
        Dictionary<Node, int> fScore = new Dictionary<Node, int> { { startNode, Heuristic(startNode, targetNode) } };

        while (openSet.Count > 0)
        {
            Node current = GetLowestFScoreNode(openSet, fScore);

            if (current == targetNode)
            {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Node neighbor in GetNeighbors(current))
            {
                if (closedSet.Contains(neighbor) || neighbor.isWall)
                {
                    continue;
                }

                int tentativeGScore = gScore[current] + 1;

                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }
                else if (tentativeGScore >= gScore[neighbor])
                {
                    continue;
                }

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, targetNode);
            }
        }

        return new List<Node>();
    }

    int Heuristic(Node a, Node b)
    {
        return Mathf.Abs(a.gridX - b.gridX) + Mathf.Abs(a.gridY - b.gridY);
    }

    Node GetLowestFScoreNode(List<Node> openSet, Dictionary<Node, int> fScore)
    {
        Node lowest = openSet[0];
        foreach (Node node in openSet)
        {
            if (fScore[node] < fScore[lowest])
            {
                lowest = node;
            }
        }
        return lowest;
    }

    List<Node> ReconstructPath(Dictionary<Node, Node> cameFrom, Node current)
    {
        List<Node> totalPath = new List<Node> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current);
        }
        return totalPath;
    }

    List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (Vector2Int direction in directions)
        {
            Node neighbor = mapManager.GetNodeAtPosition(node.GetCoordinates() + direction);
            if (neighbor != null)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    void FollowPath()
    {
        if (path.Count == 0) return;

        Vector3 targetPosition = path[0].transform.position;
        Vector3 direction = (targetPosition - monster.position).normalized;

        monster.position += direction * Time.deltaTime * 5f;

        if (Vector3.Distance(monster.position, targetPosition) < 0.1f)
        {
            path.RemoveAt(0);
        }
    }
}
