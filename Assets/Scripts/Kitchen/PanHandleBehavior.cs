using System.Collections;
using UnityEngine;

public class PanHandleBehavior : MonoBehaviour
{
    private Vector3 originalPosition; // Store the original position of the pan (parent object)
    public float moveDistance = 0.2f; // How far the pan should move
    public float moveDuration = 0.1f; // Duration of the movement
    public CookingPan cookingPan;

    private bool isMoving = false; // Flag to prevent multiple clicks during movement
    public int moveCounter = 0; // Counter to track how many times the pan has moved

    void Start()
    {
        // Store the initial position of the parent object
        if (transform.parent != null)
        {
            originalPosition = transform.parent.localPosition;
        }
    }

    private void OnEnable()
    {
        // Ensure we aren't resetting the position each time unless necessary
        if (transform.parent != null && originalPosition == Vector3.zero)
        {
            originalPosition = transform.parent.localPosition;
        }
    }

    private void OnMouseDown()
    {
        // If the pan is not moving, allow the animation and track the counter
        if (!isMoving)
        {
            moveCounter++; // Increment the move counter
            Debug.Log("Pan moved " + moveCounter + " times.");
            StartCoroutine(MovePanAnimation());
        }
    }

    private IEnumerator MovePanAnimation()
    {
        // Set the flag to true so the player can't click during movement
        isMoving = true;

        // If the original position is not set, use the current position of the parent
        if (originalPosition == Vector3.zero && transform.parent != null)
        {
            originalPosition = transform.parent.localPosition;
        }

        // Target position for the parent object (moving to the left by moveDistance)
        Vector3 targetPosition = originalPosition + new Vector3(-moveDistance, 0, 0);

        // Move to the target position
        float elapsedTime = 0;
        while (elapsedTime < moveDuration)
        {
            transform.parent.localPosition = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.parent.localPosition = targetPosition; // Ensure it's exactly at the target position

        // Move back to the original position
        elapsedTime = 0;
        while (elapsedTime < moveDuration)
        {
            transform.parent.localPosition = Vector3.Lerp(targetPosition, originalPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.parent.localPosition = originalPosition; // Ensure it's exactly at the original position
        
        if (moveCounter >= 5 && moveCounter < 10)
        {
            cookingPan.PlayAnimationInnerFry("PanInnerFryBrown");
        }
        else if (moveCounter >= 10 && moveCounter < 20)
        {
            cookingPan.PlayAnimationInnerFry("PanInnerFryBurn");
        }

        // Set the flag back to false after the animation finishes
        isMoving = false;
    }
}
