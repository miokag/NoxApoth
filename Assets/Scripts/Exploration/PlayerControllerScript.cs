using UnityEngine;
using UnityEngine.InputSystem;  // New Input System namespace

public class PlayerControllerScript : MonoBehaviour
{
    public float speed;
    public float groundDist;

    public LayerMask terrainLayer;
    public Rigidbody rb;
    public SpriteRenderer sr;

    public Sprite rightSprite; // Sprite for moving right
    public Sprite frontSprite; // Sprite for moving forward
    public Sprite backSprite;  // Sprite for moving backward

    public InteractionHandler interactionHandler; // Reference to the InteractionHandler

    // Declare InputAction variables
    private InputAction moveAction;
    private InputAction interactAction; // Input action for interaction

    private void OnEnable()
    {
        // Initialize input actions with composite bindings for keyboard and gamepad
        moveAction = new InputAction("Move", binding: "<Gamepad>/leftStick");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        moveAction.Enable();
    }

    private void OnDisable()
    {
        // Clean up
        moveAction.Disable();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate; // Enable interpolation for smoother movement
    }

    private void FixedUpdate()
    {
        // Ground alignment
        RaycastHit hit;
        Vector3 castPos = transform.position;
        castPos.y += 1;
        if (Physics.Raycast(castPos, -transform.up, out hit, Mathf.Infinity, terrainLayer))
        {
            if (hit.collider != null)
            {
                Vector3 movePos = transform.position;
                movePos.y = hit.point.y + groundDist;
                transform.position = movePos;
            }
        }

        // Capture movement input from the new Input System
        Vector2 moveInput = moveAction.ReadValue<Vector2>();  // This will give a Vector2 for movement (x and y)

        // Create movement vector and normalize
        Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        rb.velocity = moveDir * speed;

        // Sprite direction logic for horizontal movement
        if (moveInput.x < 0)
        {
            sr.sprite = rightSprite;
            sr.flipX = true;
        }
        else if (moveInput.x > 0)
        {
            sr.sprite = rightSprite;
            sr.flipX = false;
        }

        // Sprite change for vertical movement
        if (moveInput.y > 0)
        {
            sr.sprite = backSprite;
        }
        else if (moveInput.y < 0)
        {
            sr.sprite = frontSprite;
        }

    }
}
