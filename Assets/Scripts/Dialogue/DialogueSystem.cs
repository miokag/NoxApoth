using MoonSharp.Interpreter;
using System.IO;
using UnityEngine;
using TMPro;
using System.Linq;

namespace DialogueSystem
{
    public class DialogueManager : MonoBehaviour
    {

        public static DialogueManager Instance { get; private set; }

        void Awake()
        {
            // Ensure only one instance exists and make it persist across scenes
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Keeps it across scene transitions
            }
            else
            {
                Destroy(gameObject); // Destroy duplicate
            }
        }

        public GameObject dialogueBoxPrefab;
        public GameObject orderNotePrefab; // Reference to the order note prefab
        public string luaFileName = "TutorialDialogue";
        public Canvas canvas;

        private Script _luaScript;
        private string _currentNode = "start";
        private int _lineIndex = 0;
        private bool _isWaitingForAction = false;
        private GameObject _currentDialogueBox;
        private GameObject _currentOrderNote; // Stores the current active order note
        private TMP_Text _dialogueText;
        private TMP_Text _characterText;
        private bool isOrder = false;

        public event System.Action OnActionTriggeredEvent;
        public event System.Action OnDialogueFinishedEvent;

        public bool IsDialogueFinished { get; private set; } = false;

        private string _orderDescription = "";  // Stores the order description to display at the end
        private string _characterName = "";      // Stores the character's name for the order note
        private string _customerOrder = "";

        void Start()
        {
            LoadLuaScript(luaFileName);
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
            canvas = FindAnyObjectByType<Canvas>();
            _currentNode = node;
            _lineIndex = 0;
            _isWaitingForAction = false;
            IsDialogueFinished = false;

            if (_dialogueText == null || _characterText == null)
            {
                Debug.LogError("TMP_Text components are missing in the prefab.");
            }

            DisplayDialogueNode(_currentNode);
        }

        private void DisplayDialogueNode(string node)
        {
            _currentDialogueBox = Instantiate(dialogueBoxPrefab, canvas.transform);
            _dialogueText = _currentDialogueBox.transform.Find("DialogueBox/DialogueText")?.GetComponent<TMP_Text>();
            _characterText = _currentDialogueBox.transform.Find("NameBox/CharacterName")?.GetComponent<TMP_Text>();

            if (_isWaitingForAction || IsDialogueFinished) return;  // Prevent processing when the dialogue is finished
            // Prevent any further processing if node is null
            if (string.IsNullOrEmpty(node))
            {
                Debug.LogWarning("Attempted to display dialogue with null or empty node.");
                return;
            }

            DynValue luaNode = _luaScript.Call(_luaScript.Globals["getDialogueNode"], node);

            if (luaNode.Type == DataType.Table)
            {
                DynValue characterName = luaNode.Table.Get("character");
                if (characterName.Type == DataType.String)
                {
                    _characterText.text = characterName.String;
                    _characterName = characterName.String; // Store character's name for order note
                }

                DynValue textArray = luaNode.Table.Get("text");
                if (textArray.Type == DataType.Table && _lineIndex < textArray.Table.Length)
                {
                    var dialogueLine = textArray.Table.Values.ElementAt(_lineIndex).Table;

                    _dialogueText.text = dialogueLine.Get("line").String;

                    // Check if this line is an order description and store it
                    if (dialogueLine.Get("orderdescription").Type == DataType.String)
                    {
                        _orderDescription = dialogueLine.Get("orderdescription").String;
                    }

                    // Check for customer order and store it
                    if (dialogueLine.Get("customerorder").Type == DataType.String)
                    {
                        isOrder = true;
                        _customerOrder = dialogueLine.Get("customerorder").String;
                    }
                    else
                    {
                        // If customerorder doesn't exist, reset isOrder
                        isOrder = false;
                    }

                    // Check for action "destroyPrevious"
                    if (dialogueLine.Get("action").Type == DataType.String)
                    {
                        string action = dialogueLine.Get("action").String;
                        if (action == "destroyPrevious")
                        {
                            _isWaitingForAction = true;

                            OnActionTriggeredEvent?.Invoke();
                            DestroyCurrentDialogueBox();

                            Debug.Log("Waiting for next action...");
                        }
                    }
                }
                else
                {
                    // End of dialogue or no text available
                    Debug.Log("End of dialogue or no text available.");
                    DestroyCurrentDialogueBox();
                    IsDialogueFinished = true;  // Mark dialogue as finished
                    // Show order note only if we have order information (description and customer order)
                    if (!string.IsNullOrEmpty(_orderDescription) && !string.IsNullOrEmpty(_customerOrder) && isOrder == true)
                    {
                        CreateOrderNote();
                    }
                    OnDialogueFinishedEvent?.Invoke();

                    
                }
            }
        }

        public void GoToNextNode(string nextNode)
        {
            // Transition to the next node
            _currentNode = nextNode;
            _lineIndex = 0;  // Reset the line index for the new node

            // Display the new node
            DisplayDialogueNode(_currentNode);
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




        private void DestroyCurrentDialogueBox()
        {
            if (_currentDialogueBox != null)
            {
                Destroy(_currentDialogueBox);
            }
        }

        public void ResumeDialogue()
        {
            if (_isWaitingForAction)
            {
                // Destroy any existing dialogue box first to prevent overlapping UI
                DestroyCurrentDialogueBox();

                // Instantiate a new dialogue box
                _currentDialogueBox = Instantiate(dialogueBoxPrefab, canvas.transform);
                _dialogueText = _currentDialogueBox.transform.Find("DialogueBox/DialogueText").GetComponent<TMP_Text>();
                _characterText = _currentDialogueBox.transform.Find("NameBox/CharacterName").GetComponent<TMP_Text>();

                // Clear any default text (just in case)
                _dialogueText.text = "";

                // Set flag to false to resume the dialogue
                _isWaitingForAction = false;

                // Fetch the dialogue node from the Lua script
                DynValue luaNode = _luaScript.Call(_luaScript.Globals["getDialogueNode"], _currentNode);

                if (luaNode.Type == DataType.Table)
                {
                    DynValue textArray = luaNode.Table.Get("text");
                    if (textArray.Type == DataType.Table && _lineIndex < textArray.Table.Length)
                    {
                        var dialogueLine = textArray.Table.Values.ElementAt(_lineIndex).Table;

                        // Fetch and set the character's name
                        DynValue characterName = dialogueLine.Get("character");
                        if (characterName.Type == DataType.String)
                        {
                            _characterText.text = characterName.String; // Set character name
                        }

                        // Fetch and set the dialogue line
                        DynValue line = dialogueLine.Get("line");
                        if (line.Type == DataType.String)
                        {
                            _dialogueText.text = line.String; // Set dialogue text
                        }

                        // Update the _lineIndex for the next line
                        _lineIndex++;
                    }
                }
            }
        }

        void Update()
        {
            // Only proceed if there is an active dialogue node
            if (!string.IsNullOrEmpty(_currentNode) && !_isWaitingForAction && !IsDialogueFinished)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _lineIndex++;  // Move to the next line
                    DisplayDialogueNode(_currentNode);
                }
            }
        }

        public void ResetDialogue()
        {
            // Reset the current dialogue state
            _currentNode = null;
            _lineIndex = 0;
            IsDialogueFinished = false;
            _isWaitingForAction = false;


            // Destroy any existing dialogue boxes
            DestroyCurrentDialogueBox();

            // Destroy any existing order notes
            if (_currentOrderNote != null)
            {
                Destroy(_currentOrderNote);
            }

            // Optionally, trigger any other cleanup logic as needed
            Debug.Log("Dialogue has been reset.");
        }

        public void EndDialogue()
        {
            // Reset the current dialogue state
            _currentNode = null;
            _lineIndex = 0;
            IsDialogueFinished = true;
            _isWaitingForAction = true;

            // Destroy any existing dialogue boxes
            DestroyCurrentDialogueBox();

            // Destroy any existing order notes
            if (_currentOrderNote != null)
            {
                Destroy(_currentOrderNote);
            }

            // Optionally, trigger any other cleanup logic as needed
            Debug.Log("Dialogue has been ended.");
        }


        private void OnDestroy()
        {
            DestroyCurrentDialogueBox();
            if (_currentOrderNote != null)
            {
                Destroy(_currentOrderNote);
            }
        }

        public bool IsWaitingForAction()
        {
            return _isWaitingForAction;
        }
    }
}
