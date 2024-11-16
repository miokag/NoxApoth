using UnityEngine;
using TMPro;

public class CurrentOrderUI : MonoBehaviour
{
    [SerializeField] private GameObject orderNotePrefab; // Reference to the OrderNotePrefab
    [SerializeField] private Canvas canvas; // Reference to the Canvas to place order notes
    private OrderManager orderManager;

    private void Start()
    {
        // Get the OrderManager from the scene
        orderManager = FindObjectOfType<OrderManager>();

        // Instantiate the order note prefab at the top-left corner of the canvas
        GameObject orderNote = Instantiate(orderNotePrefab, canvas.transform);
        RectTransform orderNoteRect = orderNote.GetComponent<RectTransform>();

        // Set the position to top-left corner
        orderNoteRect.anchorMin = new Vector2(0, 1);
        orderNoteRect.anchorMax = new Vector2(0, 1);
        orderNoteRect.pivot = new Vector2(0, 1);
        orderNoteRect.anchoredPosition = new Vector2(10, -10); // Adjust as needed to fit the UI layout

        // Scale the order note down
        orderNote.transform.localScale = new Vector3(0.8f, 0.8f, 1f);  // Scale to 80% of its original size

        // Get the CharacterNameText and OrderDescriptionText from the prefab
        TMP_Text characterNameText = orderNote.transform.Find("CharacterNameText")?.GetComponent<TMP_Text>();
        TMP_Text orderDescriptionText = orderNote.transform.Find("OrderDescriptionText")?.GetComponent<TMP_Text>();

        // Retrieve current customer and find their order in the OrderManager
        string currentCustomer = GameManager.Instance.currentCustomer;
        Order currentOrder = orderManager.GetAllOrders().Find(order => order.CharacterName == currentCustomer);

        if (currentOrder != null && characterNameText != null && orderDescriptionText != null)
        {
            // Set the text fields with the current order data
            characterNameText.text = currentOrder.CharacterName;
            orderDescriptionText.text = currentOrder.OrderDescription;
        }
        else
        {
            Debug.LogError("Order or UI elements are missing.");
        }
    }

}
