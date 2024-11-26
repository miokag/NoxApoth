using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Kitchen : MonoBehaviour
{
    private CameraZoom _cameraZoom;
    // Start is called before the first frame update
    
    // Highlightable Objects
    private GameObject potTrigger;
    private GameObject potObject;
    
    private GameObject panTrigger;
    private GameObject panObject;
    
    private GameObject mortarObject;

    private Canvas canvas;
    private Button backMainKitchenButton;
    private FrontMortar frontMortar;
    private CookingPot pot;
    private CookingPan pan;
    public void ChangeOtherObjectTag(GameObject targetObject, string newTag)
    {
        Debug.Log("Changing tag to " + newTag);
        targetObject.tag = newTag;
    }
    void Start()
    {
        panObject = GameObject.Find("Pan");
        potObject = GameObject.Find("Pot");
        mortarObject = GameObject.Find("Mortar");
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        potTrigger = GameObject.Find("PotTrigger");
        panTrigger = GameObject.Find("PanTrigger");
        
        frontMortar = GameObject.Find("Mortar").GetComponent<FrontMortar>();
        pan = FindObjectOfType<CookingPan>();
        _cameraZoom = Camera.main.GetComponent<CameraZoom>();
        pot = FindObjectOfType<CookingPot>();
        OnBackMainKitchenButtonClicked();
    }


    // Update is called once per frame
    void Update()
    {
        if (_cameraZoom != null && _cameraZoom.isZoomedIn)
        {

            if (_cameraZoom.targetObject.name == "Pot")
            {
                ChangeOtherObjectTag(potObject, "Untagged");
                
                ChangeOtherObjectTag(panTrigger, "Untagged");
                ChangeOtherObjectTag(panObject, "Untagged");

                ChangeOtherObjectTag(mortarObject, "Untagged");
                
                if(pot.showingInventory == true) ChangeOtherObjectTag(potTrigger, "Untagged");
                else if(pot.isShowingVisuals == true) ChangeOtherObjectTag(potTrigger, "Untagged");
                else ChangeOtherObjectTag(potTrigger, "Utensil");
            }
            else if (_cameraZoom.targetObject.name == "Pan")
            {
                ChangeOtherObjectTag(potObject, "Untagged");
                ChangeOtherObjectTag(potTrigger, "Untagged");
                
                ChangeOtherObjectTag(panObject, "Untagged");

                ChangeOtherObjectTag(mortarObject, "Untagged");
                
                if(pan.showingInventory == true) ChangeOtherObjectTag(panTrigger, "Untagged");
                else if(pan.isShowingVisuals == true) ChangeOtherObjectTag(panTrigger, "Untagged");
                else ChangeOtherObjectTag(panTrigger, "Utensil");
            }
            else if (_cameraZoom.targetObject.name == "Mortar")
            {
                ChangeOtherObjectTag(potObject, "Untagged");
                ChangeOtherObjectTag(potTrigger, "Untagged");

                ChangeOtherObjectTag(panTrigger, "Untagged");
                ChangeOtherObjectTag(panObject, "Untagged");

                if(frontMortar.showingInventory == true) ChangeOtherObjectTag(mortarObject, "Untagged");
                else if(frontMortar.isShowingVisuals == true) ChangeOtherObjectTag(mortarObject, "Untagged");
                else ChangeOtherObjectTag(mortarObject, "Utensil");
                    
            }



            Button backMainKitchenButton = _cameraZoom.BackMainKitchenButton.GetComponent<Button>();
            if (backMainKitchenButton != null) 
            {
                backMainKitchenButton.onClick.AddListener(OnBackMainKitchenButtonClicked);
            }
        }
    }

    
    private void OnBackMainKitchenButtonClicked()
    {
        ChangeOtherObjectTag(potObject, "Utensil");
        ChangeOtherObjectTag(potTrigger, "Untagged");
                
        ChangeOtherObjectTag(panTrigger, "Untagged");
        ChangeOtherObjectTag(panObject, "Utensil");
                
        ChangeOtherObjectTag(mortarObject, "Utensil");
    }
}
