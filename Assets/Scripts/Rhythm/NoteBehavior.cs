using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class NoteBehavior : MonoBehaviour, IPointerClickHandler
{
    public GameObject ringPrefab; // Assign your ring prefab in the inspector
    public GameObject ringInstance;
    public bool isOtherNote;

    [SerializeField] RhythmSceneManager rhythmSceneManager;

    [SerializeField] private float shrinkDuration = 1.0f; // Time in seconds for the ring to shrink
    [SerializeField] private float minHitThreshold = 0.4f; // Minimum allowable size difference
    [SerializeField] private float maxHitThreshold = 0.6f; // Maximum allowable size difference

    void Start()
    {
        GameObject rhythmSceneManagerGameObject = GameObject.Find("RhythmSceneManager");
        rhythmSceneManager = rhythmSceneManagerGameObject?.GetComponent<RhythmSceneManager>();

        if (ringPrefab != null)
        {
            // Instantiate the ring as a child of the note
            ringInstance = Instantiate(ringPrefab, transform.position, Quaternion.identity, transform);

            RectTransform ringRectTransform = ringInstance.GetComponent<RectTransform>();
            RectTransform noteRectTransform = GetComponent<RectTransform>();

            if (ringRectTransform != null && noteRectTransform != null)
            {
                // Ensure ring is positioned correctly relative to the note
                // Use localPosition for proper parenting, and match the note's RectTransform settings
                ringRectTransform.localPosition = Vector3.zero; // Adjust if necessary
                ringRectTransform.localScale = Vector3.one;

                // Disable the raycast target to avoid blocking clicks on the note
                Graphic ringGraphic = ringInstance.GetComponent<Graphic>();
                if (ringGraphic != null)
                {
                    ringGraphic.raycastTarget = false; // This makes the ring non-interactive
                }

                // Start shrinking the ring
                StartCoroutine(ShrinkRing(ringRectTransform));
            }
            else
            {
                Debug.LogError("RingPrefab or NotePrefab is missing RectTransform component.");
            }
        }
        else
        {
            Debug.LogError("Ring prefab is not assigned in the inspector.");
        }
    }
    
    

    // This method will be triggered when the note is clicked
    public void OnPointerClick(PointerEventData eventData)
    {
        if (ringInstance == null) return;

        if (isOtherNote == true)
        {
            // Skip hit/miss detection
            Destroy(gameObject);
            Destroy(ringInstance);
        }
        else
        {
            // Calculate the size difference between the ring and the note
            float sizeDifference = Mathf.Abs(ringInstance.GetComponent<RectTransform>().localScale.x - transform.localScale.x);

            // Check if the size difference is within the desired range (0.4 to 0.6)
            if (sizeDifference >= minHitThreshold && sizeDifference <= maxHitThreshold)
            {
                rhythmSceneManager.RegisterHit();
            }

            // Destroy the note after the click
            Destroy(gameObject);
            Destroy(ringInstance);
        }
    }

    IEnumerator ShrinkRing(RectTransform ringRectTransform)
    {
        Vector3 initialScale = ringRectTransform.localScale;
        Vector3 targetScale = Vector3.zero;

        float elapsed = 0f;

        while (elapsed < shrinkDuration)
        {
            ringRectTransform.localScale = Vector3.Lerp(initialScale, targetScale, elapsed / shrinkDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        ringRectTransform.localScale = targetScale;

        // Destroy the ring and note after shrinking completes
        Destroy(ringInstance);
        Destroy(gameObject);
    }
}
