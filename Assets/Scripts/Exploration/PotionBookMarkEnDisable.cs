using UnityEngine;
using UnityEngine.UI;

public class PotionBookMarkEnDisable : MonoBehaviour
{
    private Button potionButton;

    void Awake()
    {
        potionButton = GetComponent<Button>();
    }

    public void Enable()
    {
        if (potionButton != null)
        {
            potionButton.interactable = true;  // Enable the button
        }
    }

    public void Disable()
    {
        if (potionButton != null)
        {
            potionButton.interactable = false;  // Disable the button
        }
    }
}
