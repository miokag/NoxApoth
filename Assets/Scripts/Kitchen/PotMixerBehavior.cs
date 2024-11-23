using UnityEngine;

public class PotMixerBehavior : MonoBehaviour
{
    public Transform parentTransform;
    private Vector3 offset;
    public bool isDragging = false;

    public Vector3 previousPosition;
    public Vector3 originalPosition;
    public float totalDistanceMoved = 0f;
    public int mixCount = 0;
    private float circleThreshold = 0.5f;
    
    public CookingPot cookingPot;

    private void Start()
    {
        previousPosition = transform.position;
        originalPosition = transform.position;
    }

    private void OnMouseDown()
    {
        if (enabled) // Ensure script is enabled before allowing drag
        {
            isDragging = true;
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z));
            offset = transform.position - mouseWorldPos;

            // Reset previous position to the current position to avoid carryover distances
            previousPosition = transform.position;
        }
    }
    
    private void OnMouseDrag()
    {
        if (isDragging && enabled) // Ensure script is enabled before allowing drag
        {
            //Debug.Log($"Mixer reset: mixCount = {mixCount}, totalDistanceMoved = {totalDistanceMoved}");
        
            cookingPot.isMixingStarted = true;

            // Calculate mouse position in world space
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z)) + offset;

            // Maintain Y position and clamp movement within parent bounds
            float fixedY = transform.position.y;
            Vector3 parentMin = parentTransform.position - parentTransform.localScale / 2f;
            Vector3 parentMax = parentTransform.position + parentTransform.localScale / 2f;

            mouseWorldPos.x = Mathf.Clamp(mouseWorldPos.x, parentMin.x, parentMax.x);
            mouseWorldPos.y = fixedY;
            mouseWorldPos.z = Mathf.Clamp(mouseWorldPos.z, parentMin.z, parentMax.z);

            transform.position = mouseWorldPos;

            // Update total movement and check if it exceeds threshold
            totalDistanceMoved += Vector3.Distance(previousPosition, transform.position);
            previousPosition = transform.position;
            
            if (totalDistanceMoved > circleThreshold)
            {
                totalDistanceMoved = 0f;
                mixCount++;
                if (mixCount >= 10 && mixCount < 20)
                {
                    cookingPot.potSpriteRenderer.sprite = cookingPot.potStates[1];
                }
                else if (mixCount >= 20 && mixCount < 50)
                {
                    cookingPot.potSpriteRenderer.sprite = cookingPot.potStates[2];
                }
            }
        }
    }


    private void OnMouseUp()
    {
        isDragging = false;
    }

    public void OnMixComplete()
    {
        Debug.Log("OnMixComplete() triggered.");
        isDragging = false;
        Debug.Log("Pot has been mixed successfully!");
        mixCount = 0;
        transform.position = originalPosition;  // Reset the position of the mixer
        totalDistanceMoved = 0f;

        // Prepare the mixer to accept another ingredient
        cookingPot.EnableMixerForNextIngredient();  // This method will be defined in CookingPot.
    }

}

