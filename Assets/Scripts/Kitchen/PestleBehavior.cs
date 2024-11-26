using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PestleBehavior : MonoBehaviour
{
    public bool isCrushing;

    public GameObject[] ingredientStateSprite;
    public Sprite[] ingredientStates;
    
    private Canvas _canvas;
    private Camera _mainCamera;
    public Rigidbody rigidbody;
    public Collider pestleCollider;
    
    public CameraZoom cameraZoom;
    private Vector3 _originalPosition;
    private Quaternion _originalRotation;
    private float initialZPosition;
    
    private FrontMortar _frontMortar;
    private GameObject _frontSprite;

    private bool isBeingHeld;
    private Vector3 offset;
    
    private bool isMovingDown = false; // Tracks if the pestle is moving down
    
    public float maxDragSpeed = 10f;
    public float downwardThreshold = 0.9f; // Velocity threshold for detecting downward movement
    public int crushCount = 0; // Tracks the number of valid crushes
    private void Start()
    {
        _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        
        // Get references to components
        _mainCamera = Camera.main;
        rigidbody = GetComponent<Rigidbody>();
        pestleCollider = GetComponent<Collider>();

        // Store the original position and rotation of the pestle
        _originalPosition = transform.position;
        _originalRotation = transform.rotation;

        // Set collision detection mode to Continuous for better collision handling
        rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Adjust Rigidbody drag properties for better control when dropped
        rigidbody.drag = 2f;

        // Fetch CameraZoom component and wait for zoom state to trigger UI setup
        cameraZoom = _mainCamera.GetComponent<CameraZoom>();

        // Start a coroutine to check zoom state changes
        StartCoroutine(WaitForZoomAndSetInventoryUI());
        
        // Fetch Front Mortar script
        GameObject mortarCollider = GameObject.Find("MortarColliders");

        Transform frontMortar = mortarCollider.transform.Find("FrontFace");
        _frontMortar = frontMortar.GetComponent<FrontMortar>();
        _frontSprite = GameObject.Find("Mortar");
        
    }
    
    public IEnumerator WaitForZoomAndSetInventoryUI()
    {
        yield return new WaitUntil(() => cameraZoom.clickedObjectName == "Mortar" );
    }
    
    public void ApplyIngredientColor()
    {
        // Get the ingredient from the GameManager
        Ingredient ingredient = GameManager.Instance.ingredientProcessed;

        // Ensure the ingredient exists and has a valid color
        if (ingredient != null && ColorUtility.TryParseHtmlString(ingredient.hexColor, out Color color))
        {
            foreach (var sprite in ingredientStateSprite)
            {
                // Apply the color to the SpriteRenderer
                sprite.GetComponent<SpriteRenderer>().color = color;
            }
            Debug.Log($"Applied color {ingredient.hexColor} to ingredient states.");
        }
        else
        {
            Debug.LogWarning("Invalid ingredient or hex color.");
        }
    }
    
    private void OnMouseDown()
    {
        if (pestleCollider.enabled)
        {
            rigidbody.isKinematic = false;
            _frontSprite.SetActive(false);
            ApplyIngredientColor();

            // When the pestle is clicked, start dragging it
            isBeingHeld = true;

            // Disable gravity while dragging, to stop the object from falling while held
            rigidbody.useGravity = false;

            // Freeze rotation to prevent it from spinning or flipping
            rigidbody.freezeRotation = true;

            // Straighten the pestle's rotation when picked up (reset it to a specific rotation)
            transform.rotation = Quaternion.Euler(0f, 0f, 0f); // Adjust as necessary for your setup

            // Calculate the offset of the click relative to the object's position in world space
            offset = transform.position - GetMouseWorldPosition();

            // Store the original Z position (locking it to this value)
            initialZPosition = transform.position.z;
        }
    }


    
    private void OnMouseDrag()
    {
        if (isBeingHeld)
        {
            // Calculate the target position in world space
            Vector3 targetPosition = GetMouseWorldPosition() + offset;

            // Ensure the Z position stays locked to the initial Z value
            targetPosition.z = initialZPosition;

            // Move the pestle using Rigidbody's MovePosition to avoid tunneling
            Vector3 moveDirection = targetPosition - transform.position;

            // Limit the speed of movement to prevent tunneling
            if (moveDirection.magnitude > maxDragSpeed * Time.deltaTime)
            {
                moveDirection = moveDirection.normalized * maxDragSpeed * Time.deltaTime;
            }

            // Move the pestle smoothly by using Rigidbody's position
            rigidbody.MovePosition(transform.position + moveDirection);
        }
    }

    
    private void OnMouseUp()
    {
        _frontSprite.SetActive(true);

        // Stop dragging
        isBeingHeld = false;

        // Enable gravity so the pestle will fall naturally
        rigidbody.useGravity = true;

        // Unfreeze rotation, but you may want to freeze it on a specific axis to prevent flipping
        rigidbody.freezeRotation = false;

        // If the pestle's rotation is unstable (flipping), you could lock its rotation temporarily
        // For example, locking rotation on the X and Z axis to prevent flipping:
        if (transform.eulerAngles.x > 45f || transform.eulerAngles.x < -45f)
        {
            transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f); // Keep it upright on X axis
        }

        // Zero out the velocity to avoid unexpected movement
        rigidbody.velocity = Vector3.zero;

        // Reset the position and rotation to the original ones when the mouse is released
        transform.position = _originalPosition; // Reset to the original position
        transform.rotation = _originalRotation; // Reset to the original rotation

        // Prevent any forward movement in the Z direction once released
        // Keep the Z position fixed
        Vector3 currentPosition = transform.position;
        currentPosition.z = initialZPosition; // Keep Z locked to the original Z position
        transform.position = currentPosition;
    }





    
    private void FixedUpdate()
    {
        // Check if the pestle is moving downward
        isMovingDown = rigidbody.velocity.y < downwardThreshold;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mortar") && isMovingDown && isBeingHeld)
        {
            // Increment the crush count continuously as long as it's moving downwards and in contact with the mortar
            crushCount++;
            Debug.Log("Crush count: " + crushCount);
        }
        
        if (crushCount >= 5 && crushCount < 10)
        {
            foreach (var sprite in ingredientStateSprite)
            {
                ApplyIngredientColor();
                sprite.GetComponent<SpriteRenderer>().sprite = ingredientStates[1];
            }
        }
        else if (crushCount >= 10 && crushCount < 15)
        {
            foreach (var sprite in ingredientStateSprite)
            {
                ApplyIngredientColor();
                sprite.GetComponent<SpriteRenderer>().sprite = ingredientStates[2];
            }
        }
        
        else if (crushCount >= 15 && crushCount < 20)
        {
            foreach (var sprite in ingredientStateSprite)
            {
                ApplyIngredientColor();
                sprite.GetComponent<SpriteRenderer>().sprite = ingredientStates[3];
            }
        }
        
        else if (crushCount >= 20 && crushCount < 25)
        {
            foreach (var sprite in ingredientStateSprite)
            {
                ApplyIngredientColor();
                sprite.GetComponent<SpriteRenderer>().sprite = ingredientStates[4];
            }
        }
    }
    
    private Vector3 GetMouseWorldPosition()
    {
        // Convert the mouse position to world space, assuming the pestle moves along a flat plane (e.g., XZ plane)
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = _mainCamera.WorldToScreenPoint(transform.position).z;
        return _mainCamera.ScreenToWorldPoint(mouseScreenPosition);
    }
}