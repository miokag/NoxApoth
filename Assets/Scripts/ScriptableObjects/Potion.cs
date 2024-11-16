using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Potion", menuName = "Potion/Potion")]
public class Potion : ScriptableObject
{
    public string potionName;
    public Sprite icon;
    public List<Ingredient> ingredients;

    public enum PotionState { None, Fail, Pass, Perfect }

    public PotionState currentPotionState;

    // Add the Clone function here
    public Potion Clone()
    {
        Potion clone = Instantiate(this);
        clone.ingredients = new List<Ingredient>();

        // Clone each ingredient in the ingredients list
        foreach (var ingredient in ingredients)
        {
            clone.ingredients.Add(ingredient.Clone());  // Ensure each ingredient is cloned
        }

        return clone;
    }
}