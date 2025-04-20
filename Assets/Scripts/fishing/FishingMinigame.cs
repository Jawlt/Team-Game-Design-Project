using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishingMinigame : MonoBehaviour
{
    public RectTransform fishTransform;
    public RectTransform catcherTransform;
    public bool isOverlapping = false;

    public Slider successSlider;
    float successIncrement = 15;
    float failDecrement = 12;
    float successThreshold = 100;
    float failThreshold = -100;
    float successCounter = 0;
    private PlayerExperience playerXP;

    private void Start()
    {
        playerXP = FindObjectOfType<PlayerExperience>();
    }

    private void Update()
    {
        if (CheckOverlapping(fishTransform, catcherTransform))
        {
            isOverlapping = true;
        }
        else
        {
            isOverlapping = false;
        }

        OverlappingCalc();
    }

    private void OverlappingCalc()
    {
        if (isOverlapping)
        {
            float multiplier = playerXP != null ? playerXP.catchSpeedMultiplier : 1f;
            successCounter += successIncrement * multiplier * Time.deltaTime;
        }
        else
        {
            successCounter -= failDecrement * Time.deltaTime;
        }

        // Clamp counter within limits
        successCounter = Mathf.Clamp(successCounter, failThreshold, successThreshold);

        // Update Slider value
        successSlider.value = successCounter;

        // Check if success or fail thresholds reached
        if (successCounter >= successThreshold)
        {
            FishingSystem.Instance.EndMinigame(true);
            Debug.Log("success");
            successCounter = 0;
            successSlider.value = 0;
        }
        else if (successCounter <= failThreshold)
        {
            FishingSystem.Instance.EndMinigame(false);
            Debug.Log("failed");
            successCounter = 0;
            successSlider.value = 0;
        }
    }

    private bool CheckOverlapping(RectTransform rect1, RectTransform rect2)
    {
        Rect r1 = new Rect(rect1.position.x, rect1.position.y, rect1.rect.width, rect1.rect.height);
        Rect r2 = new Rect(rect2.position.x, rect2.position.y, rect2.rect.width, rect2.rect.height);
        return r1.Overlaps(r2);
    }

}
