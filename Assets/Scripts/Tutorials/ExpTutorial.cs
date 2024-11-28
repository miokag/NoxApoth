using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting.ReorderableList;

public class ExpTutorial : MonoBehaviour
{
    private PlayerControllerScript _playerControllerScript;
    private DialogueSys dialogueManager;
    private Canvas generalCanvas;
    [SerializeField] private NotebookUIManager notebookUIManagerPrefab;
    [SerializeField] private GameObject existingNoteBookUI;

    [SerializeField] private GameObject InstructionButtonPrefab;

    private int nextStep;
    private GameObject InstrucButton;
    private NotebookUIManager notebookUIManager;


    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.Instance.GetTutorialStep() == 2)
        {
            nextStep = 0;
            GameObject playerContainer = GameObject.Find("Player");
            GameObject player = playerContainer.transform.Find("PlayerSprite").gameObject;
            _playerControllerScript = player.GetComponent<PlayerControllerScript>();

            _playerControllerScript.enabled = false;

            dialogueManager = FindObjectOfType<DialogueSys>();

            generalCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
            if (dialogueManager == null) { Debug.LogError("Canvas is not assigned."); }
            else
            {
                Debug.Log("Canvas assigned.");
                if (dialogueManager == null) { Debug.LogError("DialogueManager is not assigned."); }
                else
                {
                    dialogueManager.StartDialogue("exploration1");
                    nextStep = 1;
                    dialogueManager.OnDialogueFinished += RunNextDialogueNode;
                }

            }
        }
        
    }

    private void RunNextDialogueNode()
    {
        if (nextStep == 1)
        {
            dialogueManager.OnDialogueFinished -= RunNextDialogueNode;
            Debug.Log("Next Step = 1 is running");

            InstrucButton = Instantiate(InstructionButtonPrefab, generalCanvas.transform);
            InstrucButton.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, 50);

            TMP_Text KeyIconText = InstrucButton.transform.Find("KeyIconText")?.GetComponent<TMP_Text>();
            TMP_Text DescriptionText = InstrucButton.transform.Find("DescriptionText")?.GetComponent<TMP_Text>();

            KeyIconText.text = "N";
            DescriptionText.text = "Notebook";

            nextStep++;
            StartCoroutine(CheckForNotebookPress());
        }

        else if (nextStep == 3)
        {
            dialogueManager.OnDialogueFinished -= RunNextDialogueNode;
            Debug.Log("Next Step = 3 is running");
            dialogueManager.StartDialogue("explorationTOC2");
            
            nextStep++;
            dialogueManager.OnDialogueFinished += RunNextDialogueNode;
        }

        else if (nextStep == 4)
        {
            dialogueManager.OnDialogueFinished -= RunNextDialogueNode;
            Debug.Log("Next Step = 4 is running");

            // Access the button for the Opium Poppy Tree using its name
            Button ingredientButton = notebookUIManager.ingredientButtonDictionary["Opium Poppy Tree"];

            notebookUIManager.EnableIngredientButton("Opium Poppy Tree");
            
            if (ingredientButton == null)
            {
                Debug.LogError("Ingredient Bookmark Button is null. Cannot proceed.");
            }
            else
            {
                Debug.Log("Ingredient Bookmark Button found. Waiting for click...");
                ingredientButton.onClick.AddListener(() =>
                {
                    Debug.Log("Ingredient Bookmark Button clicked.");
                    dialogueManager.StartDialogue("explorationIngredientPage");
                    dialogueManager.OnDialogueFinished += RunNextDialogueNode;
                    nextStep++;
                });
            }
        }

        else if (nextStep == 5)
        {
            dialogueManager.OnDialogueFinished -= RunNextDialogueNode;
            Debug.Log("Next Step = 5 is running");

            notebookUIManager.EnablePotionBookmarkButton();


            Button potionBookmarkButton = notebookUIManager.potionBookmarkButton.GetComponent<Button>();

            if (potionBookmarkButton == null)
            {
                Debug.LogError("Potion Bookmark Button is null. Cannot proceed.");
            }
            else
            {
                Debug.Log("Potion Bookmark Button found. Waiting for click...");
                potionBookmarkButton.onClick.AddListener(() =>
                {
                    notebookUIManager.DisablePotionBookmarkButton();
                    nextStep++;
                    notebookUIManager.DisableAllPotionButtons();
                    notebookUIManager.EnablePotionButton("Healing Potion");
                    RunNextDialogueNode();
                });
            }
        }

        else if (nextStep == 6)
        {
            Debug.Log("Next Step = 6 is running");
            Button potionButton = notebookUIManager.potionButtonDictionary["Healing Potion"];

            if (potionButton == null) { Debug.LogError("Potion Button is null. Cannot proceed."); }
            else
            {
                potionButton.onClick.AddListener(() =>
                {
                    Debug.Log("Potion Button clicked.");
                    dialogueManager.StartDialogue("explorationPotionPage");
                    nextStep++;
                    dialogueManager.OnDialogueFinished += RunNextDialogueNode;
                });
            }
        }

        else if (nextStep == 7)
        {
            Debug.Log("Next Step = 7 is running");

            Transform notebookTransform = generalCanvas.transform.Find("Notebook(Clone)");
            Transform notebookUIManager = generalCanvas.transform.Find("NotebookUIManager(Clone)");

            if (notebookTransform != null)
            {
                Debug.Log("Destroying the notebook UI manager");

                // Destroy the GameObject (notebook)
                Destroy(notebookTransform.gameObject);
                Destroy(notebookUIManager.gameObject);

                // Optionally set notebookUIManager to null after destroying it, if needed
                notebookUIManager = null;
                _playerControllerScript.enabled = true;
                existingNoteBookUI.SetActive(true);

                // Instantiate the WASD Controls for the player

                // W Instruction Button
                GameObject WInstrucButton = Instantiate(InstructionButtonPrefab, generalCanvas.transform);
                Image WimageComponent = WInstrucButton.GetComponent<Image>();
                Transform WchildTransform = WInstrucButton.transform.Find("DescriptionText");
                WInstrucButton.GetComponent<RectTransform>().anchoredPosition += new Vector2(90, 20);

                TMP_Text WKeyIconText = WInstrucButton.transform.Find("KeyIconText")?.GetComponent<TMP_Text>();

                WKeyIconText.text = "W";
                Destroy(WchildTransform.gameObject);
                WimageComponent.enabled = false;

                // A Instruction Button
                GameObject AInstrucButton = Instantiate(InstructionButtonPrefab, generalCanvas.transform);
                Image AimageComponent = AInstrucButton.GetComponent<Image>();
                Transform AchildTransform = AInstrucButton.transform.Find("DescriptionText");
                AInstrucButton.GetComponent<RectTransform>().anchoredPosition += new Vector2(70, 0);

                TMP_Text AKeyIconText = AInstrucButton.transform.Find("KeyIconText")?.GetComponent<TMP_Text>();

                AKeyIconText.text = "A";
                Destroy(AchildTransform.gameObject);
                AimageComponent.enabled = false;

                // S Instruction Button
                GameObject SInstrucButton = Instantiate(InstructionButtonPrefab, generalCanvas.transform);
                Image SimageComponent = SInstrucButton.GetComponent<Image>();
                Transform SchildTransform = SInstrucButton.transform.Find("DescriptionText");
                SInstrucButton.GetComponent<RectTransform>().anchoredPosition += new Vector2(90, -20);

                TMP_Text SKeyIconText = SInstrucButton.transform.Find("KeyIconText")?.GetComponent<TMP_Text>();

                SKeyIconText.text = "S";
                Destroy(SchildTransform.gameObject);
                SimageComponent.enabled = false;

                // D Instruction Button
                GameObject DInstrucButton = Instantiate(InstructionButtonPrefab, generalCanvas.transform);
                Image DimageComponent = DInstrucButton.GetComponent<Image>();
                Transform DchildTransform = DInstrucButton.transform.Find("DescriptionText");
                DInstrucButton.GetComponent<RectTransform>().anchoredPosition += new Vector2(110, 0);

                TMP_Text DKeyIconText = DInstrucButton.transform.Find("KeyIconText")?.GetComponent<TMP_Text>();

                DKeyIconText.text = "D";
                Destroy(DchildTransform.gameObject);
                DimageComponent.enabled = false;

                StartCoroutine(CheckForWASDPress(WInstrucButton.gameObject, AInstrucButton.gameObject, SInstrucButton.gameObject, DInstrucButton.gameObject));
                
                GameManager.Instance.NextTutorialStep();
            }
            else
            {
                Debug.LogWarning("Notebook(Clone) not found in the Canvas.");
            }

        }

    }

    private System.Collections.IEnumerator CheckForWASDPress(GameObject WButton, GameObject AButton, GameObject SButton, GameObject DButton)
    {
        Debug.Log("Next Step: " + nextStep);
        Debug.Log("Any WASD Button Pressed");

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D));
        Destroy(WButton.gameObject);
        Destroy(AButton.gameObject);
        Destroy(SButton.gameObject);
        Destroy(DButton.gameObject);
    }

    private System.Collections.IEnumerator CheckForNotebookPress()
    {
        Debug.Log("Next Step: " + nextStep);
        Debug.Log("N Button Pressed");
        notebookUIManager = Instantiate(notebookUIManagerPrefab, generalCanvas.transform);
        
        // Wait until the "N" key is pressed, without constant updates
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.N));

        notebookUIManager.DisableAllIngredientButtons();
        notebookUIManager.DisablePotionBookmarkButton();
        notebookUIManager.DisableIngredientBookmarkButton();

        dialogueManager.StartDialogue("explorationTOC");
        Destroy(InstrucButton.gameObject);
        nextStep++;

        dialogueManager.OnDialogueFinished += RunNextDialogueNode;
    }

    private void Update()
    {
        if(nextStep!= 1 && nextStep != 7)
        {
            // Check if the notebook UI is inactive and if the player controller is enabled
            if (notebookUIManager != null && notebookUIManager.currentNotebookUI != null && notebookUIManager.currentNotebookUI.activeSelf == false)
            {
                Debug.Log("Notebook is inactive, player controller is being disabled.");
                _playerControllerScript.enabled = false;  // Disable player controls
            }
        }
    }

}
