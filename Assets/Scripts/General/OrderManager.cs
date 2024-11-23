using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }
    
    private List<Order> orders = new List<Order>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StoreOrder(string characterName, string orderDescription, string customerOrder)
    {
        Order newOrder = new Order(characterName, orderDescription, customerOrder);
        orders.Add(newOrder);
        Debug.Log($"Order stored for customer: {characterName}");
    }

    // Method to retrieve all orders
    public List<Order> GetAllOrders()
    {
        return new List<Order>(orders);
    }

    // Method to clear all orders
    public void ClearOrders()
    {
        orders.Clear();
        Debug.Log("All orders cleared.");
    }

    // Method to remove a specific order by customer name
    public bool RemoveOrderByCustomer(string characterName)
    {
        Order orderToRemove = orders.Find(order => order.CharacterName == characterName);

        if (orderToRemove != null)
        {
            orders.Remove(orderToRemove);
            Debug.Log($"Order removed for customer: {characterName}");
            return true;
        }
        else
        {
            Debug.LogWarning($"Order for customer '{characterName}' not found.");
            return false;
        }
    }
}


[System.Serializable]
public class Order
{
    public string CharacterName;
    public string OrderDescription;
    public string CustomerOrder;

    public Order(string characterName, string orderDescription, string customerOrder)
    {
        CharacterName = characterName;
        OrderDescription = orderDescription;
        CustomerOrder = customerOrder;
    }
}
