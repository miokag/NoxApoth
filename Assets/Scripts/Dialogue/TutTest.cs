using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {
        _waitingArea = GameObject.Find("WaitingArea");
        
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

            dialogueManager = FindObjectOfType<DialogueSys>();
            orderManager = FindObjectOfType<OrderManager>();

            //dialogueManager.StartDialogue("shop1");
            nextStep = 1;
            dialogueManager.OnDialogueFinished += RunNextDialogueNode;  // Subscribe to the event
        }
        else 
        {
            if (customerDatabase != null)
            {
                cedric = customerDatabase.GetCustomerByName("Cedric");

                if (cedric != null)
                {
                    // Directly spawn Cedric at the waiting area
                    GameObject cedricPrefab = cedric.customerPrefab; // Assuming Customer has a `prefab` reference
                    if (cedricPrefab != null && _waitingArea != null)
                    {
                        GameObject spawnedCedric = Instantiate(
                            cedricPrefab,
                            _waitingArea.transform.position,
                            Quaternion.identity
                        );

                        spawnedCedric.name = "Cedric"; // Ensure the spawned object's name matches Cedric

                        // Optional: Attach to the WaitingArea as a child for better organization
                        spawnedCedric.transform.SetParent(_waitingArea.transform);

                        // Set up CustomerMovement if applicable
                        CustomerMovement movement = spawnedCedric.GetComponent<CustomerMovement>();
                        if (movement != null)
                        {
                            movement.enabled = false; // Disable movement for now
                            movement.OnCustomerClicked += HandleCustomerClicked; // Add event listener
                        }
                    }
                }
            }
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
