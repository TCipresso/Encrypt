using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private int keyCount = 0; // Number of keys the player has collected
    public int keysToWin = 3; // Set this to the number of keys required to win

    private void Awake()
    {
        // Singleton pattern to ensure there is only one GameManager
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Called when a key is collected
    public void CollectKey()
    {
        keyCount++;
        Debug.Log("Key Collected! Total keys: " + keyCount);

        // Check if the player has collected enough keys to win
        if (keyCount >= keysToWin)
        {
            WinGame();
        }
    }

    // Function to display win message
    private void WinGame()
    {
        Debug.Log("YOU WON!");
    }
}
