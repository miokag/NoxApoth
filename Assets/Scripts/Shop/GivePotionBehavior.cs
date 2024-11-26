using System;
using DialogueSystem;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
public class GivePotionBehavior : MonoBehaviour
{
    private OrderManager _orderManager;
    private int correctCounter = 0;
    private bool isFail;
    private bool isPass;
    private bool isSuccess;

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
        // Get a reference to the cloned potion database
        PotionDatabase clonedPotionDatabase = GameManager.Instance.GetClonedPotionDatabase();
        
        // Check if the PotionMix is not empty
        if (GameManager.Instance.PotionMix == null || GameManager.Instance.PotionMix.Count == 0)
        {
            Debug.Log("Potion Mix is empty!");
            return;
        }

        // Retrieve the potion name to compare with (assuming it's determined elsewhere)
        string potionName = "Healing Potion"; // Replace with the actual potion name logic
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
        foreach (string ingredientName in requiredIngredientNames)
        {
            if (potionMixIngredientNames.Contains(ingredientName))
            {
                Debug.Log($"Ingredient '{ingredientName}' matches.");
                correctCounter++;

                // Now check if the ingredient is found in the clonedPotionDatabase
                Ingredient clonedIngredient = clonedPotionDatabase.potions
                    .SelectMany(potion => potion.ingredients)
                    .FirstOrDefault(i => i.ingredientName == ingredientName);

                if (clonedIngredient != null)
                {
                    // Check if the ingredient is found
                    bool isFound = clonedIngredient.FoundState;

                    if (isFound)
                    {
                        Debug.Log($"Ingredient '{ingredientName}' is found.");
                        correctCounter++; // Increase the counter if the ingredient is found
                    }
                    else
                    {
                        Debug.Log($"Ingredient '{ingredientName}' is not found.");
                        allMatch = false; // If not found, mark as a mismatch
                    }
                }
                else
                {
                    Debug.LogWarning($"Ingredient '{ingredientName}' not found in cloned potion database.");
                    allMatch = false; // If the ingredient is not in the cloned database, it's a mismatch
                }
            }
            else
            {
                Debug.Log($"Ingredient '{ingredientName}' is missing in the PotionMix.");
                allMatch = false; // If the ingredient is missing, mark as a mismatch
            }
        }

        // Log the result of the comparison
        if (allMatch && potionMixIngredientNames.Count == requiredIngredientNames.Count)
        {
            Debug.Log("All ingredients match! The potion is correct.");
            Checker();
            Debug.Log($"Total correct counter: {correctCounter}");
            PotionResults();
        }
        else
        {
            isFail = true;
            Debug.Log("The potion is incorrect! Ingredients do not match.");
            Checker();
            PotionResults();
        }
    }

    private void Checker()
    {
        PotionDatabase clonedPotionDatabase = GameManager.Instance.GetClonedPotionDatabase();
        
        // Iterate over the ingredients in the PotionMix
        foreach (Ingredient ingredient in GameManager.Instance.PotionMix)
        {
            Debug.Log($"Comparing states for ingredient: {ingredient.ingredientName}");

            // Find the cloned ingredient from the clonedPotionDatabase
            Ingredient clonedIngredient = clonedPotionDatabase.potions
                .SelectMany(potion => potion.ingredients)
                .FirstOrDefault(i => i.ingredientName == ingredient.ingredientName);

            if (clonedIngredient != null)
            {
                // Now check if the ingredient is cooked properly in the cloned ingredient
                bool isCookedProperly = clonedIngredient.isCookedProperly;

                // First, check if the current processed state matches the needed processed state
                if (ingredient.currentProcessedState != ingredient.neededProcessedState)
                {
                    Debug.Log($"{ingredient.ingredientName} - Current State: {ingredient.currentProcessedState}, Needed State: {ingredient.neededProcessedState}");
                    Debug.Log($"{ingredient.ingredientName} does not match needed process state.");
                }
                else
                {
                    Debug.Log($"{ingredient.ingredientName} matches needed process state.");
                    
                    // Check if the ingredient is cooked properly in the cloned ingredient
                    if (isCookedProperly)
                    {
                        Debug.Log($"{ingredient.ingredientName} is cooked properly.");
                        if (isFail != true) correctCounter++;
                    }
                    else
                    {
                        Debug.Log($"{ingredient.ingredientName} is not cooked properly.");
                    }
                }
            }
            else
            {
                Debug.LogWarning($"Ingredient {ingredient.ingredientName} not found in cloned potion database.");
            }

            // Check gathered state
            if (ingredient.currentGatheredState == Ingredient.GatheredState.Perfect)
            {
                Debug.Log($"{ingredient.ingredientName} gathered state is perfect.");
                if (isFail != true) correctCounter += 2;
            }
            else if (ingredient.currentGatheredState == Ingredient.GatheredState.Good)
            {
                Debug.Log($"{ingredient.ingredientName} gathered state is good.");
                if (isFail != true) correctCounter++;
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
    }
    
    public List<string> GetIngredientNamesByPotion(string potionName)
    {
        // Find the potion by its name
        Potion potion = GameManager.Instance.GetClonedPotionDatabase().potions
            .FirstOrDefault(p => p.potionName == potionName);

        if (potion == null)
        {
            Debug.LogWarning($"Potion with name {potionName} not found in the database.");
            return null;
        }

        // Extract and return the names of the ingredients
        List<string> ingredientNames = potion.ingredients.Select(ingredient => ingredient.ingredientName).ToList();
        return ingredientNames;
    }


}
