using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class ActualCustomerSpawner : MonoBehaviour
{
    private CustomerDatabase customerDatabase; 
    private CustomerSpawner _customerSpawner;
    private CustomerMovement customerMovement;
    private string _dialogueScript;

    private void Start()
    {
        customerDatabase = GameManager.Instance.customerDatabase;
        foreach (Customer customer in customerDatabase.customers)
        {
            Debug.Log(customer.name);
        }
        _customerSpawner = FindObjectOfType<CustomerSpawner>();

        // Subscribe to the event to start the movement when the customer is spawned
        _customerSpawner.OnCustomerSpawned += HandleCustomerSpawned;

        SpawnRandomCustomer();
    }

    public void SpawnRandomCustomer()
    {
        if (customerDatabase != null)
        {
            // Get a random customer from the database
            Customer randomCustomer = GetRandomCustomer();

            if (randomCustomer != null)
            {
                // Pass the random customer to the spawner for instantiation
                SpawnCustomer(randomCustomer);
            }
        }
    }

    private Customer GetRandomCustomer()
    {
        List<Customer> customers = customerDatabase.customers;
        if (customers.Count > 0)
        {
            int randomIndex = Random.Range(0, customers.Count);
            Customer randomCustomer = customers[randomIndex];

            // Assign the name of the selected customer to _dialogueScript
            _dialogueScript = randomCustomer.name;

            return randomCustomer;
        }

        _dialogueScript = null; // Clear _dialogueScript if no customers are available
        return null;
    }

    public void SpawnCustomer(Customer customer)
    {
        _customerSpawner.SpawnCustomer(customer);
    }

    private void HandleCustomerSpawned(GameObject spawnedCustomer)
    {
        customerMovement = spawnedCustomer.GetComponent<CustomerMovement>();

        if (customerMovement != null)
        {
            customerMovement.enabled = true;
            customerMovement.StartMove();

            // Subscribe to the OnCustomerClicked event
            customerMovement.OnCustomerClicked += HandleCustomerClicked;
        }
        else
        {
            Debug.LogError("CustomerMovement component not found on the spawned customer!");
        }
    }

    private void HandleCustomerClicked()
    {
        if (customerMovement.canBeClicked == false)
        {
            Debug.Log("Customer was clicked! Handle your custom logic here.");

            PotionDatabase potionDatabase = GameManager.Instance.potionDatabase;

            if (potionDatabase != null && potionDatabase.potions.Count > 0)
            {
                List<Potion> potions = potionDatabase.potions;
                int randomIndex = Random.Range(0, potions.Count);
                Potion randomPotion = potions[randomIndex];

                if (randomPotion != null)
                {
                    Debug.Log($"Random Potion Selected: {randomPotion.potionName}");
                    _dialogueScript += " " + randomPotion.potionName;
                    DialogueSys.Instance.luaFileName = _dialogueScript;

                    int randomInt = Random.Range(1, 5);
                    DialogueSys.Instance.LoadLuaScript(_dialogueScript);
                    DialogueSys.Instance.StartDialogue("node" + randomInt);

                    // Subscribe to dialogue finished event
                    DialogueSys.Instance.OnDialogueFinished += HandleDialogueFinished;

                    // Disable further clicks during the dialogue
                    customerMovement.canBeClicked = false;
                }
                else
                {
                    Debug.LogError("Failed to select a random potion.");
                }
            }
            else
            {
                Debug.LogError("PotionDatabase is null or empty.");
            }
        }
    }

// Callback for when dialogue finishes
    private void HandleDialogueFinished()
    {
        Debug.Log("Dialogue finished! Proceed with next action.");

        CheckTextbox();
        
        // Unsubscribe from the event to prevent multiple calls
        DialogueSys.Instance.OnDialogueFinished -= HandleDialogueFinished;
    }


    private IEnumerator CheckTextbox()
    {
        // Check every frame until the Textbox is found and destroyed
        while (true)
        {
            Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
            GameObject textBox = canvas.transform.Find("TextboxCanvas(Clone)")?.gameObject;

            if (textBox != null)
            {
                // Destroy the text box if found
                Destroy(textBox);
                break; // Exit the loop once the textBox is destroyed
            }

            // If not found, wait for the next frame
            yield return null;
        }
    }
    private void OnDestroy()
    {
        if (_customerSpawner != null)
        {
            _customerSpawner.OnCustomerSpawned -= HandleCustomerSpawned;
        }

        if (customerMovement != null)
        {
            customerMovement.OnCustomerClicked -= HandleCustomerClicked;
        }

        // Ensure to unsubscribe from the dialogue event in case the object is destroyed
        if (DialogueSys.Instance != null)
        {
            DialogueSys.Instance.OnDialogueFinished -= HandleDialogueFinished;
        }
    }
}
