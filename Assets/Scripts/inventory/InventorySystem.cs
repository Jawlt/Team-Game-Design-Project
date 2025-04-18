using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; set; }
    public GameObject inventoryScreenUI;
    public GameObject crosshairUI;
    public List<GameObject> slotList = new List<GameObject>();
    public List<string> itemList = new List<string>();
    private GameObject itemToAdd;
    private GameObject slotToEquip;
    public bool isOpen;
    //public bool isFull;

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

    void Start()
    {
        isOpen = false;
        //isFull = false;
        PopulateSlotList();
    }

    private void PopulateSlotList()
    {
        foreach (Transform child in inventoryScreenUI.transform)
        {
            if (child.CompareTag("Slot"))
            {
                slotList.Add(child.gameObject);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
    }

    private void ToggleInventory()
    {
        isOpen = !isOpen;
        inventoryScreenUI.SetActive(isOpen);
        crosshairUI.SetActive(!isOpen);
        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void AddToInventory(string itemName)
    {
        if (CheckIfFull())
        {
            //isFull = true;
            Debug.Log("Inventory is full");
        }
        else
        {
            slotToEquip = FindNextEmptySlot();
            itemToAdd = (GameObject)Instantiate(Resources.Load<GameObject>(itemName), slotToEquip.transform.position, slotToEquip.transform.rotation);
            itemToAdd.transform.SetParent(slotToEquip.transform);

            itemList.Add(itemName);
        }
    }

    public bool CheckIfFull()
    {
        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount == 0)
                return false;
        }
        return true;
    }

    private GameObject FindNextEmptySlot()
    {
        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount == 0)
            {
                return slot;
            }
        }
        return null;
    }
}
