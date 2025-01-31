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
    private GameObject _pestle;
    private CameraZoom _cameraZoom;
    private GameObject _uiManager;
    
    private FrontMortar _frontMortar;
    private CookingPot _pot;
    private CookingPan _pan;
    private void Start()
    {
        _pan = FindObjectOfType<CookingPan>();
        _frontMortar = FindObjectOfType<FrontMortar>();
        _uiManager = GameObject.Find("UIManager");
        mixer = GameObject.Find("Mixer").GetComponent<PotMixerBehavior>();
        GameObject camera = GameObject.Find("Main Camera");
        _cameraZoom = camera.GetComponent<CameraZoom>();
        _pot = FindObjectOfType<CookingPot>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("UtensilCollider"))
        {
            // Destroy the ingredient
            Destroy(gameObject);
            
            // Become current ingredient being processed
            GameManager.Instance.CurrentlyProcessing(ingredient);

            // Remove Inventory UI components
            InventoryUIKitchen inventoryUIKitchen = FindObjectOfType<InventoryUIKitchen>();
            if (inventoryUIKitchen != null && _cameraZoom.clickedObjectName == "Pot")
            {
                Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
                GameObject inventoryUIPanel = canvas.transform.Find("InventoryUIPanel(Clone)").gameObject;
                GameObject InventoryUICanvas = canvas.transform.Find("InventoryUICanvas(Clone)").gameObject;
                _pot.showingInventory = false;
                
                Destroy(inventoryUIPanel);
                Destroy(InventoryUICanvas);
                Destroy(inventoryUIKitchen);
            }
            
            InventoryUIPan inventoryUIPan = FindObjectOfType<InventoryUIPan>();
            if (inventoryUIPan != null && _cameraZoom.clickedObjectName == "Pan")
            {
                Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
                GameObject inventoryUIPanel = canvas.transform.Find("InventoryUIPanel(Clone)").gameObject;
                GameObject InventoryUICanvas = canvas.transform.Find("InventoryUICanvas(Clone)").gameObject;
                
                _panTrigger = GameObject.Find("PanTrigger");
                CookingPan cookingPan = _panTrigger.GetComponent<CookingPan>();
                _panHandle = cookingPan.panHandle;

                _panHandle.SetActive(true);
                _pan.showingInventory = false;
                
                cookingPan.isFryingStarted = true;
                cookingPan.outerFry.SetActive(true);
                cookingPan.innerFry.SetActive(true);
                
                Destroy(inventoryUIPanel);
                Destroy(InventoryUICanvas);
                Destroy(inventoryUIPan);
            }
            
            InventoryUIMortar inventoryUIMortar = FindObjectOfType<InventoryUIMortar>();
            if (inventoryUIMortar != null && _cameraZoom.clickedObjectName == "Mortar")
            {
                Debug.Log("Ingredient Collider Worked!");
                Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
                GameObject inventoryUIPanel = canvas.transform.Find("InventoryUIPanel(Clone)").gameObject;
                GameObject InventoryUICanvas = canvas.transform.Find("InventoryUICanvas(Clone)").gameObject;
    
                _pestle = GameObject.Find("Pestle");
                PestleBehavior pestleBehavior = _pestle.GetComponent<PestleBehavior>();

                pestleBehavior.isCrushing = true;
                pestleBehavior.pestleCollider.enabled = true;
                _frontMortar.showingInventory = false;
    
                Destroy(inventoryUIPanel);
                Destroy(InventoryUICanvas);
                Destroy(inventoryUIMortar);
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

