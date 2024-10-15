using System.Collections;
using UnityEngine;

public class LKPUpdater : MonoBehaviour
{
    public Transform player;
    public float updateInterval = 30f;

    private void Start()
    {
        StartCoroutine(UpdateLKPPosition());
    }

    IEnumerator UpdateLKPPosition()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateInterval);

            if (player != null)
            {
                transform.position = player.position;
                Debug.Log("LKP updated to player's position: " + transform.position);
            }
        }
    }
}
