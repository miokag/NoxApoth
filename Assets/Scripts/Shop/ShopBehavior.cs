using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Add this if you're using Unity UI components

public class ShopBehavior : MonoBehaviour
{
    [SerializeField] private GameObject backButtonPrefab; // Reference to the Back Button Prefab
    [SerializeField] private Canvas canvas; // Reference to Canvas to place order notes
    [SerializeField] private SceneChanger changer;


    private OrderManager orderManager; // Reference to the OrderManager script

    private void Start()
    {
        orderManager = FindObjectOfType<OrderManager>(); // Find the OrderManager in the scene

        // Check if there are any orders
        List<Order> orders = orderManager.GetAllOrders();
        if (orders.Count > 0)
        {
            if (orders != null)
            {
                foreach (Order order in orders)
                {
                    Debug.Log("Order Found: ");
                    Debug.Log("Character Name: " + order.CharacterName);
                    Debug.Log("Order Description: " + order.OrderDescription);
                }
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
