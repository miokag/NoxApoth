using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class StoveCircleMover : MonoBehaviour
{
    public RectTransform stoveClickerPanel; // The panel containing the sections
    public List<RectTransform> redSections;
    public List<RectTransform> orangeSections;
    public List<RectTransform> greenSections;

    public float moveSpeed = 200f;
    private RectTransform rectTransform;

    private float leftBound;
    private float rightBound;
    private bool isMoving = true;

    private Canvas StoveLineCanvas;

    public bool isGreen;
    public bool isOrange;
    public bool isRed;

    private float startTime;
    private Canvas StoveOnCanvas;

    // Called immediately when the object is instantiated or enabled
    private void Start()
    {
        Debug.Log("IsMoving = " + isMoving);
        // Initialize the stoveClickerPanel and other components
        rectTransform = GetComponent<RectTransform>();

        Canvas mainCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        StoveOnCanvas = mainCanvas.transform.Find("StoveOnCanvas(Clone)").GetComponent<Canvas>();
        StoveLineCanvas = StoveOnCanvas.transform.Find("StoveLineCanvas").GetComponent<Canvas>();

        RectTransform redSection1 = StoveLineCanvas.transform.Find("RedArea1").GetComponent<RectTransform>();
        RectTransform redSection2 = StoveLineCanvas.transform.Find("RedArea2").GetComponent<RectTransform>();

        RectTransform orangeSection1 = StoveLineCanvas.transform.Find("OrangeArea1").GetComponent<RectTransform>();
        RectTransform orangeSection2 = StoveLineCanvas.transform.Find("OrangeArea2").GetComponent<RectTransform>();

        RectTransform greenSection = StoveLineCanvas.transform.Find("GreenArea").GetComponent<RectTransform>();

        RectTransform stoveLineRange = GameObject.Find("StoveLineRange").GetComponent<RectTransform>();
        stoveClickerPanel = stoveLineRange;

        redSections.Add(redSection1);
        redSections.Add(redSection2);
        orangeSections.Add(orangeSection1);
        orangeSections.Add(orangeSection2);
        greenSections.Add(greenSection);

        // Calculate movement bounds within the stoveClickerPanel
        float panelWidth = stoveClickerPanel.rect.width;
        leftBound = -panelWidth / 2;
        rightBound = panelWidth / 2;

        startTime = Time.time; // Track the start time

        // Set the initial position at the left bound
        rectTransform.anchoredPosition = new Vector2(leftBound, rectTransform.anchoredPosition.y);

        StartMoving();
    }

    public void StartMoving()
    {
        isMoving = true; // Starts the movement
        startTime = Time.time; // Reset start time to allow smooth movement
        MoveCircle();
    }

    private void Update()
    {
        // Move the circle only when it is NOT moving
        if (!isMoving)
        {
            MoveCircle();  // Move circle when it is stopped (isMoving is false)
        }

        // Toggle movement on mouse click
        if (Input.GetMouseButtonDown(0))
        {
            isMoving = !isMoving;  // Toggle isMoving state

            if (isMoving)
            {
                CheckStoppedColor();
            }
        }
    }


    private void MoveCircle()
    {
        // Move the circle back and forth within the specified bounds
        float elapsedTime = (Time.time - startTime) * moveSpeed;
        float newXPosition = Mathf.PingPong(elapsedTime, rightBound - leftBound) + leftBound;
        rectTransform.anchoredPosition = new Vector2(newXPosition, rectTransform.anchoredPosition.y);
    }

    public void CheckStoppedColor()
    {
        Vector2 circlePosition = rectTransform.anchoredPosition;
        Debug.Log($"Circle position: {circlePosition}");

        bool isOverlapping = false;

        // Check if the circle is overlapping with any section
        if (IsOverlappingWithAny(circlePosition, redSections))
        {
            Debug.Log("Circle stopped on Red section!");
            isOverlapping = true;
            isRed = true;
        }
        else if (IsOverlappingWithAny(circlePosition, orangeSections))
        {
            Debug.Log("Circle stopped on Orange section!");
            isOverlapping = true;
            isOrange = true;
        }
        else if (IsOverlappingWithAny(circlePosition, greenSections))
        {
            Debug.Log("Circle stopped on Green section!");
            isOverlapping = true;
            isGreen = true;
        }

        if (!isOverlapping)
        {
            Debug.Log("Circle is outside of the color sections.");
        }

        // Start the coroutine to wait for 1 second before destroying the StoveOnCanvas GameObject
        StartCoroutine(DestroyStoveOnCanvasAfterDelay());
    }

    private IEnumerator DestroyStoveOnCanvasAfterDelay()
    {
        // Wait for 1 second
        yield return new WaitForSeconds(1f);

        // Destroy the StoveOnCanvas after the delay
        Destroy(StoveOnCanvas.gameObject);
    }


    private bool IsOverlappingWithAny(Vector2 position, List<RectTransform> sections)
    {
        Vector3 worldPosition = stoveClickerPanel.TransformPoint(position);

        foreach (var section in sections)
        {
            Vector3[] corners = new Vector3[4];
            section.GetWorldCorners(corners);

            if (worldPosition.x >= corners[0].x && worldPosition.x <= corners[2].x &&
                worldPosition.y >= corners[0].y && worldPosition.y <= corners[1].y)
            {
                Debug.Log($"Circle overlaps with {section.name} at position: {worldPosition}");
                return true;
            }
        }
        return false;
    }
}
