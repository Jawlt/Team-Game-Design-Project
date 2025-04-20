using UnityEngine;
using UnityEngine.UI;            // for Slider / Text
using TMPro;                     // if you’re using TextMeshPro

public class HealthUI : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public PlayerHealth playerHealth;    // drag your Player GameObject here
    public Slider healthBar;             // optional, for a bar
    public Text healthText;              // for standard UI Text
    public TextMeshProUGUI healthTMP;    // for TextMeshPro

    void Update()
    {
        if (playerHealth == null) return;

        float hp = playerHealth.GetCurrentHealth();
        float max = playerHealth.maxHealth;

        // 1) Update a slider
        if (healthBar != null)
        {
            healthBar.maxValue = max;
            healthBar.value = hp;
        }

        // 2) Update numeric text
        string display = $"{Mathf.CeilToInt(hp)}/{Mathf.CeilToInt(max)} HP";
        if (healthText != null)
            healthText.text = display;
        if (healthTMP != null)
            healthTMP.text = display;
    }
}
