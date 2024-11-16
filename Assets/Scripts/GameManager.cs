using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public OrderManager orderManager;
    private int _tutorialStep = 0;
    public string currentCustomer;

    public IngredientDatabase ingredientDatabase;
    public PotionDatabase potionDatabase;

    private IngredientDatabase clonedIngredientDatabase;
    private PotionDatabase clonedPotionDatabase;

    public List<Ingredient> Inventory { get; private set; }
    private int inventoryLimit = 3;  // Initial inventory size limit

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (orderManager == null)
            {
                orderManager = FindObjectOfType<OrderManager>();
                if (orderManager == null)
                {
                    Debug.LogError("OrderManager not found in the scene.");
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        orderManager.ClearOrders();
        Inventory = new List<Ingredient>();
        currentCustomer = null;

        Debug.Log(ingredientDatabase);

        clonedIngredientDatabase = ingredientDatabase.Clone();
        clonedPotionDatabase = potionDatabase.Clone(clonedIngredientDatabase);

        Debug.Log("Cloned IngredientDatabase and PotionDatabase.");

        AddToInventory(clonedIngredientDatabase.GetIngredientByName("Opium Poppy Tree"));
    }

    public void setCurrentCustomer(string customerName)
    {
        currentCustomer = customerName;
        Debug.Log($"Current customer set to: {currentCustomer}");
    }
    public int GetTutorialStep()
    {
        return _tutorialStep;
    }

    public void NextTutorialStep()
    {
        _tutorialStep++;
        Debug.Log("Moved to tutorial step: " + _tutorialStep);
    }

    public int ClearTutorialStep()
    {
        return _tutorialStep = 0;
    }

    public IngredientDatabase GetClonedIngredientDatabase()
    {
        return clonedIngredientDatabase;
    }

    public PotionDatabase GetClonedPotionDatabase()
    {
        return clonedPotionDatabase;
    }

    public void ChangeIngredientFoundState(string ingredientName, bool foundState)
    {
        Ingredient ingredient = clonedIngredientDatabase.GetIngredientByName(ingredientName);

        Debug.Log(ingredient.GetInstanceID());

        if (ingredient != null)
        {
            Debug.Log($"Before change, {ingredientName} FoundState: {ingredient.FoundState}");
            ingredient.AdjustFoundState(foundState);  // Modify the FoundState of the cloned ingredient
            Debug.Log($"After change, {ingredientName} FoundState: {ingredient.FoundState}");
        }
        else
        {
            Debug.LogWarning($"Ingredient '{ingredientName}' not found in cloned database.");
        }
    }

    public void LogIngredientDatabaseContents()
    {
        if (clonedIngredientDatabase == null || clonedIngredientDatabase.ingredients == null)
        {
            Debug.LogError("Ingredient database is null or empty.");
            return;
        }

        Debug.Log("Logging ingredient database contents:");

        foreach (Ingredient ingredient in clonedIngredientDatabase.ingredients)
        {
            string ingredientInfo = $"Name: {ingredient.ingredientName}, " +
                                    $"Description: {string.Join(", ", ingredient.description)}, " +
                                    $"Effects: {string.Join(", ", ingredient.effects)}, " +
                                    $"FoundState: {ingredient.FoundState}";
            Debug.Log(ingredientInfo);
        }
    }

    public bool GetInventory()
    {
        if (Inventory == null)
        {
            Debug.LogError("Inventory is null.");
            return false;
        }
        else if (Inventory.Count == 0)
        {
            Debug.Log("Inventory is empty.");
            return false;
        }
        else
        {
            Debug.Log("Inventory has items. Total items: " + Inventory.Count);
            return true;
        }
    }

    public bool FullInventory()
    {
        if (Inventory.Count == inventoryLimit)
        {
            Debug.Log("Inventory is full.");
            return true;
        }
        else { return false; }

    }

    public void AddToInventory(Ingredient ingredient)
    {
        if (Inventory.Count < inventoryLimit)  // Check if inventory is within the limit
        {
            Ingredient clonedIngredient = (Ingredient)ingredient.Clone();
            Inventory.Add(clonedIngredient);
            Debug.Log($"{ingredient.ingredientName} added to inventory. Total items: {Inventory.Count}");
        }
        else
        {
            Debug.LogWarning("Inventory limit reached. Cannot add more items.");
        }
    }


    public void RemoveToInventory(Ingredient ingredient)
    {
        Ingredient clonedIngredient = (Ingredient)ingredient.Clone();
        Inventory.Remove(clonedIngredient);
        Debug.Log($"{ingredient.ingredientName} removed from inventory.");
    }

    public void IncreaseInventoryLimit()
    {
        inventoryLimit++;  // Increase inventory limit by 1
        Debug.Log($"Inventory limit increased to {inventoryLimit}.");
    }

    public void DebugInventory()
    {
        string inventoryInfo = "INVENTORY:\n";
        foreach (Ingredient ingredient in Inventory)
        {
            string ingredientInfo = $"Name: {ingredient.ingredientName}, " +
                                    $"Description: {string.Join(", ", ingredient.description)}, " +
                                    $"Effects: {string.Join(", ", ingredient.effects)}, " +
                                    $"FoundState: {ingredient.FoundState}";
            inventoryInfo += ingredientInfo + "\n";
        }

        Debug.Log(inventoryInfo);
    }

}
