using UnityEngine;
using Cinemachine;

public class MonsterProximityShake : MonoBehaviour
{
    public CinemachineImpulseSource impulseSource; // Cinemachine Impulse Source reference
    public Transform player;  // Reference to the player transform
    public float maxShakeDistance = 10f; // Maximum distance at which shake starts
    public float maxShakeMagnitude = 5f; // Maximum shake magnitude when the monster is very close
    public float minShakeMagnitude = 0.1f; // Minimum shake magnitude when far away

    private bool playerInRange = false;

    private void Start()
    {
        if (impulseSource == null)
        {
            impulseSource = GetComponent<CinemachineImpulseSource>();
            if (impulseSource == null)
            {
                Debug.LogError("Cinemachine Impulse Source is not attached or assigned!");
            }
        }
    }

    private void Update()
    {
        // Continuously check distance between player and monster
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= maxShakeDistance)
        {
            // Calculate the shake intensity based on how close the monster is
            float intensity = Mathf.Lerp(maxShakeMagnitude, minShakeMagnitude, distance / maxShakeDistance);

            // Generate the impulse with the calculated intensity
            impulseSource.GenerateImpulse(intensity);
        }
    }
}
