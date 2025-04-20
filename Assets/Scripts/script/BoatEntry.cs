using UnityEngine;

public class BoatEntry : MonoBehaviour
{
    [Header("Player Settings (Assign in Inspector)")]
    public GameObject player;               // The Player GameObject
    public MouseMovement playerMouse;       // The player's MouseMovement component
    public PlayerMovement playerMovement;   // The player's PlayerMovement component
    public GameObject boat;                 // The Boat GameObject (with the WaterBoat script)

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

        // Cache camera transform and original parent/local transform
        cameraTransform = Camera.main.transform;
        originalCameraParent = cameraTransform.parent;
        originalCameraLocalPosition = cameraTransform.localPosition;
        originalCameraLocalRotation = cameraTransform.localRotation;

        // Cache and disable boat control
        waterBoat = boat.GetComponent<WaterBoat>();
        if (waterBoat != null)
            waterBoat.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isInBoat && other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            // TODO: Show UI prompt "Press E to enter boat"
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!isInBoat && other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            // TODO: Hide UI prompt
        }
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
            if (Input.GetKeyDown(KeyCode.E))
                ExitBoat();
        }
    }

    void EnterBoat()
    {
        isInBoat = true;
        isPlayerNearby = false;

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

        // Hide player
        foreach (var col in playerColliders) col.enabled = false;
        foreach (var rend in playerRenderers) rend.enabled = false;

        // Parent player to boat and enable boat controls
        player.transform.SetParent(boat.transform);
        if (waterBoat) waterBoat.enabled = true;

        Debug.Log("Player entered boat");
    }

    void ExitBoat()
    {
        isInBoat = false;

        // Unparent player and position atop boat
        player.transform.SetParent(null);
        player.transform.position = boat.transform.position + exitOffset;

        // Reset player rotation to original (upright)
        player.transform.rotation = originalPlayerRotation;

        // Restore camera parent and local transform
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

        // Show player
        foreach (var col in playerColliders) col.enabled = true;
        foreach (var rend in playerRenderers) rend.enabled = true;

        // Restore player controls
        if (playerMouse) playerMouse.enabled = true;
        if (playerMovement) playerMovement.enabled = true;
        if (controller) controller.enabled = true;

        Debug.Log("Player exited boat, rotation and camera reset");
    }
}
