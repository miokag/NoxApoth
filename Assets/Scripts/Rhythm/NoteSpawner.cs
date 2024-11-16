using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private RectTransform spawnArea;
    [SerializeField] private float spawnInterval = 1.0f;
    public GameObject RhythmManagerGameObject;  // Set by RhythmSceneManager

    private float timer;

    // Initialize method for setup after instantiation
    public void Initialize(GameObject rhythmManagerGameObject)
    {
        RhythmManagerGameObject = rhythmManagerGameObject;

        if (RhythmManagerGameObject != null)
        {
            // Find the "RhythmRange" within the RhythmManagerGameObject
            spawnArea = RhythmManagerGameObject.transform.Find("RhythmRange")?.GetComponent<RectTransform>();

            if (spawnArea == null)
            {
                Debug.LogError("RhythmRange child not found or missing RectTransform in RhythmManagerGameObject.");
            }
        }
        else
        {
            Debug.LogError("RhythmManagerGameObject is not assigned or instantiated.");
        }
    }

    private void Start()
    {
        // Additional setup can be done here if needed
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnNote();
            timer = 0f;
        }
    }

    private void SpawnNote()
    {
        if (spawnArea == null) return;

        Vector2 spawnPosition = GetRandomPointInUI(spawnArea);

        GameObject newNote = Instantiate(notePrefab, spawnArea);
        RectTransform noteRect = newNote.GetComponent<RectTransform>();
        noteRect.anchoredPosition = spawnPosition;
    }

    private Vector2 GetRandomPointInUI(RectTransform rectTransform)
    {
        Vector2 size = rectTransform.rect.size;
        float randomX = Random.Range(-size.x / 2, size.x / 2);
        float randomY = Random.Range(-size.y / 2, size.y / 2);
        return new Vector2(randomX, randomY);
    }
}
