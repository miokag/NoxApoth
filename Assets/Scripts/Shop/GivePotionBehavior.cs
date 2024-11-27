using System;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

public class GivePotionBehavior : MonoBehaviour
{
    private OrderManager _orderManager;
    private int correctCounter = 0;
    private bool isFail;
    private bool isPass;
    private bool isSuccess;

    // New list to store wrong ingredient names
    private List<string> wrongIngredients = new List<string>();
    private List<string> wrongProcess = new List<string>();

    private void Start()
    {
        _orderManager = FindObjectOfType<OrderManager>();
    }

    private void OnMouseDown()
    {
        Debug.Log("OnMouseDown Triggered!"); // To confirm the method is being called

        if (GameManager.Instance.PotionMix == null || GameManager.Instance.PotionMix.Count == 0)
        {
            Debug.Log("No Potion Mix!");
            return;
        }
        else
        {
            Debug.Log("Potion Mix is given");
            CompareStates();
        }
    }

    private void CompareStates()
    {
        // Get the current customer’s name from GameManager
        string customerName = GameManager.Instance.currentCustomer;
        
        Order customerOrder = OrderManager.Instance.GetAllOrders()
            .Find(order => order.CharacterName == customerName);
        
        // Get a reference to the cloned potion database
        PotionDatabase clonedPotionDatabase = GameManager.Instance.GetClonedPotionDatabase();
    
        // Check if the PotionMix is not empty
        if (GameManager.Instance.PotionMix == null || GameManager.Instance.PotionMix.Count == 0)
        {
            Debug.Log("Potion Mix is empty!");
            return;
        }
        // Retrieve the potion name to compare with (assuming it's determined elsewhere)
        string potionName = customerOrder.CustomerOrder; // Replace with the actual potion name logic
        List<string> requiredIngredientNames = GetIngredientNamesByPotion(potionName);
        if (requiredIngredientNames == null || requiredIngredientNames.Count == 0)
        {
            Debug.Log($"No ingredients found for potion: {potionName}");
            return;
        }
        // Get the ingredient names from the PotionMix
        List<string> potionMixIngredientNames = GameManager.Instance.PotionMix
            .Select(ingredient => ingredient.ingredientName)
            .ToList();
        // Compare the ingredients
        bool allMatch = true; // Flag to check if all ingredients match
        foreach (string ingredientName in potionMixIngredientNames)
        {
            if (requiredIngredientNames.Contains(ingredientName))
            {
                Debug.Log($"Ingredient '{ingredientName}' matches.");
                correctCounter++;
            }
            else
            {
                Debug.Log($"Ingredient '{ingredientName}' is not required or missing in the required list.");
                allMatch = false;
                wrongIngredients.Add(ingredientName);
            }
        }
        // Log the result of the comparison
        if (allMatch && potionMixIngredientNames.Count == requiredIngredientNames.Count)
        {
            Debug.Log("All ingredients match! The potion is correct.");
            Checker();
            GameManager.Instance.ClearPotionMix();
            Debug.Log($"Total correct counter: {correctCounter}");
            PotionResults();
        }
        else
        {
            isFail = true;
            Debug.Log("The potion is incorrect! Ingredients do not match.");
            PotionResults();
        }
    }
    private void Checker()
    {
        // Iterate over the ingredients in the PotionMix
        foreach (Ingredient ingredient in GameManager.Instance.PotionMix)
        {
            Debug.Log($"Comparing states for ingredient: {ingredient.ingredientName}");
            if (ingredient.currentProcessedState != ingredient.neededProcessedState)
            {
                Debug.Log($"{ingredient.ingredientName} - Current State: {ingredient.currentProcessedState}, Needed State: {ingredient.neededProcessedState}");
                Debug.Log($"{ingredient.ingredientName} does not match needed process state.");
                wrongProcess.Add(ingredient.name);
            }
            else
            {
                Debug.Log($"{ingredient.ingredientName} - Current State: {ingredient.currentProcessedState}, Needed State: {ingredient.neededProcessedState}");
                Debug.Log($"{ingredient.ingredientName} matches needed process state.");
                if(isFail != true) correctCounter++; 
            }
            if (ingredient.currentGatheredState == Ingredient.GatheredState.Perfect)
            {
                Debug.Log($"{ingredient.ingredientName} gathered state is perfect.");
                if(isFail != true) correctCounter = correctCounter + 2;
            }
            else if (ingredient.currentGatheredState == Ingredient.GatheredState.Good)
            {
                Debug.Log($"{ingredient.ingredientName} gathered state is good.");
                if(isFail != true) correctCounter++;
            }
        }
    }

    private void PotionResults()
    {
        if (correctCounter >= 7 && correctCounter <= 10) isPass = true;
        else if (correctCounter >= 11 && correctCounter <= 20) isSuccess = true;
        else if (correctCounter >= 0 && correctCounter <= 6) isFail = true;

        Debug.Log("Potion Results: isPass: " + isPass + ", isSuccess: " + isSuccess + ", isFail: " + isFail);

        GameManager.Instance.ClearPotionMix();
        _orderManager.RemoveOrderByCustomer(GameManager.Instance.currentCustomer);

        // After checking potion, show dialogues for wrong ingredients
        ShowWrongIngredientDialogues();
        ShowWrongProcessDialogues();
    }

    
    private void ShowWrongProcessDialogues()
{
    StartCoroutine(ShowIngredientDialogues(wrongProcess, "WrongProcess"));
}

private void ShowWrongIngredientDialogues()
{
    StartCoroutine(ShowIngredientDialogues(wrongIngredients, "WrongIngredients"));
}

private IEnumerator ShowIngredientDialogues(List<string> ingredients, string resourceFolder)
{
    Debug.Log($"{resourceFolder}: " + string.Join(", ", ingredients));

    // Get the current customer
    var currentCustomer = GameManager.Instance.currentCustomer;

    // Get the ingredient dialogue list for the respective resource (WrongProcess or WrongIngredients)
    var ingredientDialogueList = Resources.Load<IngredientDialogueList>($"IngredientDialogueList/{resourceFolder}");
    if (ingredientDialogueList == null)
    {
        Debug.LogError($"IngredientDialogueList for {resourceFolder} not found in resources.");
        yield break;
    }

    // Show the "Well, it works but..." dialogue if it's the wrong process
    if (wrongProcess.Count > 0)
    {
        string introDialogue = "Well, it works but...";  // The message to show before the ingredient dialogues
        DialogueManager.Instance.ShowDialogue(currentCustomer, introDialogue);

        // Wait for the player to press Space before continuing to the ingredient-specific dialogues
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
    }
    
    if (wrongIngredients.Count > 0)
    {
        string introDialogue = "Uh, oh... I don't feel right!";  // The message to show before the ingredient dialogues
        DialogueManager.Instance.ShowDialogue(currentCustomer, introDialogue);

        // Wait for the player to press Space before continuing to the ingredient-specific dialogues
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
    }

    // Iterate through the list of ingredients (wrongProcess or wrongIngredients)
    foreach (string ingredient in ingredients)
    {
        // Clean up the name by removing "(Clone)" and any extra spaces
        string cleanedIngredientName = ingredient.Replace("(Clone)", "").Trim();
        
        Debug.Log($"Processing ingredient: {cleanedIngredientName}");

        // Find the corresponding dialogue entry based on the cleaned ingredient name (case insensitive)
        var dialogueEntry = ingredientDialogueList.ingredientDialogues
            .FirstOrDefault(entry => entry.ingredient.ingredientName.Equals(cleanedIngredientName, StringComparison.OrdinalIgnoreCase));

        // Check if a dialogue entry was found and if it has dialogues available
        if (dialogueEntry != null && dialogueEntry.dialogues.Count > 0)
        {
            // Get a random dialogue from the list
            string randomDialogue = dialogueEntry.dialogues[UnityEngine.Random.Range(0, dialogueEntry.dialogues.Count)];

            // Log the random dialogue
            Debug.Log($"{currentCustomer} ({resourceFolder}): {randomDialogue}");

            // Show the dialogue
            DialogueManager.Instance.ShowDialogue(currentCustomer, randomDialogue);

            // Wait for the player to press Space before continuing to the next dialogue
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        }
        else
        {
            // Log a warning if no dialogue entry is found for the current ingredient
            Debug.LogWarning($"No dialogue found for ingredient: {cleanedIngredientName}");
        }
    }

    Debug.Log("All dialogues shown.");
}


    public List<string> GetIngredientNamesByPotion(string potionName)
    {
        Potion potion = GameManager.Instance.GetClonedPotionDatabase().potions
            .FirstOrDefault(p => p.potionName == potionName);

        if (potion == null)
        {
            Debug.LogWarning($"Potion with name {potionName} not found in the database.");
            return null;
        }

        List<string> ingredientNames = potion.ingredients.Select(ingredient => ingredient.ingredientName).ToList();
        return ingredientNames;
    }
}
