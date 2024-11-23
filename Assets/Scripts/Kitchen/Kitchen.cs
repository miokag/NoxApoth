using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Kitchen : MonoBehaviour
{
    private CameraZoom _cameraZoom;
    private InteractionManager interactionManager;
    // Start is called before the first frame update
    
    // Highlightable Objects
    private HighlightableObject potTrigger;
    private HighlightableObject potObject;
    
    private HighlightableObject panTrigger;
    private HighlightableObject panObject;
    
    private HighlightableObject mortarObject;

    private Canvas canvas;
    private Button backMainKitchenButton;
    void Start()
    {
        panObject = GameObject.Find("PanSprite").GetComponent<HighlightableObject>();
        potObject = GameObject.Find("Pot").GetComponent<HighlightableObject>();
        mortarObject = GameObject.Find("Mortar").GetComponent<HighlightableObject>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        potTrigger = GameObject.Find("PotTrigger").GetComponent<HighlightableObject>();
        panTrigger = GameObject.Find("PanTrigger").GetComponent<HighlightableObject>();

        panTrigger.canHighlight = false;
        potTrigger.canHighlight = false;
        
        interactionManager = GameObject.Find("InteractionManager").GetComponent<InteractionManager>();
        
        _cameraZoom = Camera.main.GetComponent<CameraZoom>();
    }


    // Update is called once per frame
    void Update()
    {
        if (_cameraZoom != null && _cameraZoom.isZoomedIn)
        {

            if (_cameraZoom.targetObject.name == "Pot")
            {
                potObject.canHighlight = false;
                potTrigger.canHighlight = true;
                
                panTrigger.canHighlight = false;
                panObject.canHighlight = false;
                
                mortarObject.canHighlight = false;
            }
            else if (_cameraZoom.targetObject.name == "Pan")
            {
                potObject.canHighlight = false;
                potTrigger.canHighlight = false;
                
                panTrigger.canHighlight = true;
                panObject.canHighlight = false;
                
                mortarObject.canHighlight = false;
            }
            else if (_cameraZoom.targetObject.name == "Mortar")
            {
                potObject.canHighlight = false;
                potTrigger.canHighlight = false;
                
                panTrigger.canHighlight = false;
                panObject.canHighlight = false;
                panObject.canHighlight = false;
                
                mortarObject.canHighlight = true;
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
        potObject.canHighlight = true;
        potTrigger.canHighlight = false;
                
        panTrigger.canHighlight = false;
        panObject.canHighlight = true;
                
        mortarObject.canHighlight = true;
    }
}
