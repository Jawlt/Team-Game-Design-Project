using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;

    [Header("Footsteps")]
    private AudioSource audioSource;
    public AudioClip footstepClip;
    public float stepInterval = 0.5f;
    private float stepTimer = 0f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("No AudioSource found on PlayerMovement GameObject.");
        }

        if (footstepClip == null)
        {
            Debug.LogWarning("Footstep AudioClip not assigned.");
        }
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // --- Footstep Sound Handling ---
        bool isMoving = move.magnitude > 0.1f;
        if (isGrounded && isMoving)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f && audioSource && footstepClip)
            {
                // Optional: Check surface tag
                if (Physics.Raycast(groundCheck.position, Vector3.down, out RaycastHit hit, groundDistance + 0.1f, groundMask))
                {
                    if (hit.collider.CompareTag("Grass") || hit.collider.CompareTag("Untagged"))
                    {
                        audioSource.PlayOneShot(footstepClip, 1f); // reliable
                    }
                }
                else
                {
                    audioSource.PlayOneShot(footstepClip, 1f); // fallback
                }

                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = stepInterval; // prevent instant replays on resume
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
