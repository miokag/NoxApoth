using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpiumPoppyTreeNoteSpawner : MonoBehaviour
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
        maxNotes = Random.Range(3, 5);
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

        if (rhythmSceneManager == null)
        {
            Debug.LogError("RhythmSceneManager component not found.");
            yield break;
        }

        rhythmSceneManager.ScoreHandler(maxNotes);

        Ingredient ingredient = GameManager.Instance.GetClonedIngredientDatabase().GetIngredientByName("Opium Poppy Tree");
        if (ingredient != null)
        {
            GameManager.Instance.AddToInventory(ingredient);
            inventory.UpdateUI();

            Debug.Log($"{ingredient.ingredientName} has been added to inventory.");
        }
        else
        {
            Debug.LogError("Ingredient not found in the database!");
        }

        // Destroy Rhythm UI after completion
        rhythmSceneManager.DestroyRhythmUI();
    }

}