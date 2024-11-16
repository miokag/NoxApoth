using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    // The singleton instance
    public static InventoryManager Instance { get; private set; }

    // A list to hold the items in the inventory
    private List<string> inventory = new List<string>();

    // Optional: UI elements to display inventory (like a UI text or image)
    // public GameObject inventoryUI;

    // Ensures that there is only one instance of InventoryManager in the scene
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }


    // Add an item to the inventory
    public void AddItem(string itemName)
    {
        inventory.Add(itemName);
        Debug.Log(itemName + " has been added to the inventory.");
        UpdateInventoryUI(); // Update the UI (if you have one)

    }

    // Remove an item from the inventory
    public void RemoveItem(string itemName)
    {
        if (inventory.Contains(itemName))
        {
            inventory.Remove(itemName);
            Debug.Log(itemName + " has been removed from the inventory.");
            UpdateInventoryUI(); // Update the UI (if you have one)
        }
        else
        {
            Debug.Log(itemName + " is not in the inventory.");
        }
    }

    // Check if an item exists in the inventory
    public bool HasItem(string itemName)
    {
        return inventory.Contains(itemName);
    }

    // Optionally: Method to display inventory contents (for debugging purposes)
    public void DisplayInventory()
    {
        if (inventory.Count > 0)
        {
            string inventoryContents = "Inventory: ";
            foreach (var item in inventory)
            {
                inventoryContents += item + ", ";
            }
            Debug.Log(inventoryContents);
        }
        else
        {
            Debug.Log("Inventory is empty.");
        }
    }

    // Optional: Update inventory UI (if using one)
    private void UpdateInventoryUI()
    {
        // Update your UI here (e.g., populate an inventory UI element with item names)
        // inventoryUI.GetComponent<Text>().text = string.Join(", ", inventory);
    }
}
