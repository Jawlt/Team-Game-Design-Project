using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float gasDamagePerSecond = 5f;

    [Header("Runtime (read‐only)")]
    [SerializeField]
    private float currentHealth;

    private bool inGas = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (inGas)
        {
            currentHealth -= gasDamagePerSecond * Time.deltaTime;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

            if (currentHealth <= 0f)
                Die();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GasMist"))
            inGas = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("GasMist"))
            inGas = false;
    }

    private void Die()
    {
        Debug.Log("Player has died.");
        // TODO: play death animation, reload scene, disable controls, etc.
    }

    /// <summary>
    /// Public getter in case you want to drive a UI health bar.
    /// </summary>
    public float GetCurrentHealth() => currentHealth;
}
