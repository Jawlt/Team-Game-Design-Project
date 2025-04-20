using UnityEngine;
using UnityEngine.UI; // or TMPro if using TextMeshPro

[RequireComponent(typeof(AudioSource))]
public class BoatEntry : MonoBehaviour
{
    [Header("Player Settings (Assign in Inspector)")]
    public GameObject player;
    public MouseMovement playerMouse;
    public PlayerMovement playerMovement;
    public GameObject boat;

    [Header("UI Prompts (Assign in Inspector)")]
    public GameObject enterPromptUI;
    public GameObject exitPromptUI;

    [Header("Audio Settings")]
    public AudioClip vroomClip;
    private AudioSource audioSource;

    [Header("Exit Settings")]
    public Vector3 exitOffset = new Vector3(0, 2f, 0);

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
        playerRenderers = player.GetComponentsInChildren<Renderer>();
        playerColliders = player.GetComponentsInChildren<Collider>();
        controller = player.GetComponent<CharacterController>();
        rb = player.GetComponent<Rigidbody>();
        if (rb != null)
            originalConstraints = rb.constraints;

        originalPlayerRotation = player.transform.rotation;

        cameraTransform = Camera.main.transform;
        originalCameraParent = cameraTransform.parent;
        originalCameraLocalPosition = cameraTransform.localPosition;
        originalCameraLocalRotation = cameraTransform.localRotation;

        if (enterPromptUI) enterPromptUI.SetActive(false);
        if (exitPromptUI) exitPromptUI.SetActive(false);

        waterBoat = boat.GetComponent<WaterBoat>();
        if (waterBoat != null)
        {
            waterBoat.enabled = false;
            waterBoat.ExitBoat(); // Ensure boat starts with physics disabled
        }

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = vroomClip;
        audioSource.loop = true;
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

        if (enterPromptUI) enterPromptUI.SetActive(false);
        if (exitPromptUI) exitPromptUI.SetActive(true);

        if (audioSource && !audioSource.isPlaying)
            audioSource.Play();

        if (playerMouse) playerMouse.enabled = false;
        if (playerMovement) playerMovement.enabled = false;

        if (controller) controller.enabled = false;
        if (rb)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        foreach (var col in playerColliders) col.enabled = false;
        foreach (var rend in playerRenderers) rend.enabled = false;

        player.transform.SetParent(boat.transform);

        if (waterBoat)
        {
            waterBoat.enabled = true;
            waterBoat.EnterBoat(); // ✅ enable physics & control
        }

        Debug.Log("Player entered boat");
    }

    void ExitBoat()
    {
        isInBoat = false;

        if (exitPromptUI) exitPromptUI.SetActive(false);
        if (audioSource && audioSource.isPlaying)
            audioSource.Stop();

        player.transform.SetParent(null);
        player.transform.position = boat.transform.position + exitOffset;
        player.transform.rotation = originalPlayerRotation;

        cameraTransform.SetParent(originalCameraParent);
        cameraTransform.localPosition = originalCameraLocalPosition;
        cameraTransform.localRotation = originalCameraLocalRotation;

        if (waterBoat)
        {
            waterBoat.ExitBoat(); // ✅ disable physics
            waterBoat.enabled = false;
        }

        if (rb)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.constraints = originalConstraints;
        }

        foreach (var col in playerColliders) col.enabled = true;
        foreach (var rend in playerRenderers) rend.enabled = true;

        if (playerMouse) playerMouse.enabled = true;
        if (playerMovement) playerMovement.enabled = true;
        if (controller) controller.enabled = true;

        Debug.Log("Player exited boat, rotation and camera reset");
    }
}
