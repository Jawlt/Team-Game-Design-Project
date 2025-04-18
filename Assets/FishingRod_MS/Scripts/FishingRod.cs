using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FishingRod : MonoBehaviour
{
    public bool isEquipped;
    public bool isFishingAvailable;

    public bool isCasted;
    public bool isPulling;

    Animator animator;
    public GameObject baitPrefab;
    public GameObject endof_of_rope;  // --- > IF USING ROPE
    public GameObject start_of_rope;   // --- > IF USING ROPE   
    public GameObject start_of_rod;    // --- > IF USING ROPE   

    Transform baitPosition;
    GameObject baitReference;

    private void OnEnable()
    {
        // Subscribe to event
        FishingSystem.OnEndFishing += HandleFishingEnd;

        // Reset state
        isEquipped = true;
        isCasted = false;
        isPulling = false;

        if (baitPosition != null)
        {
            Destroy(baitPosition.gameObject);
            baitPosition = null;
            Debug.Log("Old bait destroyed on enable.");
        }

        Debug.Log("FishingRod enabled → state reset.");
    }

    void OnDestroy()
    {
        // Unsubscribe to event
        FishingSystem.OnEndFishing -= HandleFishingEnd;
    }

    private void HandleFishingEnd()
    {
        Destroy(baitReference);
    }



    private void Start()
    {
        animator = GetComponent<Animator>();
        isEquipped = true;
    }

    private void OnDisable()
    {
        isEquipped = false;

        if (baitPosition != null)
        {
            Destroy(baitPosition.gameObject);
            baitPosition = null;
            Debug.Log("Old bait destroyed on disable.");
        }

        Debug.Log("FishingRod disabled.");
    }

    void Update()
    {
        if (isEquipped)
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {

                if (hit.collider.CompareTag("FishingArea"))
                {
                    isFishingAvailable = true;

                    if (Input.GetMouseButtonDown(0) && !isCasted && !isPulling)
                    {
                        Watersource watersource = hit.collider.gameObject.GetComponent<FishingArea>().waterSource;
                        StartCoroutine(CastRod(hit.point, watersource));
                    }
                }
                else
                {
                    isFishingAvailable = false;
                }

            }
            else
            {
                isFishingAvailable = false;
            }
        }

        // --- > IF USING ROPE < --- //
        if (isCasted || isPulling)
        {
            if (start_of_rope != null && start_of_rod != null && endof_of_rope != null)
            {
                start_of_rope.transform.position = start_of_rod.transform.position;

                if (baitPosition != null)
                {
                    endof_of_rope.transform.position = baitPosition.position;
                }
            }
            else
            {
                Debug.Log("MISSING ROPE REFERENCES");
            }
        }

        if (isCasted && Input.GetMouseButtonDown(1) && FishingSystem.Instance.isThereABite) // player can only pull when there is a bite
        {
            PullRod();
        }
    }


    IEnumerator CastRod(Vector3 targetPosition, Watersource watersource)
    {
        isCasted = true;
        animator.SetTrigger("Cast");

        // Create a delay between the animation and when the bait appears in the water
        yield return new WaitForSeconds(1f);

        GameObject instantiatedBait = Instantiate(baitPrefab);
        instantiatedBait.transform.position = targetPosition;

        baitPosition = instantiatedBait.transform;
        baitReference = instantiatedBait;

        // ---- > Start Fish Bite Logic
        FishingSystem.Instance.StartFishing(watersource);
    }

    private void PullRod()
    {
        animator.SetTrigger("Pull");
        isCasted = false;
        isPulling = true;

        // ---- > Start Minigame Logic
        FishingSystem.Instance.SetHasPulled();
    }
}
