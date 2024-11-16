using System.Collections;
using UnityEngine;
using System;

public class CustomerMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    private bool isMoving = true;

    public event Action OnCustomerClicked;


    public void StartMove()
    {
        StartCoroutine(MoveCustomer());
    }

    private IEnumerator MoveCustomer()
    {
        while (isMoving)
        {
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void OnMouseDown()
    {
        if(!isMoving)
        {

                Debug.Log("Cedric is Clicked");
                OnCustomerClicked?.Invoke();           
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        isMoving = false;
    }
}
