using UnityEngine;

public class SnapRotation : MonoBehaviour
{
    // Optional: Expose for external setting, e.g., from a pathfinding component
    private Vector3 currentDirection;

    // Update is called once per frame
    void Update()
    {
        // Get the current y-rotation of the GameObject
        float currentRotationY = transform.eulerAngles.y;
        // Calculate the snapped angle by rounding the current rotation to the nearest 90 degrees
        float snappedAngle = Mathf.Round(currentRotationY / 90f) * 90f;

        // Set the current direction based on the snapped angle
        if (snappedAngle == 0)
            currentDirection = Vector3.forward;
        else if (snappedAngle == 90)
            currentDirection = Vector3.right;
        else if (snappedAngle == 180 || snappedAngle == -180)
            currentDirection = Vector3.back;
        else if (snappedAngle == -90 || snappedAngle == 270)
            currentDirection = Vector3.left;

        // Apply the snapped rotation to the transform
        transform.rotation = Quaternion.Euler(0, snappedAngle, 0);
    }
}
