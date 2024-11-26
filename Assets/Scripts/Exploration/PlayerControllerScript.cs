using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerScript : MonoBehaviour
{
    public float speed;
    public float groundDist;
    public LayerMask terrainLayer;
    public Rigidbody rb;
    public Animator animator;

    // Declare InputAction variables
    private InputAction moveAction;

    private Vector2 lastDirection; // Store the last movement direction

    private void OnEnable()
    {
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
        moveAction.Disable();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
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

        // Capture movement input
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        
        // Check if there is movement
        if (moveInput.sqrMagnitude > 0.01f)
        {
            // Player is moving, update lastDirection and animator
            lastDirection = moveInput.normalized; // Save the normalized direction
            animator.SetBool("isIdle", false);    // Switch to walking state
            animator.SetFloat("moveX", moveInput.x);
            animator.SetFloat("moveY", moveInput.y);
        }
        else
        {
            // Player is idle, use lastDirection for idleX and idleY
            animator.SetBool("isIdle", true);     // Switch to idle state
            animator.SetFloat("idleX", lastDirection.x); // Use last direction for idle
            animator.SetFloat("idleY", lastDirection.y);
        }

        // Create movement vector and normalize
        Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        rb.velocity = moveDir * speed;
    }
}