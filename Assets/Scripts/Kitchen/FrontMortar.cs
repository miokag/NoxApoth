using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrontMortar : MonoBehaviour
{
    public GameObject mortarGameObject;
    public GameObject inventoryEmptyPrefab;
    public GameObject inventoryUIPrefab;
    public GameObject inventoryPanelPrefab;
    
    private CameraZoom _cameraZoom;
    private Canvas _canvas;
    private PestleBehavior _pestleBehavior;

    private GameObject _inventoryUI;
    private GameObject _inventoryPanel;

    private GameObject _inventoryEmpty;
    private GameObject _uiManager;
    private InventoryUIMortar _inventoryUIScript;

    private Button _backMainKitchenButton;
    private void Start()
    {
        mortarGameObject = GameObject.Find("Mortar");
        _uiManager = GameObject.Find("UIManager");
        
        _pestleBehavior = GameObject.Find("Pestle").GetComponent<PestleBehavior>();
        _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        _cameraZoom = GameObject.Find("Main Camera").GetComponent<CameraZoom>();
        
    }

    private void OnMouseDown()
    {
        if (_pestleBehavior.isCrushing)
        {
            FinishGrinding();
        }
        else if (_cameraZoom.clickedObjectName == "Mortar" && !_pestleBehavior.isCrushing)
        {
            bool inventoryStatus = GameManager.Instance.GetInventory();
            
            if (inventoryStatus)
            {
                _pestleBehavior.cameraZoom.BackMainKitchenButton.SetActive(false);
                Debug.Log("Mortar Zoomed In");
                GameManager.Instance.DebugInventory();
                ShowInventoryItems();
            }
            else
            {
                if (_inventoryEmpty == null)
                {
                    _inventoryEmpty = Instantiate(inventoryEmptyPrefab, _canvas.transform);
                    Debug.Log("Inventory is empty");

                    StartCoroutine(DestroyInventoryEmptyAfterDelay(1f)); // Destroy after 2 seconds
                }
            }
        }
    }
    
    private void FinishGrinding()
    {
        Debug.Log("Finish Grinding");
        
        int finalCount;
        finalCount = _pestleBehavior.crushCount;
        
        Debug.Log("Final Count: " + finalCount);
        
        Ingredient ingredient = GameManager.Instance.ingredientProcessed;
        Ingredient clonedIngredient = (Ingredient)ingredient.Clone();
        
        if (finalCount >= 10 && finalCount < 15)
        {
            clonedIngredient.currentProcessedState = Ingredient.ProcessedState.Powdered;
        }
        
        else if (finalCount >= 15 && finalCount < 20)
        {
            clonedIngredient.currentProcessedState = Ingredient.ProcessedState.Powdered;
        }
        
        else if (finalCount >= 20 && finalCount < 25)
        {
            clonedIngredient.currentProcessedState = Ingredient.ProcessedState.Paste;
        }

        _pestleBehavior.pestleCollider.enabled = false;
        _pestleBehavior.rigidbody.isKinematic = true;
        _pestleBehavior.crushCount = 0;
        finalCount = 0;
        foreach (var sprite in _pestleBehavior.ingredientStateSprite)
        {
            sprite.GetComponent<SpriteRenderer>().sprite = _pestleBehavior.ingredientStates[0];
        }
        
        _pestleBehavior.isCrushing = false;
        
        // Add the ingredient to the potion mix
        GameManager.Instance.AddToPotionMix(clonedIngredient);
        GameManager.Instance.DebugPotionMix();
        
        _pestleBehavior.cameraZoom.BackMainKitchenButton.SetActive(true);
        //_backMainKitchenButton.onClick.AddListener(OnBackButtonClick);
    }
    
    void OnBackButtonClick()
    {
        Destroy(_inventoryPanel);
        Destroy(_inventoryUI);
        Destroy(_inventoryUIScript);

    }
    
    private IEnumerator DestroyInventoryEmptyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_inventoryEmpty)
        {
            Destroy(_inventoryEmpty);
            _inventoryEmpty = null; // Reset to ensure it can be instantiated again if needed
        }
    }
    
    public void ShowInventoryItems()
    {
        // Instantiates inventory UI (slots) and panel
        _inventoryPanel = Instantiate(inventoryPanelPrefab, _canvas.transform);
        _inventoryUI = Instantiate(inventoryUIPrefab, _canvas.transform);
        
        // Adjust inventory UI Transform
        RectTransform inventoryRect = _inventoryUI.GetComponent<RectTransform>();
        inventoryRect.anchoredPosition = new Vector2(150, 0); // Adjust X and Y values as needed
        
        // Adjust inventory panel Transform
        RectTransform inventoryPanelRect = _inventoryPanel.GetComponent<RectTransform>();
        inventoryPanelRect.anchoredPosition = new Vector2(150, 100); // Adjust X and Y values as needed
        
        // Enable the mortar collision for ingredient collision
        mortarGameObject.SetActive(true);
        Collider mortarCollision = mortarGameObject.GetComponent<Collider>();
        mortarCollision.isTrigger = false;
        
        // Add InventoryUIMortar script
        if (_inventoryUI)
        {
            _inventoryUIScript = _uiManager.AddComponent<InventoryUIMortar>();
            _inventoryUIScript.inventoryUICanvas = _inventoryUI.GetComponent<Canvas>();
        }
    }
    
}
