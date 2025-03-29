using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatcherMovement : MonoBehaviour
{
    public float maxLeft = -250f;
    public float maxRight = 250f;
    public float maxSpeed = 250f;

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");

        // If there is an input
        if (moveInput != 0)
        {
            MoveCatcher(moveInput);
        }
    }

    private void MoveCatcher(float moveInput)
    {
        Vector3 movement = Vector3.right * moveInput * maxSpeed * Time.deltaTime;

        Vector3 newPosition = transform.localPosition + movement;
        newPosition.x = Mathf.Clamp(newPosition.x, maxLeft, maxRight);

        transform.localPosition = newPosition;
    }


}
