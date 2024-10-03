using UnityEngine;

public class HeadBob : MonoBehaviour
{
    public AnimationCurve bobbingCurve; // The curve controlling the head bob
    public float bobbingSpeed = 5f;      // How fast the head bob occurs
    public float bobbingAmount = 0.05f;  // The magnitude of the head bob

    private float defaultY;              // The default y position of the camera
    private float timer;                 // Timer to track the position on the animation curve

    void Start()
    {
        defaultY = transform.localPosition.y;  // Store the starting y position
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
        {
            // Calculate the offset using the animation curve
            float offsetY = bobbingCurve.Evaluate(timer) * bobbingAmount;
            transform.localPosition = new Vector3(transform.localPosition.x, defaultY + offsetY, transform.localPosition.z);

            // Increment timer based on speed and reset if it exceeds 1 (one full cycle of the curve)
            timer += Time.deltaTime * bobbingSpeed;
            if (timer > 1f)
                timer -= 1f;
        }
        else
        {
            // Reset to the default position if not moving
            timer = 0;
            transform.localPosition = new Vector3(transform.localPosition.x, defaultY, transform.localPosition.z);
        }
    }
}
