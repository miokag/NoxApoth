using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DoorBehavior : MonoBehaviour
{
    [SerializeField] private SceneChanger changer;
    [SerializeField] private GameObject orderNotePrefab; // Reference to OrderNotePrefab
    [SerializeField] private Canvas canvas; // Reference to Canvas to place order notes
    [SerializeField] private GameObject leftButtonPrefab; // Reference to Left Button Prefab
    [SerializeField] private GameObject rightButtonPrefab; // Reference to Right Button Prefab

    private List<Order> orders;
    private int currentOrderIndex = 0;
    private GameObject orderNote;
    private TMP_Text characterNameText;
    private TMP_Text orderDescriptionText;

    private Collider doorCollider;  // Store the collider of the door
    public Button leftButton;      // Store Left Button reference
    public Button rightButton;     // Store Right Button reference

    private bool isClickable = true; // Flag to track if the door is clickable

    private void Start()
    {
        // Get the collider for disabling interaction
        doorCollider = GetComponent<Collider>();
    }

    private void OnMouseDown()
    {
        // Prevent interaction if the door is not clickable
        if (!isClickable)
            return;

        // Load orders from the JSON file
        orders = OrderManager.Instance.GetAllOrders();

        if (orders.Count > 0)
        {
            if (GameManager.Instance.currentCustomer != null)
            {
                SceneManager.LoadScene("Exploration");
            }
            else
            {
                // Instantiate the Order Note prefab if not already created
                if (orderNote == null)
                {
                    orderNote = Instantiate(orderNotePrefab, canvas.transform);
                    characterNameText = orderNote.transform.Find("CharacterNameText")?.GetComponent<TMP_Text>();
                    orderDescriptionText = orderNote.transform.Find("OrderDescriptionText")?.GetComponent<TMP_Text>();

                    // Instantiate and setup the Left and Right buttons
                    GameObject leftButtonObj = Instantiate(leftButtonPrefab, canvas.transform);
                    leftButton = leftButtonObj.GetComponent<Button>();
                    leftButton.onClick.AddListener(ShowPreviousOrder);

                    GameObject rightButtonObj = Instantiate(rightButtonPrefab, canvas.transform);
                    rightButton = rightButtonObj.GetComponent<Button>();
                    rightButton.onClick.AddListener(ShowNextOrder);
                }

                // Display the first order
                DisplayOrder(currentOrderIndex);
            }
        }
        else
        {
            Debug.Log("No orders found in the JSON file.");
        }

        // After clicking, disable the interaction
        DisableInteraction();
    }

    private void DisplayOrder(int index)
    {
        if (index >= 0 && index < orders.Count)
        {
            characterNameText.text = orders[index].CharacterName;
            orderDescriptionText.text = orders[index].OrderDescription;
            GameManager.Instance.setCurrentCustomer(characterNameText.text);
        }
        else
        {
            Debug.LogError("Invalid order index.");
        }
    }

    private void ShowPreviousOrder()
    {
        if (currentOrderIndex > 0)
        {
            currentOrderIndex--;
        }
        else
        {
            currentOrderIndex = orders.Count - 1; // Loop to the last order
        }
        DisplayOrder(currentOrderIndex);
    }

    private void ShowNextOrder()
    {
        if (currentOrderIndex < orders.Count - 1)
        {
            currentOrderIndex++;
        }
        else
        {
            currentOrderIndex = 0; // Loop back to the first order
        }
        DisplayOrder(currentOrderIndex);
    }

    // Disable further interaction with the door (making it unclickable)
    public void DisableInteraction()
    {
        isClickable = false;  // Set the flag to false
        if (doorCollider != null)
        {
            doorCollider.enabled = false;  // Disable the door's collider so it can't be clicked again
        }
    }

    public void EnableInteraction()
    {
        isClickable = true;  // Set the flag to true
        if (doorCollider != null)
        {
            doorCollider.enabled = true;  // Enables the door's collider so it can be clicked again
        }
    }
}
