using UnityEngine;

public class RhythmSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject noteSpawnerPrefab;
    [SerializeField] private GameObject RhythmUIPrefab;
    [SerializeField] private Canvas canvas;

    public bool isSuccess;
    public bool isPass;
    public bool isFail;

    public int hitCount { get; private set; }
    public int missCount { get; private set; }

    private GameObject rhythmUI; // Store the instantiated Rhythm UI

    public void SpawnRhythmPrefab(string ingredientName)
    {
        // Instantiate the UI for the rhythm game
        rhythmUI = Instantiate(RhythmUIPrefab, canvas.transform);

        // Instantiate the NoteSpawner
        GameObject noteSpawnerGameObject = Instantiate(noteSpawnerPrefab);

        // Check if the ingredient has a specific spawner behavior
        if (ingredientName == "Opium Poppy Tree")
        {
            OpiumPoppyTreeNoteSpawner noteSpawner = noteSpawnerGameObject.GetComponent<OpiumPoppyTreeNoteSpawner>();
            if (noteSpawner != null)
            {
                noteSpawner.Initialize(this, rhythmUI);
                Debug.Log("Rhythm Game for Opium Poppy Tree has started!");
            }
            else
            {
                Debug.LogError("OpiumPoppyTreeNoteSpawner component not found.");
            }
        }
        else
        {
            Debug.LogWarning($"No specific behavior defined for the ingredient: {ingredientName}");
        }
    }

    public void ClearHitCount()
    {
        hitCount = 0;
        isFail = false;
        isPass = false;
        isSuccess = false;
    }

    public void RegisterHit()
    {
        hitCount++;
        Debug.Log("Hit registered! Total hits: " + hitCount);
    }

    public void ScoreHandler(int maxScore)
    {
        if (hitCount < maxScore / 2)
        {
            Debug.Log("Fail!");
            isFail = true;
        }
        else if (hitCount >= maxScore / 2 && hitCount < maxScore - 3)
        {
            Debug.Log("Pass");
            isPass = true;
        }
        else if (hitCount >= maxScore - 3)
        {
            Debug.Log("Success!");
            isSuccess = true;
        }
    }

    public void DestroyRhythmUI()
    {
        if (rhythmUI != null)
        {
            Destroy(rhythmUI);
            Debug.Log("Rhythm UI has been destroyed.");
        }
    }
}
