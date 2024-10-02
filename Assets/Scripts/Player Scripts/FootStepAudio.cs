using UnityEngine;

public class FootstepAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip walkClip;
    public AudioClip runClip;
    private bool isMoving;

    void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            isMoving = true;
            PlayFootstep(Input.GetKey(KeyCode.LeftShift) ? runClip : walkClip);
        }
        else if (isMoving)
        {
            isMoving = false;
            Invoke("StopFootsteps", 0.1f);
        }
    }

    private void PlayFootstep(AudioClip clip)
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    private void StopFootsteps()
    {
        audioSource.Stop();
    }
}
