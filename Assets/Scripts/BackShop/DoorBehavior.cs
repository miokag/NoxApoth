using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class DoorBehavior : MonoBehaviour
{
    [SerializeField] private SceneChanger changer;
    [SerializeField] private GameObject orderNotePrefab; // Reference to OrderNotePrefab
    [SerializeField] private Canvas canvas; // Reference to Canvas to place order notes
    [SerializeField] private GameObject leftButtonPrefab; // Reference to Left Button Prefab
    [SerializeField] private GameObject rightButtonPrefab; // Reference to Right Button Prefab
    [SerializeField] private GameObject noOrderPrefab; // Reference to Right Button Prefab

    private List<Order> orders;
    private int currentOrderIndex = 0;
    private GameObject orderNote;
    private TMP_Text characterNameText;
    private TMP_Text orderDescriptionText;

    private Collider doorCollider;  // Store the collider of the door
    public Button leftButton;      // Store Left Button reference
    public Button rightButton;     // Store Right Button reference

    private bool isClickable = true; // Flag to track if the door is clickable
    private GameObject _noOrder;
    private GameObject _tutorial;
    private BackShopTutorial _backShopTutorial;

    private void Start()
    {
        _tutorial = GameObject.Find("Tutorial");
        _backShopTutorial = _tutorial.GetComponent<BackShopTutorial>();
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

                    if (orders.Count > 1)
                    {
                        // Instantiate and setup the Left and Right buttons
                        GameObject leftButtonObj = Instantiate(leftButtonPrefab, canvas.transform);
                        leftButton = leftButtonObj.GetComponent<Button>();
                        leftButton.onClick.AddListener(ShowPreviousOrder);

                        GameObject rightButtonObj = Instantiate(rightButtonPrefab, canvas.transform);
                        rightButton = rightButtonObj.GetComponent<Button>();
                        rightButton.onClick.AddListener(ShowNextOrder);
                    }
                    this.gameObject.tag = "Untagged";
                    GameObject toKitchen = _backShopTutorial.bookCase;
                    toKitchen.tag = "Untagged";

                }

                // Display the first order
                DisplayOrder(currentOrderIndex);
            }
        }
        else
        {
            if (_noOrder == null)
            {
                _noOrder = Instantiate(noOrderPrefab, canvas.transform);
                TextMeshProUGUI inventoryText = _noOrder.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
                inventoryText.text = "No Customer Order";
                // Optionally, you can add a timer to destroy it after a set time if necessary
                StartCoroutine(DestroyInventoryFullAfterDelay(1f)); // Destroy after 2 seconds
            }
        }

        // After clicking, disable the interaction
        DisableInteraction();
    }
    
    private IEnumerator DestroyInventoryFullAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (GameManager.Instance.FullInventory())
        {
            Destroy(_noOrder);
            _noOrder = null; // Reset to ensure it can be instantiated again if needed
        }
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
