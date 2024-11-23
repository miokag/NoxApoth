using UnityEngine;

public class HighlightableObject : MonoBehaviour, IHighlightable
{
    private Renderer objectRenderer3D;
    private SpriteRenderer objectRenderer2D;
    private Color originalColor;
    public Color highlightColor = Color.yellow;

    // Flag to determine if the object is highlightable
    public bool canHighlight = true;

    private void Awake()
    {
        objectRenderer3D = GetComponent<Renderer>();
        objectRenderer2D = GetComponent<SpriteRenderer>();

        if (objectRenderer3D != null)
        {
            originalColor = objectRenderer3D.material.color;
            Debug.Log($"{name}: 3D Renderer found.");
        }
        else if (objectRenderer2D != null)
        {
            originalColor = objectRenderer2D.color;
            Debug.Log($"{name}: 2D SpriteRenderer found.");
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
            objectRenderer3D.material.color = highlightColor;
        }
        else if (objectRenderer2D != null)
        {
            objectRenderer2D.color = highlightColor;
        }
    }

    public void Unhighlight()
    {
        if (objectRenderer3D != null)
        {
            objectRenderer3D.material.color = originalColor;
        }
        else if (objectRenderer2D != null)
        {
            objectRenderer2D.color = originalColor;
        }
    }

    public bool CanHighlight => canHighlight; // Expose the flag
}