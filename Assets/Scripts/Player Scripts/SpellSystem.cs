using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellSystem : MonoBehaviour
{
    public enum Spell { Fireball, Haste, Truesight }
    public Spell currentSpell = Spell.Fireball;
    public GameObject fireballPrefab;

    public PlayerController playerController;
    public Camera playerCamera; // Reference to the camera for zooming effect

    private float holdTime = 0f;
    private float castDelay = 0.5f;

    private bool hasteActive = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CycleSpell(-1);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            CycleSpell(1);
        }

        if (Input.GetButton("Fire1"))
        {
            holdTime += Time.deltaTime;

            if (holdTime >= castDelay)
            {
                CastSpell();
                holdTime = 0f;
            }
        }
        else
        {
            holdTime = 0f;
        }
    }

    void CycleSpell(int direction)
    {
        int spellCount = System.Enum.GetValues(typeof(Spell)).Length;
        currentSpell = (Spell)(((int)currentSpell + direction + spellCount) % spellCount);
        Debug.Log("Current Spell: " + currentSpell);
    }

    void CastSpell()
    {
        switch (currentSpell)
        {
            case Spell.Fireball:
                HandleFireball();
                break;
            case Spell.Haste:
                if (!hasteActive)
                {
                    HandleHaste();
                }
                else
                {
                    Debug.Log("Haste is already active. Cannot cast again.");
                }
                break;
            case Spell.Truesight:
                HandleTruesight();
                break;
        }
    }

    void HandleFireball()
    {
        Debug.Log("Casting Fireball...");
        GameObject fireball = Instantiate(fireballPrefab, transform.position + transform.forward, transform.rotation);
    }

    void HandleHaste()
    {
        Debug.Log("Casting Haste...");
        hasteActive = true;
        StartCoroutine(HasteEffect());
        StartCoroutine(ChangeFOV(120f, 0.5f));
    }

    IEnumerator HasteEffect()
    {
        float originalWalkSpeed = playerController.walkSpeed;
        float originalRunSpeed = playerController.runSpeed;
        playerController.walkSpeed = 10f;
        playerController.runSpeed = 18f;
        yield return new WaitForSeconds(10f);
        playerController.walkSpeed = originalWalkSpeed;
        playerController.runSpeed = originalRunSpeed;

        StartCoroutine(ChangeFOV(90f, 0.5f));
        Debug.Log("Haste ended. Reverting to original speeds.");
        hasteActive = false;
    }

    IEnumerator ChangeFOV(float targetFOV, float duration)
    {
        float startFOV = playerCamera.fieldOfView;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            playerCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        playerCamera.fieldOfView = targetFOV;
    }

    void HandleTruesight()
    {
        Debug.Log("Casting Truesight...");
    }
}
