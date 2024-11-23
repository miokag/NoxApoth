using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientCollider : MonoBehaviour
{
    [SerializeField] private Ingredient ingredient;
    private PotMixerBehavior mixer;
    private GameObject _panTrigger;
    private GameObject _panHandle;

    private void Start()
    {
        mixer = GameObject.Find("Mixer").GetComponent<PotMixerBehavior>();
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Utensil"))
        {
            // Destroy the ingredient
            Destroy(gameObject);

            // Remove the ingredient from inventory
            GameManager.Instance.RemoveFromInventory(ingredient);
            
            // Become current ingredient being processed
            GameManager.Instance.CurrentlyProcessing(ingredient);

            // Remove Inventory UI components
            InventoryUIKitchen inventoryUIKitchen = FindObjectOfType<InventoryUIKitchen>();
            if (inventoryUIKitchen != null)
            {
                Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
                GameObject inventoryUIPanel = canvas.transform.Find("InventoryUIPanel(Clone)").gameObject;
                GameObject InventoryUICanvas = canvas.transform.Find("InventoryUICanvas(Clone)").gameObject;
                
                Destroy(inventoryUIPanel);
                Destroy(InventoryUICanvas);
                Destroy(inventoryUIKitchen);
            }
            
            InventoryUIPan inventoryUIPan = FindObjectOfType<InventoryUIPan>();
            if (inventoryUIPan != null)
            {
                Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
                GameObject inventoryUIPanel = canvas.transform.Find("InventoryUIPanel(Clone)").gameObject;
                GameObject InventoryUICanvas = canvas.transform.Find("InventoryUICanvas(Clone)").gameObject;
                
                _panTrigger = GameObject.Find("PanTrigger");
                CookingPan cookingPan = _panTrigger.GetComponent<CookingPan>();
                _panHandle = cookingPan.panHandle;

                _panHandle.SetActive(true);
                
                cookingPan.isFryingStarted = true;
                cookingPan.outerFry.SetActive(true);
                cookingPan.innerFry.SetActive(true);
                
                Destroy(inventoryUIPanel);
                Destroy(InventoryUICanvas);
                Destroy(inventoryUIKitchen);
            }

            // Enable the PotMixerBehavior to allow dragging the mixer
            if (mixer != null)
            {
                mixer.enabled = true;
                mixer.isDragging = true;
            } // Ensure that mixer is draggable now 

            // Debugging Inventory and Potion Mix
            GameManager.Instance.DebugInventory();
            GameManager.Instance.DebugPotionMix();
        }
    }
}

