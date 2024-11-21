using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingPot : MonoBehaviour
{
    private CameraZoom mainCamera; // Reference to the CameraZoom script
    public Canvas canvas;

    public GameObject inventoryPanelPrefab;
    public GameObject inventoryPanel;

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
        _cameraZoom = Camera.main.GetComponent<CameraZoom>();
        potMixer.enabled = false;
        hasStoveOnUI = false;
        mainCamera = GameObject.Find("Main Camera").GetComponent<CameraZoom>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        UIManager = GameObject.Find("UIManager");
    }

    private void OnMouseDown()
    {
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
                // Destroy back button
                Destroy(_cameraZoom.BackMainKitchenButton);
                
                // Instantiate StoveOnUI and set the flag to true
                StoveOnUI = Instantiate(StoveOnUIPrefab, canvas.transform);
                Debug.Log("Pot Turned On");
                hasStoveOnUI = true;
                isStoveOn = true; // Mark the stove as on
                this.enabled = false;
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
    private void HandleMixingStarted()
    {
        finalMixCount = potMixer.mixCount;
        if (finalMixCount >= 10 && finalMixCount < 20)
        {
            GameManager.Instance.ingredientProcessed.currentProcessedState = Ingredient.ProcessedState.Simmered;
        }
        else if (finalMixCount >= 20 && finalMixCount < 50)
        {
            GameManager.Instance.ingredientProcessed.currentProcessedState = Ingredient.ProcessedState.Boiled;
        }
        potMixer.enabled = false;
        Debug.Log("Cooking finished, mix count: " + finalMixCount);
        GameManager.Instance.DebugPotionMix();
    }

    public void StartMixing()
    {
        // Only allow mixing if the stove is turned on
        if (!isStoveOn)
        {
            Debug.Log("Can't start mixing. The stove is off.");
            return;
        }

        // This method can be called when the mixing process is triggered
        isMixingStarted = true;
        Debug.Log("Mixing started. You can now interact with the pot differently.");
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
