using UnityEngine;

public class HighlightableObject : MonoBehaviour, IHighlightable
{
    private Renderer objectRenderer3D;
    private SpriteRenderer objectRenderer2D;
    private Color originalColor;
    public Color highlightColor = Color.yellow;

    // Flag to determine if the object is highlightable
    public bool canHighlight = true;
    private Material materialInstance;

    private void Awake()
    {
        // Check if this object is a 3D or 2D object, and assign the material accordingly
        objectRenderer3D = GetComponent<Renderer>();
        objectRenderer2D = GetComponent<SpriteRenderer>();

        if (objectRenderer3D != null)
        {
            // Create a new material instance to avoid affecting shared materials
            materialInstance = new Material(objectRenderer3D.material);
            objectRenderer3D.material = materialInstance;

            originalColor = materialInstance.color;
            Debug.Log($"{name}: 3D Renderer found.");

            // Also apply to all child objects with a Renderer
            ApplyHighlightToChildren();
        }
        else if (objectRenderer2D != null)
        {
            originalColor = objectRenderer2D.color;
            Debug.Log($"{name}: 2D SpriteRenderer found.");
            
            // Apply to all child objects with a SpriteRenderer
            ApplyHighlightToChildren();
        }
        else
        {
            Debug.LogError($"{name}: No Renderer or SpriteRenderer found.");
        }
    }

    public void Highlight()
    {
        if (!canHighlight) return;

        if (objectRenderer3D != null)
        {
            Color highlightedColor = highlightColor;
            highlightedColor.a = materialInstance.color.a;  // Keep original transparency
            materialInstance.SetColor("_Color", highlightedColor); // Or "_BaseColor"
        }
        else if (objectRenderer2D != null)
        {
            Color highlightedColor = highlightColor;
            highlightedColor.a = objectRenderer2D.color.a; // Keep original transparency
            objectRenderer2D.color = highlightedColor;
        }

        // Apply highlight to all children as well
        ApplyHighlightToChildren();
    }

    public void Unhighlight()
    {
        if (objectRenderer3D != null)
        {
            // Reset color if it's a material with a texture or base map
            materialInstance.SetColor("_Color", originalColor);  // Or "_BaseColor" for URP/HDRP
        }
        else if (objectRenderer2D != null)
        {
            objectRenderer2D.color = originalColor;
        }

        // Unhighlight all children as well
        RemoveHighlightFromChildren();
    }

    // Method to apply highlight to all child renderers (3D and 2D)
    private void ApplyHighlightToChildren()
    {
        // Apply to 3D child objects with Renderers
        foreach (Renderer childRenderer in GetComponentsInChildren<Renderer>())
        {
            if (childRenderer != objectRenderer3D)  // Avoid applying to the parent itself
            {
                Material childMaterial = new Material(childRenderer.material);  // Clone material to avoid modifying shared materials
                childRenderer.material = childMaterial;

                // Set the color for the highlighted state
                Color highlightedColor = highlightColor;
                highlightedColor.a = childMaterial.color.a; // Keep original transparency
                childMaterial.SetColor("_Color", highlightedColor);
            }
        }

        // Apply to 2D child objects with SpriteRenderers
        foreach (SpriteRenderer childSprite in GetComponentsInChildren<SpriteRenderer>())
        {
            if (childSprite != objectRenderer2D)  // Avoid applying to the parent itself
            {
                Color highlightedColor = highlightColor;
                highlightedColor.a = childSprite.color.a; // Keep original transparency
                childSprite.color = highlightedColor;
            }
        }
    }


    // Method to remove highlight from all child renderers
    private void RemoveHighlightFromChildren()
    {
        // Remove highlight from 3D child objects
        foreach (Renderer childRenderer in GetComponentsInChildren<Renderer>())
        {
            if (childRenderer != objectRenderer3D)
            {
                Material childMaterial = childRenderer.material;
                childMaterial.SetColor("_Color", originalColor); // Or "_BaseColor" for URP/HDRP
            }
        }

        // Remove highlight from 2D child objects
        foreach (SpriteRenderer childSprite in GetComponentsInChildren<SpriteRenderer>())
        {
            if (childSprite != objectRenderer2D)
            {
                childSprite.color = originalColor;
            }
        }
    }

    public bool CanHighlight => canHighlight; // Expose the flag
}
