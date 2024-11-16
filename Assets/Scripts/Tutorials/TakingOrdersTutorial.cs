using UnityEngine;

public class TakingOrdersTutorial : MonoBehaviour
{
    public CustomerDatabase customerDatabase; // Reference to the CustomerDatabase
    public CustomerSpawner customerSpawner;   // Reference to the CustomerSpawner

    void Start()
    {
        if (customerDatabase != null)
        {
            // Get the customer named "Cedric" from the database
            Customer cedric = customerDatabase.GetCustomerByName("Cedric");

            if (cedric != null)
            {
                if (customerSpawner != null)
                {
                    // Pass the customer to the spawner for instantiation
                    customerSpawner.SpawnCustomer(cedric);
                }
                else
                {
                    Debug.LogError("CustomerSpawner not assigned in the Inspector.");
                }
            }
            else
            {
                Debug.LogError("Customer Cedric not found in the database.");
            }
        }
        else
        {
            Debug.LogError("CustomerDatabase not assigned in the Inspector.");
        }
    }

}
