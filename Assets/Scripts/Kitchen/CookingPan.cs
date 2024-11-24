using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingPan : MonoBehaviour
{
    public static bool IsActive { get; private set; }

    private CameraZoom _mainCamera; // Reference to the CameraZoom script
    public Canvas canvas;

    public GameObject inventoryPanelPrefab;
    public GameObject inventoryPanel;

    [SerializeField] private GameObject inventoryEmptyPrefab;
    private GameObject _inventoryEmpty;
    public GameObject inventoryUIPrefab;
    public GameObject inventoryUI;
    
    public GameObject stoveOnUIPrefab;
    private GameObject _stoveOnUI;
    private bool _hasStoveOnUI = true;

    private GameObject _uiManager;
    private InventoryUIPan _inventoryUIScript;

    public bool isFryingStarted = false; // New flag to check if mixing has started
    private bool _isStoveOn = false; // New flag to track if the stove is on

    private int _finalMixCount;
    private CameraZoom _cameraZoom;
    
    public GameObject outerFry;
    public Animator outerFryAnimator;
    
    public GameObject innerFry;
    public Animator innerFryAnimator;

    public GameObject panFire;
    public GameObject panHandle;
    private PanHandleBehavior _panHandleBehavior;

    public GameObject panSprite;
    private SpriteRenderer _panSpriteRenderer;
    
    
    void Start()
    {
        panHandle.SetActive(false);
        
        _panHandleBehavior = panHandle.GetComponent<PanHandleBehavior>();
        outerFryAnimator = outerFry.GetComponent<Animator>();
        innerFryAnimator = innerFry.GetComponent<Animator>();
        
        _panSpriteRenderer = panSprite.GetComponent<SpriteRenderer>();
        
        IsActive = true;
        CookingPan.IsActive = false;
        _cameraZoom = Camera.main.GetComponent<CameraZoom>();
        _hasStoveOnUI = false;
        _mainCamera = GameObject.Find("Main Camera").GetComponent<CameraZoom>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        _uiManager = GameObject.Find("UIManager");
    }
    
    public void PlayAnimationInnerFry (string animationStateName)
    {
        innerFryAnimator.Play(animationStateName);
    }
    
    private IEnumerator PlayInnerFryAnimationAfterDelay(float delay, string animationStateName)
    {
        if (isFryingStarted)
        {
            yield return new WaitForSeconds(delay);
            PlayAnimationInnerFry(animationStateName);
        }
    }

    private void HandleFrying()
    {
        int finalCount;
        finalCount = _panHandleBehavior.moveCounter;
        
        Ingredient ingredient = GameManager.Instance.ingredientProcessed;
        Ingredient clonedIngredient = (Ingredient)ingredient.Clone();
        
        if (finalCount >= 5  && finalCount < 10)
        {
            clonedIngredient.currentProcessedState = Ingredient.ProcessedState.Cooked;
        }
        else if (finalCount >= 10 && finalCount < 20)
        {
            clonedIngredient.currentProcessedState = Ingredient.ProcessedState.Burned;
        }

        finalCount = 0;
        panHandle.SetActive(false);
        _panHandleBehavior.moveCounter = 0;
        panFire.SetActive(false);
        isFryingStarted = false;
        
        // Stove Reset
        _isStoveOn = false;
        _hasStoveOnUI = false;
        
        innerFry.SetActive(false);
        outerFry.SetActive(false);
        
        // Add the ingredient to the potion mix
        GameManager.Instance.AddToPotionMix(clonedIngredient);
        GameManager.Instance.DebugPotionMix();
        
        _cameraZoom.BackMainKitchenButton.SetActive(true);
    }
    
    private void OnMouseDown()
    {
        bool inventoryStatus = GameManager.Instance.GetInventory();
        Debug.Log("Inventory status: " + inventoryStatus + ", Total Items: " + GameManager.Instance.Inventory.Count);

        // If mixing has started, run a different set of behaviors
        if (isFryingStarted)
        {
            Debug.Log("Player is frying, so no inventory actions.");
            HandleFrying();
        }
        else
        {
            // Handle actions before mixing starts
            if (_mainCamera != null && _mainCamera.isZoomedIn && _mainCamera.clickedObjectName == "Pan" && !_hasStoveOnUI && inventoryStatus)
            {
                // set inactive back button
                _cameraZoom.BackMainKitchenButton.SetActive(false);
                
                // Instantiate StoveOnUI and set the flag to true
                _stoveOnUI = Instantiate(stoveOnUIPrefab, canvas.transform);
                Debug.Log("Cooking Pan Running");
                _hasStoveOnUI = true;
                _isStoveOn = true; // Mark the stove as on
                
                GameManager.Instance.DebugInventory();
            }
            else if (!inventoryStatus)
            {
                // Only instantiate inventoryEmpty if it hasn't been instantiated yet AND if mixing hasn't started
                if (_inventoryEmpty == null)
                {
                    _inventoryEmpty = Instantiate(inventoryEmptyPrefab, canvas.transform);
                    Debug.Log("Inventory is empty");
                    // Optionally, you can add a timer to destroy it after a set time if necessary
                    StartCoroutine(DestroyInventoryEmptyAfterDelay(1f)); // Destroy after 2 seconds
                }
            }
        }
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
        if (inventoryUI)
        {
            _inventoryUIScript = _uiManager.AddComponent<InventoryUIPan>();
            _inventoryUIScript.inventoryUICanvas = inventoryUI.GetComponent<Canvas>();
        }
    }
}
