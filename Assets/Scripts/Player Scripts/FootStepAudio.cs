using UnityEngine;

public class FootstepAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip walkClip;
    public AudioClip runClip;
    private bool isMoving;
    private bool isRunning;

    void Update()
    {
        bool currentIsRunning = Input.GetKey(KeyCode.LeftShift);
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            if (!isMoving || currentIsRunning != isRunning)
            {
                isMoving = true;
                isRunning = currentIsRunning;
                PlayFootstep(isRunning ? runClip : walkClip);
            }
        }
        else if (isMoving)
        {
            isMoving = false;
            Invoke("StopFootsteps", 0.1f);
        }
    }

    private void PlayFootstep(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    private void StopFootsteps()
    {
        audioSource.Stop();
    }
}
