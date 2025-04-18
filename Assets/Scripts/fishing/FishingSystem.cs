using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Watersource
{
    Lake, River, Ocean, island1, island2, island3
}
public class FishingSystem : MonoBehaviour
{
    public static FishingSystem Instance { get; set; }
    [SerializeField] private GameObject fishingRodPrefab; // Assign your rod prefab in Inspector
    [SerializeField] private Transform toolHolder;         // Assign your ToolHolder (parent object
    private GameObject currentRodInstance;
    public List<FishData> lakeFishList;
    public List<FishData> riverFishList;
    public List<FishData> oceanFishList;

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

        // waits till player has pulled rod
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

        // Destroy current rod if it exists
        if (currentRodInstance != null)
        {
            Destroy(currentRodInstance);
            Debug.Log("Old fishing rod destroyed.");
        }
        currentRodInstance = Instantiate(fishingRodPrefab, toolHolder);
    }

    private FishData CalculateBite(Watersource watersource)
    {
        List<FishData> availableFish = GetAvailableFish(watersource);

        // Calculate total probability
        float totalProbability = 0f;
        foreach (FishData fish in availableFish)
        {
            totalProbability += fish.probability;
        }

        // Generate random number between 0 and total probability
        int randomValue = UnityEngine.Random.Range(0, Mathf.FloorToInt(totalProbability) + 1);
        Debug.Log("Random value generated: " + randomValue);

        // Loop through the fish and check if the random number falls into their probability
        float cumulativeProbability = 0f;
        foreach (FishData fish in availableFish)
        {
            cumulativeProbability += fish.probability;
            if (randomValue <= cumulativeProbability)
            {
                // This fish is biting
                return fish;
            }
        }
        return null; // shouldn't happen
    }

    private List<FishData> GetAvailableFish(Watersource watersource)
    {
        switch (watersource)
        {
            case Watersource.Lake:
                return lakeFishList;
            case Watersource.River:
                return riverFishList;
            case Watersource.Ocean:
                return oceanFishList;
            default:
                return new List<FishData>(); // safer than returning null
        }
    }

    internal void EndMinigame(bool success)
    {
        minigame.gameObject.SetActive(false);

        if (success)
        {
            InventorySystem.Instance.AddToInventory(fishType.fishName);
            Debug.Log("Fish Caught");
            EndFishing();
        }
        else
        {
            Debug.Log("Fish Escaped");
            EndFishing();
        }
    }
}
