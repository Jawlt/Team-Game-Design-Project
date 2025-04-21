using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float gasDamagePerSecond = 5f;
    public float poisonWaterDamagePerSecond = 7f;

    [Header("Respawn Settings")]
    public Transform spawnPoint;

    [Header("Audio Settings")]
    public AudioClip poisonClip;
    public AudioClip deathClip;
    private AudioSource audioSource;

    [Header("Poison Water Settings")]
    public LayerMask poisonWaterLayer;
    public Transform waterCheck;
    public float checkRadius = 0.5f;

    [Header("Runtime (read‐only)")]
    [SerializeField]
    private float currentHealth;

    private bool inGas = false;
    private bool isDead = false;
    private bool inPoisonWater = false;

    void Start()
    {
        currentHealth = maxHealth;
        isDead = false;

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        inPoisonWater = Physics.CheckSphere(waterCheck.position, checkRadius, poisonWaterLayer);

        bool isInPoison = (inGas || inPoisonWater);

        // Handle poison audio loop
        if (isInPoison && poisonClip != null && !isDead)
        {
            if (!audioSource.isPlaying || audioSource.clip != poisonClip)
            {
                audioSource.clip = poisonClip;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            // Stop poison clip only if it's the currently playing one
            if (audioSource != null && audioSource.clip == poisonClip && audioSource.isPlaying)
            {
                audioSource.Stop();
                audioSource.clip = null;
                audioSource.loop = false;
            }
        }

        // Apply damage
        if (isInPoison && !isDead)
        {
            float totalDPS = 0f;
            if (inGas) totalDPS += gasDamagePerSecond;
            if (inPoisonWater) totalDPS += poisonWaterDamagePerSecond;

            currentHealth -= totalDPS * Time.deltaTime;
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
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("GasMist"))
        {
            inGas = false;
        }
    }

    private void Die()
    {
        Debug.Log("Die() CALLED — should teleport player.");
        isDead = true;
        inGas = false;

        // Stop poison loop if it was playing
        if (audioSource != null && audioSource.clip == poisonClip && audioSource.isPlaying)
        {
            audioSource.Stop();
            audioSource.clip = null;
            audioSource.loop = false;
        }

        // Play death OneShot
        if (audioSource && deathClip)
            audioSource.PlayOneShot(deathClip);

        // Respawn logic
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
