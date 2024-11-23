using DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackShopTutorial : MonoBehaviour
{
    [SerializeField] private GameObject door; // Reference to the Door GameObject
    private DialogueSys dialogueManager;
    private DoorBehavior doorBehavior;

    void Start()
    {

        dialogueManager = FindObjectOfType<DialogueSys>();
        doorBehavior = door.transform.Find("Cube")?.GetComponent<DoorBehavior>();

        // Check if the tutorial step is 1, and if so, start the dialogue
        if (GameManager.Instance.GetTutorialStep() == 1)
        {
            Debug.Log("Next TUTORIAL Step == 1");
            dialogueManager.StartDialogue("backshop");

            // Find the DoorBehavior on the child Cube GameObject
            if (doorBehavior != null)
            {
                // Disable interaction with the door after starting the tutorial
                doorBehavior.DisableInteraction(); // Disable interaction with the door
                dialogueManager.OnDialogueFinished += OnDialogueFinished;
            }
            else
            {
                Debug.LogError("DoorBehavior component not found on the Cube GameObject.");
            }
        }
    }

    private void OnDialogueFinished()
    {
        // Once the dialogue finishes, move to the next tutorial step
        if (GameManager.Instance.GetTutorialStep() == 1)
        {
            doorBehavior.EnableInteraction();
            Debug.Log("Tutorial Step Completed, moving to the next step.");
            GameManager.Instance.NextTutorialStep();
            dialogueManager.OnDialogueFinished -= OnDialogueFinished;
        }
    }
}
