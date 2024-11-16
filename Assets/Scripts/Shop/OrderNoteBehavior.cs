using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static UnityEditor.FilePathAttribute;

public class OrderNoteBehavior : MonoBehaviour, IPointerDownHandler
{
    public GameObject toBackShopButtonPrefab; // Reference to the ToBackShop button prefab
    public Canvas canvas; // Reference to the Canvas where the button will be placed

    void Start()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        // Check if the active scene is "Exploration" and make the object unclickable
        if (SceneManager.GetActiveScene().name == "Exploration")
        {
            DisableInteraction();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Only proceed if the object is clickable (i.e., not in Exploration scene)
        if (SceneManager.GetActiveScene().name == "Exploration")
            return; // Do nothing if we're in the "Exploration" scene

        Debug.Log("Clicked on Order Note");

        Destroy(gameObject);

        // Check if we're in the "Shop" scene
        if (SceneManager.GetActiveScene().name == "Shop")
        {
            Destroy(gameObject);

            // Check if ToBackShop button already exists in the scene
            if (GameObject.Find("ToBackShopButton") == null)
            {
                // Instantiate ToBackShop button and set it in the canvas
                GameObject toBackShopButton = Instantiate(toBackShopButtonPrefab, canvas.transform);

                // Optionally, set a name for the button so we can easily check for it later
                toBackShopButton.name = "ToBackShopButton";
            }
            else
            {
                Debug.Log("ToBackShop button already exists in the scene.");
            }
        }

        if (SceneManager.GetActiveScene().name == "BackShop")
        {
            GameObject location = GameObject.Find("Location");
            Transform door = location.transform.Find("Door");
            Transform actualDoor = door.transform.Find("Cube");

            DoorBehavior doorBehavior = actualDoor.GetComponent<DoorBehavior>();
            doorBehavior.DestroyButtons();
            SceneManager.LoadScene("Exploration");
        }
    }

    public void LogOrderList()
    {
        // Load orders directly from the JSON file
        List<Order> orders = OrderManager.LoadOrdersFromJson();

        // Log each order for debugging
        if (orders != null && orders.Count > 0)
        {
            foreach (var order in orders)
            {
                Debug.Log($"Character: {order.CharacterName}, Description: {order.OrderDescription}, Customer Order: {order.CustomerOrder}");
            }
        }
        else
        {
            Debug.Log("No orders found in the JSON file.");
        }
    }

    // Disable interaction by turning off the Collider or EventTrigger component
    private void DisableInteraction()
    {
        // Option 1: If using a Collider for interaction, disable it
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // Option 2: If using an EventTrigger for interaction, disable it
        EventTrigger eventTrigger = GetComponent<EventTrigger>();
        if (eventTrigger != null)
        {
            eventTrigger.enabled = false;
        }

        // Option 3: If using IPointerDownHandler, simply ignore pointer events by returning early from OnPointerDown (done above)
    }
}
