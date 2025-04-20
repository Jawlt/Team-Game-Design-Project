using System.Collections;
using UnityEngine;
using TMPro;

public class NPCDialogue : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [TextArea(2, 5)]
    public string[] dialogueLines;
    public float typeSpeed = 0.03f;

    [Header("UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    private bool isPlayerNearby = false;
    private bool isTalking = false;
    private bool isTyping = false;
    private int currentLine = 0;

    void Update()
    {
        if (isPlayerNearby && isTalking && Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                // Instantly show full line if player click left mouse button mid-typing
                StopAllCoroutines();
                dialogueText.text = dialogueLines[currentLine];
                isTyping = false;
            }
            else
            {
                NextLine();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            StartDialogue();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            EndDialogue();
        }
    }

    private void StartDialogue()
    {
        if (!isTalking && dialogueLines.Length > 0)
        {
            currentLine = 0;
            dialoguePanel.SetActive(true);
            isTalking = true;
            StartCoroutine(TypeLine(dialogueLines[currentLine]));
        }
    }

    private void NextLine()
    {
        currentLine++;
        if (currentLine < dialogueLines.Length)
        {
            StartCoroutine(TypeLine(dialogueLines[currentLine]));
        }
        else
        {
            EndDialogue();
        }
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
    }

    private void EndDialogue()
    {
        StopAllCoroutines();
        dialogueText.text = "";
        dialoguePanel.SetActive(false);
        isTalking = false;
        isTyping = false;
        currentLine = 0;
    }
}
