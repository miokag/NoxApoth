using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public GameObject TextboxCanvasPrefab;
    public GameObject TextboxPrefab;
    private Canvas canvas;
    private GameObject textboxCanvas;
    private GameObject textbox;
    private TextMeshProUGUI characterText;
    private TextMeshProUGUI dialogueText;
    private string currentDialogue;
    private int currentCharacterIndex;
    private bool isTyping;
    private bool isWaitingForInput;
    private float typingSpeed = 0.05f;
    
    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);  // Destroy duplicate if any
        }
    }

    private void Start()
    {
        // Find canvas and instantiate textbox prefab
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        textboxCanvas = Instantiate(TextboxCanvasPrefab, canvas.transform);
        textbox = Instantiate(TextboxPrefab, textboxCanvas.transform);

        characterText = textbox.transform.Find("NameBox/CharacterName").GetComponent<TextMeshProUGUI>();
        dialogueText = textbox.transform.Find("DialogueBox/DialogueText").GetComponent<TextMeshProUGUI>();

        // Hide textbox initially
        textboxCanvas.SetActive(false);
    }

    // Show dialogue with a character name and text
    public void ShowDialogue(string characterName, string dialogue)
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        // Ensure textboxCanvas is not destroyed or null before trying to activate it
        if (textboxCanvas == null)
        {
            // If textboxCanvas is destroyed, recreate it
            textboxCanvas = Instantiate(TextboxCanvasPrefab, canvas.transform);
            textbox = Instantiate(TextboxPrefab, textboxCanvas.transform);

            characterText = textbox.transform.Find("NameBox/CharacterName").GetComponent<TextMeshProUGUI>();
            dialogueText = textbox.transform.Find("DialogueBox/DialogueText").GetComponent<TextMeshProUGUI>();
        }

        characterText.text = characterName;
        currentDialogue = dialogue;
        currentCharacterIndex = 0;
        dialogueText.text = "";

        // Show the textbox
        textboxCanvas.SetActive(true);

        // Start the typing coroutine
        StartCoroutine(TypeDialogue());
    }

    private IEnumerator TypeDialogue()
    {
        isTyping = true;
        isWaitingForInput = false;

        while (currentCharacterIndex < currentDialogue.Length)
        {
            // Add one character at a time
            dialogueText.text += currentDialogue[currentCharacterIndex];
            currentCharacterIndex++;

            // Wait for a short time between characters (typing effect)
            yield return new WaitForSeconds(typingSpeed);
        }

        // Dialogue finished typing, now waiting for player input
        isTyping = false;
        isWaitingForInput = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (isWaitingForInput && Input.GetKeyDown(KeyCode.Space))
        {
            // If dialogue is finished, proceed to next step
            if (!isTyping)
            {
                textboxCanvas.SetActive(false); // Hide the textbox
                // Trigger any post-dialogue actions here
            }
        }
    }
}
