using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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
    public bool isWaitingForInput;
    public bool skipText; // Flag to control skipping text
    private float typingSpeed = 0.09f;

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
        // If there's already an ongoing dialogue, do not destroy the current canvas
        if (textboxCanvas != null && !isTyping)
        {
            // Hide the previous dialogue
            textboxCanvas.SetActive(false);
        }

        // Instantiate the new textbox canvas if no current canvas exists
        if (textboxCanvas == null || isWaitingForInput)
        {
            canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
            textboxCanvas = Instantiate(TextboxCanvasPrefab, canvas.transform);
            textbox = Instantiate(TextboxPrefab, textboxCanvas.transform);

            characterText = textbox.transform.Find("NameBox/CharacterName").GetComponent<TextMeshProUGUI>();
            dialogueText = textbox.transform.Find("DialogueBox/DialogueText").GetComponent<TextMeshProUGUI>();
        }

        characterText.text = characterName;
        currentDialogue = dialogue;
        currentCharacterIndex = 0;
        dialogueText.text = "";

        textboxCanvas.SetActive(true);

        StartCoroutine(TypeDialogue());
    }

    private IEnumerator TypeDialogue()
    {
        isTyping = true;
        isWaitingForInput = false; // Reset this flag as we start typing

        // Make sure the skipText flag is reset when a new dialogue starts
        skipText = false;
    
        dialogueText.text = "";

        while (currentCharacterIndex < currentDialogue.Length)
        {
            if (skipText)
            {
                dialogueText.text = currentDialogue;  // Skip typing and show the full dialogue
                currentCharacterIndex = currentDialogue.Length;  // Finish typing immediately
                break;
            }

            dialogueText.text += currentDialogue[currentCharacterIndex];  // Add one character at a time
            currentCharacterIndex++;

            // Adjust typing speed to prevent skipping too fast
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false; // Done typing
        //isWaitingForInput = true; // Now we're waiting for the user to press space to move to the next dialogue

    }



    private void Update()
    {
        // If we are waiting for input and the user presses space, process the next line or skip
        if (!isWaitingForInput && Input.GetKeyDown(KeyCode.Space) && !isTyping)
        {
            // Skip to next dialogue if necessary
            textboxCanvas.SetActive(false);
            isWaitingForInput = true;
            
        }

        // If typing, and user presses space, skip the current typing (force full text)
        if (isTyping && Input.GetKeyDown(KeyCode.Space))
        {
            skipText = true;
            isWaitingForInput = false;
        }
    }



    public void DestroyTextboxCanvas()
    {
        if (textboxCanvas != null)
        {
            Destroy(textboxCanvas);
            textboxCanvas = null;
        }
    }
}
