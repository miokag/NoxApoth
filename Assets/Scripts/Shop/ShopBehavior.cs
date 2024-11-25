using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Add this if you're using Unity UI components

public class ShopBehavior : MonoBehaviour
{
    [SerializeField] private GameObject backButtonPrefab;
    [SerializeField] private Canvas canvas; 
    [SerializeField] private SceneChanger changer;
    [SerializeField] private CustomerDatabase _customerDatabase;
    [SerializeField] private List<Customer> shopClonedCustomers; // List of cloned customers

    private OrderManager orderManager; 

    private void Start()
    {
        orderManager = FindObjectOfType<OrderManager>(); 

        // Initialize shopClonedCustomers via GameManager
        GameManager.Instance.InitializeShopClonedCustomers();
        
        // Now check and log customers after initialization
        CheckAndLogCustomers();
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
}
