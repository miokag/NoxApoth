using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Canvas inventoryUICanvas;
    public InventoryUI inventoryUI;

    private void Start()
    {
        InstantiateUI();
    }

    public void InstantiateUI()
    {
        // Initialize the UI with the current inventory items
        UpdateUI();
    }

    public void UpdateUI()
    {
        // Get the UI slots and the current inventory
        List<GameObject> getSlots = GetChildrenList(inventoryUICanvas);
        List<Ingredient> inventoryItems = GameManager.Instance.Inventory;

        // Check if we have valid slots and inventory items
        if (getSlots != null && inventoryItems != null)
        {
            // Update each slot with the corresponding inventory item, if available
            for (int i = 0; i < getSlots.Count; i++)
            {
                GameObject slot = getSlots[i];
                Image slotImage = slot.GetComponentInChildren<Image>();

                // Check if there is an ingredient for this slot index
                if (i < inventoryItems.Count)
                {
                    Ingredient ingredient = inventoryItems[i];
                    if (ingredient.icon != null)
                    {
                        if (slot.transform.childCount == 0)
                        {
                            // Instantiate a new Image object and set it as a child of the slot
                            GameObject newImageObject = new GameObject("IngredientImage");
                            newImageObject.transform.SetParent(slot.transform, false); // Make it a child of the slot

                            // Add an Image component to the new GameObject
                            Image newImage = newImageObject.AddComponent<Image>();
                            newImage.sprite = ingredient.icon; // Set the sprite to the ingredient's icon
                            
                            // Optionally, set the Image's size and position (if needed)
                            RectTransform newImageRect = newImage.GetComponent<RectTransform>();
                            newImageRect.sizeDelta = new Vector2(30, 30); // Set appropriate size
                            newImageRect.anchoredPosition = Vector2.zero; // Center it within the slot
                        }
                    }
                }
            }
        }

    }

    public List<GameObject> GetChildrenList(Canvas inventoryUICanvas)
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in inventoryUICanvas.transform)
        {
            children.Add(child.gameObject);
        }
        return children;
    }
}
