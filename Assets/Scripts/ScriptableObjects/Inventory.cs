using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Inventory Database", menuName = "Inventory/Inventory Database")]

public class Inventory : ScriptableObject
{
    // List of ingredients in the inventory
    public List<Ingredient> ingredients = new List<Ingredient>();


    // Clears the entire inventory list
    public void ClearInventory()
    {
        ingredients.Clear();
    }

    // Adds an ingredient to the inventory
    public void AddToInventory(Ingredient ingredient)
    {
        ingredients.Add(ingredient);
    }

    // Removes a specific ingredient from the inventory
    public void RemoveInInventory(Ingredient ingredient)
    {
        ingredients.Remove(ingredient);
    }

    // Displays the inventory (you can add your own logic for this)
    public void DisplayInventory()
    {
        foreach (Ingredient ingredient in ingredients)
        {
            Debug.Log("Ingredient: " + ingredient.name + ", Found State: " + ingredient.FoundState);
        }
    }
}
