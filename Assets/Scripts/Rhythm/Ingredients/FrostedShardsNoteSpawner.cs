using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrostedShardsNoteSpawner : MonoBehaviour
{
    [SerializeField] private GameObject frozenBarrierPrefab; // Changed from Image to GameObject
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private RectTransform spawnArea;
    [SerializeField] private int clicksToBreak = 5;
    [SerializeField] private float spawnInterval = 1.0f;
    [SerializeField] private GameObject UIManager;

    RhythmSceneManager rhythmSceneManager;
    private int totalNotes;
    private int maxNotes;
    private int currentClicks;
    private bool barrierBroken;

    private GameObject currentFrozenBarrier; // Instance of the frozen barrier
    private List<GameObject> spawnedNotes = new List<GameObject>();

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
        currentClicks = 0;
        barrierBroken = false;

        CreateFrozenBarrier();
        StartCoroutine(SpawnNotes());
    }

    private void CreateFrozenBarrier()
    {
        // Instantiate the frozen barrier prefab
        currentFrozenBarrier = Instantiate(frozenBarrierPrefab, spawnArea);
        RectTransform barrierRect = currentFrozenBarrier.GetComponent<RectTransform>();
        barrierRect.sizeDelta = spawnArea.rect.size;

        Button barrierButton = currentFrozenBarrier.GetComponent<Button>();
        if (barrierButton != null)
        {
            barrierButton.onClick.AddListener(() => OnBarrierClicked());
        }
        else
        {
            Debug.LogError("Frozen barrier prefab is missing a Button component!");
        }
    }

    private void OnBarrierClicked()
    {
        currentClicks++;
        Debug.Log($"Frozen barrier clicked {currentClicks}/{clicksToBreak} times");

        // Update visual feedback (e.g., reduce opacity slightly)
        Image barrierImage = currentFrozenBarrier.GetComponent<Image>();
        if (barrierImage != null)
        {
            barrierImage.color = new Color(0, 0.5f, 1f, Mathf.Lerp(0.7f, 0, (float)currentClicks / clicksToBreak));
        }

        // Shake effect when clicked
        StartCoroutine(ShakeBarrier());

        if (currentClicks >= clicksToBreak)
        {
            BreakBarrier();
        }
    }

    private IEnumerator ShakeBarrier()
    {
        float shakeDuration = 0.1f; // How long the shake lasts
        float shakeAmount = 10f; // How much the barrier shakes
        Vector3 originalPosition = currentFrozenBarrier.transform.position;

        // Shake effect
        float elapsedTime = 0;
        while (elapsedTime < shakeDuration)
        {
            currentFrozenBarrier.transform.position = originalPosition + new Vector3(Random.Range(-shakeAmount, shakeAmount), 0, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Return to the original position
        currentFrozenBarrier.transform.position = originalPosition;
    }

    private void BreakBarrier()
    {
        barrierBroken = true;
        currentClicks = 0;

        // Destroy the frozen barrier
        if (currentFrozenBarrier != null)
        {
            Destroy(currentFrozenBarrier);
            Debug.Log("Frozen barrier broken!");
        }

        // Start respawning the barrier after a delay
        StartCoroutine(RespawnFrozenBarrier());
    }

    private IEnumerator RespawnFrozenBarrier()
    {
        // Wait for a few seconds before respawning the barrier
        yield return new WaitForSeconds(3f);

        // Respawn the frozen barrier
        CreateFrozenBarrier();
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
        if (spawnArea == null || !barrierBroken) return;

        Vector2 spawnPosition = GetRandomPointInUI(spawnArea);

        GameObject newNote = Instantiate(notePrefab, spawnArea);
        RectTransform noteRect = newNote.GetComponent<RectTransform>();
        noteRect.anchoredPosition = spawnPosition;

        spawnedNotes.Add(newNote);
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
        Ingredient ingredient = GameManager.Instance.GetClonedIngredientDatabase().GetIngredientByName("Frosted Shards");
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
