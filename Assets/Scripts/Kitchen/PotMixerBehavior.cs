using UnityEngine;

public class PotMixerBehavior : MonoBehaviour
{
    public Transform parentTransform;
    private Vector3 offset;
    private bool isDragging = false;

    private Vector3 previousPosition;
    private float totalDistanceMoved = 0f;
    public int mixCount = 0;
    private float circleThreshold = 0.5f;
    
    public CookingPot cookingPot;
    private void Start()
    {
        previousPosition = transform.position;
    }

    private void OnMouseDown()
    {
        if (enabled) // Ensure script is enabled before allowing drag
        {
            isDragging = true;
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z));
            offset = transform.position - mouseWorldPos;
        }
    }

    private void OnMouseDrag()
    {
        if (isDragging && enabled) // Ensure script is enabled before allowing drag
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z)) + offset;
            float fixedY = transform.position.y;
            Vector3 parentMin = parentTransform.position - parentTransform.localScale / 2f;
            Vector3 parentMax = parentTransform.position + parentTransform.localScale / 2f;

            mouseWorldPos.x = Mathf.Clamp(mouseWorldPos.x, parentMin.x, parentMax.x);
            mouseWorldPos.y = fixedY;
            mouseWorldPos.z = Mathf.Clamp(mouseWorldPos.z, parentMin.z, parentMax.z);

            transform.position = mouseWorldPos;

            totalDistanceMoved += Vector3.Distance(previousPosition, transform.position);
            previousPosition = transform.position;

            if (totalDistanceMoved > circleThreshold)
            {
                totalDistanceMoved = 0f;
                mixCount++;
                Debug.Log($"Mix count: {mixCount}");
                cookingPot.enabled = true;
                cookingPot.StartMixing();
            }
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;
    }

    private void OnMixComplete()
    {
        Debug.Log("Pot has been mixed successfully!");
        mixCount = 0;
    }
}

