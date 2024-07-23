using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // public variables
    [Header("Mouse settings")]
    [Range(1.0f, 100.0f)]
    public float mouseSensitivity = 100f;
    public float minClamp, maxClamp;
    public bool smoothCameraMovement = false;
    [Range(1.0f, 50.0f)]
    public float mouseSmooth = 1f;

    [Header("Movement settings")]
    public float charSpeed = 2f;
    public float charRunSpeed = 2.8f;
    public float jumpHeight = 0.75f;
    public float leanAmount = 12.5f;
    public float leanSpeed = 3.5f;
    public float gravity = -20f;
    public float groundCheckerDistance = 0.4f;
    public LayerMask groundMask;

    [Header("References")]
    public Transform playerTransform;
    public Transform playerTarget;
    public Transform cameraTransform;
    public Transform cameraTarget;
    public Transform cameraPivot;
    public Transform groundChecker;
    public CharacterController charController;

    // private variables
    float xRotation = 0f;
    Vector3 velocity;
    float charvel = 0;
    bool isGrounded;

    void Start() {
        // Locks and hides the cursor
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update() {
        // mouse inputs
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        // keyboard inputs
        float keyX = Input.GetAxis("Horizontal");
        float keyY = Input.GetAxis("Vertical");

        // keyboard function call
        Movement(keyX, keyY);

        // mouse function call
        if (smoothCameraMovement == true) {
            SmoothMouseLook(mouseX, mouseY);
        }
        else {
            NormalMouseLook(mouseX, mouseY);
        }

        // lean function call
        CameraLean();
    }

    void Movement(float x, float y) {
        // movement
        Vector3 MovementVec = transform.right * x + transform.forward * y;

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Vertical") >= 0) {
            charController.Move(MovementVec * charRunSpeed * Time.deltaTime);
        }
        charController.Move(MovementVec * charSpeed * Time.deltaTime);

        // jump
        if (Input.GetButtonDown("Jump") && isGrounded) {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        // physics
        isGrounded = Physics.CheckSphere(groundChecker.position, groundCheckerDistance, groundMask);

        if (isGrounded && velocity.y < 0) {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
        charController.Move(velocity * Time.deltaTime);
    }

    void NormalMouseLook(float x, float y) {
        xRotation -= y;
        xRotation = Mathf.Clamp(xRotation, minClamp, maxClamp);

        // camera rotation
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // player rotation
        playerTransform.Rotate(Vector3.up * x);
    }

    void SmoothMouseLook(float x, float y) {
        // camera movement
        xRotation -= y;
        xRotation = Mathf.Clamp(xRotation, minClamp, maxClamp);

        // camera rotation
        cameraTarget.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        cameraTransform.localRotation = Quaternion.Slerp(cameraTransform.localRotation, cameraTarget.localRotation, mouseSmooth * Time.deltaTime);

        // player rotation
        playerTarget.Rotate(Vector3.up * x);
        playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, playerTarget.rotation, mouseSmooth * Time.deltaTime);
    }

    void CameraLean()
    {
        bool leanR = Input.GetKey(KeyCode.E);
        bool leanL = Input.GetKey(KeyCode.Q);

        // if player is leaning
        if (leanR || leanL)
        {
            // right
            if (leanR && cameraTarget.eulerAngles.z != 350 && !leanL)
            {
                cameraTarget.localRotation = Quaternion.Euler(0f, 0f, leanAmount * -1);
                cameraPivot.localRotation = Quaternion.Slerp(cameraPivot.localRotation, cameraTarget.localRotation, leanSpeed * Time.deltaTime);
            }
            // left
            if (leanL && cameraPivot.eulerAngles.z != 370 && !leanR)
            {
                cameraTarget.localRotation = Quaternion.Euler(0f, 0f, leanAmount);
                cameraPivot.localRotation = Quaternion.Slerp(cameraPivot.localRotation, cameraTarget.localRotation, leanSpeed * Time.deltaTime);
            }
        }
        // return camerapivot to original position if not leaning
        else
        {
            cameraTarget.localRotation = Quaternion.Euler(0f, 0f, 0f);
            cameraPivot.localRotation = Quaternion.Slerp(cameraPivot.localRotation, cameraTarget.localRotation, leanSpeed * Time.deltaTime);
        }
    }
}