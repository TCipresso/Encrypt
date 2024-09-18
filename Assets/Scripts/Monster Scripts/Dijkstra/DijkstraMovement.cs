using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DijkstraMovement : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform target;
    public float speed = 5f;
    public float rotationSpeed = 10f;

    private Vector3 previousPosition;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.updateRotation = false;
        SetDestination(target.position);
        previousPosition = transform.position;

        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    void Update()
    {
        MoveAlongNavMeshPath();
        HandleTurn();
    }

    void SetDestination(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    void MoveAlongNavMeshPath()
    {
        agent.Move(transform.forward * speed * Time.deltaTime);
    }

    void HandleTurn()
    {
        Vector3 movementDirection = (transform.position - previousPosition).normalized;
        float directionChange = Vector3.Dot(movementDirection, transform.forward);

        if (directionChange < 0.99f)
        {
            Vector3 nextDirection = agent.steeringTarget - transform.position;
            SnapToCardinalDirection(nextDirection);
        }

        previousPosition = transform.position;
    }

    void SnapToCardinalDirection(Vector3 direction)
    {
        direction.y = 0;

        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float snappedAngle = 0;

        if (targetAngle >= -45 && targetAngle < 45)
        {
            snappedAngle = 0;
        }
        else if (targetAngle >= 45 && targetAngle < 135)
        {
            snappedAngle = 90;
        }
        else if (targetAngle >= 135 || targetAngle < -135)
        {
            snappedAngle = 180;
        }
        else if (targetAngle >= -135 && targetAngle < -45)
        {
            snappedAngle = 270;
        }

        transform.rotation = Quaternion.Euler(0, snappedAngle, 0);
    }
}
