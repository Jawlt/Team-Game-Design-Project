using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float gasDamagePerSecond = 5f;

    [Header("Respawn Settings")]
    public Transform spawnPoint; // Assign in Inspector

    [Header("Runtime (read‐only)")]
    [SerializeField]
    private float currentHealth;

    [Header("Audio Settings")]
    public AudioClip poisonClip;             
    private AudioSource audioSource;        // Source to play the poison sound

    private bool inGas = false;
    private bool isDead = false;
    private AudioSource poisonAudio;


    void Start()
    {
        currentHealth = maxHealth;
        isDead = false;
        // Setup audio source
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = poisonClip;
        audioSource.loop = true; // keep looping while in gas
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (inGas && !isDead)
        {
            currentHealth -= gasDamagePerSecond * Time.deltaTime;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

            if (currentHealth <= 0f)
            {
                Die();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GasMist"))
        {
            inGas = true;
            // Play the looping poison sound
            if (audioSource && !audioSource.isPlaying)
                audioSource.Play();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("GasMist"))
        {
            inGas = false;
            // Stop the sound when you leave
            if (audioSource != null && audioSource.isPlaying)
                audioSource.Stop();
        }
    }

    private void Die()
    {
        Debug.Log("Die() CALLED — should teleport player.");
        isDead = true;
        inGas = false;

        if (spawnPoint != null)
        {
            Debug.Log("Spawn point found at position: " + spawnPoint.position);

            CharacterController controller = GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;
                transform.position = spawnPoint.position;
                transform.rotation = spawnPoint.rotation;
                controller.enabled = true;
            }
            else
            {
                transform.position = spawnPoint.position;
                transform.rotation = spawnPoint.rotation;
            }

            Debug.Log("Player respawned.");
        }
        else
        {
            Debug.LogWarning("No spawn point assigned!");
        }

        currentHealth = maxHealth;
        isDead = false;
    }


    /// <summary>
    /// Public getter for current health (e.g., for UI display).
    /// </summary>
    public float GetCurrentHealth() => currentHealth;
}
