using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VipersVineNoteSpawner : MonoBehaviour
{
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private GameObject ghostNotePrefab; // Ghost note prefab
    [SerializeField] private RectTransform spawnArea;
    [SerializeField] private float spawnInterval = 0.5f; // Faster spawn interval
    [SerializeField] private float ghostSpawnInterval = 1.5f; // Interval for spawning ghost notes
    [SerializeField] RhythmSceneManager rhythmSceneManager;
    [SerializeField] private GameObject UIManager;
    
    public RhythmSceneManager RhythmManagerGameObject;  // Set by RhythmSceneManager
    public GameObject OtherRhythmManagerGameObject;

    public int maxNotes;
    private int totalNotes;

    private GameObject RhythmUI;

    private float timer;
    private List<GameObject> spawnedNotes = new List<GameObject>(); // Track spawned non-ghost notes

    private Vector2 lastNotePosition; // Store the position of the last spawned note

    public void Initialize(RhythmSceneManager OtherRhythmManagerGameObject, GameObject rhythmUI)
    {
        RhythmManagerGameObject = OtherRhythmManagerGameObject;
        rhythmSceneManager = RhythmManagerGameObject.GetComponent<RhythmSceneManager>();
        rhythmSceneManager.ClearHitCount();

        if (rhythmUI != null)
        {
            spawnArea = rhythmUI.transform.Find("RhythmRange")?.GetComponent<RectTransform>();

            if (spawnArea != null)
            {
                Debug.Log("RhythmRange child found RectTransform in RhythmManagerGameObject.");
            }
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
        UIManager = GameObject.Find("UIManager");
        maxNotes = Random.Range(5, 10);
        totalNotes = 0;
        StartCoroutine(SpawnNotes());
    }

    private IEnumerator SpawnNotes()
    {
        while (totalNotes < maxNotes)
        {
            totalNotes++;
            SpawnRegularNote(); // Spawn a regular note
            yield return new WaitForSeconds(spawnInterval); // Wait before spawning the next regular note

            SpawnGhostNote(); // Spawn a ghost note at a defined interval
            yield return new WaitForSeconds(ghostSpawnInterval); // Wait before spawning the next ghost note
        }

        // After all notes are spawned, check for completion
        yield return StartCoroutine(CheckForNotesCompletion());
    }

    private void SpawnRegularNote()
    {
        if (spawnArea == null) return;

        // Determine the spawn position: if it's the first note, spawn randomly, else near the last note
        Vector2 spawnPosition;
        if (totalNotes == 1)
        {
            spawnPosition = GetRandomPointInUI(spawnArea); // First note spawns randomly
        }
        else
        {
            spawnPosition = GetClosePosition(lastNotePosition); // Subsequent notes spawn near the last one
        }

        // Create the new note
        GameObject newNote = Instantiate(notePrefab, spawnArea);
        RectTransform noteRect = newNote.GetComponent<RectTransform>();
        noteRect.anchoredPosition = spawnPosition;

        // Store the position of the last spawned note
        lastNotePosition = spawnPosition;

        spawnedNotes.Add(newNote); // Track the regular note
    }

    private void SpawnGhostNote()
    {
        if (spawnArea == null) return;

        // If there are spawned notes, spawn the ghost note near the last regular note
        Vector2 spawnPosition;
        if (totalNotes == 1)
        {
            spawnPosition = GetRandomPointInUI(spawnArea); // First ghost note spawns randomly
        }
        else
        {
            spawnPosition = GetClosePosition(lastNotePosition); // Subsequent ghost notes spawn near the last regular note
        }

        // Instantiate the ghost note at the calculated position
        GameObject ghostNote = Instantiate(ghostNotePrefab, spawnArea);
        RectTransform ghostNoteRect = ghostNote.GetComponent<RectTransform>();
        ghostNoteRect.anchoredPosition = spawnPosition;

        // Set the ghost note's behavior
        NoteBehavior ghostNoteBehavior = ghostNote.GetComponent<NoteBehavior>();

        if (ghostNoteBehavior != null)
        {
            ghostNoteBehavior.isOtherNote = true; // Mark this as a ghost note
        }
        else
        {
            Debug.LogError("Note Behavior is Null");
        }
    }


    private Vector2 GetRandomPointInUI(RectTransform rectTransform)
    {
        Vector2 size = rectTransform.rect.size;
        float randomX = Random.Range(-size.x / 2, size.x / 2);
        float randomY = Random.Range(-size.y / 2, size.y / 2);
        return new Vector2(randomX, randomY);
    }

    private Vector2 GetClosePosition(Vector2 previousPosition)
    {
        // Define a small range for random movement (e.g., a radius of 50 units)
        float offsetX = Random.Range(-50f, 50f);  // Adjust the range to control how close the notes spawn
        float offsetY = Random.Range(-50f, 50f);  // Adjust the range to control how close the notes spawn

        // Return the new position based on the last note’s position + the random offset
        return new Vector2(previousPosition.x + offsetX, previousPosition.y + offsetY);
    }

    private IEnumerator CheckForNotesCompletion()
    {
        InventoryUI inventory = UIManager.GetComponent<InventoryUI>();

        // Wait for all notes to be destroyed
        while (spawnedNotes.Count > 0)
        {
            spawnedNotes.RemoveAll(note => note == null);
            yield return null;
        }

        Debug.Log("All notes have been destroyed!");

        Ingredient ingredient = GameManager.Instance.GetClonedIngredientDatabase().GetIngredientByName("Viper's Vine");
        if (ingredient != null)
        {
            rhythmSceneManager.ScoreHandler(maxNotes);
            Debug.Log("Instantiating NoteSpawnerPrefab for " + ingredient);

            // Assign GatheredState based on the rhythmResult
            switch (rhythmSceneManager.rhythmResult)
            {
                case RhythmResult.Fail:
                    ingredient.currentGatheredState = Ingredient.GatheredState.Bad;
                    Debug.Log("Result: Fail. Ingredient state set to Bad.");
                    break;
                case RhythmResult.Pass:
                    ingredient.currentGatheredState = Ingredient.GatheredState.Good;
                    Debug.Log("Result: Pass. Ingredient state set to Good.");
                    break;
                case RhythmResult.Success:
                    ingredient.currentGatheredState = Ingredient.GatheredState.Perfect;
                    Debug.Log("Result: Success. Ingredient state set to Perfect.");
                    break;
                default:
                    Debug.LogWarning("Unexpected result. Ingredient state unchanged.");
                    break;
            }

            // Add ingredient to inventory and update the UI
            GameManager.Instance.AddToInventory(ingredient);
            inventory.UpdateUI();

            GameManager.Instance.DebugInventory();

            Debug.Log($"{ingredient.ingredientName} has been added to inventory.");
        }
        else
        {
            Debug.LogError("Ingredient not found in the database!");
        }

        // Destroy Rhythm UI after completion
        this.enabled = false;
        rhythmSceneManager.DestroyRhythmUI();
        rhythmSceneManager.ResetRhythmGame();
    }
}
