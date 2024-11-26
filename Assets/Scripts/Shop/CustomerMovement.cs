using System.Collections;
using UnityEngine;
using System;

public class CustomerMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    private bool isMoving = true;

    public event Action OnCustomerClicked;
    private GameObject currentObject;

    public void ChangeOtherObjectTag(GameObject targetObject, string newTag)
    {
        targetObject.tag = newTag;
    }

    
    public void StartMove()
    {
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
        if(!isMoving)
        {
                ChangeOtherObjectTag(currentObject, "Untagged");
                Debug.Log("Cedric is Clicked");
                OnCustomerClicked?.Invoke();           
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        isMoving = false;
    }
}
