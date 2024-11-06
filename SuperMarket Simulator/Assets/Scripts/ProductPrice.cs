using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductPrice : MonoBehaviour
{
    public string productName;
    public float price;

    public ProductPrice(string itemName, float price)
    {
        this.productName = itemName;
        this.price = price;
    }
}
