using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    private List<Order> orders = new List<Order>();
    private string saveFilePath;

    private void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "orders.json");
        LoadOrders();
    }

    public void StoreOrder(string characterName, string orderDescription, string customerOrder)
    {
        Order newOrder = new Order(characterName, orderDescription, customerOrder);
        orders.Add(newOrder);
        SaveOrders();
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
        SaveOrders();
    }

    // Method to remove a specific order by customer name
    public bool RemoveOrderByCustomer(string characterName)
    {
        Order orderToRemove = orders.Find(order => order.CharacterName == characterName);

        if (orderToRemove != null)
        {
            orders.Remove(orderToRemove);
            SaveOrders();
            return true;
        }
        else
        {
            Debug.LogWarning($"Order for customer '{characterName}' not found.");
            return false;
        }
    }

    private void SaveOrders()
    {
        string json = JsonUtility.ToJson(new OrderList { Orders = orders }, true);
        File.WriteAllText(saveFilePath, json);
    }

    private void LoadOrders()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            OrderList wrapper = JsonUtility.FromJson<OrderList>(json);
            orders = wrapper.Orders ?? new List<Order>();
        }
    }

    public static List<Order> LoadOrdersFromJson()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "orders.json");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            OrderList wrapper = JsonUtility.FromJson<OrderList>(json);
            return wrapper.Orders ?? new List<Order>();
        }
        return new List<Order>();
    }

    // Wrapper class for JSON serialization
    [System.Serializable]
    public class OrderList
    {
        public List<Order> Orders;
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
