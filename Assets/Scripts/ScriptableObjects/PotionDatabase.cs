using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Potion Database", menuName = "Potion/Potion Database")]
public class PotionDatabase : ScriptableObject
{
    public List<Potion> potions = new List<Potion>();

    // Add the Clone function here
    public PotionDatabase Clone(IngredientDatabase clonedIngredientDatabase)
    {
        PotionDatabase clone = Instantiate(this);  // Clone the potion database itself
        clone.potions = new List<Potion>();

        // Clone each potion in the list
        foreach (var potion in potions)
        {
            // Clone the potion
            Potion clonedPotion = potion.Clone();

            // Now update the cloned potion's ingredients with ingredients from the cloned IngredientDatabase
            List<Ingredient> clonedIngredients = new List<Ingredient>();
            foreach (var ingredient in clonedPotion.ingredients)
            {
                // Find the ingredient in the cloned IngredientDatabase by name (or any other suitable unique identifier)
                Ingredient clonedIngredient = clonedIngredientDatabase.GetIngredientByName(ingredient.ingredientName);
                if (clonedIngredient != null)
                {
                    clonedIngredients.Add(clonedIngredient);
                }
                else
                {
                    Debug.LogWarning($"Ingredient {ingredient.ingredientName} not found in cloned ingredient database.");
                }
            }

            clonedPotion.ingredients = clonedIngredients;  // Assign the cloned ingredients to the potion
            clone.potions.Add(clonedPotion);  // Add the cloned potion to the clone
        }

        return clone;
    }


    public Potion GetRandomPotion()
    {
        if (potions == null || potions.Count == 0)
        {
            Debug.LogWarning("Potion database is empty or not initialized!");
            return null;
        }

        int randomIndex = Random.Range(0, potions.Count);
        return potions[randomIndex];
    }
}