using UnityEngine;
using UnityEngine.UI;

public class MerchantUI : MonoBehaviour
{
    public GameObject panel;
    public Transform buyItemParent;
    public GameObject buyButtonPrefab;

    public MerchantItem[] itemsForSale;

    [Header("Interaction Settings")]
    public KeyCode interactKey = KeyCode.E;
    public FishDatabase fishDatabase;

    [Header("Unlockable Objects")]
    public GameObject boatObject; // Assign in inspector

    [System.Serializable]
    public class HazardBinding
    {
        public string itemName;
        public GameObject hazardToRemove;
    }
    public HazardBinding[] hazardBindings;

    private bool playerNearby = false;

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
        if (playerNearby && Input.GetKeyDown(interactKey))
        {
            ToggleShop(!panel.activeSelf); // Toggle open/close
        }

        if (panel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleShop(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            ToggleShop(false); // close panel on exit
        }
    }

    void PopulateBuyUI()
    {
        foreach (MerchantItem item in itemsForSale)
        {
            if (ItemAlreadyOwned(item)) continue;

            GameObject btnObj = Instantiate(buyButtonPrefab, buyItemParent);
            btnObj.transform.GetChild(0).GetComponent<Image>().sprite = item.icon;
            btnObj.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = item.itemName + "\n$ " + item.cost;

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
        if (!PlayerData.Instance.CanAfford(item.cost)) return;
        if (ItemAlreadyOwned(item)) return;

        PlayerData.Instance.SpendCash(item.cost);

        switch (item.type)
        {
            case MerchantItem.ItemType.Boat:
                PlayerData.Instance.hasBoat = true;
                if (boatObject) boatObject.SetActive(true);
                break;

            case MerchantItem.ItemType.IslandUnlock:
                PlayerData.Instance.unlockedIslands.Add(item.itemName);
                GameObject hazard = GetHazardForItem(item.itemName);
                if (hazard) Destroy(hazard);
                break;
        }

        buttonObj.SetActive(false);
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
            if (fish != null) totalEarned += fish.fishCost;
        }

        InventorySystem.Instance.itemList.Clear();

        foreach (GameObject slot in InventorySystem.Instance.slotList)
        {
            if (slot.transform.childCount > 0)
                Destroy(slot.transform.GetChild(0).gameObject);
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
}
