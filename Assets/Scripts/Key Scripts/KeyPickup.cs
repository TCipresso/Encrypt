using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Access GameManager and increment key count
            GameManager.instance.CollectKey();
            Destroy(gameObject);
        }
    }
}
