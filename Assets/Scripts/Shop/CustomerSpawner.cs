using UnityEngine;
using System;

public class CustomerSpawner : MonoBehaviour
{
    public Transform spawnLocation; // The location where the customer will be spawned

    // Define an event to notify when a customer has been spawned
    public event Action<GameObject> OnCustomerSpawned;

    // Modify the SpawnCustomer method to take in the Customer ScriptableObject
    public void SpawnCustomer(Customer customer)
    {
        if (customer != null && customer.customerPrefab != null)
        {
            // Ensure spawnLocation is assigned, otherwise fallback to the spawner's position
            Vector3 spawnPosition = spawnLocation != null ? spawnLocation.position : transform.position;

            // Instantiate the customer prefab at the specified spawn position
            GameObject spawnedCustomer = Instantiate(customer.customerPrefab, spawnPosition, transform.rotation);

            // Set the name of the instantiated customer based on the customer name
            spawnedCustomer.name = customer.customerName;

            // Invoke the OnCustomerSpawned event
            OnCustomerSpawned?.Invoke(spawnedCustomer);
        }
        else
        {
            Debug.LogError("Customer prefab is not assigned for " + customer?.customerName);
        }
    }
}