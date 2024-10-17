using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPathfinding : MonoBehaviour
{
    public MapRegister mapRegister; // Reference to the MapRegister
    public float moveSpeed = 2f;    // Speed at which the monster moves
    private List<Cell> path = new List<Cell>(); // List of cells representing the path to the player
    private bool isMoving = false;

    private void Start()
    {
        FindPathToPlayer();
    }

    void Update()
    {
        if (!isMoving && path.Count > 0)
        {
            StartCoroutine(MoveAlongPath());
        }
    }

    // A* Pathfinding algorithm
    void FindPathToPlayer()
    {
        Vector2Int start = mapRegister.monsterStart - new Vector2Int(1, 1); // Adjusted for 0-based indexing
        Vector2Int goal = mapRegister.playerStart - new Vector2Int(1, 1);   // Adjusted for 0-based indexing

        List<Vector2Int> openSet = new List<Vector2Int> { start };
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

        Dictionary<Vector2Int, Vector2Int?> cameFrom = new Dictionary<Vector2Int, Vector2Int?>();

        Dictionary<Vector2Int, float> gScore = new Dictionary<Vector2Int, float>();
        gScore[start] = 0;

        Dictionary<Vector2Int, float> fScore = new Dictionary<Vector2Int, float>();
        fScore[start] = GetHeuristic(start, goal);

        while (openSet.Count > 0)
        {
            // Find node in openSet with the lowest fScore
            Vector2Int current = openSet[0];
            foreach (var node in openSet)
            {
                if (fScore.ContainsKey(node) && fScore[node] < fScore[current])
                {
                    current = node;
                }
            }

            // If we have reached the goal
            if (current == goal)
            {
                ReconstructPath(cameFrom, current);
                return;
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Vector2Int neighbor in GetNeighbors(current))
            {
                if (closedSet.Contains(neighbor) || !mapRegister.GetCell(neighbor.x, neighbor.y).isWalkable)
                {
                    continue; // Ignore non-walkable cells or already evaluated cells
                }

                float tentativeGScore = gScore[current] + 1; // Assume each move costs 1

                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }
                else if (tentativeGScore >= gScore[neighbor])
                {
                    continue;
                }

                // This path is the best so far
                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + GetHeuristic(neighbor, goal);
            }
        }
    }

    // Reconstruct path from cameFrom map
    void ReconstructPath(Dictionary<Vector2Int, Vector2Int?> cameFrom, Vector2Int current)
    {
        path.Clear();
        while (cameFrom.ContainsKey(current))
        {
            path.Insert(0, mapRegister.GetCell(current.x, current.y));
            current = cameFrom[current].Value;
        }
    }

    // Heuristic function (using Manhattan distance)
    float GetHeuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    // Get neighboring cells (up, down, left, right)
    IEnumerable<Vector2Int> GetNeighbors(Vector2Int position)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>
        {
            new Vector2Int(position.x + 1, position.y), // Right
            new Vector2Int(position.x - 1, position.y), // Left
            new Vector2Int(position.x, position.y + 1), // Up
            new Vector2Int(position.x, position.y - 1)  // Down
        };

        // Filter out invalid positions (outside the grid bounds)
        return neighbors.FindAll(p => p.x >= 0 && p.x < mapRegister.gridWidth && p.y >= 0 && p.y < mapRegister.gridHeight);
    }

    // Move the monster along the path found by the A* algorithm
    IEnumerator MoveAlongPath()
    {
        isMoving = true;

        foreach (Cell cell in path)
        {
            Vector3 targetPosition = cell.transform.position;

            // Move the monster to the next cell smoothly
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            // Snap the monster to the exact position of the cell
            transform.position = targetPosition;
        }

        isMoving = false;
    }
}
