using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenBehavior : MonoBehaviour
{
    private bool isClickable = true;
    private Collider kitchenCollider;

    private void Start()
    {
        kitchenCollider = GetComponent<Collider>();
    }
    

    private void OnMouseDown()
    {
        if (!isClickable)
            return;
        else
            SceneManager.LoadScene("Kitchen");
    }
    
    public void DisableInteraction()
    {
        isClickable = false;  // Set the flag to false
        if (kitchenCollider != null)
        {
            kitchenCollider.enabled = false;  // Disable the door's collider so it can't be clicked again
        }
    }

    public void EnableInteraction()
    {
        isClickable = true;  // Set the flag to true
        if (kitchenCollider != null)
        {
            kitchenCollider.enabled = true;  // Enables the door's collider so it can be clicked again
        }
    }
}
