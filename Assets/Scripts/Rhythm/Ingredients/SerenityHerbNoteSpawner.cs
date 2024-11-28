using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SerenityHerbNoteSpawner : MonoBehaviour
{
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private GameObject ghostNotePrefab;
    [SerializeField] private RectTransform spawnArea;
    [SerializeField] private float spawnInterval = 1.0f;
    [SerializeField] private float ghostSpawnInterval = 2.0f;
    [SerializeField] RhythmSceneManager rhythmSceneManager;
    [SerializeField] private GameObject UIManager;

    Ingredient ingredient;
    public int maxNotes;
    private int totalNotes;

    public RhythmSceneManager RhythmManagerGameObject;  // Set by RhythmSceneManager
    public GameObject OtherRhythmManagerGameObject;

    private GameObject RhythmUI;

    private float timer;
    private List<GameObject> spawnedNotes = new List<GameObject>(); // Track spawned non-ghost notes

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
            SpawnNonGhostNote();
            totalNotes++;
            yield return new WaitForSeconds(spawnInterval);

            SpawnGhostNotes();
            yield return new WaitForSeconds(ghostSpawnInterval);
        }

        // After all notes are spawned, check for completion
        yield return StartCoroutine(CheckForNotesCompletion());
    }

    private void SpawnNonGhostNote()
    {
        if (spawnArea == null) return;

        Vector2 spawnPosition = GetRandomPointInUI(spawnArea);

        GameObject newNote = Instantiate(notePrefab, spawnArea);
        RectTransform noteRect = newNote.GetComponent<RectTransform>();
        noteRect.anchoredPosition = spawnPosition;

        spawnedNotes.Add(newNote); // Track the non-ghost note
    }

    private void SpawnGhostNotes()
    {
        if (spawnArea == null) return;

        Vector2 spawnPosition = GetRandomPointInUI(spawnArea);

        GameObject ghostNote = Instantiate(ghostNotePrefab, spawnArea);
        RectTransform ghostNoteRect = ghostNote.GetComponent<RectTransform>();
        ghostNoteRect.anchoredPosition = spawnPosition;

        NoteBehavior ghostNoteBehavior = ghostNote.GetComponent<NoteBehavior>();

        if (ghostNoteBehavior != null)
        {
            ghostNoteBehavior.isOtherNote = true;

            // Change the color of ghost notes to blue or red
            Image ghostNoteImage = ghostNote.GetComponent<Image>();
            if (ghostNoteImage != null)
            {
                // Set alternating colors (Blue and Red)
                ghostNoteImage.color = totalNotes % 2 == 0 ? Color.blue : Color.red;
            }
            else
            {
                Debug.LogError("Image component missing on ghost note prefab.");
            }
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

    private IEnumerator CheckForNotesCompletion()
    {
        InventoryUI inventory = UIManager.GetComponent<InventoryUI>();

        // Wait for all non-ghost notes to be destroyed
        while (spawnedNotes.Count > 0)
        {
            spawnedNotes.RemoveAll(note => note == null);
            yield return null;
        }

        Debug.Log("All notes have been destroyed!");

        // Retrieve the ingredient
        Ingredient ingredient = GameManager.Instance.GetClonedIngredientDatabase().GetIngredientByName("Serenity Herb");
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
