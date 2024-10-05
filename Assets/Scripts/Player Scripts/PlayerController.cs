using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public bool CanLook = true;

    public AudioSource walk;
    public AudioSource run;

    // Head bobbing variables
    public float bobbingSpeed = 14f;
    public float runBobbingSpeed = 20f;
    public float walkBobbingAmount = 0.05f;
    public float runBobbingAmount = 0.1f;

    [Range(0f, 1f)]
    public float flashlightBobbingFactor = 0.5f;

    private float defaultPosY = 0;
    private float cameraYOffset = 0;
    private float timer = 0;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private Vector3 originalCameraPosition;

    public bool canMove = true;
    private CharacterController characterController;
    public Animator animator;

    // Spell variables
    public Image spellImage;
    public Sprite[] spellIcons;
    private int currentSpellIndex = 0;
    public GameObject DataBreach;
    public GameObject Override;
    public GameObject Untraceable;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        originalCameraPosition = playerCamera.transform.localPosition;
        defaultPosY = playerCamera.transform.localPosition.y;
        DataBreach.SetActive(true);
        Override.SetActive(false);
        Untraceable.SetActive(false);
    }

    void Update()
    {
        // Handle Movement
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = canMove ? (isRunning ? runSpeed : walkSpeed) : 0;
        moveDirection = forward * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal");
        if (moveDirection.sqrMagnitude > 1) moveDirection.Normalize();
        moveDirection *= currentSpeed;

        characterController.Move(moveDirection * Time.deltaTime);

        // Handle Rotation
        if (CanLook)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        // Spell Cycling
        if (Input.GetKeyDown(KeyCode.Q)) ChangeSpell(-1);  // Previous spell
        else if (Input.GetKeyDown(KeyCode.E)) ChangeSpell(1);  // Next spell

        // Head Bobbing
        float waveslice = 0.0f;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float currentBobbingSpeed = isRunning ? runBobbingSpeed : bobbingSpeed;
        float currentBobbingAmount = isRunning ? runBobbingAmount : walkBobbingAmount;

        if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0) timer = 0.0f;
        else
        {
            waveslice = Mathf.Sin(timer);
            timer += currentBobbingSpeed * Time.deltaTime;
            if (timer > Mathf.PI * 2) timer -= Mathf.PI * 2;
        }

        if (waveslice != 0)
        {
            float translateChange = waveslice * currentBobbingAmount;
            float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
            translateChange = totalAxes * translateChange;
            float modifiedTranslateChange = translateChange * flashlightBobbingFactor;
            playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, defaultPosY + cameraYOffset + modifiedTranslateChange, playerCamera.transform.localPosition.z);
        }
        else
        {
            playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, defaultPosY + cameraYOffset, playerCamera.transform.localPosition.z);
        }

        // Animator parameters
        bool isWalking = moveDirection.magnitude > 0 && !isRunning;
        animator.SetBool("IsWalking", isWalking);
        animator.SetBool("IsRunning", isRunning);
    }

    void ChangeSpell(int changeAmount)
    {
        currentSpellIndex = (currentSpellIndex + changeAmount) % spellIcons.Length;
        if (currentSpellIndex < 0) currentSpellIndex = spellIcons.Length - 1;

        spellImage.sprite = spellIcons[currentSpellIndex];

        // Deactivate all spells
        DataBreach.SetActive(false);
        Override.SetActive(false);
        Untraceable.SetActive(false);

        // Activate the selected spell
        switch (currentSpellIndex)
        {
            case 0:
                DataBreach.SetActive(true);
                break;
            case 1:
                Override.SetActive(true);
                break;
            case 2:
                Untraceable.SetActive(true);
                break;
        }
    }
}