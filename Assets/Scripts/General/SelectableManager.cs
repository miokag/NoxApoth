using UnityEngine;

public class SelectableManager : MonoBehaviour
{
    public Camera playerCamera; // Reference to the player's camera
    public float raycastDistance = 10f; // Max distance for raycasting
    private GameObject currentHoveredObject; // Currently hovered object
    private Material normalMaterial; // Reference to the normal material for 3D objects
    public Material highlightMaterial; // Reference to the highlight material for 3D objects
    private SpriteRenderer currentHoveredSpriteRenderer; // For 2D objects
    public Color highlightColor = Color.yellow; // Highlight color for 2D

    void Update()
    {
        HandleHover();
    }

    void HandleHover()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Raycasting for 3D objects
        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            if (hit.collider.CompareTag("Selectable") || hit.collider.CompareTag("Utensil"))
            {
                GameObject hitObject = hit.collider.gameObject;

                // If hovering over a new object
                if (currentHoveredObject != hitObject)
                {
                    ResetHoveredObject();

                    currentHoveredObject = hitObject;
                    MeshRenderer renderer = hitObject.GetComponent<MeshRenderer>();

                    // For 3D objects with MeshRenderer
                    if (renderer != null)
                    {
                        normalMaterial = renderer.material;
                        renderer.material = highlightMaterial;
                    }

                    // For 2D objects with SpriteRenderer
                    else
                    {
                        currentHoveredSpriteRenderer = hitObject.GetComponent<SpriteRenderer>();
                        if (currentHoveredSpriteRenderer != null)
                        {
                            Color originalColor = currentHoveredSpriteRenderer.color;
                            currentHoveredSpriteRenderer.color = highlightColor;
                        }
                        // Highlight the children (if any)
                        HighlightChildren(hitObject);
                    }
                }
                return;
            }
        }

        // Raycasting for 2D objects (if using 2D colliders)
        RaycastHit2D hit2D = Physics2D.Raycast(playerCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, raycastDistance);
        if (hit2D.collider != null && hit2D.collider.CompareTag("Selectable"))
        {
            GameObject hitObject = hit2D.collider.gameObject;

            // If hovering over a new object
            if (currentHoveredObject != hitObject)
            {
                ResetHoveredObject();

                currentHoveredObject = hitObject;
                SpriteRenderer spriteRenderer = hitObject.GetComponent<SpriteRenderer>();

                // For 2D objects with SpriteRenderer
                if (spriteRenderer != null)
                {
                    Color originalColor = spriteRenderer.color;
                    spriteRenderer.color = highlightColor;
                    currentHoveredSpriteRenderer = spriteRenderer;
                }

                // Highlight the children (if any)
                HighlightChildren(hitObject);
            }
            return;
        }

        // Reset hover if no object is selected
        ResetHoveredObject();
    }

    // Highlight all child objects of the hovered object (both 3D and 2D)
    void HighlightChildren(GameObject parentObject)
    {
        // For 3D children (MeshRenderer)
        MeshRenderer[] childRenderers = parentObject.GetComponentsInChildren<MeshRenderer>();
        foreach (var renderer in childRenderers)
        {
            if (renderer != null)
            {
                normalMaterial = renderer.material; // Store original material
                renderer.material = highlightMaterial; // Apply highlight material
            }
        }

        // For 2D children (SpriteRenderer)
        SpriteRenderer[] childSpriteRenderers = parentObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (var spriteRenderer in childSpriteRenderers)
        {
            if (spriteRenderer != null)
            {
                Color originalColor = spriteRenderer.color; // Store original color
                spriteRenderer.color = highlightColor; // Apply highlight color
            }
        }
    }

    void ResetHoveredObject()
    {
        if (currentHoveredObject != null)
        {
            // Reset the parent
            if (currentHoveredObject.GetComponent<MeshRenderer>() != null)
            {
                MeshRenderer renderer = currentHoveredObject.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.material = normalMaterial; // Reset 3D material
                }
            }
            else if (currentHoveredSpriteRenderer != null)
            {
                currentHoveredSpriteRenderer.color = Color.white; // Reset 2D color to original
            }

            // Reset the children
            ResetChildren(currentHoveredObject);

            currentHoveredObject = null;
        }
    }

    // Reset the children of the hovered object (both 3D and 2D)
    void ResetChildren(GameObject parentObject)
    {
        // For 3D children (MeshRenderer)
        MeshRenderer[] childRenderers = parentObject.GetComponentsInChildren<MeshRenderer>();
        foreach (var renderer in childRenderers)
        {
            if (renderer != null)
            {
                renderer.material = normalMaterial; // Reset 3D material
            }
        }

        // For 2D children (SpriteRenderer)
        SpriteRenderer[] childSpriteRenderers = parentObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (var spriteRenderer in childSpriteRenderers)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.white; // Reset 2D color to original
            }
        }
    }
}