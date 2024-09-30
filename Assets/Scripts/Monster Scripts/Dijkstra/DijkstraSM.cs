using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class DijkstraSM : MonoBehaviour
{
    public enum MonsterState { Search, Targeting, Chase }
    public MonsterState currentState;

    public NavMeshAgent agent;
    public Transform player;
    public float searchSpeed = 3f;
    public float chaseSpeed = 6f;
    public float targetingTime = 5f; // Time spent in Targeting state
    public float lostSightTime = 5f; // Time to transition back to Search if player is out of sight
    public AudioClip targetingClip;
    public float lineOfSightRange = 15f; // Distance for line of sight check
    public LayerMask obstaclesLayer;

    private AudioSource audioSource;
    private float lostSightTimer;
    private Vector3 currentDirection;

    // Search points
    public List<GameObject> searchPoints; // Preplaced search points
    private GameObject currentSearchPoint; // The search point currently being targeted

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        agent.updateRotation = false; // Disable NavMeshAgent auto-rotation
        currentDirection = Vector3.right; // Set the initial direction
        ChangeState(MonsterState.Search);
    }

    void Update()
    {
        switch (currentState)
        {
            case MonsterState.Search:
                Search();
                break;
            case MonsterState.Targeting:
                // Targeting is handled in coroutine
                break;
            case MonsterState.Chase:
                Chase();
                break;
        }
    }

    void ChangeState(MonsterState newState)
    {
        currentState = newState;
        switch (currentState)
        {
            case MonsterState.Search:
                agent.speed = searchSpeed;
                SelectNewSearchPoint();
                break;
            case MonsterState.Targeting:
                StartCoroutine(TargetingState());
                break;
            case MonsterState.Chase:
                agent.speed = chaseSpeed;
                Chase();
                break;
        }
    }

    // Search state logic
    void Search()
    {
        // Move toward the current search point
        if (currentSearchPoint != null)
        {
            SetDestination(currentSearchPoint.transform.position);
        }

        // Move and snap direction like in DijkstraMovement
        MoveForward();
        DetectTurns();
        CenterOnPath();

        // Check for player in line of sight
        if (PlayerInLineOfSight())
        {
            ChangeState(MonsterState.Targeting);
        }
    }

    // Targeting state logic
    IEnumerator TargetingState()
    {
        // Stop moving and play targeting audio
        agent.isStopped = true;
        audioSource.PlayOneShot(targetingClip);
        yield return new WaitForSeconds(targetingTime);

        // Transition to chase state after targeting time
        ChangeState(MonsterState.Chase);
    }

    // Chase state logic
    void Chase()
    {
        agent.isStopped = false;
        SetDestination(player.position);

        // Move and snap direction like in DijkstraMovement
        MoveForward();
        DetectTurns();
        CenterOnPath();

        // Check if player is still in line of sight
        if (!PlayerInLineOfSight())
        {
            lostSightTimer += Time.deltaTime;
            if (lostSightTimer >= lostSightTime)
            {
                // Transition back to search state
                ChangeState(MonsterState.Search);
            }
        }
        else
        {
            lostSightTimer = 0f;
        }
    }

    // Line of sight detection
    bool PlayerInLineOfSight()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Visualize raycast with debug ray
        Debug.DrawRay(transform.position, directionToPlayer, Color.red);

        if (distanceToPlayer <= lineOfSightRange)
        {
            // Raycast from the monster to the player, checking if there are obstacles
            if (!Physics.Raycast(transform.position, directionToPlayer.normalized, distanceToPlayer, obstaclesLayer))
            {
                return true;
            }
        }

        return false;
    }

    // Movement logic (snapping and path centering) - DijkstraMovement
    void SetDestination(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    void MoveForward()
    {
        transform.position += currentDirection * agent.speed * Time.deltaTime;
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
            transform.position -= offset * Time.deltaTime;
        }
    }

    // Search Point logic
    void SelectNewSearchPoint()
    {
        if (searchPoints.Count == 0)
        {
            Debug.LogError("No search points assigned!");
            return;
        }

        // Select a random search point
        int randomIndex = Random.Range(0, searchPoints.Count);
        currentSearchPoint = searchPoints[randomIndex];

        // Enable the selected search point (if necessary)
        currentSearchPoint.SetActive(true);

        // Set the monster's destination to the selected search point
        SetDestination(currentSearchPoint.transform.position);
    }

    // Trigger event when reaching a search point
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SearchPoint"))
        {
            Debug.Log("SP Hit");
            // Disable the current search point
            currentSearchPoint.SetActive(false);

            // Select a new search point
            SelectNewSearchPoint();
        }
    }
}
