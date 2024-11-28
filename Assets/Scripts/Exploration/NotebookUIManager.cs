using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.SceneManagement;

public class NotebookUIManager : MonoBehaviour
{
    [SerializeField] private GameObject notebookUI;
    [SerializeField] private Canvas canvas;

    [SerializeField] private IngredientDatabase ingredientDatabase;
    [SerializeField] private GameObject ingredientListPagePrefab;
    [SerializeField] private GameObject ingredientButtonPrefab; // Prefab for individual ingredient buttons
    [SerializeField] private GameObject ingredientPagePrefab;   // Prefab for ingredient details page

    [SerializeField] private PotionDatabase potionDatabase;
    [SerializeField] private GameObject potionListPagePrefab;
    [SerializeField] private GameObject potionButtonPrefab; // Prefab for individual ingredient buttons
    [SerializeField] private GameObject potionPagePrefab;   // Prefab for ingredient details page
    [SerializeField] private PlayerControllerScript playerController;
    
    public Dictionary<string, Button> ingredientButtonDictionary = new Dictionary<string, Button>();
    public Dictionary<string, Button> potionButtonDictionary = new Dictionary<string, Button>();

    public Button ingredientBookmarkButton;
    public Button potionBookmarkButton;

    private GameObject ingredientListPage;
    private Transform ingredientListContainer;

    private GameObject potionListPage;
    private Transform  potionListContainer;

    public GameObject currentNotebookUI;
    private GameObject currentIngredientPage;
    private GameObject currentPotionPage;
    private List<Button> potionButtons = new List<Button>();
    private List<Button> ingredientButtons = new List<Button>();

    public GameObject ingredientButton;
    public GameObject potionButton;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Exploration")
        {
            GameObject playerContainer = GameObject.Find("Player");
            GameObject player = playerContainer.transform.Find("PlayerSprite").gameObject;
            playerController = player.GetComponent<PlayerControllerScript>();
        }

        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    private void OnEnable()
    {
        var openNotebookAction = new InputAction("OpenNotebook", binding: "<Keyboard>/n");
        openNotebookAction.Enable();
        //LogIngredientDatabaseContents();
    }

    private void Update()
    {
        if (Keyboard.current.nKey.wasPressedThisFrame)
        {
            ToggleNotebookUI();
        }
    }

    private void ToggleNotebookUI()
    {
        if (currentNotebookUI == null)
        {
            currentNotebookUI = Instantiate(notebookUI, canvas.transform);
            currentNotebookUI.SetActive(true);
            Debug.Log("Notebook UI instantiated and displayed.");

            ingredientDatabase = GameManager.Instance.GetClonedIngredientDatabase();
            potionDatabase = GameManager.Instance.GetClonedPotionDatabase();

            // Setup default view to show the ingredient list page only
            InstantiateIngredientListPage();


            if (potionListPage != null) potionListPage.SetActive(false);

            // Add listeners for bookmark buttons
            ingredientBookmarkButton = currentNotebookUI.transform.Find("IngredientBookmark")?.GetComponent<Button>();
            
            if (ingredientBookmarkButton != null)
            {
                ingredientBookmarkButton.onClick.AddListener(() => SwitchToIngredientListPage());
                Debug.Log("IngredientBookmark button listener added.");
            }

            potionBookmarkButton = currentNotebookUI.transform.Find("PotionBookmark")?.GetComponent<Button>();
            if (potionBookmarkButton != null)
            {
                potionBookmarkButton.onClick.AddListener(() => SwitchToPotionListPage());
                Debug.Log("PotionBookmark button listener added.");
            }
        }
        else
        {
            bool isActive = currentNotebookUI.activeSelf;
            currentNotebookUI.SetActive(!isActive);
            Debug.Log("Toggled Notebook UI visibility to: " + !isActive);
            if (SceneManager.GetActiveScene().name == "Exploration")
            { 
                if (!isActive && playerController != null)
                {
                    playerController.enabled = false;  // Disable PlayerController when the UI is active
                    Debug.Log("PlayerController disabled.");
                }
                else if (isActive && playerController != null)
                {
                    playerController.enabled = true;   // Enable PlayerController when the UI is hidden
                    Debug.Log("PlayerController enabled.");
                }
            }
            
        }
    }

    public void DisableIngredientBookmarkButton()
    {
        if (ingredientBookmarkButton != null)
        {
            ingredientBookmarkButton.interactable = false;  // Disable the button
            Debug.Log("IngredientBookmark button disabled.");
        }
    }

    // Example function to disable Potion Bookmark button
    public void DisablePotionBookmarkButton()
    {
        if (potionBookmarkButton != null)
        {
            potionBookmarkButton.interactable = false;  // Disable the button
            Debug.Log("PotionBookmark button disabled.");
        }
    }

    public void EnableIngredientBookmarkButton()
    {
        if (ingredientBookmarkButton != null)
        {
            ingredientBookmarkButton.interactable = true;  // Disable the button
            Debug.Log("IngredientBookmark button enabled.");
        }
    }

    // Example function to disable Potion Bookmark button
    public void EnablePotionBookmarkButton()
    {
        if (potionBookmarkButton != null)
        {
            potionBookmarkButton.interactable = true;  // Disable the button
            Debug.Log("PotionBookmark button enabled.");
        }
    }

    private void SwitchToIngredientListPage()
    {
        if (currentIngredientPage != null)
        {
            Destroy(currentIngredientPage);
        }
        else if (currentPotionPage != null)
        {
            Destroy(currentPotionPage);
        }

        if (potionListPage != null) potionListPage.SetActive(false);
        if (ingredientListPage == null)
        {
            InstantiateIngredientListPage();
        }
        else
        {
            ingredientListPage.SetActive(true);
        }
        Debug.Log("Switched to Ingredient List Page.");
    }

    private void SwitchToPotionListPage()
    {
        if (currentIngredientPage != null)
        {
            Destroy(currentIngredientPage);
        }
        else if (currentPotionPage != null)
        {
            Destroy(currentPotionPage);
        }

        if (ingredientListPage != null) ingredientListPage.SetActive(false);
        if (potionListPage == null)
        {
            InstantiatePotionsListPage();
        }
        else
        {
            potionListPage.SetActive(true);
        }
        Debug.Log("Switched to Potion List Page.");
    }



    private void InstantiateIngredientListPage()
    {
        // Instantiate ingredient list page as a child of notebook UI
        ingredientListPage = Instantiate(ingredientListPagePrefab, currentNotebookUI.transform);
        ingredientListPage.SetActive(true);

        // Find the container inside ingredient list page where the buttons will go
        ingredientListContainer = ingredientListPage.transform.Find("IngredientsListContainer");

        if (ingredientListContainer != null)
        {
            // Loop through each ingredient in the database and create a button for each
            foreach (Ingredient ingredient in ingredientDatabase.ingredients)
            {
                ingredientButton = Instantiate(ingredientButtonPrefab, ingredientListContainer);
                TMP_Text buttonText = ingredientButton.GetComponentInChildren<TMP_Text>();

                if (buttonText != null)
                {
                    buttonText.text = ingredient.ingredientName;  // Set button text to ingredient name
                }

                // Add listener to handle button click for each ingredient
                Button buttonComponent = ingredientButton.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.onClick.AddListener(() => OnIngredientButtonClicked(ingredient));
                    ingredientButtons.Add(buttonComponent);

                    // Store the button in the dictionary using the ingredient name as the key
                    ingredientButtonDictionary[ingredient.ingredientName] = buttonComponent;
                }
            }
        }
        else
        {
            Debug.LogError("IngredientsListContainer not found in ingredient list page.");
        }
    }

    private void InstantiatePotionsListPage()
    {
        potionListPage = Instantiate(potionListPagePrefab, currentNotebookUI.transform);
        potionListPage.SetActive(true);

        potionListContainer = potionListPage.transform.Find("PotionsListContainer");

        if (potionListContainer != null)
        {
            foreach (Potion potion in potionDatabase.potions)
            {
                potionButton = Instantiate(potionButtonPrefab, potionListContainer);
                TMP_Text buttonText = potionButton.GetComponentInChildren<TMP_Text>();

                if (buttonText != null)
                {
                    buttonText.text = potion.potionName;
                }

                Button buttonComponent = potionButton.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.onClick.AddListener(() => OnPotionButtonClicked(potion));
                    potionButtons.Add(buttonComponent);

                    potionButtonDictionary[potion.potionName] = buttonComponent;
                }
            }
        }
        else
        {
            Debug.LogError("PotionsListContainer not found in ingredient list page.");
        }
    }


    public void DisableAllPotionButtons()
    {
        Debug.Log("Disabling all potion buttons...");
        Debug.Log($"Total buttons found: {potionButtons.Count}");

        foreach (Button potionButton in potionButtons)
        {
            if (potionButton != null)
            {
                potionButton.interactable = false;
                Debug.Log($"Disabled button: {potionButton.name}");
            }
            else
            {
                Debug.LogWarning("Found a null button in potionButtons list.");
            }
        }
    }

    public void EnableAllPotionButtons()
    {
        Debug.Log("Disabling all potion buttons...");
        Debug.Log($"Total buttons found: {potionButtons.Count}");

        foreach (Button potionButton in potionButtons)
        {
            if (potionButton != null)
            {
                potionButton.interactable = true;
                Debug.Log($"Enabled button: {potionButton.name}");
            }
            else
            {
                Debug.LogWarning("Found a null button in potionButtons list.");
            }
        }
    }

    public void DisableAllIngredientButtons()
    {
        Debug.Log("Disabling all potion buttons...");
        Debug.Log($"Total buttons found: {ingredientButtons.Count}");

        foreach (Button inredientButton in ingredientButtons)
        {
            if (inredientButton != null)
            {
                inredientButton.interactable = false;
                Debug.Log($"Disabled button: {inredientButton.name}");
            }
            else
            {
                Debug.LogWarning("Found a null button in inredientButtons list.");
            }
        }
    }

    public void EnableAllIngredientButtons()
    {
        Debug.Log("Disabling all potion buttons...");
        Debug.Log($"Total buttons found: {ingredientButtons.Count}");

        foreach (Button inredientButton in ingredientButtons)
        {
            if (inredientButton != null)
            {
                inredientButton.interactable = true;
                Debug.Log($"Enabled button: {inredientButton.name}");
            }
            else
            {
                Debug.LogWarning("Found a null button in inredientButtons list.");
            }
        }
    }

    public void DisableIngredientButton(string ingredientName)
    {
        foreach (Button ingredientButton in ingredientButtons)
        {
            // Check if this button corresponds to the given potion (you can add a condition to match)
            if (ingredientButton.GetComponentInChildren<TMP_Text>().text == ingredientName)
            {
                ingredientButton.interactable = false;  // Disable the button
                Debug.Log($"Potion button for {ingredientName} disabled.");
                break;
            }
        }
    }

    public void DisablePotionButton(string potionName)
    {
        foreach (Button potionButton in potionButtons)
        {
            // Check if this button corresponds to the given potion (you can add a condition to match)
            if (potionButton.GetComponentInChildren<TMP_Text>().text == potionName)
            {
                potionButton.interactable = false;  // Disable the button
                Debug.Log($"Potion button for {potionName} disabled.");
                break;
            }
        }
    }

    public void EnableIngredientButton(string inrgedientName)
    {
        foreach (Button ingredientButton in ingredientButtons)
        {
            // Check if this button corresponds to the given potion name
            if (ingredientButton.GetComponentInChildren<TMP_Text>().text == inrgedientName)
            {
                ingredientButton.interactable = true;  // Enable the button
                Debug.Log($"Potion button for {inrgedientName} enabled.");
                break;
            }
        }
    }

    public void EnablePotionButton(string potionName)
    {
        foreach (Button potionButton in potionButtons)
        {
            // Check if this button corresponds to the given potion name
            if (potionButton.GetComponentInChildren<TMP_Text>().text == potionName)
            {
                potionButton.interactable = true;  // Enable the button
                Debug.Log($"Potion button for {potionName} enabled.");
                break;
            }
        }
    }


    private void OnIngredientButtonClicked(Ingredient ingredient)
    {
        Debug.Log("Clicked on ingredient: " + ingredient.ingredientName);

        // Destroy any existing ingredient page to avoid duplicates
        if (currentIngredientPage != null)
        {
            Destroy(currentIngredientPage);
        }

        // Instantiate the ingredient page as a child of the notebook UI
        currentIngredientPage = Instantiate(ingredientPagePrefab, currentNotebookUI.transform);

        // Find and set the text fields for ingredient name, description, and effects
        TMP_Text nameText = currentIngredientPage.transform.Find("IngredientNameText")?.GetComponent<TMP_Text>();
        TMP_Text descriptionText = currentIngredientPage.transform.Find("IngredientDescriptionText")?.GetComponent<TMP_Text>();
        TMP_Text effectsText = currentIngredientPage.transform.Find("IngredientEffectsText")?.GetComponent<TMP_Text>();

        if (nameText != null) nameText.text = ingredient.ingredientName;

        if (descriptionText != null)
        {
            descriptionText.text = string.Join("\n\n", ingredient.description);
        }

        if (effectsText != null)
        {
            effectsText.text = string.Join("\n\n", ingredient.effects);
        }

        Debug.Log("Ingredient page displayed with details for: " + ingredient.ingredientName);
    }

    private void OnPotionButtonClicked(Potion potion)
    {
        
        Debug.Log("Clicked on potion: " + potion.potionName);

        // Destroy any existing potion page to avoid duplicates
        if (currentPotionPage != null)
        {
            Destroy(currentPotionPage);
        }


        currentPotionPage = Instantiate(potionPagePrefab, currentNotebookUI.transform);
        GameObject potionPageContainer = currentPotionPage.transform.Find("PotionPageContainer").gameObject;

        // Find and set the text fields for potion name and ingredients (no image for now)
        TMP_Text nameText = currentPotionPage.transform.Find("PotionNameText")?.GetComponent<TMP_Text>();
        TMP_Text potionIngredients = potionPageContainer.transform.Find("PotionIngredientsText")?.GetComponent<TMP_Text>();
        TMP_Text potionProcess = potionPageContainer.transform.Find("PotionProcessText")?.GetComponent<TMP_Text>();

        if (nameText != null) nameText.text = potion.potionName;

        if (potionIngredients != null)
        {
            List<string> ingredientStrings = new List<string>();
            List<string> ingredientProcessStrings = new List<string>();

            foreach (Ingredient ingredient in potion.ingredients)
            {
                // Check if the ingredient is found in the cloned ingredient database
                if (ingredient.FoundState)
                {
                    ingredientStrings.Add(ingredient.ingredientName); // Add ingredient name to list
                }
                else
                {
                    ingredientStrings.Add("???"); // Add "???" if the ingredient is not found
                }

                if (ingredient.isinPotion)
                {
                    ingredientProcessStrings.Add(ingredient.neededProcessedState.ToString());
                }
                else
                {
                    ingredientProcessStrings.Add("???");
                }
            }

            // Join the ingredients list into a string with line breaks
            potionIngredients.text = string.Join("\n\n", ingredientStrings);
            potionProcess.text = string.Join("\n\n", ingredientProcessStrings);
        }

        Debug.Log("Potion page displayed with details for: " + potion.potionName);
    }


    // Destroy the ingredient page when the IngredientBookmark button is clicked
    private void DestroyIngredientPage()
    {
        if (currentIngredientPage != null)
        {
            Destroy(currentIngredientPage);
            Debug.Log("Ingredient page destroyed.");
        }
        else
        {
            Debug.Log("No ingredient page to destroy.");
        }
    }

    private void DestroyPotionPage()
    {
        if (currentPotionPage != null)
        {
            Destroy(currentPotionPage);
            Debug.Log("Potion page destroyed.");
        }
        else
        {
            Debug.Log("No potion page to destroy.");
        }
    }

    private void LogIngredientDatabaseContents()
{
    if (ingredientDatabase == null || ingredientDatabase.ingredients == null)
    {
        Debug.LogError("Ingredient database is null or empty.");
        return;
    }

    Debug.Log("Logging ingredient database contents:");

    foreach (Ingredient ingredient in ingredientDatabase.ingredients)
    {
        string ingredientInfo = $"Name: {ingredient.ingredientName}, " +
                                $"Description: {string.Join(", ", ingredient.description)}, " +
                                $"Effects: {string.Join(", ", ingredient.effects)}, " +
                                $"FoundState: {ingredient.FoundState}";
        Debug.Log(ingredientInfo);
    }
}


}
