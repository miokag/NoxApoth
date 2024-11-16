using UnityEngine;

public class InteractableInfo : MonoBehaviour
{
    public string ingredientName;
    public string ingredientDescription;

    // Method to get the ingredient information (can be used by the InteractionHandler)
    public string GetIngredientInfo()
    {
        return "Name: " + ingredientName + "\nDescription: " + ingredientDescription;
    }
}
