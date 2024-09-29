using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DijkstraMovement : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform target; // This should be the player's transform
    public float speed = 5f;
    public float centeringForce = 1f;
    private Vector3 currentDirection;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.updateRotation = false;
        currentDirection = Vector3.right; // Assuming initial movement along the X-axis; adjust as needed
    }

    void Update()
    {
        // Continuously set the player's current position as the destination
        SetDestination(target.position);

        MoveForward();
        DetectTurns();
        CenterOnPath();
    }

    void SetDestination(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    void MoveForward()
    {
        transform.position += currentDirection * speed * Time.deltaTime;
    }

    void DetectTurns()
    {
        if (agent.hasPath && agent.path.corners.Length > 1)
        {
            Vector3 directionToNextCorner = agent.path.corners[1] - transform.position;
            float angleToNextCorner = Vector3.Angle(currentDirection, directionToNextCorner);

            if (angleToNextCorner > 45f) // Adjust the threshold if needed for earlier or sharper turns
            {
                RotateToNextDirection(directionToNextCorner);
            }
        }
    }

    void RotateToNextDirection(Vector3 direction)
    {
        direction.y = 0;
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float snappedAngle = Mathf.Round(targetAngle / 90f) * 90f;
        transform.rotation = Quaternion.Euler(0, snappedAngle, 0);

        // Update current direction based on the snapped angle
        if (snappedAngle == 0)
            currentDirection = Vector3.forward;
        else if (snappedAngle == 90)
            currentDirection = Vector3.right;
        else if (snappedAngle == 180 || snappedAngle == -180)
            currentDirection = Vector3.back;
        else if (snappedAngle == -90 || snappedAngle == 270)
            currentDirection = Vector3.left;
    }

    void CenterOnPath()
    {
        if (agent.hasPath && agent.path.corners.Length > 1)
        {
            Vector3 pathStart = agent.path.corners[0];
            Vector3 pathEnd = agent.path.corners[1];
            Vector3 pathDirection = (pathEnd - pathStart).normalized;

            // Project the current position onto the line defined by the path segment
            Vector3 projectedPoint = Vector3.Project(transform.position - pathStart, pathDirection) + pathStart;

            // Calculate how far off-center the monster is from the ideal path
            Vector3 offset = transform.position - projectedPoint;

            // Correct the position by moving towards the projected point
            transform.position -= offset * centeringForce * Time.deltaTime;
        }
    }
}
