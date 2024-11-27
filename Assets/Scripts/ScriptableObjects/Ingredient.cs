using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Ingredient", menuName = "Potion/Ingredient")]
public class Ingredient : ScriptableObject
{
    public string ingredientName;
    public Sprite icon;
    public GameObject prefab;
    public GameObject iconGameObject;
    public List<string> description;
    public List<string> effects;
    public bool FoundState;  // Indicates if the ingredient has been found
    public bool isinPotion;
    public bool isCookedProperly;
    public float bottomOffset;

    public string hexColor;
    public enum GatheredState { None, Bad, Good, Perfect }
    public enum ProcessedState { None, Burned, Cooked, Paste, Powdered, Simmered, Boiled }

    public ProcessedState neededProcessedState;

    public GatheredState currentGatheredState;
    public ProcessedState currentProcessedState;

    // Clone function to create a duplicate of the ingredient
    public Ingredient Clone()
    {
        Ingredient clone = Instantiate(this);  // Clone the ingredient itself

        // Deep clone mutable fields
        clone.description = new List<string>(this.description);
        clone.effects = new List<string>(this.effects);

        // Create a new reference for FoundState and other enum states
        clone.FoundState = this.FoundState;
        clone.currentGatheredState = this.currentGatheredState;
        clone.currentProcessedState = this.currentProcessedState;

        return clone;
    }

    // Method to adjust the FoundState of the ingredient
    public void AdjustFoundState(bool found)
    {
        FoundState = found;
        Debug.Log($"Ingredient '{ingredientName}' FoundState adjusted to: {FoundState}");
    }
    
    public void AdjustInPotionState(bool inPotion)
    {
        inPotion = inPotion;
        Debug.Log($"Ingredient '{ingredientName}' InPotionState adjusted to: {inPotion}");
    }

    void Reset()
    {
        currentGatheredState = GatheredState.None;  // Reset to 'None'
        currentProcessedState = ProcessedState.None; // Reset to 'None'
        Debug.Log($"Ingredient '{ingredientName}' has been reset.");
    }
}