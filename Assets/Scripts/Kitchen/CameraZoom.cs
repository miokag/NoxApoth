using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraZoom : MonoBehaviour
{
    private Camera mainCamera;
    private float defaultFOV; // Default camera field of view
    [SerializeField] private float zoomFOV = 30f; // Zoomed-in field of view
    [SerializeField] private float zoomSpeed = 5f; // Speed of the zoom transition

    [Header("Zoom Settings")]
    public Vector3 zoomOffset = new Vector3(0, 1, -2); // Editable zoom offset
    public Vector3 defaultPosition; // Default position (optional override)
    public GameObject BackMainKitchenButtonPrefab;

    public GameObject BackMainKitchenButton;
    private Canvas canvas;
    private Transform targetObject; // The object to focus on
    public bool isZoomedIn = false; // To track the zoom state
    private bool canZoomIn = true;

    void Start()
    {
        mainCamera = Camera.main;
        defaultFOV = mainCamera.fieldOfView;
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        // Initialize default position if not set in the Inspector
        if (defaultPosition == Vector3.zero)
        {
            defaultPosition = mainCamera.transform.position;
        }
    }

    void Update()
    {
        // Detect click and perform a raycast
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            // Check if zooming is allowed
            if (canZoomIn)
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    // Check if the clicked object has the "Utensil" tag
                    if (hit.collider.CompareTag("Utensil"))
                    {
                        targetObject = hit.transform;
                        isZoomedIn = true;
                        canZoomIn = false;  // Disable zooming in while transitioning
                        BackMainKitchenButton = Instantiate(BackMainKitchenButtonPrefab, canvas.transform);
                        Button backButton = BackMainKitchenButton.GetComponent<Button>();
                        backButton.onClick.AddListener(BackToMainKitchen);
                    }
                }
            }
        }

        // Smoothly adjust FOV and camera position
        if (isZoomedIn && targetObject != null)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position,
                                                         targetObject.position + zoomOffset,
                                                         Time.deltaTime * zoomSpeed);
        }
        else
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, defaultPosition, Time.deltaTime * zoomSpeed);
        }

        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, isZoomedIn ? zoomFOV : defaultFOV, Time.deltaTime * zoomSpeed);
    }

    void BackToMainKitchen()
    {
        isZoomedIn = false;
        targetObject = null;
        

        // Destroy the back button and re-enable zooming
        Destroy(BackMainKitchenButton);

        // Start the coroutine to delay zoom-in until the zoom-out is complete
        StartCoroutine(EnableZoomAfterDelay());
    }

    private IEnumerator EnableZoomAfterDelay()
    {
        // Wait until zoom-out is completed (you can adjust the delay as needed)
        yield return new WaitForSeconds(0.5f);  // Example delay to allow for smooth zoom-out

        // Re-enable zooming
        canZoomIn = true;
    }

}
