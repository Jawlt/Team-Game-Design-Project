using UnityEngine;

public class RodPickup : MonoBehaviour
{
    private bool playerInRange = false;
    public GameObject rodPrefab; // Assign the actual rod prefab you want to equip

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            EquipRod();
        }
    }

    private void EquipRod()
    {
        FishingSystem.Instance.EquipRod(rodPrefab); // Calls a method on FishingSystem to equip the rod
        Destroy(gameObject); // Removes the rod from the ground
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Press E to pick up the fishing rod");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
