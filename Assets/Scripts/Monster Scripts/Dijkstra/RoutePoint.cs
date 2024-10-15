using UnityEngine;

public class RoutePoint : MonoBehaviour
{
    public DijkstraRoute monster; // Reference to the monster following the route

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster")) // Ensure only the monster triggers it
        {
            monster.SwitchState(DijkstraRoute.MonsterState.Stop); // Switch to the Stop state when the monster hits the point
        }
    }
}
