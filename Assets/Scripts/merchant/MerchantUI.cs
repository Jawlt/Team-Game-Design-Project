using UnityEngine;
using UnityEngine.UI;

public class MerchantUI : MonoBehaviour
{
    public GameObject panel;
    public Transform buyItemParent;
    public GameObject buyButtonPrefab;

    public MerchantItem[] itemsForSale;

    [Header("Interaction Settings")]
    public float interactDistance = 3f;
    public KeyCode interactKey = KeyCode.E;
    public Transform playerCamera;
    public FishDatabase fishDatabase;

    [System.Serializable]
    public class HazardBinding
    {
        public string itemName;              // e.g., "Island1"
        public GameObject hazardToRemove;
    }
    public HazardBinding[] hazardBindings;
    private GameObject GetHazardForItem(string itemName)
    {
        foreach (var binding in hazardBindings)
        {
            if (binding.itemName == itemName)
                return binding.hazardToRemove;
        }
        return null;
    }

    private void Start()
    {
        PopulateBuyUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(interactKey) && PlayerLookingAtMe())
        {
            ToggleShop(true);
        }

        if (panel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleShop(false);
        }
    }

    void PopulateBuyUI()
    {
        foreach (MerchantItem item in itemsForSale)
        {
            // Skip if player already owns this item
            if (ItemAlreadyOwned(item))
                continue;

            GameObject btnObj = Instantiate(buyButtonPrefab, buyItemParent);
            btnObj.transform.GetChild(0).GetComponent<Image>().sprite = item.icon;
            btnObj.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = item.itemName + "\n$ " + item.cost;

            // Store item reference for hiding later
            Button button = btnObj.GetComponent<Button>();
            button.onClick.AddListener(() => BuyAndHide(item, btnObj));
        }
    }

    private bool ItemAlreadyOwned(MerchantItem item)
    {
        switch (item.type)
        {
            case MerchantItem.ItemType.Boat:
                return PlayerData.Instance.hasBoat;
            case MerchantItem.ItemType.IslandUnlock:
                return PlayerData.Instance.unlockedIslands.Contains(item.itemName);
            default:
                return false;
        }
    }

    private void BuyAndHide(MerchantItem item, GameObject buttonObj)
    {
        if (!PlayerData.Instance.CanAfford(item.cost))
        {
            Debug.Log("Not enough cash.");
            return;
        }

        if (ItemAlreadyOwned(item))
        {
            Debug.Log($"{item.itemName} already unlocked.");
            return;
        }

        PlayerData.Instance.SpendCash(item.cost);

        switch (item.type)
        {
            case MerchantItem.ItemType.Boat:
                PlayerData.Instance.hasBoat = true;
                Debug.Log("Boat unlocked!");
                break;

            case MerchantItem.ItemType.IslandUnlock:
                PlayerData.Instance.unlockedIslands.Add(item.itemName);
                Debug.Log(item.itemName + " unlocked!");

                // 🔥 Remove the hazard associated with this island
                GameObject hazard = GetHazardForItem(item.itemName);
                if (hazard != null)
                {
                    Destroy(hazard);
                    Debug.Log($"Removed hazard for {item.itemName}");
                }
                else
                {
                    Debug.LogWarning($"No hazard bound for: {item.itemName}");
                }
                break;
        }

        // Hide the button after successful purchase
        buttonObj.SetActive(false);
    }



    public void Buy(MerchantItem item)
    {
        // Prevent rebuying
        switch (item.type)
        {
            case MerchantItem.ItemType.Boat:
                if (PlayerData.Instance.hasBoat)
                {
                    Debug.Log("You already own a boat.");
                    return;
                }
                break;
            case MerchantItem.ItemType.IslandUnlock:
                if (PlayerData.Instance.unlockedIslands.Contains(item.itemName))
                {
                    Debug.Log($"Island '{item.itemName}' is already unlocked.");
                    return;
                }
                break;
        }

        if (!PlayerData.Instance.CanAfford(item.cost))
        {
            Debug.Log("Not enough cash.");
            return;
        }

        PlayerData.Instance.SpendCash(item.cost);

        switch (item.type)
        {
            case MerchantItem.ItemType.Boat:
                PlayerData.Instance.hasBoat = true;
                Debug.Log("Boat unlocked!");
                break;
            case MerchantItem.ItemType.IslandUnlock:
                PlayerData.Instance.unlockedIslands.Add(item.itemName);
                Debug.Log(item.itemName + " unlocked!");
                break;
        }
    }


    public void SellAll()
    {
        if (InventorySystem.Instance.itemList.Count == 0)
        {
            Debug.Log("Nothing to sell.");
            return;
        }

        int totalEarned = 0;
        foreach (string itemName in InventorySystem.Instance.itemList)
        {
            FishData fish = fishDatabase.GetFishByName(itemName);

            if (fish != null)
            {
                Debug.Log($"Selling {fish.fishName} for ${fish.fishCost}");
                totalEarned += fish.fishCost;
            }
            else
            {
                Debug.LogWarning($"Fish not found in database: {itemName}");
            }
        }

        InventorySystem.Instance.itemList.Clear();

        foreach (GameObject slot in InventorySystem.Instance.slotList)
        {
            if (slot.transform.childCount > 0)
            {
                Destroy(slot.transform.GetChild(0).gameObject);
            }
        }

        PlayerData.Instance.AddCash(totalEarned);
        Debug.Log("Sold all for $" + totalEarned);
    }


    private void ToggleShop(bool open)
    {
        panel.SetActive(open);

        Time.timeScale = open ? 0f : 1f;
        Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = open;
    }

    private bool PlayerLookingAtMe()
    {
        if (playerCamera == null) return false;

        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            return hit.transform == transform;
        }

        return false;
    }
}
