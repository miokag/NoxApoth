using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Customer Database", menuName = "Shop/Customer Database")]
public class CustomerDatabase : ScriptableObject
{
    public List<Customer> customers = new List<Customer>();
    public Customer GetCustomerByName(string name)
    {
        return customers.Find(c => c.customerName == name);
    }

    public List<Customer> GetAllCustomers()
    {
        return customers;
    }
}
