using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    
    private bool isShowingVisuals;
    private Transform potionPanel;
    public Animator liquidAnimator;
    private HighlightableObject _thisHighlightableObject;
    
    private void Start()
    {
        mortarGameObject = GameObject.Find("Mortar");
        _uiManager = GameObject.Find("UIManager");
        
        _pestleBehavior = GameObject.Find("Pestle").GetComponent<PestleBehavior>();
        _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        _cameraZoom = GameObject.Find("Main Camera").GetComponent<CameraZoom>();
        
        isShowingVisuals = false;
        potionPanel = _canvas.transform.Find("PotionPanel");
        _thisHighlightableObject = GetComponent<HighlightableObject>();
        
    }
    
    public void PlayLiquidAnimationDirectly(string animationStateName)
    {
        liquidAnimator.Play(animationStateName);
    }

    private void OnMouseDown()
    {
        bool inventoryStatus = GameManager.Instance.GetInventory();
        
        if (_pestleBehavior.isCrushing)
        {
            FinishGrinding();
        }
        else if (isShowingVisuals == true)
        {
            Debug.Log("Player is showing, so no inventory actions.");
        }
        else if (GameManager.Instance.PotionMix.Count == 3 && inventoryStatus)
        {
            _inventoryEmpty = Instantiate(inventoryEmptyPrefab, _canvas.transform);
            TextMeshProUGUI inventoryText = _inventoryEmpty.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
            inventoryText.text = "Potion Bottle is Full";
            Debug.Log("Inventory is empty");
            // Optionally, you can add a timer to destroy it after a set time if necessary
            StartCoroutine(DestroyInventoryEmptyAfterDelay(1f));
        }
        else if (_cameraZoom.clickedObjectName == "Mortar" && !_pestleBehavior.isCrushing)
        {
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
        StartCoroutine(PotionVisuals());
    }
    
    public IEnumerator PotionVisuals()
    {
        isShowingVisuals = true;
        int potionMixCount = GameManager.Instance.PotionMix.Count;
        _thisHighlightableObject.Unhighlight();
        
        // Enable the potion panel to make it visible
        potionPanel.gameObject.SetActive(true);
        if (potionMixCount == 1) PlayAnimationDirectly("LiquidPotionAnim");
        else if (potionMixCount == 2) PlayAnimationDirectly("LiquidPotionAnim2");
        else if (potionMixCount == 3) PlayAnimationDirectly("LiquidPotionAnim3");
        
        // Wait for 3 seconds
        yield return new WaitForSeconds(1.5f);

        // Disable the potion panel to hide it
        potionPanel.gameObject.SetActive(false);
        isShowingVisuals = false;
        _thisHighlightableObject.Highlight();
    }
    
    public void PlayAnimationDirectly(string animationStateName)
    {
        liquidAnimator.Play(animationStateName);
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
