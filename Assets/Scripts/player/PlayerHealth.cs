using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float gasDamagePerSecond = 5f;

    [Header("Respawn Settings")]
    public Transform spawnPoint;

    [Header("Audio Settings")]
    public AudioClip poisonClip;
    public AudioClip deathClip;
    private AudioSource audioSource;

    [Header("Runtime (read‐only)")]
    [SerializeField]
    private float currentHealth;

    private bool inGas = false;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        isDead = false;

        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.clip = poisonClip;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }
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
            if (audioSource && poisonClip && !audioSource.isPlaying)
                audioSource.Play();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("GasMist"))
        {
            inGas = false;
            if (audioSource && audioSource.isPlaying)
                audioSource.Stop();
        }
    }

    private void Die()
    {
        Debug.Log("Die() CALLED — should teleport player.");
        isDead = true;
        inGas = false;

        // Stop poison loop
        if (audioSource && audioSource.isPlaying)
            audioSource.Stop();

        if (deathClip != null)
            audioSource.PlayOneShot(deathClip);

        // Move player to spawn
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

    public float GetCurrentHealth() => currentHealth;
}
