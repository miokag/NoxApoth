using System.Collections;
using UnityEngine;
using System;

public class CustomerMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    private bool isMoving = true;
    public bool canBeClicked = true; // Add this flag

    public event Action OnCustomerClicked;
    private GameObject currentObject;

    public void ChangeOtherObjectTag(GameObject targetObject, string newTag)
    {
        targetObject.tag = newTag;
    }

    public void StartMove()
    {
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
            // Wait one frame before starting the movement coroutine
            StartCoroutine(WaitAndStartMove());
        }
        else
        {
            StartCoroutine(MoveCustomer());
        }
    }

    private IEnumerator WaitAndStartMove()
    {
        // Yielding one frame to ensure the GameObject is fully active
        yield return null;  
        StartCoroutine(MoveCustomer());
    }

    private IEnumerator MoveCustomer()
    {
        currentObject = this.gameObject;
        while (isMoving)
        {
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
            yield return null;
            ChangeOtherObjectTag(currentObject, "Untagged");
        }

        ChangeOtherObjectTag(currentObject, "Selectable");
    }

    private void OnMouseDown()
    {
        if (!isMoving && canBeClicked) // Only respond to click if it's allowed
        {
            canBeClicked = false; // Disable further clicks
            ChangeOtherObjectTag(currentObject, "Untagged");
            Debug.Log("Customer is Clicked");
            OnCustomerClicked?.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        isMoving = false;
    }

    // Method to re-enable clickability if needed
    public void EnableClick()
    {
        canBeClicked = true;
    }
}

