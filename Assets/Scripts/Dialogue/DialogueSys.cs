using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoonSharp.Interpreter;
using TMPro;
using System.IO;
using System.Linq;
using System;

public class DialogueSys : MonoBehaviour
{
    public event Action OnDialogueFinished;  // Event to notify when dialogue is finished

    private TMP_Text characterText;
    private TMP_Text dialogueText;
    public GameObject TextboxPrefab;
    public Canvas TextboxCanvasPrefab;
    public GameObject orderNotePrefab;

    public string luaFileName = "TutorialDialogue";
    private Script _luaScript;

    private string _currentNode;
    private int _lineIndex;

    private bool _isDialogueFinished;
    private string _characterName;
    
    private string _orderDescription = "";
    private string _customerOrder = "";

    private GameObject _currentOrderNote; // Stores the current active order note


    private GameObject textbox;
    private Canvas canvas;
    private Canvas textboxCanvas;

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

        if(textboxCanvas != null)
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
                dialogueText.text = dialogueLine.Get("line").String;
                _orderDescription = dialogueLine.Get("orderdescription").String;
                _customerOrder = dialogueLine.Get("customerorder").String;
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
                OnDialogueFinished?.Invoke();  // Trigger the event when dialogue finishes
            }
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

    public void NextLine()
    {
        if (!_isDialogueFinished)
        {
            _lineIndex++;
            DisplayDialogueNode(_currentNode);
        }
    }

    // This method handles destroying the textbox once the last line is finished
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

    // Option 1: Use OnGUI to check for spacebar press
    void OnGUI()
    {
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space)
        {
            NextLine();
        }
    }
}
