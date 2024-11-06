using UnityEngine;

public class Checkout : MonoBehaviour
{
    public Transform counter; // The location where the AI places items for checkout
    public Cart cart; // Reference to the cart
    public CheckOutUI checkoutUI; // Reference to the UI manager

    private bool isAtCounter = false;

    void Update()
    {
        // For testing, simulate the AI/player at the counter
        if (isAtCounter && Input.GetKeyDown(KeyCode.Space))
        {
            PlaceItemsForCheckout();
            DecidePaymentMethod();
        }
    }

    public void HandleCashPayment(int cashGiven, float totalPrice)
    {
        Debug.Log("Total price is: $" + totalPrice);

        // Calculate the change required
        float change = cashGiven - totalPrice;

        if (change >= 0)
        {
            Debug.Log("AI needs $" + change + " back in change.");

            // Display change UI and ask the player to return the correct amount
            ShowChangeUI(change);
        }
        else
        {

        }
    }

    public void PlaceItemsForCheckout()
    {
        if (cart.items.Count == 0)
        {
            Debug.LogWarning("No items in cart to checkout.");
            return;
        }

        Vector3 placementPosition = counter.position;
        foreach (ProductPrice item in cart.items)
        {
            item.transform.position = placementPosition;
            item.gameObject.SetActive(true); // Ensure the item is visible
            placementPosition += new Vector3(0, 0, -0.5f); // Offset the next item placement
        }

        Debug.Log("Items placed at the counter for checkout.");
    }

    void ShowChangeUI(float change)
    {
        // Open the UI for returning change
        Debug.Log("Showing UI to return $" + change + " in change.");
        checkoutUI.DisplayChangeUI(change);
    }

    public void DecidePaymentMethod()
    {
        string paymentMethod = Random.Range(0, 2) == 0 ? "Cash" : "Card";
        Debug.Log("AI chose payment method: " + paymentMethod);
        OpenCheckoutMenu(paymentMethod);
    }

    public void OpenCheckoutMenu(string paymentMethod)
    {
        checkoutUI.DisplayCheckout(cart.totalCost, paymentMethod);
    }

    public void SetAtCounter(bool value)
    {
        isAtCounter = value;
    }
}