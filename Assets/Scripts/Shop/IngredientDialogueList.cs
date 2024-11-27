using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewIngredientDialogueList", menuName = "ScriptableObjects/IngredientDialogueList")]
public class IngredientDialogueList : ScriptableObject
{
    [System.Serializable]
    public class IngredientDialogue
    {
        public Ingredient ingredient; // Reference to the Ingredient ScriptableObject
        [TextArea] public List<string> dialogues = new List<string>(); // Multiple dialogues
    }

    public List<IngredientDialogue> ingredientDialogues = new List<IngredientDialogue>();
}