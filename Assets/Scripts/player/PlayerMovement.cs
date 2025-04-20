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
    private AudioSource audioSource;   // Assign your AudioSource here
    public AudioClip footstepClip;    // Or assign in Inspector
    public float stepInterval = 0.5f; // Seconds between steps
    private float stepTimer = 0f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = footstepClip;
    }
    // Update is called once per frame
    void Update()
    {
        //reset velocity when hitting the ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //right is the red Axis, foward is the blue axis
        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        // --- Footstep Logic ---
        bool isMoving = move.magnitude > 0.1f;
        if (isGrounded && isMoving)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                // Raycast down to check for “Grass” tag
                RaycastHit hit;
                if (Physics.Raycast(groundCheck.position, Vector3.down, out hit, groundDistance + 0.1f, groundMask))
                {
                    if (hit.collider.CompareTag("Grass"))
                    {
                        audioSource.PlayOneShot(footstepClip);
                    }
                }
                stepTimer = stepInterval;
            }
        }
        else
        {
            // reset timer when not moving or airborne
            stepTimer = 0f;
        }

        //jump if player is on the ground
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            //equation for jump velocity
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}