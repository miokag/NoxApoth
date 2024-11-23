using DialogueSystem;
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

    // Start is called before the first frame update
    void Start()
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
                    currentCustomerMovement.enabled = false;
                    currentCustomerMovement.OnCustomerClicked += HandleCustomerClicked;
                }
            }
        }

        dialogueManager = FindObjectOfType<DialogueSys>();
        orderManager = FindObjectOfType<OrderManager>();

        orderManager.StoreOrder("Luna", "Potion for calming nightmares", "Nightmare-Calming Potion, Moonlight Essence, Dreamroot");
        orderManager.StoreOrder("Elric", "Sword sharpening oil", "Dragon's Scale Oil, Hardened Iron Filings, Ember Shard");
        orderManager.StoreOrder("Mira", "Healing herb mixture", "Life Blossom Petals, Fresh Honeydew, Morning Dew Drop");
        orderManager.StoreOrder("Garrick", "Flame-resistant potion for blacksmithing", "Fireproof Potion, Salamander's Scale, Mountain Ash");

        if (dialogueManager == null) { Debug.LogError("DialogueManager is not assigned."); }
        else
        {
            if (GameManager.Instance.GetTutorialStep() == 0)
            {
                dialogueManager.StartDialogue("shop1");
                nextStep = 1;
                dialogueManager.OnDialogueFinished += RunNextDialogueNode;  // Subscribe to the event
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
