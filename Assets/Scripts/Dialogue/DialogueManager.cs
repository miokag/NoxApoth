using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI dialogueText;

    private Queue<string> dialogueLines; // Store the dialogue lines
    private bool isTyping = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        dialogueLines = new Queue<string>(); // Initialize the queue
    }

    // This function starts the dialogue by passing the character's name and a list of dialogue lines
    public void StartDialogue(string characterName, List<string> lines)
    {
        // Clear any existing dialogue
        dialogueLines.Clear();

        // Set the character name
        characterNameText.text = characterName;

        // Add all dialogue lines to the queue
        foreach (string line in lines)
        {
            dialogueLines.Enqueue(line);
        }

        // Show the first line of dialogue
        ShowNextLine();
    }

    // This function is called when the player presses the spacebar
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isTyping && dialogueLines.Count > 0)
        {
            ShowNextLine();
        }
    }

    // This function handles the display of the next line of dialogue
    private void ShowNextLine()
    {
        if (dialogueLines.Count == 0)
        {
            EndDialogue();
            return;
        }

        // Get the next line of dialogue
        string line = dialogueLines.Dequeue();

        // Start typing animation (optional, for now we just show it directly)
        StartCoroutine(TypeLine(line));
    }

    // Type the line one character at a time (optional animation)
    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = ""; // Clear existing text

        foreach (char letter in line)
        {
            dialogueText.text += letter; // Add one letter at a time
            yield return null; // Wait until the next frame
        }

        isTyping = false; // Finish typing
    }

    // This function ends the dialogue and hides the textbox
    private void EndDialogue()
    {
        // You can hide the textbox here, or trigger any other end-of-dialogue behavior
        Debug.Log("Dialogue ended.");
    }
}
