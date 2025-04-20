using UnityEngine;
using UnityEngine.UI;

public class PlayerExperience : MonoBehaviour
{
    public int currentXP = 0;
    public int currentLevel = 1;
    public int xpToNextLevel = 100;
    public float xpMultiplier = 1f;
    public int cashBonusPerCatch = 0;

    public GameObject levelUpPanel;
    public Button catchSpeedButton;
    public Button xpBoostButton;
    public Button extraCashButton;

    public float catchSpeedMultiplier = 1f; // You can use this in the fishing system

    [Header("UI")]
    public Slider xpBar;

    private void Start()
    {
        levelUpPanel.SetActive(false);

        catchSpeedButton.onClick.AddListener(() => SelectUpgrade(UpgradeType.CatchSpeed));
        xpBoostButton.onClick.AddListener(() => SelectUpgrade(UpgradeType.XPBoost));
        extraCashButton.onClick.AddListener(() => SelectUpgrade(UpgradeType.ExtraCash));
    }

    public void GainXP(int amount)
    {
        int gained = Mathf.RoundToInt(amount * xpMultiplier);
        currentXP += gained;
        Debug.Log($"Gained {gained} XP. Total: {currentXP}/{xpToNextLevel}");

        UpdateXPBar();

        if (currentXP >= xpToNextLevel)
            LevelUp();
    }

    private void UpdateXPBar()
    {
        if (xpBar != null)
        {
            xpBar.maxValue = xpToNextLevel;
            xpBar.value = currentXP;
        }
    }


    private void LevelUp()
    {
        currentLevel++;
        currentXP -= xpToNextLevel;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.5f);

        Debug.Log($"Leveled up to {currentLevel}!");
        levelUpPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }

    private enum UpgradeType { CatchSpeed, XPBoost, ExtraCash }

    private void SelectUpgrade(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.CatchSpeed:
                catchSpeedMultiplier *= 1.15f;
                break;
            case UpgradeType.XPBoost:
                xpMultiplier += 0.25f;
                break;
            case UpgradeType.ExtraCash:
                cashBonusPerCatch += 5;
                break;
        }

        Debug.Log($"Selected upgrade: {type}");
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
