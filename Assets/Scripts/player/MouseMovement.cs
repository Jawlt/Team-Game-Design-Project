using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{

    public float mouseSensitivity = 100f;

    float xRotation = 0f;
    float yRotation = 0f;

    void Start()
    {
        //Locks cursor to middle of screen and makes it invisible
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (InventorySystem.Instance.isOpen == false)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            //control rotation on x axis (Look up and down)
            xRotation -= mouseY;

            //we clamp the rotation so we cant over-rotate 
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            //control rotation on y axis (Look up and down)
            yRotation += mouseX;

            //applies both roations
            transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
    }
}