using UnityEngine;

public class ToolController : MonoBehaviour
{
    [Header("Tool Holder (e.g. includes Fishing Rod)")]
    public GameObject toolHolder;  // Assign the ToolHolder GameObject that contains the fishing rod

    private bool isToolActive = false;

    private void Start()
    {
        if (toolHolder != null)
        {
            toolHolder.SetActive(false); // Start hidden
            isToolActive = false;
        }
        else
        {
            Debug.LogError("ToolHolder GameObject not assigned in PlayerToolController.");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleToolHolder();
        }
    }

    private void ToggleToolHolder()
    {
        if (toolHolder == null) return;

        isToolActive = !isToolActive;
        toolHolder.SetActive(isToolActive);

        Debug.Log(isToolActive ? "Tool equipped." : "Tool hidden.");
    }
}
