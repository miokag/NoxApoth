using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ingredient Database", menuName = "Potion/Ingredient Database")]
public class IngredientDatabase : ScriptableObject
{
    public List<Ingredient> ingredients = new List<Ingredient>();

    // Clone function for the IngredientDatabase
    public IngredientDatabase Clone()
    {
        IngredientDatabase clone = Instantiate(this);  // Clone the database itself
        clone.ingredients = new List<Ingredient>();

        foreach (var ingredient in ingredients)
        {
            // Deep clone each ingredient using a helper method (this ensures all internal references are copied)
            clone.ingredients.Add(ingredient.Clone());
        }

        return clone;
    }
    // To find ingredients
    public Ingredient GetIngredientByName(string name)
    {
        return ingredients.Find(i => i.ingredientName == name);
    }


}