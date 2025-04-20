using UnityEngine;
using UnityEngine.UI; // or TMPro if using TextMeshPro

[RequireComponent(typeof(AudioSource))]
public class BoatEntry : MonoBehaviour
{
    [Header("Player Settings (Assign in Inspector)")]
    public GameObject player;               // The Player GameObject
    public MouseMovement playerMouse;       // The player's MouseMovement component
    public PlayerMovement playerMovement;   // The player's PlayerMovement component
    public GameObject boat;                 // The Boat GameObject (with the WaterBoat script)

    [Header("UI Prompts (Assign in Inspector)")]
    public GameObject enterPromptUI;        // The "Press E to enter boat" prompt
    public GameObject exitPromptUI;         // The "Press E to exit boat" prompt

    [Header("Audio Settings")]
    public AudioClip vroomClip;             // Assign your vroom.m4a clip here
    private AudioSource audioSource;        // Source to play the vroom sound

    [Header("Exit Settings")]
    public Vector3 exitOffset = new Vector3(0, 2f, 0);  // Offset to place player when exiting

    private bool isPlayerNearby = false;
    private bool isInBoat = false;

    // Cached components
    private Renderer[] playerRenderers;
    private Collider[] playerColliders;
    private WaterBoat waterBoat;
    private CharacterController controller;
    private Rigidbody rb;
    private RigidbodyConstraints originalConstraints;

    // Original transforms for reset
    private Quaternion originalPlayerRotation;
    private Transform cameraTransform;
    private Transform originalCameraParent;
    private Vector3 originalCameraLocalPosition;
    private Quaternion originalCameraLocalRotation;

    void Start()
    {
        // Cache player components
        playerRenderers = player.GetComponentsInChildren<Renderer>();
        playerColliders = player.GetComponentsInChildren<Collider>();
        controller = player.GetComponent<CharacterController>();
        rb = player.GetComponent<Rigidbody>();
        if (rb != null)
            originalConstraints = rb.constraints;

        // Save original player rotation
        originalPlayerRotation = player.transform.rotation;

        // Cache main camera transform and its original parent/local offsets
        cameraTransform = Camera.main.transform;
        originalCameraParent = cameraTransform.parent;
        originalCameraLocalPosition = cameraTransform.localPosition;
        originalCameraLocalRotation = cameraTransform.localRotation;

        // Hide both prompts at start
        if (enterPromptUI) enterPromptUI.SetActive(false);
        if (exitPromptUI) exitPromptUI.SetActive(false);

        // Cache and disable boat control
        waterBoat = boat.GetComponent<WaterBoat>();
        if (waterBoat != null)
            waterBoat.enabled = false;

        // Setup audio source
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = vroomClip;
        audioSource.loop = true; // keep looping while in boat
        audioSource.playOnAwake = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isInBoat && other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (enterPromptUI) enterPromptUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!isInBoat && other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (enterPromptUI) enterPromptUI.SetActive(false);
        }
    }

    // Optional: show enter prompt on mouse hover
    void OnMouseOver()
    {
        if (!isInBoat && enterPromptUI)
            enterPromptUI.SetActive(true);
    }

    void OnMouseExit()
    {
        if (!isInBoat && !isPlayerNearby && enterPromptUI)
            enterPromptUI.SetActive(false);
    }

    void Update()
    {
        if (!isInBoat)
        {
            if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
                EnterBoat();
        }
        else
        {
            // Show exit prompt while in boat
            if (exitPromptUI && !exitPromptUI.activeSelf)
                exitPromptUI.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
                ExitBoat();
        }
    }

    void EnterBoat()
    {
        isInBoat = true;
        isPlayerNearby = false;

        // Hide enter prompt and show exit prompt
        if (enterPromptUI) enterPromptUI.SetActive(false);
        if (exitPromptUI) exitPromptUI.SetActive(true);

        // Play vroom sound
        if (audioSource && !audioSource.isPlaying)
            audioSource.Play();

        // Disable player controls
        if (playerMouse) playerMouse.enabled = false;
        if (playerMovement) playerMovement.enabled = false;

        // Disable CharacterController and physics
        if (controller) controller.enabled = false;
        if (rb)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Hide player model
        foreach (var col in playerColliders) col.enabled = false;
        foreach (var rend in playerRenderers) rend.enabled = false;

        // Parent to boat and enable boat controls
        player.transform.SetParent(boat.transform);
        if (waterBoat) waterBoat.enabled = true;

        Debug.Log("Player entered boat");
    }

    void ExitBoat()
    {
        isInBoat = false;

        // Hide exit prompt
        if (exitPromptUI) exitPromptUI.SetActive(false);

        // Stop vroom sound
        if (audioSource && audioSource.isPlaying)
            audioSource.Stop();

        // Unparent and reposition player
        player.transform.SetParent(null);
        player.transform.position = boat.transform.position + exitOffset;

        // Reset player rotation
        player.transform.rotation = originalPlayerRotation;

        // Restore camera
        cameraTransform.SetParent(originalCameraParent);
        cameraTransform.localPosition = originalCameraLocalPosition;
        cameraTransform.localRotation = originalCameraLocalRotation;

        // Disable boat controls
        if (waterBoat) waterBoat.enabled = false;

        // Restore physics
        if (rb)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.constraints = originalConstraints;
        }

        // Show player model
        foreach (var col in playerColliders) col.enabled = true;
        foreach (var rend in playerRenderers) rend.enabled = true;

        // Restore player controls
        if (playerMouse) playerMouse.enabled = true;
        if (playerMovement) playerMovement.enabled = true;
        if (controller) controller.enabled = true;

        Debug.Log("Player exited boat, rotation and camera reset");
    }
}
