using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RanPortal : MonoBehaviour
{
    public List<GameObject> Locations;
    public Transform player;
    private CharacterController characterController;

    private void Start()
    {
        characterController = player.GetComponent<CharacterController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TeleportPlayer();
        }
    }

    void TeleportPlayer()
    {
        if (Locations.Count < 3)
        {
            Debug.LogWarning("Not enough locations to find the three farthest.");
            return;
        }

        List<GameObject> farthestLocations = new List<GameObject>(Locations);

        farthestLocations.Sort((loc1, loc2) =>
        {
            float dist1 = Vector3.Distance(player.position, loc1.transform.position);
            float dist2 = Vector3.Distance(player.position, loc2.transform.position);
            return dist2.CompareTo(dist1);
        });

        List<GameObject> topThreeFarthest = farthestLocations.GetRange(0, 3);

        int randomIndex = Random.Range(0, topThreeFarthest.Count);
        GameObject selectedLocation = topThreeFarthest[randomIndex];

        if (characterController != null)
        {
            characterController.enabled = false;
            player.position = selectedLocation.transform.position;
            characterController.enabled = true;
        }
        else
        {
            player.position = selectedLocation.transform.position;
        }

        Debug.Log("Player teleported to: " + selectedLocation.name);

        Destroy(gameObject);
    }
}
