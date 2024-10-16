using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lockdown : MonoBehaviour
{
    public List<GameObject> Sweepers; // List of monsters (Dijkstras)
    public float lockdownDelay = 30f; // Time before lockdown starts
    public int monstersToActivate = 2; // Number of monsters to activate during lockdown

    void Start()
    {
        DisableAllMonsters(); // Ensure all monsters are disabled at the start
        StartCoroutine(InitiateLockdown()); // Start the lockdown countdown
    }

    // Disable all monsters in the list
    private void DisableAllMonsters()
    {
        foreach (GameObject monster in Sweepers)
        {
            monster.SetActive(false);
        }
    }

    // Coroutine to handle lockdown initiation
    private IEnumerator InitiateLockdown()
    {
        yield return new WaitForSeconds(lockdownDelay); // Wait for the specified delay

        EnableRandomMonsters(); // Enable a random selection of monsters
    }

    // Enable a random selection of monsters
    private void EnableRandomMonsters()
    {
        if (Sweepers.Count == 0) return; // Ensure there are monsters to activate

        List<int> selectedIndexes = new List<int>();

        // Randomly select the specified number of monsters
        while (selectedIndexes.Count < monstersToActivate)
        {
            int randomIndex = Random.Range(0, Sweepers.Count);

            // Ensure no duplicate selections
            if (!selectedIndexes.Contains(randomIndex))
            {
                selectedIndexes.Add(randomIndex);
            }
        }

        // Enable the selected monsters
        foreach (int index in selectedIndexes)
        {
            Sweepers[index].SetActive(true);
        }
    }
}
