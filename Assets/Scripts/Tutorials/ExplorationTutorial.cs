using DialogueSystem;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ExplorationTutorial : MonoBehaviour
{
    [SerializeField] private GameObject buttonInstructionUI;
    [SerializeField] private Canvas canvas;
    [SerializeField] private NotebookUIManager notebookUIManager;  // Reference to NotebookUIManager
    [SerializeField] private PlayerControllerScript playerController;

    private GameObject NotebookButtonInstruction;
    private DialogueManager dialogueManager;
    private int tutorialCounter;

    void Start()
    {
        if (playerController != null)
        {
            playerController.enabled = false;
        }
        else
        {
            Debug.LogWarning("PlayerControllerScript is not assigned in the Inspector.");
        }
        canvas = GameObject.Find("YourCanvasName").GetComponent<Canvas>();
        dialogueManager = FindObjectOfType<DialogueManager>();
        notebookUIManager = FindObjectOfType<NotebookUIManager>();

        if (GameManager.Instance.GetTutorialStep() == 2)
        {
            DialogueManager.Instance.StartDialogue("exploration");
            DialogueManager.Instance.OnActionTriggeredEvent += OnActionTriggered;
            DialogueManager.Instance.OnDialogueFinishedEvent += OnDialogueFinished;

        }
    }

    private void OnActionTriggered()
    {
        switch (tutorialCounter)
        {
            case 0:
                NotebookButtonInstruction = Instantiate(buttonInstructionUI, canvas.transform);
                TMP_Text IconText = NotebookButtonInstruction.transform.Find("KeyIconText")?.GetComponent<TMP_Text>();
                TMP_Text DescriptionText = NotebookButtonInstruction.transform.Find("DescriptionText")?.GetComponent<TMP_Text>();

                RectTransform rectTransform = NotebookButtonInstruction.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + 70f);

                IconText.text = "N";
                DescriptionText.text = "Notebook";
                tutorialCounter++;
                Debug.Log("Tutorial Counter: " + tutorialCounter);

                // Optionally call case 1 directly, if it should start immediately
                OnActionTriggered();
                break;

            case 1:
                Debug.Log("Case 1 Started.");
                break;
        }
    }

    private void Update()
    {
        if (Keyboard.current.nKey.isPressed)
        {
            if (NotebookButtonInstruction != null && NotebookButtonInstruction.transform.IsChildOf(canvas.transform))
            {
                Destroy(NotebookButtonInstruction);
                dialogueManager.ResumeDialogue();
            }
        }
    }

    private void OnDialogueFinished()
    {
        if (GameManager.Instance.GetTutorialStep() == 0)
        {
            Debug.Log("Action triggered in CustomerActionsTutorial.");

            playerController.enabled = true;
            DialogueManager.Instance.OnActionTriggeredEvent -= OnActionTriggered;
        }
    }
}
