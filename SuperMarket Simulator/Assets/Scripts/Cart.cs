using System.Collections.Generic;
using UnityEngine;

public class Cart : MonoBehaviour
{
    public List<ProductPrice> items = new List<ProductPrice>();
    public float totalCost = 0f;

    public void AddItem(ProductPrice item)
    {
        items.Add(item);
        totalCost += item.price;
        Debug.Log(item.productName + " added to cart. Total cost: $" + totalCost);
    }

    public void ClearCart()
    {
        items.Clear();
        totalCost = 0f;
        Debug.Log("Cart cleared.");
    }
}
