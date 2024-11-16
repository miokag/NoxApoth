using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class DatabaseInitializer : MonoBehaviour
{
    public PotionDatabase mainPotionDatabase;
    public IngredientDatabase mainIngredientDatabase;
    private PotionDatabase potionDatabase;
    private IngredientDatabase ingredientDatabase;

    private string potionDatabasePath = "Assets/Scripts/ScriptableObjects/potionDatabase.json";
    private string ingredientDatabasePath = "Assets/Scripts/ScriptableObjects/ingredientDatabase.json";

    // Wrapper classes for serialization
    [System.Serializable]
    public class PotionDatabaseWrapper
    {
        public List<Potion> potions;
    }

    [System.Serializable]
    public class IngredientDatabaseWrapper
    {
        public List<Ingredient> ingredients;
    }

    private void Start()
    {
        potionDatabase = Instantiate(mainPotionDatabase);
        ingredientDatabase = Instantiate(mainIngredientDatabase);

        // Initialize with default scriptable objects and save them to JSON
        InitializeDatabases();
    }

    // Initialize the databases with default ScriptableObject data
    public void InitializeDatabases()
    {
        if (potionDatabase.potions == null || potionDatabase.potions.Count == 0)
        {
            Debug.LogWarning("Potion database is empty. Initializing with default data.");
            potionDatabase.potions = new List<Potion>(Resources.LoadAll<Potion>("PotionDefaults"));
        }

        if (ingredientDatabase.ingredients == null || ingredientDatabase.ingredients.Count == 0)
        {
            Debug.LogWarning("Ingredient database is empty. Initializing with default data.");
            ingredientDatabase.ingredients = new List<Ingredient>(Resources.LoadAll<Ingredient>("IngredientDefaults"));
        }

        // Clone the database to prevent modification of the original ScriptableObject data
        ingredientDatabase = CloneIngredientDatabase(mainIngredientDatabase);
        potionDatabase = ClonePotionDatabase(mainPotionDatabase);

        // Save the default data to JSON files
        SaveDatabases();
    }

    // Clone the IngredientDatabase, including deep cloning the Ingredients
    public IngredientDatabase CloneIngredientDatabase(IngredientDatabase original)
    {
        IngredientDatabase clone = Instantiate(original);
        clone.ingredients = new List<Ingredient>();

        foreach (Ingredient ingredient in original.ingredients)
        {
            // Deep clone each Ingredient ScriptableObject
            Ingredient clonedIngredient = Instantiate(ingredient);
            clone.ingredients.Add(clonedIngredient);
        }

        return clone;
    }

    // Clone the PotionDatabase
    public PotionDatabase ClonePotionDatabase(PotionDatabase original)
    {
        PotionDatabase clone = Instantiate(original);
        clone.potions = new List<Potion>(original.potions);
        return clone;
    }

    // Load the databases from JSON
    public void LoadDatabases()
    {
        if (File.Exists(potionDatabasePath))
        {
            string potionJson = File.ReadAllText(potionDatabasePath);
            PotionDatabaseWrapper loadedPotionDatabase = JsonUtility.FromJson<PotionDatabaseWrapper>(potionJson);
            potionDatabase.potions = loadedPotionDatabase.potions;
            Debug.Log("Potion database loaded.");
        }
        else
        {
            Debug.LogWarning("Potion database JSON file not found.");
        }

        if (File.Exists(ingredientDatabasePath))
        {
            string ingredientJson = File.ReadAllText(ingredientDatabasePath);
            IngredientDatabaseWrapper loadedIngredientDatabase = JsonUtility.FromJson<IngredientDatabaseWrapper>(ingredientJson);
            ingredientDatabase.ingredients = loadedIngredientDatabase.ingredients;
            Debug.Log("Ingredient database loaded.");
        }
        else
        {
            Debug.LogWarning("Ingredient database JSON file not found.");
        }
    }

    // Save the databases to JSON (only the copied data, not ScriptableObject)
    public void SaveDatabases()
    {
        // Clone the data from ScriptableObject to local variables
        PotionDatabaseWrapper potionWrapper = new PotionDatabaseWrapper
        {
            potions = new List<Potion>(potionDatabase.potions) // Clone the list
        };

        IngredientDatabaseWrapper ingredientWrapper = new IngredientDatabaseWrapper
        {
            ingredients = new List<Ingredient>(ingredientDatabase.ingredients) // Clone the list
        };

        string potionJson = JsonUtility.ToJson(potionWrapper, true);
        File.WriteAllText(potionDatabasePath, potionJson);
        Debug.Log("Potion database saved.");

        string ingredientJson = JsonUtility.ToJson(ingredientWrapper, true);
        File.WriteAllText(ingredientDatabasePath, ingredientJson);
        Debug.Log("Ingredient database saved.");
    }

    // Update the potion database during runtime and save (only JSON, not ScriptableObject)
    public void UpdatePotionDatabase(List<Potion> updatedPotions)
    {
        PotionDatabaseWrapper potionWrapper = new PotionDatabaseWrapper
        {
            potions = updatedPotions
        };

        string potionJson = JsonUtility.ToJson(potionWrapper, true);
        File.WriteAllText(potionDatabasePath, potionJson);
        Debug.Log("Potion database updated and saved.");
    }

    // Update the ingredient database during runtime and save (only JSON, not ScriptableObject)
    public void UpdateIngredientDatabase(List<Ingredient> updatedIngredients)
    {
        IngredientDatabaseWrapper ingredientWrapper = new IngredientDatabaseWrapper
        {
            ingredients = updatedIngredients
        };

        string ingredientJson = JsonUtility.ToJson(ingredientWrapper, true);
        File.WriteAllText(ingredientDatabasePath, ingredientJson);
        Debug.Log("Ingredient database updated and saved.");
    }

    // Debug log to inspect the content of the databases
    public void DebugLogDatabase()
    {
        Debug.Log("==== Potion Database ====");
        foreach (Potion potion in potionDatabase.potions)
        {
            string ingredientsList = string.Join(", ", potion.ingredients.ConvertAll(i => i.ingredientName));
            Debug.Log($"Potion: {potion.potionName}, State: {potion.currentPotionState}, Ingredients: [{ingredientsList}]");
        }

        Debug.Log("==== Ingredient Database ====");
        foreach (Ingredient ingredient in ingredientDatabase.ingredients)
        {
            Debug.Log($"Ingredient: {ingredient.ingredientName}, Gathered State: {ingredient.currentGatheredState}, Processed State: {ingredient.currentProcessedState}, Found State: {ingredient.FoundState}");
        }
    }

    // Edit an existing potion by its name (only updates the JSON, not ScriptableObject)
    public void EditPotion(string potionName, string newPotionName = null, Potion.PotionState? newState = null, List<Ingredient> newIngredients = null)
    {
        List<Potion> clonedPotions = new List<Potion>(potionDatabase.potions);

        Potion potionToEdit = clonedPotions.Find(p => p.potionName == potionName);

        if (potionToEdit != null)
        {
            if (newPotionName != null)
            {
                potionToEdit.potionName = newPotionName;
            }

            if (newState.HasValue)
            {
                potionToEdit.currentPotionState = newState.Value;
            }

            if (newIngredients != null && newIngredients.Count > 0)
            {
                potionToEdit.ingredients = newIngredients;
            }

            UpdatePotionDatabase(clonedPotions);
            Debug.Log($"Potion '{potionName}' updated successfully!");
        }
        else
        {
            Debug.LogError($"Potion with name '{potionName}' not found.");
        }
    }

    // Edit an existing ingredient by its name (only updates the JSON, not ScriptableObject)
    public void EditIngredient(string ingredientName, string newIngredientName = null, Ingredient.GatheredState? newGatheredState = null, Ingredient.ProcessedState? newProcessedState = null, bool? newFoundState = null, List<string> newEffects = null, List<string> newDescription = null)
    {
        List<Ingredient> clonedIngredients = new List<Ingredient>(ingredientDatabase.ingredients);

        Ingredient ingredientToEdit = clonedIngredients.Find(i => i.ingredientName == ingredientName);

        if (ingredientToEdit != null)
        {
            if (newIngredientName != null)
            {
                ingredientToEdit.ingredientName = newIngredientName;
            }

            if (newGatheredState.HasValue)
            {
                ingredientToEdit.currentGatheredState = newGatheredState.Value;
            }

            if (newProcessedState.HasValue)
            {
                ingredientToEdit.currentProcessedState = newProcessedState.Value;
            }

            if (newFoundState.HasValue)
            {
                ingredientToEdit.FoundState = newFoundState.Value;
            }

            if (newEffects != null && newEffects.Count > 0)
            {
                ingredientToEdit.effects = newEffects;
            }

            if (newDescription != null && newDescription.Count > 0)
            {
                ingredientToEdit.description = newDescription;
            }

            UpdateIngredientDatabase(clonedIngredients);
            Debug.Log($"Ingredient '{ingredientName}' updated successfully!");
        }
        else
        {
            Debug.LogError($"Ingredient with name '{ingredientName}' not found.");
        }
    }
}
