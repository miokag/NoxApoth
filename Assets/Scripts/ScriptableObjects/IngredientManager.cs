using UnityEngine;
using System.Collections.Generic;

public class IngredientManager : MonoBehaviour
{
    public List<Ingredient> ingredientsList = new List<Ingredient>();  // A list to hold all ingredients

    void Start()
    {
        InitializeIngredients();
    }

    void InitializeIngredients()
    {
        Ingredient opiumPoppyTree = ScriptableObject.CreateInstance<Ingredient>();
        opiumPoppyTree.ingredientName = "Opium Poppy Tree";

        opiumPoppyTree.icon = null; // Load sprite from Resources folder
        //opiumPoppyTree.icon = Resources.Load<Sprite>("Icons/opiumPoppyTree");  // Load sprite from Resources folder
        opiumPoppyTree.prefab = Resources.Load<GameObject>("Objects/Ingredients/Prefabs/OpiumPoppyTreePrefab");  // Load prefab from Resources folder
        opiumPoppyTree.description = new List<string> { "Opium Poppy Tree Description" };
        opiumPoppyTree.effects = new List<string> { "Can soothe anxiety and calm the mind when consumed in small doses.", "May induce a temporary feeling of numbness or disorientation if overused.", "Can cause hallucinations" };

        // Add to the list
        ingredientsList.Add(opiumPoppyTree);

        // Similarly, add more ingredients if needed

        Ingredient serenityHerb = ScriptableObject.CreateInstance<Ingredient>();
        serenityHerb.ingredientName = "Serenity Herb";

        serenityHerb.icon = null; // Load sprite from Resources folder
        //serenityHerb.icon = Resources.Load<Sprite>("Icons/serenityHerb");  // Load sprite from Resources folder
        serenityHerb.prefab = Resources.Load<GameObject>("Objects/Ingredients/Prefabs/SerenityHerbPrefab");  // Load prefab from Resources folder
        serenityHerb.description = new List<string> { "Serenity Herb Description" };
        serenityHerb.effects = new List<string> { "Provides intense pain relief and a deep sense of relaxation.", "Can lead to dizziness or a heavy, sluggish feeling if consumed in excess." };

        // Add to the list
        ingredientsList.Add(serenityHerb);

        DisplayIngredients();
    }

    // You can access this list later in your game for any purpose
    void DisplayIngredients()
    {
        foreach (var ingredient in ingredientsList)
        {
            Debug.Log("Ingredient Name: " + ingredient.ingredientName);
            Debug.Log("Description: " + string.Join(", ", ingredient.description));
        }
    }
}
