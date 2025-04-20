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
    public GameObject endof_of_rope;
    public GameObject start_of_rope;
    public GameObject start_of_rod;

    Transform baitPosition;
    GameObject baitReference;

    [Header("Audio Clips")]
    public AudioClip castSound;
    public AudioClip biteSound;
    private AudioSource audioSource;

    private void OnEnable()
    {
        FishingSystem.OnEndFishing += HandleFishingEnd;

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

    private void OnDestroy()
    {
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

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
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

                    if (isCasted && Input.GetMouseButtonDown(1) && FishingSystem.Instance.isThereABite)
                    {
                        PullRod();
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
    }

    IEnumerator CastRod(Vector3 targetPosition, Watersource watersource)
    {
        isCasted = true;
        animator.SetTrigger("Cast");

        if (castSound != null)
            audioSource.PlayOneShot(castSound);

        yield return new WaitForSeconds(1f);

        GameObject instantiatedBait = Instantiate(baitPrefab);
        instantiatedBait.transform.position = targetPosition;

        baitPosition = instantiatedBait.transform;
        baitReference = instantiatedBait;

        FishingSystem.Instance.StartFishing(watersource);
    }

    private void PullRod()
    {
        animator.SetTrigger("Pull");
        isCasted = false;
        isPulling = true;

        if (biteSound != null)
            audioSource.PlayOneShot(biteSound);

        FishingSystem.Instance.SetHasPulled();
    }
}
