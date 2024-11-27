using System.Collections;
using System.Linq;
using UnityEngine;
using TMPro;
using System.IO;
using MoonSharp.Interpreter;
using System;
public class DialogueSys : MonoBehaviour
{
    public static DialogueSys Instance { get; private set; }
    public event Action OnDialogueFinished; // Event to notify when dialogue is finished

    private TMP_Text characterText;
    private TMP_Text dialogueText;
    public GameObject TextboxPrefab;
    public Canvas TextboxCanvasPrefab;
    public GameObject orderNotePrefab;

    public string luaFileName = "TutorialDialogue";
    private Script _luaScript;

    private string _currentNode;
    private int _lineIndex;
    private bool _isTyping; // Tracks whether text is currently being typed
    private bool _isDialogueFinished;
    private string _characterName;

    private string _orderDescription = "";
    private string _customerOrder = "";

    private GameObject _currentOrderNote; // Stores the current active order note

    private GameObject textbox;
    private Canvas canvas;
    private Canvas textboxCanvas;

    private float typingSpeed = 0.05f; // Speed for typing effect
    private bool _skipText = false;

    private void Awake()
    {
        // Singleton implementation
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optionally keep the instance across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
            return;
        }
    }

    void Start()
    {
        LoadLuaScript(luaFileName);

        if (_luaScript == null)
        {
            Debug.LogError("Lua Script is not initialized!");
        }
        _lineIndex = 0;
        _isDialogueFinished = false;
    }

    private void LoadLuaScript(string fileName)
    {
        _luaScript = new Script();
        string filePath = Path.Combine(Application.dataPath, "Scripts", "Dialogue", fileName + ".lua");

        if (File.Exists(filePath))
        {
            string luaCode = File.ReadAllText(filePath);
            _luaScript.DoString(luaCode);
        }
        else
        {
            Debug.LogError("Lua file not found: " + filePath);
            return;
        }
    }

    public void StartDialogue(string node)
    {
        _currentNode = node;
        _lineIndex = 0;
        _isDialogueFinished = false;

        // Destroy the existing textbox if it's already active
        if (textbox != null)
        {
            Destroy(textbox);
        }

        if (textboxCanvas != null)
        {
            Destroy(textboxCanvas.gameObject);
        }

        // Create the new canvas and textbox
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        textboxCanvas = Instantiate(TextboxCanvasPrefab, canvas.transform);
        textbox = Instantiate(TextboxPrefab, textboxCanvas.transform);

        characterText = textbox.transform.Find("NameBox/CharacterName").GetComponent<TextMeshProUGUI>();
        dialogueText = textbox.transform.Find("DialogueBox/DialogueText").GetComponent<TextMeshProUGUI>();

        DisplayDialogueNode(_currentNode);
    }

    private void DisplayDialogueNode(string node)
    {
        if (_isDialogueFinished) return;

        DynValue luaNode = _luaScript.Call(_luaScript.Globals["GetDialogueNode"], node);

        if (luaNode == null)
        {
            Debug.LogError("The Lua function 'GetDialogueNode' returned null.");
            return;
        }

        if (luaNode.Type == DataType.Table)
        {
            DynValue characterName = luaNode.Table.Get("character");
            if (characterName.Type == DataType.String)
            {
                _characterName = characterName.String;
                characterText.text = _characterName;
            }

            DynValue textArray = luaNode.Table.Get("text");
            if (textArray.Type == DataType.Table && _lineIndex < textArray.Table.Length)
            {
                var dialogueLine = textArray.Table.Values.ElementAt(_lineIndex).Table;
                _orderDescription = dialogueLine.Get("orderdescription").String;
                _customerOrder = dialogueLine.Get("customerorder").String;

                // Start the typing effect for the dialogue line
                string fullLine = dialogueLine.Get("line").String;

                StopAllCoroutines(); // Stop any ongoing typing
                _skipText = false; // Reset skip flag
                StartCoroutine(TypeDialogue(fullLine)); // Start typing the new line
            }

            // When lineIndex exceeds the available dialogue lines, set dialogue finished and destroy the textbox
            if (_lineIndex >= textArray.Table.Length)
            {
                _isDialogueFinished = true;
                if (!string.IsNullOrEmpty(_orderDescription) && !string.IsNullOrEmpty(_customerOrder))
                {
                    CreateOrderNote(); // Call CreateOrderNote if these values exist
                }
                DestroyTextbox();
                OnDialogueFinished?.Invoke(); // Trigger the event when dialogue finishes
            }
        }
    }

    private IEnumerator TypeDialogue(string fullLine)
    {
        _isTyping = true;
        dialogueText.text = "";

        foreach (char letter in fullLine)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);

            // If the spacebar is pressed, break out and show the full text
            if (_skipText)
            {
                dialogueText.text = fullLine; // Show the full line immediately
                break;
            }
        }

        _isTyping = false;
        _skipText = false; // Reset skipText flag after the line is done
    }
    
    public void NextLine()
    {
        if (_isTyping)
        {
            // If typing, immediately finish typing and show the full line
            _skipText = true; // Skip current typing effect
            dialogueText.text = dialogueText.text; // Ensure the full text is displayed
            _isTyping = false;
        }
        else if (!_isDialogueFinished)
        {
            // Move to the next line
            _lineIndex++;
            DisplayDialogueNode(_currentNode);
        }
    }

    private void CreateOrderNote()
    {
        if (_currentOrderNote != null)
        {
            Destroy(_currentOrderNote);
        }

        // Instantiate a new order note
        _currentOrderNote = Instantiate(orderNotePrefab, canvas.transform);
        TMP_Text characterNameText = _currentOrderNote.transform.Find("CharacterNameText")?.GetComponent<TMP_Text>();
        TMP_Text orderDescriptionText = _currentOrderNote.transform.Find("OrderDescriptionText")?.GetComponent<TMP_Text>();

        if (characterNameText != null && orderDescriptionText != null)
        {
            characterNameText.text = _characterName;
            orderDescriptionText.text = _orderDescription;

            // Find the OrderManager in the scene and save the order
            OrderManager orderManager = FindObjectOfType<OrderManager>();
            if (orderManager != null)
            {
                orderManager.StoreOrder(_characterName, _orderDescription, _customerOrder);
            }
            else
            {
                Debug.LogError("OrderManager is not found in the scene.");
            }
        }
        else
        {
            Debug.LogError("Order note prefab is missing text components.");
        }
    }

    private void DestroyTextbox()
    {
        if (textbox != null)
        {
            Destroy(textbox);
        }

        // Reset dialogue state
        _lineIndex = 0;
        _isDialogueFinished = false;
    }

    void OnGUI()
    {
        // Handle spacebar press in OnGUI
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space)
        {
            if (_isTyping)
            {
                // Skip the typing effect if currently typing
                _skipText = true;
            }
            else
            {
                // Otherwise, go to the next line
                NextLine();
            }
        }
    }
}
