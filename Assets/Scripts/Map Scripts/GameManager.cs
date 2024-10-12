using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private int keyCount = 0;
    public int keysToWin = 3;
    public AudioClip KeyPickUp;
    private AudioSource audioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void CollectKey()
    {
        keyCount++;
        Debug.Log("Key Collected! Total keys: " + keyCount);
        audioSource.PlayOneShot(KeyPickUp);

        if (keyCount >= keysToWin)
        {
            WinGame();
        }
    }

    private void WinGame()
    {
        Debug.Log("YOU WON!");
    }
}
