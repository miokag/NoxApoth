using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopBehavior : MonoBehaviour
{
    [SerializeField] private GameObject backButtonPrefab;
    [SerializeField] private Canvas canvas; 
    [SerializeField] private SceneChanger changer;
    [SerializeField] private CustomerDatabase _customerDatabase;
    [SerializeField] private List<Customer> shopClonedCustomers;
    [SerializeField] private GameObject potionImage;
    [SerializeField] private Transform potionLiquidImage;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] potionspriteRenderer;
    public GameObject _spawnCustomer;
    public ActualCustomerSpawner _actualCustomerSpawner;
    [SerializeField] private TutTest _tutTest;

    [SerializeField]
    private GameObject waitingArea;

    private OrderManager orderManager;
    private GameManager gameManager;

    private void Start()
    {
        if (GameManager.Instance.tutorialDone)
        {
            if (OrderManager.Instance.GetAllOrders().Count == 0)
            {
                _actualCustomerSpawner.enabled = true;
            }
            else
            {
                _actualCustomerSpawner.enabled = false;
            }
        }

        if (!GameManager.Instance.tutorialDone)
        {
            _tutTest.enabled = true;
            _actualCustomerSpawner.enabled = false;
        }
        
        if (_spawnCustomer == null) Debug.LogError("No SpawnCustomer found");
        
        spriteRenderer = potionLiquidImage.GetComponent<SpriteRenderer>();
        
        // Initialize shopClonedCustomers via GameManager
        GameManager.Instance.InitializeShopClonedCustomers();
        
        // Now check and log customers after initialization
        CheckAndLogCustomers();

        ShowPotionImage();
        Debug.Log("Tutorial Done? " + GameManager.Instance.tutorialDone + " GetAllOrders Count: " + OrderManager.Instance.GetAllOrders().Count);

        if (OrderManager.Instance.GetAllOrders().Count > 0)
        {
            Debug.Log("On Waiting Area Customer Name: " + OrderManager.Instance.GetAllOrders()[0].CharacterName);
            OnWaitingArea();
        }
        
        
    }

    private void ShowPotionImage()
    {
        if (GameManager.Instance.PotionMix.Count > 0)
        {
            potionImage.SetActive(true);
            if (GameManager.Instance.PotionMix.Count == 1) spriteRenderer.sprite = potionspriteRenderer[0];
            else if (GameManager.Instance.PotionMix.Count == 2) spriteRenderer.sprite = potionspriteRenderer[1];
            else if (GameManager.Instance.PotionMix.Count == 3) spriteRenderer.sprite = potionspriteRenderer[2];
        }
    }

    private void CheckAndLogCustomers()
    {
        // Ensure shopClonedCustomers has been populated
        shopClonedCustomers = GameManager.Instance.shopClonedCustomers;

        if (shopClonedCustomers.Count == 0)
        {
            Debug.Log("No customers available in the shop.");
        }
        else
        {
            Debug.Log("List of customers in the shop:");
            foreach (Customer customer in shopClonedCustomers)
            {
                Debug.Log("Customer Name: " + customer.customerName);
                Debug.Log("Order: " + (customer.customerOrder != null ? customer.customerOrder.name : "No order"));
            }

            InstantiateBackButton();
        }
    }

    private void InstantiateBackButton()
    {
        if (GameObject.Find("ToBackShopButton") == null)
        {
            // Instantiate ToBackShop button and set it in the canvas
            GameObject toBackShopButton = Instantiate(backButtonPrefab, canvas.transform);

            // Optionally, set a name for the button so we can easily check for it later
            toBackShopButton.name = "ToBackShopButton";
        }
        else
        {
            Debug.Log("ToBackShop button already exists in the scene.");
        }
    }

    // New method to spawn customer in the waiting area
    public void OnWaitingArea()
    {
        // Get the customer name from the first order in the list (or implement any logic for selecting the correct order)
        string currentCustomerName = OrderManager.Instance.GetAllOrders()[0].CharacterName;
        string currentCustomerOrder = OrderManager.Instance.GetAllOrders()[0].CustomerOrder;
        
        Debug.Log("Current Customer Order: " + currentCustomerOrder + ", Current Customer Name: " + currentCustomerName);

        // Get the actual customer from CustomerDatabase using the name
        Customer customer = _customerDatabase.GetCustomerByName(currentCustomerName); 
        
        if (customer != null)
        {
            GameObject customerPrefab = customer.customerPrefab; // Assuming Customer has a `customerPrefab` reference
            
            if (customerPrefab != null && waitingArea != null)
            {
                GameObject spawnedCustomer = Instantiate(
                    customerPrefab,
                    waitingArea.transform.position,
                    Quaternion.identity
                );

                spawnedCustomer.name = customer.customerName; // Ensure the spawned object's name matches the customer's name

                // Optional: Attach to the WaitingArea as a child for better organization
                spawnedCustomer.transform.SetParent(waitingArea.transform);

                // Set up CustomerMovement if applicable
                CustomerMovement movement = spawnedCustomer.GetComponent<CustomerMovement>();
                if (movement != null)
                {
                    movement.enabled = false; // Disable movement for now
                    movement.OnCustomerClicked += HandleCustomerClicked; // Add event listener
                }
            }
        }
        else
        {
            Debug.LogError("Customer with name " + currentCustomerName + " not found in the database.");
        }
    }

    // Handle customer click event (implement as needed)
    private void HandleCustomerClicked()
    {
        // Logic when the customer is clicked (this will depend on your game's functionality)
    }
}
