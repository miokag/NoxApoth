using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShadowSporesNoteSpawner : MonoBehaviour
{
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private RectTransform spawnArea;
    [SerializeField] private float spawnInterval = 1.0f;
    [SerializeField] private GameObject UIManager;

    private int totalNotes;
    private int maxNotes;

    private List<GameObject> spawnedNotes = new List<GameObject>();

    RhythmSceneManager rhythmSceneManager;

    public RhythmSceneManager RhythmManagerGameObject;  // Set by RhythmSceneManager
    public GameObject OtherRhythmManagerGameObject;

    private GameObject RhythmUI;

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
            SpawnNote();
            totalNotes++;
            yield return new WaitForSeconds(spawnInterval);
        }

        // After spawning notes, check for completion
        yield return StartCoroutine(CheckForNotesCompletion());
    }

    private void SpawnNote()
    {
        if (spawnArea == null) return;

        Vector2 spawnPosition = GetRandomPointInUI(spawnArea);

        GameObject newNote = Instantiate(notePrefab, spawnArea);
        RectTransform noteRect = newNote.GetComponent<RectTransform>();
        noteRect.anchoredPosition = spawnPosition;

        // Add CanvasGroup for fading effect
        CanvasGroup canvasGroup = newNote.AddComponent<CanvasGroup>();

        // Add the spawned note to the list
        spawnedNotes.Add(newNote);

        // Start the fade-in and fade-out effect (looping)
        StartCoroutine(FadeNoteInAndOut(canvasGroup));
    }

    private Vector2 GetRandomPointInUI(RectTransform rectTransform)
    {
        Vector2 size = rectTransform.rect.size;
        float randomX = Random.Range(-size.x / 2, size.x / 2);
        float randomY = Random.Range(-size.y / 2, size.y / 2);
        return new Vector2(randomX, randomY);
    }

    private IEnumerator FadeNoteInAndOut(CanvasGroup canvasGroup)
    {
        int fadeCount = 10;  // Number of fade in/out cycles
        float fadeInDuration = 0.1f; // Fade-in duration
        float fadeOutDuration = 0.1f; // Fade-out duration

        for (int i = 0; i < fadeCount; i++)
        {
            // Fade-in
            float elapsedTime = 0f;
            while (elapsedTime < fadeInDuration)
            {
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            canvasGroup.alpha = 1f;

            // Wait for a short moment before starting to fade out
            yield return new WaitForSeconds(0.2f);  // Adjust this wait time to control the pause before fading out

            // Fade-out
            elapsedTime = 0f;
            while (elapsedTime < fadeOutDuration)
            {
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            canvasGroup.alpha = 0f;

            // Wait before the next fade cycle
            yield return new WaitForSeconds(0.5f);  // Adjust the wait time between cycles
        }

        // Destroy the note after completing the fade cycles
        Destroy(canvasGroup.gameObject);
    }



    private IEnumerator CheckForNotesCompletion()
    {
        InventoryUI inventory = UIManager.GetComponent<InventoryUI>();

        // Wait for all notes to be destroyed (faded out)
        while (spawnedNotes.Count > 0)
        {
            spawnedNotes.RemoveAll(note => note == null);
            yield return null;
        }

        Debug.Log("All notes have been destroyed!");

        // Retrieve the ingredient
        Ingredient ingredient = GameManager.Instance.GetClonedIngredientDatabase().GetIngredientByName("Shadow Spores");
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
