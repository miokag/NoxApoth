using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Customer", menuName = "Shop/Customer")]
public class Customer : ScriptableObject
{
    public string customerName;
    public GameObject customerPrefab;
    public Potion customerOrder;
}
