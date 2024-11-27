using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewPotionDialogueList", menuName = "ScriptableObjects/PotionDialogueList")]
public class PotionDialogueList : ScriptableObject
{
    [System.Serializable]
    public class PotionDialogue
    {
        public Potion Potion; // Reference to the Ingredient ScriptableObject
        [TextArea] public List<string> dialogues = new List<string>(); // Multiple dialogues
    }

    public List<PotionDialogue> potionDialogues = new List<PotionDialogue>();
}