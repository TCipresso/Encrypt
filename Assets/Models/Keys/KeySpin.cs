using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySpin : MonoBehaviour
{
    public float rotationSpeed = 100.0f;
    public float bobbingSpeed = 2.0f;
    public float bobbingAmount = 0.5f;

    private float startY;
    private float tempVal;

    void Start()
    {
        startY = transform.position.y;
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        tempVal = startY + Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmount;
        transform.position = new Vector3(transform.position.x, tempVal, transform.position.z);
    }
}
