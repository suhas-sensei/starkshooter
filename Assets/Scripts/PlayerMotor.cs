using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool isGrounded;
    public float gravity = -9.8f;

    // Movement variables
    public float speed = 5f;

    // Mouse look variables
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    public Camera playerCamera;
    private float xRotation = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        // Lock cursor to center of screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        isGrounded = controller.isGrounded;

        // Apply gravity
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }
        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    // Process movement input
    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;

        // Transform movement relative to player's facing direction
        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);
    }

    // Process mouse look input
    public void ProcessLook(Vector2 input)
    {
        float mouseX = input.x;
        float mouseY = input.y;

        // Rotate the player body left and right
        playerBody.Rotate(Vector3.up * mouseX * mouseSensitivity * Time.deltaTime);

        // Rotate the camera up and down
        xRotation -= mouseY * mouseSensitivity * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    // Add this to your PlayerMotor.cs
    public void ApplyRecoil(Vector2 recoil)
    {
        // Apply recoil to camera rotation
        float mouseX = recoil.x;
        float mouseY = recoil.y;

        // Rotate the player body left and right
        playerBody.Rotate(Vector3.up * mouseX);

        // Rotate the camera up and down
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}