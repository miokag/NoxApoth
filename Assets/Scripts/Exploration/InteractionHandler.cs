using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class InteractionHandler : MonoBehaviour
{
    public LayerMask ingredientLayer; // The layer to check for interactable objects
    public LayerMask sceneLayer;
    
    private Collider currentInteractableObject = null; // Track the current interactable object in the trigger zone
    private NotebookUIManager notebookUIManager;
    [SerializeField] private RhythmSceneManager rhythmSceneManager;
    [SerializeField] private Canvas interactableCanvasPrefab; //For the Instruction Button
    [SerializeField] private GameObject instructionUIPrefab;
    [SerializeField] private GameObject inventoryFullPrefab;

    private Canvas interactableCanvas;
    private GameObject instructionUI;
    private GameObject player;
    private bool isIngredient = false;
    private bool isSceneChanger = false;
    private Canvas _canvas;

    private GameObject _inventoryFull;

    private void Start()
    {
        _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        notebookUIManager = FindAnyObjectByType<NotebookUIManager>();
        GameObject player = transform.root.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        // Check for interaction input (E key)
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isIngredient)
            {
                if (!GameManager.Instance.FullInventory())
                {
                    HandleInteraction();
                    Destroy(interactableCanvas.gameObject);
                }
                else
                {
                    if (_inventoryFull == null)
                    {
                        _inventoryFull = Instantiate(inventoryFullPrefab, _canvas.transform);
                        TextMeshProUGUI inventoryText = _inventoryFull.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
                        inventoryText.text = "Inventory is Full!";
                        // Optionally, you can add a timer to destroy it after a set time if necessary
                        StartCoroutine(DestroyInventoryFullAfterDelay(1f)); // Destroy after 2 seconds
                    }
                }
            }
            else if (isSceneChanger)
            {
                BackToShop();
                Destroy(interactableCanvas.gameObject);
            }
        }
    }
    
    private IEnumerator DestroyInventoryFullAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (GameManager.Instance.FullInventory())
        {
            Destroy(_inventoryFull);
            _inventoryFull = null; // Reset to ensure it can be instantiated again if needed
        }
    }

    // A method that is triggered by the PlayerControllerScript when interacting
    public void HandleInteraction()
    {
        // If there's an interactable object, interact with it
        if (currentInteractableObject != null)
        {
            Debug.Log("Interacting with: " + currentInteractableObject.gameObject.name);

            // Get the Interactable component
            Interactable interactable = currentInteractableObject.GetComponent<Interactable>();
            if (interactable != null && interactable.CanInteract())
            {
                interactable.Interact(); // Perform the interaction

                // Check if the ingredient has an InteractableInfo component
                InteractableInfo info = currentInteractableObject.GetComponent<InteractableInfo>();
                if (info != null)
                {
                    // Safely access the ingredient name after confirming info is not null
                    string interactableName = info.ingredientName;
                    Debug.Log("Ingredient Info: " + info.GetIngredientInfo()); // Log info, or display it to the UI

                    // Instantiate the RhythmSceneManager prefab and pass the ingredient name
                    if (rhythmSceneManager != null)
                    {
                        rhythmSceneManager.SpawnRhythmPrefab(interactableName); // Pass the ingredient name to RhythmSceneManager

                    }
                    else
                    {
                        Debug.LogError("RhythmSceneManager prefab not assigned in the Inspector!");
                    }
                }
                else
                {
                    Debug.LogWarning("InteractableInfo component not found on: " + currentInteractableObject.gameObject.name);
                }

                // After interaction, destroy the object (only if inside the trigger zone)
                interactable.DestroyInteractable();
            }
        }
        else
        {
            Debug.Log("No interactable objects nearby.");
        }
    }


    // This method is called when the player enters the trigger collider
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider is in the ingredient layer
        if (((1 << other.gameObject.layer) & ingredientLayer) != 0)
        {
            isIngredient = true;
            Debug.Log("Entered trigger with: " + other.gameObject.name);

            // Set the current interactable object to the one that entered the trigger zone
            currentInteractableObject = other;

            Vector3 offset = currentInteractableObject.transform.right * 0.5f + currentInteractableObject.transform.up * 0.2f ;

            if (interactableCanvas != null)
            {
                Destroy(interactableCanvas.gameObject);
                interactableCanvas = null;
                OnTriggerEnter(other); // Avoid recursion. This will trigger the same method again.
            }
            else
            {
                interactableCanvas = Instantiate(interactableCanvasPrefab);

                // Calculate canvas position with offset on x, y, and z axes
                Vector3 canvasPosition = currentInteractableObject.transform.position + offset;

                // Set the canvas position
                interactableCanvas.transform.position = canvasPosition;

                // Instantiate instruction UI as a child of interactable canvas
                instructionUI = Instantiate(instructionUIPrefab, interactableCanvas.transform);

                // Find the ButtonInstruction GameObject inside the interactableCanvas
                GameObject ButtonInstruction = interactableCanvas.transform.Find("ButtonInstruction(Clone)")?.gameObject;

                // Ensure ButtonInstruction exists before trying to find the child components
                if (ButtonInstruction != null)
                {
                    // Find the KeyIconText and DescriptionText under ButtonInstruction using GetComponentInChildren
                    TMP_Text KeyIconText = ButtonInstruction.transform.Find("KeyIconText")?.GetComponent<TMP_Text>();
                    TMP_Text DescriptionText = ButtonInstruction.transform.Find("DescriptionText")?.GetComponent<TMP_Text>();

                    KeyIconText.text = "E";
                    DescriptionText.text = "Gather";
                }
                else
                {
                    Debug.LogWarning("ButtonInstruction not found!");
                }
            }
        }
        else if (((1 << other.gameObject.layer) & sceneLayer) != 0)
        {
            isSceneChanger = true;
            Debug.Log("Entered trigger with: " + other.gameObject.name);

            // Set the current interactable object to the one that entered the trigger zone
            currentInteractableObject = other;

            Vector3 offset = currentInteractableObject.transform.right * 0.5f + currentInteractableObject.transform.up * 0.2f;

            if (interactableCanvas != null)
            {
                Destroy(interactableCanvas.gameObject);
                interactableCanvas = null;
                OnTriggerEnter(other); // Avoid recursion. This will trigger the same method again.
            }
            else
            {
                interactableCanvas = Instantiate(interactableCanvasPrefab);

                // Calculate canvas position with offset on x, y, and z axes
                Vector3 canvasPosition = currentInteractableObject.transform.position + offset;

                // Set the canvas position
                interactableCanvas.transform.position = canvasPosition;

                // Instantiate instruction UI as a child of interactable canvas
                instructionUI = Instantiate(instructionUIPrefab, interactableCanvas.transform);

                // Find the ButtonInstruction GameObject inside the interactableCanvas
                GameObject ButtonInstruction = interactableCanvas.transform.Find("ButtonInstruction(Clone)")?.gameObject;

                // Ensure ButtonInstruction exists before trying to find the child components
                if (ButtonInstruction != null)
                {
                    // Find the KeyIconText and DescriptionText under ButtonInstruction using GetComponentInChildren
                    TMP_Text KeyIconText = ButtonInstruction.transform.Find("KeyIconText")?.GetComponent<TMP_Text>();
                    TMP_Text DescriptionText = ButtonInstruction.transform.Find("DescriptionText")?.GetComponent<TMP_Text>();

                    KeyIconText.text = "E";
                    DescriptionText.text = "To Shop";
                }
                else
                {
                    Debug.LogWarning("ButtonInstruction not found!");
                }
            }
        }
    }

    private void BackToShop()
    {
        SceneManager.LoadScene("BackShop");
    }

    // This method is called when the player exits the trigger collider
    private void OnTriggerExit(Collider other)
    {
        isIngredient = false;
        isSceneChanger = false;
        // If the object exiting the trigger is the current interactable object, clear it
        if (other == currentInteractableObject)
        {
            Debug.Log("Exited trigger with: " + other.gameObject.name);
            currentInteractableObject = null; // Clear the current interactable object
            if(interactableCanvas != null)
            {
                Destroy(interactableCanvas.gameObject);
                interactableCanvas = null;
            }
        }
    }
}
