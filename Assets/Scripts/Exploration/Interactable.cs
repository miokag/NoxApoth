using UnityEngine;

public class Interactable : MonoBehaviour
{
    private bool hasInteracted = false;

    // You can customize this method to add logic on whether or not the object can be interacted with
    public bool CanInteract()
    {
        return !hasInteracted;  // Only interact if it hasn't been interacted with yet
    }

    public void Interact()
    {
        if (CanInteract())
        {
            Debug.Log("Object interacted with!");

            // Add interaction logic here, for example:
            // - Change object state (e.g., change color, destroy, etc.)
            // - Play animation, etc.

            // Set the interacted flag to true (or reset after a delay if needed)
            hasInteracted = true;
            
        }
        else
        {
            Debug.Log("Object has already been interacted with.");
        }
    }

    public void DestroyInteractable()
    {
        Destroy(gameObject);
    }
}
