using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Watersource
{
    island1, island2, island3
}

public class FishingSystem : MonoBehaviour
{
    public static FishingSystem Instance { get; set; }

    [SerializeField] private GameObject fishingRodPrefab;
    [SerializeField] private Transform toolHolder;
    private GameObject currentRodInstance;

    public List<FishData> island1FishList;
    public List<FishData> island2FishList;
    public List<FishData> island3FishList;

    public bool isThereABite;
    bool hasPulled;

    public static event Action OnEndFishing;

    public GameObject minigame;
    FishData fishType;
    public FishMovement fishMovement;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        if (toolHolder.childCount > 0)
        {
            currentRodInstance = toolHolder.GetChild(0).gameObject;
            Debug.Log("Existing rod found at start.");
        }
        else
        {
            Debug.Log("No fishing rod found at start.");
        }
    }

    internal void StartFishing(Watersource watersource)
    {
        StartCoroutine(FishingCoroutine(watersource));
    }

    IEnumerator FishingCoroutine(Watersource watersource)
    {
        yield return new WaitForSeconds(3f);

        FishData fish = CalculateBite(watersource);

        if (fish.fishName == "NoBite")
        {
            Debug.LogWarning("No Fish Caught");
            EndFishing();
        }
        else
        {
            Debug.LogWarning(fish.fishName + " is biting");
            StartCoroutine(StartFishStruggle(fish));
        }
    }

    IEnumerator StartFishStruggle(FishData fish)
    {
        isThereABite = true;

        while (!hasPulled)
        {
            yield return null;
        }

        Debug.LogWarning("Start MiniGame");
        fishType = fish;
        StartMinigame();
    }

    private void StartMinigame()
    {
        minigame.SetActive(true);
        fishMovement.SetDifficulty(fishType);
    }

    public void SetHasPulled()
    {
        hasPulled = true;
    }

    private void EndFishing()
    {
        isThereABite = false;
        hasPulled = false;
        fishType = null;

        OnEndFishing?.Invoke();

        if (currentRodInstance != null)
        {
            Destroy(currentRodInstance);
            Debug.Log("Old fishing rod destroyed.");
        }

        currentRodInstance = Instantiate(fishingRodPrefab, toolHolder);
    }

    public void EquipRod(GameObject rodPrefab)
    {
        if (currentRodInstance != null)
        {
            Destroy(currentRodInstance);
        }

        currentRodInstance = Instantiate(rodPrefab, toolHolder);
        Debug.Log("Fishing rod equipped.");
    }

    private FishData CalculateBite(Watersource watersource)
    {
        List<FishData> availableFish = GetAvailableFish(watersource);

        float totalProbability = 0f;
        foreach (FishData fish in availableFish)
        {
            totalProbability += fish.probability;
        }

        int randomValue = UnityEngine.Random.Range(0, Mathf.FloorToInt(totalProbability) + 1);
        Debug.Log("Random value generated: " + randomValue);

        float cumulativeProbability = 0f;
        foreach (FishData fish in availableFish)
        {
            cumulativeProbability += fish.probability;
            if (randomValue <= cumulativeProbability)
            {
                return fish;
            }
        }

        return null; // shouldn't happen
    }

    private List<FishData> GetAvailableFish(Watersource watersource)
    {
        switch (watersource)
        {
            case Watersource.island1:
                return island1FishList;
            case Watersource.island2:
                return island2FishList;
            case Watersource.island3:
                return island3FishList;
            default:
                Debug.LogWarning("Invalid water source.");
                return new List<FishData>();
        }
    }

    internal void EndMinigame(bool success)
    {
        minigame.gameObject.SetActive(false);

        if (success)
        {
            InventorySystem.Instance.AddToInventory(fishType.fishName);
            Debug.Log("Fish Caught");

            PlayerExperience playerXP = FindObjectOfType<PlayerExperience>();
            if (playerXP != null)
            {
                playerXP.GainXP(fishType.baseXP);

                if (playerXP.cashBonusPerCatch > 0)
                {
                    PlayerData.Instance.AddCash(playerXP.cashBonusPerCatch);
                    Debug.Log($"Bonus Cash: +${playerXP.cashBonusPerCatch}");
                }
            }

            EndFishing();
        }
        else
        {
            Debug.Log("Fish Escaped");
            EndFishing();
        }
    }
}
