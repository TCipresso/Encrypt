using UnityEngine;
using System.Collections.Generic;

public class KeyManager : MonoBehaviour
{
    public List<Transform> keyLocations = new List<Transform>();
    public List<GameObject> keyPrefabs = new List<GameObject>();
    public int numberOfKeysToSpawn = 3;

    void Start()
    {
        if (numberOfKeysToSpawn > keyLocations.Count)
        {
            numberOfKeysToSpawn = keyLocations.Count;
        }

        SpawnKeys();
    }

    void SpawnKeys()
    {
        List<Transform> availableLocations = new List<Transform>(keyLocations);

        for (int i = 0; i < numberOfKeysToSpawn; i++)
        {
            int randomLocationIndex = Random.Range(0, availableLocations.Count);
            Transform selectedLocation = availableLocations[randomLocationIndex];
            int randomKeyIndex = Random.Range(0, keyPrefabs.Count);
            GameObject selectedKeyPrefab = keyPrefabs[randomKeyIndex];
            Instantiate(selectedKeyPrefab, selectedLocation.position, Quaternion.identity);
            availableLocations.RemoveAt(randomLocationIndex);
        }
    }
}
