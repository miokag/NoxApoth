using UnityEngine;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class TutTest : MonoBehaviour
{
    public CustomerDatabase customerDatabase; // Reference to the CustomerDatabase
    public CustomerSpawner customerSpawner;   // Reference to the CustomerSpawner
    public DialogueSys dialogueManager;
    private int nextStep;
    private CustomerMovement currentCustomerMovement; // Reference to the CustomerMovement script
    private Customer cedric;
    private OrderManager orderManager;
    private GameObject _waitingArea;
    [SerializeField] private GameObject buttonInstructionPrefab;
    private GameObject buttonInstruction;
    private Canvas _canvas;
    private GivePotionBehaviorTutorial givePotionBehaviorTutorial;

    // Start is called before the first frame update
    void Start()
    {
        _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        _waitingArea = GameObject.Find("WaitingArea");
        dialogueManager = FindObjectOfType<DialogueSys>();
        orderManager = FindObjectOfType<OrderManager>();
        
        if (GameManager.Instance.GetTutorialStep() == 0)
        {
            if (customerDatabase != null)
            {
                cedric = customerDatabase.GetCustomerByName("Cedric");

                if (cedric != null)
                {
                    if (customerSpawner != null)
                    {
                        // Pass the customer to the spawner for instantiation
                        customerSpawner.SpawnCustomer(cedric);
                        
                        // Get the CustomerMovement component from the spawned customer (assuming the customer prefab has it)
                        currentCustomerMovement = GameObject.Find("Cedric").GetComponent<CustomerMovement>();
                        GameObject cedricGameObject = GameObject.Find("Cedric");
                        currentCustomerMovement.ChangeOtherObjectTag(cedricGameObject, "Untagged");
                        currentCustomerMovement.enabled = false;
                        currentCustomerMovement.OnCustomerClicked += HandleCustomerClicked;
                    }
                }
            }
            
            dialogueManager.StartDialogue("shop1");
            
            buttonInstruction = Instantiate(buttonInstructionPrefab, _canvas.transform);
            Image KeyIcon = buttonInstruction.transform.Find("KeyIcon").GetComponent<Image>();
            TMP_Text KeyIconText = buttonInstruction.transform.Find("KeyIconText")?.GetComponent<TMP_Text>();
            Destroy(KeyIcon.gameObject);
            Destroy(KeyIconText.GameObject());
            TMP_Text DescriptionText = buttonInstruction.transform.Find("DescriptionText")?.GetComponent<TMP_Text>();
            
            DescriptionText.text = "Spacebar";
            DescriptionText.GetComponent<RectTransform>().anchoredPosition += new Vector2(-10.1f, 0);
            buttonInstruction.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, -82.6f);
            nextStep = 1;
            dialogueManager.OnDialogueFinished += RunNextDialogueNode;  // Subscribe to the event
        }

        if (GameManager.Instance.GetTutorialStep() == 3)
        {
            givePotionBehaviorTutorial = cedric.AddComponent<GivePotionBehaviorTutorial>();
        }
    }
    
    
    void Update()
    {
        // Poll input once per frame (avoid in final implementation)
        if (Input.GetKeyDown(KeyCode.Space) && buttonInstruction != null)
        {
            Destroy(buttonInstruction);
        }
        
        if (GameManager.Instance.GetTutorialStep() == 3 && givePotionBehaviorTutorial.isDone == true)
        {
            dialogueManager.LoadLuaScript("TutorialDialogue");
            Destroy(givePotionBehaviorTutorial);
            dialogueManager.StartDialogue("review");
            GameManager.Instance.tutorialDone = true;
        }
    }

    // This method will be called when the dialogue finishes
    private void RunNextDialogueNode()
    {
        if (nextStep == 1)
        {
            Debug.Log("Next Step = 1 is running");
            currentCustomerMovement.enabled = true;
            currentCustomerMovement.StartMove();
            dialogueManager.OnDialogueFinished -= RunNextDialogueNode;
            nextStep++;
        }
    }

    private void HandleCustomerClicked()
    {
        Debug.Log("Cedric was clicked and is now marked as clicked");

        if (nextStep == 2)
        {
            dialogueManager.StartDialogue("shop2");
            nextStep++;
            GameManager.Instance.NextTutorialStep();
        }
    }

    // Unsubscribe from the event when this object is destroyed
    void OnDestroy()
    {
        if (dialogueManager != null)
        {
            dialogueManager.OnDialogueFinished -= RunNextDialogueNode;
        }

        // Unsubscribe from customer movement events
        if (currentCustomerMovement != null)
        {
            currentCustomerMovement.OnCustomerClicked -= HandleCustomerClicked;
        }
    }
}
