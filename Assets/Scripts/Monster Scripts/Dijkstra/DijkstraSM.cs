using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DijkstraSM : MonoBehaviour
{
    public enum MonsterState { Search, Targeting, Chase }
    public MonsterState currentState;

    public NavMeshAgent agent;
    public Transform player;
    public Transform LKP; // Reference to the Last Known Position object

    public float searchSpeed = 3f;
    public float chaseSpeed = 6f;
    public float targetingTime = 5f;
    public float lostSightTime = 5f;
    public AudioClip targetingClip;
    public float lineOfSightRange = 15f;
    public LayerMask obstaclesLayer;

    private AudioSource audioSource;
    private float lostSightTimer;
    private Vector3 currentDirection;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        agent.updateRotation = false;
        currentDirection = Vector3.right;
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
                GoToLKP(); // Use LKP for search state
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

    void Search()
    {
        if (LKP != null)
        {
            SetDestination(LKP.position); // Move to the last known position
        }

        MoveForward();
        DetectTurns();
        CenterOnPath();

        if (PlayerInLineOfSight())
        {
            ChangeState(MonsterState.Targeting);
        }
    }

    IEnumerator TargetingState()
    {
        agent.isStopped = true;
        audioSource.PlayOneShot(targetingClip);
        yield return new WaitForSeconds(targetingTime);
        ChangeState(MonsterState.Chase);
    }

    void Chase()
    {
        agent.isStopped = false;
        SetDestination(player.position);

        MoveForward();
        DetectTurns();
        CenterOnPath();

        if (!PlayerInLineOfSight())
        {
            lostSightTimer += Time.deltaTime;
            if (lostSightTimer >= lostSightTime)
            {
                ChangeState(MonsterState.Search);
            }
        }
        else
        {
            lostSightTimer = 0f;
        }
    }

    bool PlayerInLineOfSight()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Debug.DrawRay(transform.position, directionToPlayer, Color.red);

        if (distanceToPlayer <= lineOfSightRange)
        {
            if (!Physics.Raycast(transform.position, directionToPlayer.normalized, distanceToPlayer, obstaclesLayer))
            {
                return true;
            }
        }
        return false;
    }

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

            if (angleToNextCorner > 45f)
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

            Vector3 projectedPoint = Vector3.Project(transform.position - pathStart, pathDirection) + pathStart;
            Vector3 offset = transform.position - projectedPoint;
            transform.position -= offset * Time.deltaTime;
        }
    }

    void GoToLKP()
    {
        if (LKP != null)
        {
            SetDestination(LKP.position);
            Debug.Log("Going to Last Known Position.");
        }
    }
}
