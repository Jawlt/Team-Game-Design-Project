using UnityEngine;

public class BoatEntry : MonoBehaviour
{
    // Assign these in the Inspector:
    public GameObject player;            // The Player GameObject
    public MouseMovement playerMouse;    // The player's MouseMovement component
    public PlayerMovement playerMovement; // The player's PlayerMovement component
    public GameObject boat;              // The Boat GameObject (with the WaterBoat script)

    private bool isPlayerNearby = false;

    // Detect when the player enters the trigger area
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            // Optionally, display a UI prompt (e.g., "Press E to enter the boat")
        }
    }

    // Detect when the player leaves the trigger area
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            // Optionally, hide the UI prompt
        }
    }

    // Check for input when the player is in the trigger
    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            EnterBoat();
        }
    }

    void EnterBoat()
    {
        // Disable player's control scripts
        if (playerMouse != null)
            playerMouse.enabled = false;
        if (playerMovement != null)
            playerMovement.enabled = false;

        // Disable the CharacterController to prevent it from interfering with boat physics
        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
            controller.enabled = false;

        // Enable boat control
        WaterBoat waterBoat = boat.GetComponent<WaterBoat>();
        if (waterBoat != null)
            waterBoat.enabled = true;

        // Re-parent the player to the boat so they move together.
        player.transform.SetParent(boat.transform);

        Debug.Log("Player has entered the boat and is now parented to the boat.");
    }
}
