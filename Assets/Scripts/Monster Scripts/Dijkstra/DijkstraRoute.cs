using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DijkstraRoute : MonoBehaviour
{
    public enum MonsterState { Move, Stop, Turn }
    public MonsterState currentState = MonsterState.Move;

    public List<Transform> routePoints; // List of waypoints
    public float speed = 3f; // Movement speed
    public float turnSpeed = 5f; // Turning speed

    private int currentPointIndex = 0; // Current waypoint index
    private bool isStateTransitioning = false; // To prevent overlapping state changes

    void Start()
    {
        DisableAllPoints(); // Disable all waypoints initially
        EnableCurrentPoint(); // Enable the first waypoint
    }

    void Update()
    {
        switch (currentState)
        {
            case MonsterState.Move:
                MoveState();
                break;

            case MonsterState.Stop:
                StopState();
                break;

            case MonsterState.Turn:
                StartCoroutine(TurnState());
                break;
        }
    }

    // State: Move - Moves towards the current waypoint
    void MoveState()
    {
        if (routePoints.Count == 0) return; // Ensure waypoints are assigned

        Transform targetPoint = routePoints[currentPointIndex];

        // Move towards the target waypoint
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        // If reached the waypoint, switch to Stop state
        if (Vector3.Distance(transform.position, targetPoint.position) <= 0.1f)
        {
            SwitchState(MonsterState.Stop);
        }
    }

    // State: Stop - Briefly stops at the waypoint
    void StopState()
    {
        if (isStateTransitioning) return; // Prevent multiple triggers

        isStateTransitioning = true;
        DisableCurrentPoint(); // Disable the current waypoint

        // Go to the next waypoint and switch to Turn state
        currentPointIndex = (currentPointIndex + 1) % routePoints.Count;
        EnableCurrentPoint(); // Enable the next waypoint

        SwitchState(MonsterState.Turn);
    }

    // State: Turn - Rotates towards the next waypoint
    IEnumerator TurnState()
    {
        isStateTransitioning = true;

        // Get the next waypoint and calculate direction
        Transform nextPoint = routePoints[currentPointIndex];
        Vector3 directionToNext = (nextPoint.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToNext);

        // Smoothly rotate towards the next waypoint
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            yield return null;
        }

        // Ensure rotation aligns perfectly at the end
        transform.rotation = targetRotation;

        isStateTransitioning = false; // Allow state transitions again

        // Switch to Move state to continue towards the next waypoint
        SwitchState(MonsterState.Move);
    }

    public void SwitchState(MonsterState newState)
    {
        currentState = newState;
    }

    void EnableCurrentPoint()
    {
        routePoints[currentPointIndex].gameObject.SetActive(true);
    }

    void DisableCurrentPoint()
    {
        routePoints[currentPointIndex].gameObject.SetActive(false);
    }

    public void DisableAllPoints()
    {
        foreach (Transform point in routePoints)
        {
            point.gameObject.SetActive(false);
        }
    }

    public void StartRoute()
    {
        currentPointIndex = 0;
        DisableAllPoints();
        EnableCurrentPoint();
        SwitchState(MonsterState.Move);
    }
}
