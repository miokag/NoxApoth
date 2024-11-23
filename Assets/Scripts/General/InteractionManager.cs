using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public Camera playerCamera;
    public float raycastDistance = 5f;

    private IHighlightable currentHighlightable;

    private void Update()
    {
        HandleHighlighting();
    }

    private void HandleHighlighting()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
        {
            IHighlightable highlightable = hit.collider.GetComponent<IHighlightable>();
            if (highlightable != null && highlightable.CanHighlight) // Check if highlight is allowed
            {
                if (currentHighlightable != highlightable)
                {
                    currentHighlightable?.Unhighlight();
                    highlightable.Highlight();
                    currentHighlightable = highlightable;
                }
            }
            else
            {
                currentHighlightable?.Unhighlight();
                currentHighlightable = null;
            }
        }
        else
        {
            currentHighlightable?.Unhighlight();
            currentHighlightable = null;
        }
    }
}