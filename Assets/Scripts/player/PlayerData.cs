using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;

    public int cash = 0;
    public bool hasBoat = false;
    public List<string> unlockedIslands = new List<string>();

    [Header("UI")]
    public TextMeshProUGUI cashTextUI;
    public Color gainColor = Color.green;
    public Color lossColor = Color.red;
    public float flashDuration = 0.5f;

    [Header("Sound (Optional)")]
    public AudioSource audioSource;
    public AudioClip gainSound;
    public AudioClip spendSound;

    private Color defaultColor;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        if (cashTextUI != null)
        {
            defaultColor = cashTextUI.color;
            UpdateCashUI();
        }
    }

    public bool CanAfford(int amount) => cash >= amount;

    public void SpendCash(int amount)
    {
        cash -= amount;
        Debug.Log("Cash left: " + cash);
        UpdateCashUI(true);
        PlaySound(spendSound);
    }

    public void AddCash(int amount)
    {
        cash += amount;
        Debug.Log("Cash now: " + cash);
        UpdateCashUI(false);
        PlaySound(gainSound);
    }

    private void UpdateCashUI(bool isSpending = false)
    {
        if (cashTextUI == null) return;

        cashTextUI.text = $"Cash: {cash:N0}";
        StopAllCoroutines();
        StartCoroutine(FlashColor(isSpending));
    }

    private IEnumerator FlashColor(bool isSpending)
    {
        cashTextUI.color = isSpending ? lossColor : gainColor;
        yield return new WaitForSeconds(flashDuration);
        cashTextUI.color = defaultColor;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
