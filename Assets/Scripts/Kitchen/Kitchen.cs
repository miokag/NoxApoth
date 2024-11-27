using UnityEngine;
using UnityEngine.UI;

public class Kitchen : MonoBehaviour
{
    private CameraZoom _cameraZoom;
    private GameObject potTrigger, potObject, panTrigger, panObject, mortarObject;
    private Canvas canvas;
    private Button backMainKitchenButton;
    private FrontMortar frontMortar;
    private CookingPot pot;
    private CookingPan pan;

    void Start()
    {
        // Initialize references
        potObject = GameObject.Find("Pot");
        panObject = GameObject.Find("Pan");
        mortarObject = GameObject.Find("Mortar");
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        potTrigger = GameObject.Find("PotTrigger");
        panTrigger = GameObject.Find("PanTrigger");

        frontMortar = GameObject.Find("Mortar").GetComponent<FrontMortar>();
        pan = FindObjectOfType<CookingPan>();
        _cameraZoom = Camera.main.GetComponent<CameraZoom>();
        pot = FindObjectOfType<CookingPot>();

        backMainKitchenButton = _cameraZoom.BackMainKitchenButton.GetComponent<Button>();
        backMainKitchenButton.onClick.AddListener(OnBackMainKitchenButtonClicked);

        // Set initial object states
        OnBackMainKitchenButtonClicked();
    }

    // This function changes the tag of an object
    public void ChangeOtherObjectTag(GameObject targetObject, string newTag)
    {
        Debug.Log("Changing tag to " + newTag);
        targetObject.tag = newTag;
    }

    // This function is triggered when the zoom state changes
    public void OnZoomStateChanged()
    {
        if (_cameraZoom.isZoomedIn)
        {
            // Change tags based on the zoomed-in object
            if (_cameraZoom.targetObject != null)
            {
                string objectName = _cameraZoom.targetObject.name;

                // Adjust tags based on the target object in zoom view
                if (objectName == "Pot")
                {
                    UpdateObjectTags("Pot");
                }
                else if (objectName == "Pan")
                {
                    UpdateObjectTags("Pan");
                }
                else if (objectName == "Mortar")
                {
                    UpdateObjectTags("Mortar");
                }
            }
        }
        else
        {
            // Reset all tags when zoomed out
            ResetObjectTags();
        }
    }

    // Helper function to update tags based on the selected object
    private void UpdateObjectTags(string objectName)
    {
        // Reset tags
        ResetObjectTags();

        // Update tags based on the selected object
        if (objectName == "Pot")
        {
            ChangeOtherObjectTag(potObject, "Untagged");
            ChangeOtherObjectTag(panTrigger, "Untagged");
            ChangeOtherObjectTag(panObject, "Untagged");
            ChangeOtherObjectTag(mortarObject, "Untagged");
            if (pot.showingInventory || pot.isShowingVisuals)
                ChangeOtherObjectTag(potTrigger, "Untagged");
            else
                ChangeOtherObjectTag(potTrigger, "Utensil");
        }
        else if (objectName == "Pan")
        {
            ChangeOtherObjectTag(potObject, "Untagged");
            ChangeOtherObjectTag(potTrigger, "Untagged");
            ChangeOtherObjectTag(panObject, "Untagged");
            ChangeOtherObjectTag(mortarObject, "Untagged");
            if (pan.showingInventory || pan.isShowingVisuals)
                ChangeOtherObjectTag(panTrigger, "Untagged");
            else
                ChangeOtherObjectTag(panTrigger, "Utensil");
        }
        else if (objectName == "Mortar")
        {
            ChangeOtherObjectTag(potObject, "Untagged");
            ChangeOtherObjectTag(potTrigger, "Untagged");
            ChangeOtherObjectTag(panTrigger, "Untagged");
            ChangeOtherObjectTag(panObject, "Untagged");
            if (frontMortar.showingInventory || frontMortar.isShowingVisuals)
                ChangeOtherObjectTag(mortarObject, "Untagged");
            else
                ChangeOtherObjectTag(mortarObject, "Utensil");
        }
    }

    // Helper function to reset all tags
    private void ResetObjectTags()
    {
        ChangeOtherObjectTag(potObject, "Utensil");
        ChangeOtherObjectTag(panObject, "Utensil");
        ChangeOtherObjectTag(mortarObject, "Utensil");
        ChangeOtherObjectTag(potTrigger, "Untagged");
        ChangeOtherObjectTag(panTrigger, "Untagged");
    }

    // This function is triggered when the back button is clicked
    private void OnBackMainKitchenButtonClicked()
    {
        ResetObjectTags();
    }
}
