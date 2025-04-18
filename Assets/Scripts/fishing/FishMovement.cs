using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovement : MonoBehaviour
{
    public float maxLeft = -250f;
    public float maxRight = 250f;
    public float maxSpeed = 250f;
    public float changeFrequency = 0.01f;
    public float targetPosition;
    public bool movingRight = true;

    public void SetDifficulty(FishData fishType)
    {
        switch (fishType.fishDifficulty)
        {
            case 1:
                maxSpeed = 200; //level 1
                return;
            case 2:
                maxSpeed = 300; //level 2
                return;
            case 3:
                maxSpeed = 350; //level 3
                return;
            case 0:
                Debug.LogWarning("No diffculty found for this fish!");
                maxSpeed = 200;
                return;
        }
    }

    void Start()
    {
        targetPosition = Random.Range(maxLeft, maxRight);
    }

    void Update()
    {
        // Move fish towards the target position
        transform.localPosition = Vector3.MoveTowards(
            transform.localPosition,
            new Vector3(targetPosition, transform.localPosition.y, transform.localPosition.z),
            maxSpeed * Time.deltaTime
        );

        // Check if the fish reached the target position
        if (Mathf.Approximately(transform.localPosition.x, targetPosition))
        {
            // Choose a new position
            targetPosition = Random.Range(maxLeft, maxRight);
        }

        // Change direction randomly
        if (Random.value < changeFrequency)
        {
            movingRight = !movingRight;
            targetPosition = movingRight ? maxRight : maxLeft;
        }
    }

}
