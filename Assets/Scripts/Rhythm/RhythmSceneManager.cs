using System;
using UnityEngine;
using UnityEngine.Video;

public enum RhythmResult
{
    None,
    Fail,
    Pass,
    Success
}

public class RhythmSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject noteSpawnerPrefab;
    [SerializeField] private GameObject RhythmUIPrefab;
    [SerializeField] private Canvas canvas;
    [SerializeField] private VideoClip[] videoClips;
    [SerializeField] private VideoPlayer videoPlayer;
    public RhythmResult rhythmResult { get; private set; } = RhythmResult.None;

    private void Start()
    {
        videoPlayer = GameObject.Find("VideoPlayer").GetComponent<VideoPlayer>();
    }

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
            videoPlayer.clip = videoClips[4];
            OpiumPoppyTreeNoteSpawner noteSpawner = noteSpawnerGameObject.GetComponent<OpiumPoppyTreeNoteSpawner>();
            noteSpawner.enabled = true;
            if (noteSpawner != null)
            {
                noteSpawner.Initialize(this, rhythmUI);
            }
        }
        else if (ingredientName == "Serenity Herb")
        {
            videoPlayer.clip = videoClips[2];
            SerenityHerbNoteSpawner noteSpawner = noteSpawnerGameObject.GetComponent<SerenityHerbNoteSpawner>();
            noteSpawner.enabled = true;
            if (noteSpawner != null)
            {
                noteSpawner.Initialize(this, rhythmUI);
            }
        }
        else if (ingredientName == "Viper's Vine")
        {
            videoPlayer.clip = videoClips[5];
            VipersVineNoteSpawner noteSpawner = noteSpawnerGameObject.GetComponent<VipersVineNoteSpawner>();
            noteSpawner.enabled = true;
            if (noteSpawner != null)
            {
                noteSpawner.Initialize(this, rhythmUI);
            }
        }
        else if (ingredientName == "Frosted Shards")
        {
            videoPlayer.clip = videoClips[0];
            FrostedShardsNoteSpawner noteSpawner = noteSpawnerGameObject.GetComponent<FrostedShardsNoteSpawner>();
            noteSpawner.enabled = true;
            if (noteSpawner != null)
            {
                noteSpawner.Initialize(this, rhythmUI);
            }
        }
        else if (ingredientName == "Gale Fern Fronds")
        {
            videoPlayer.clip = videoClips[1];
            GaleFernFrondsNoteSpawner noteSpawner = noteSpawnerGameObject.GetComponent<GaleFernFrondsNoteSpawner>();
            noteSpawner.enabled = true;
            if (noteSpawner != null)
            {
                noteSpawner.Initialize(this, rhythmUI);
            }
        }
        else if (ingredientName == "Shadow Spores")
        {
            videoPlayer.clip = videoClips[3];
            ShadowSporesNoteSpawner noteSpawner = noteSpawnerGameObject.GetComponent<ShadowSporesNoteSpawner>();
            noteSpawner.enabled = true;
            if (noteSpawner != null)
            {
                noteSpawner.Initialize(this, rhythmUI);
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
            rhythmResult = RhythmResult.Fail;
        }
        else if (hitCount >= maxScore / 2 && hitCount < maxScore - 3)
        {
            Debug.Log("Pass");
            isPass = true;
            rhythmResult = RhythmResult.Pass;
        }
        else if (hitCount >= maxScore - 3)
        {
            Debug.Log("Success!");
            isSuccess = true;
            rhythmResult = RhythmResult.Success;
        }
    }
    
    public void ResetRhythmGame()
    {
        // Reset hit and miss counts
        hitCount = 0;
        missCount = 0;

        // Reset result and flags
        rhythmResult = RhythmResult.None;
        isFail = false;
        isPass = false;
        isSuccess = false;

        // Destroy remaining Rhythm UI
        DestroyRhythmUI();

        Debug.Log("Rhythm game has been reset.");
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
