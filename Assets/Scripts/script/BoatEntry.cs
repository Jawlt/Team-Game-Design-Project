using UnityEngine;

public class BoatEntry : MonoBehaviour
{
    // Assign these in the Inspector
    public GameObject player;         // Reference to the Player GameObject
    public MouseMovement playerMouse; // Reference to the Player's MouseMovement script
    public GameObject boat;           // Reference to the Boat GameObject

    private bool isPlayerNearby = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            // Optionally display a UI prompt (e.g., "Press E to enter the boat")
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            // Hide the UI prompt if displayed
        }
    }

    void Update()
    {
        // When the player is in range and presses E, switch control
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            EnterBoat();
        }
    }

    void EnterBoat()
    {
        // Disable player's mouse movement (or other player control scripts)
        if (playerMouse != null)
            playerMouse.enabled = false;

        // Enable the boat's control script (WaterBoat)
        WaterBoat waterBoat = boat.GetComponent<WaterBoat>();
        if (waterBoat != null)
            waterBoat.enabled = true;

        Debug.Log("Player has entered the boat! Boat control is now active.");

        // Optionally, you can also parent the camera to the boat or reassign its target here.
    }
}
