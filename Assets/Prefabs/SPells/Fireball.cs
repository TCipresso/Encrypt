using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 10f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Fireball collided with: " + collision.gameObject.tag);

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Fireball hit an enemy!");
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            // Ignores Player
            return;
        }

        Destroy(gameObject);
    }
}
