using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingPot : MonoBehaviour
{
    public static bool IsActive { get; private set; }
    
    private CameraZoom mainCamera; // Reference to the CameraZoom script
    public Canvas canvas;

    public GameObject inventoryPanelPrefab;
    public GameObject inventoryPanel;

    public GameObject bubbleSprites;
    public GameObject potFire;
    public GameObject PotSprite;
    public SpriteRenderer potSpriteRenderer;

    public Sprite[] potStates;

    [SerializeField] private GameObject inventoryEmptyPrefab;
    private GameObject inventoryEmpty;
    public GameObject inventoryUIPrefab;
    public GameObject inventoryUI;

    public PotMixerBehavior potMixer;

    public GameObject StoveOnUIPrefab;
    private GameObject StoveOnUI;
    private bool hasStoveOnUI = true;

    private GameObject UIManager;
    private InventoryUIKitchen inventoryUIScript;

    public bool isMixingStarted = false; // New flag to check if mixing has started
    private bool isStoveOn = false; // New flag to track if the stove is on

    private int finalMixCount;
    private CameraZoom _cameraZoom;

    void Start()
    {
        potSpriteRenderer = PotSprite.GetComponent <SpriteRenderer>();
        _cameraZoom = Camera.main.GetComponent<CameraZoom>();
        potMixer.enabled = false;
        hasStoveOnUI = false;
        mainCamera = GameObject.Find("Main Camera").GetComponent<CameraZoom>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        UIManager = GameObject.Find("UIManager");
    }

    public void EnableMixerForNextIngredient()
    {
        potMixer.enabled = true;
        potMixer.mixCount = 0;  // Reset the mix count
        potMixer.totalDistanceMoved = 0f; // Reset distance
        potMixer.previousPosition = potMixer.transform.position; // Reset previous position
        isMixingStarted = false;
        Debug.Log("Mixer enabled for the next ingredient.");
    }


    private void OnMouseDown()
    {
        IsActive = true;
        CookingPot.IsActive = false;
        bool inventoryStatus = GameManager.Instance.GetInventory();
        Debug.Log("Inventory status: " + inventoryStatus + ", Total Items: " + GameManager.Instance.Inventory.Count);

        // If mixing has started, run a different set of behaviors
        if (isMixingStarted)
        {
            Debug.Log("Player is mixing, so no inventory actions.");
            // Do something different when the player has started mixing
            HandleMixingStarted();
        }
        else
        {
            // Handle actions before mixing starts
            if (mainCamera != null && mainCamera.isZoomedIn && !hasStoveOnUI && inventoryStatus)
            {
                // set inactive back button
                _cameraZoom.BackMainKitchenButton.SetActive(false);
                
                // Instantiate StoveOnUI and set the flag to true
                StoveOnUI = Instantiate(StoveOnUIPrefab, canvas.transform);
                Debug.Log("Pot Turned On");
                hasStoveOnUI = true;
                isStoveOn = true; // Mark the stove as on
                GameManager.Instance.DebugInventory();
            }
            else if (!inventoryStatus)
            {
                // Only instantiate inventoryEmpty if it hasn't been instantiated yet AND if mixing hasn't started
                if (inventoryEmpty == null)
                {
                    inventoryEmpty = Instantiate(inventoryEmptyPrefab, canvas.transform);
                    Debug.Log("Inventory is empty");
                    // Optionally, you can add a timer to destroy it after a set time if necessary
                    StartCoroutine(DestroyInventoryEmptyAfterDelay(1f)); // Destroy after 2 seconds
                }
            }
        }

    }

    // Method to handle the behavior when mixing has started
    public void HandleMixingStarted()
    {
        Debug.Log("HandleMixingStarted() triggered.");
        Ingredient ingredient = GameManager.Instance.ingredientProcessed;
        Ingredient clonedIngredient = (Ingredient)ingredient.Clone(); // Use the cloned version
        finalMixCount = potMixer.mixCount;

        if (finalMixCount >= 10 && finalMixCount < 20)
        {
            clonedIngredient.currentProcessedState = Ingredient.ProcessedState.Simmered;
        }
        else if (finalMixCount >= 20 && finalMixCount < 50)
        {
            clonedIngredient.currentProcessedState = Ingredient.ProcessedState.Boiled;
        }
        else
        {
            clonedIngredient.currentProcessedState = Ingredient.ProcessedState.None;
        }

        finalMixCount = 0;
        potMixer.OnMixComplete();  // Ensure this is being reached
        potMixer.enabled = false;

        potSpriteRenderer.sprite = potStates[0];
        bubbleSprites.SetActive(false);
        potFire.SetActive(false);
        
        // Stove Reset
        isStoveOn = false;
        hasStoveOnUI = false;
        
        // Add the ingredient to the potion mix
        GameManager.Instance.AddToPotionMix(clonedIngredient);
        GameManager.Instance.DebugPotionMix();
    
        _cameraZoom.BackMainKitchenButton.SetActive(true);
    }

    // Coroutine to destroy inventoryEmpty after a delay
    private IEnumerator DestroyInventoryEmptyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (inventoryEmpty)
        {
            Destroy(inventoryEmpty);
            inventoryEmpty = null; // Reset to ensure it can be instantiated again if needed
        }
    }

    public void ShowInventoryItems()
    {
        if (inventoryUI)
        {
            inventoryUIScript = UIManager.AddComponent<InventoryUIKitchen>();
            inventoryUIScript.inventoryUICanvas = inventoryUI.GetComponent<Canvas>();
        }
    }
    
    
}

