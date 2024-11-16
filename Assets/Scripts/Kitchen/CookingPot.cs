using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingPot : MonoBehaviour
{
    private CameraZoom mainCamera; // Reference to the CameraZoom script
    private Canvas canvas;

    [SerializeField] private GameObject inventoryEmptyPrefab;
    private GameObject inventoryEmpty;

    public GameObject StoveOnUIPrefab;
    private GameObject StoveOnUI;
    private bool hasStoveOnUI = true;

    void Start()
    {
        hasStoveOnUI = false;
        mainCamera = GameObject.Find("Main Camera").GetComponent<CameraZoom>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    private void OnMouseDown()
    {
        bool inventoryStatus = GameManager.Instance.GetInventory();
        Debug.Log("Inventory status: " + inventoryStatus + ", Total Items: " + GameManager.Instance.Inventory.Count);

        // Directly check if the camera is zoomed in when the pot is clicked
        if (mainCamera != null && mainCamera.isZoomedIn && !hasStoveOnUI && inventoryStatus)
        {
            // Instantiate StoveOnUI and set the flag to true
            StoveOnUI = Instantiate(StoveOnUIPrefab, canvas.transform);
            Debug.Log("Pot Turned On");
            hasStoveOnUI = true;
            GameManager.Instance.DebugInventory();
        }
        else if (!inventoryStatus)
        {
            // Only instantiate inventoryEmpty if it hasn't been instantiated yet
            if (inventoryEmpty == null)
            {
                inventoryEmpty = Instantiate(inventoryEmptyPrefab, canvas.transform);
                Debug.Log("Inventory is empty");
                // Optionally, you can add a timer to destroy it after a set time if necessary
                StartCoroutine(DestroyInventoryEmptyAfterDelay(2f)); // Destroy after 2 seconds
            }
        }
    }


    // Coroutine to destroy inventoryEmpty after a delay
    private IEnumerator DestroyInventoryEmptyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (inventoryEmpty != null)
        {
            Destroy(inventoryEmpty);
            inventoryEmpty = null; // Reset to ensure it can be instantiated again if needed
        }
    }
}
