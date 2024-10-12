using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalSpawner : MonoBehaviour
{
    public List<GameObject> portals;
    public int numberOfPortalsToEnable;

    void Start()
    {
        EnableRandomPortals();
    }

    void EnableRandomPortals()
    {
        if (numberOfPortalsToEnable > portals.Count)
        {
            Debug.LogWarning("Number of portals to enable exceeds available portals.");
            numberOfPortalsToEnable = portals.Count;
        }

        List<GameObject> shuffledPortals = new List<GameObject>(portals);
        for (int i = 0; i < shuffledPortals.Count; i++)
        {
            int randomIndex = Random.Range(i, shuffledPortals.Count);
            GameObject temp = shuffledPortals[i];
            shuffledPortals[i] = shuffledPortals[randomIndex];
            shuffledPortals[randomIndex] = temp;
        }

        for (int i = 0; i < numberOfPortalsToEnable; i++)
        {
            shuffledPortals[i].SetActive(true);
        }
    }
}
