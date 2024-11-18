using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenBehavior : MonoBehaviour
{
    private void OnMouseDown()
    {
        SceneManager.LoadScene("Kitchen");
    }
}
