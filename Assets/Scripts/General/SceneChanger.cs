using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // Method to load a scene by its name

    public void ToFrontShop()
    {
        SceneManager.LoadScene("Shop");
    }
    public void ToBackShop()
    {
        SceneManager.LoadScene("BackShop");
    }

    public void ToExploration()
    {
        SceneManager.LoadScene("Exploration");
    }

}
