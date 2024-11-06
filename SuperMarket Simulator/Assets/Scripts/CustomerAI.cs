using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class CustomerAI : MonoBehaviour
{
    // AI States
    public enum AIState
    {
        Idle,
        SearchingShelf,
        NavigatingToShelf,
        PickingItem,
        NavigatingToCounter,
        PlacingItem,
        CheckingOut
    }

    private AIState currentState = AIState.Idle;
    private Seeker seeker;
    private AIPath aiPath;
    private Animator animator;

    public float stopDistance = 0.5f;
    public int itemsToPick = 3;
    private int itemsPicked = 0;

    private Shelf[] shelves;
    private Transform currentTarget;
    private bool hasItem = false;

    public Transform counter;
    public Transform itemHoldPosition;
    private Shelf currentShelf;

    private List<PickableItem> pickedItems = new List<PickableItem>();
    private List<PickableItem> itemsOnCounter = new List<PickableItem>(); // New list for items on the counter


    public Cart cart;
    public Checkout checkoutManager;


    void Start()
    {
        seeker = GetComponent<Seeker>();
        aiPath = GetComponent<AIPath>();
        animator = GetComponent<Animator>();

        shelves = FindObjectsOfType<Shelf>();

        ChangeState(AIState.SearchingShelf);

        cart = FindObjectOfType<Cart>();

    }

    void Update()
    {
        HandleState();
    }


    void HandleState()
    {
        switch (currentState)
        {
            case AIState.Idle:
                animator.SetFloat("Speed", 0);
                break;

            case AIState.SearchingShelf:
                SearchForShelf();
                break;

            case AIState.NavigatingToShelf:
            case AIState.NavigatingToCounter:
                NavigateToTarget();
                break;

            case AIState.PickingItem:
                PickItem();
                break;

            case AIState.PlacingItem:
                PlaceItem();
                break;

            case AIState.CheckingOut:
                PerformCheckOut();
                break;

        }
    }


    void ChangeState(AIState newState)
    {
        currentState = newState;
        aiPath.isStopped = false;
        Debug.Log("AI state changed to: " + newState);
    }

    void SearchForShelf()
    {
        if (shelves.Length == 0)
        {
            Debug.LogWarning("No shelves found.");
            ChangeState(AIState.Idle);
            return;
        }

        bool foundShelf = false;


        for (int i = 0; i < shelves.Length; i++)
        {
            Shelf shelf = shelves[Random.Range(0, shelves.Length)];
            if (shelf.HasAvailableSlot(out Transform slot))
            {
                currentShelf = shelf;
                currentTarget = slot;
                foundShelf = true;
                break;
            }
        }

        if (foundShelf)
        {
            ChangeState(AIState.NavigatingToShelf);
        }
        else
        {
            Debug.LogWarning("No available items found.");
            ChangeState(AIState.Idle);
        }
    }

    void NavigateToTarget()
    {
        if (currentTarget != null)
        {
            aiPath.destination = currentTarget.position;

            float speed = aiPath.velocity.magnitude;
            animator.SetFloat("Speed", speed);

            if (!aiPath.pathPending && aiPath.remainingDistance > stopDistance)
            {
                aiPath.isStopped = false;
            }

            else if (!aiPath.pathPending && aiPath.remainingDistance <= stopDistance)
            {
                aiPath.isStopped = true;
                animator.SetFloat("Speed", 0);

                if (currentState == AIState.NavigatingToShelf)
                {
                    ChangeState(AIState.PickingItem);
                }
                else if (currentState == AIState.NavigatingToCounter)
                {
                    if (HasReachedCounter())
                    {
                        ChangeState(AIState.PlacingItem);
                    }
                }
            }
        }
    }

    void PickItem()
    {
        if (hasItem)
        {
            return;
        }

        Debug.Log("Picking item from shelf.");

        PickableItem item = currentShelf.GetItemFromSlot(currentTarget);
        if (item != null)
        {
            item.gameObject.SetActive(false);
            currentShelf.OccupySlot(currentTarget);
            pickedItems.Add(item);
            hasItem = false;
            ProductPrice productPrice = item.GetComponent<ProductPrice>(); // Assuming PickableItem has a ProductPrice component
            if (productPrice != null)
            {
                cart.AddItem(productPrice);  // Add the product to the cart
            }

            itemsPicked++;
            StartCoroutine(WaitBeforePickingNextItem(2f));


            if (itemsPicked < itemsToPick)
            {
                ChangeState(AIState.SearchingShelf);
            }
            else
            {

                currentTarget = counter;
                ChangeState(AIState.NavigatingToCounter);
            }
        }
        else
        {
            Debug.LogWarning("No item found in the selected slot.");
            ChangeState(AIState.SearchingShelf);
        }
    }


    void PlaceItem()
    {
        if (pickedItems.Count == 0)
        {
            Debug.LogWarning("No items to place.");
            return;
        }

        Debug.Log("Placing items on counter.");


        foreach (PickableItem item in pickedItems)
        {
            item.transform.SetParent(null);
            item.transform.position = counter.position + new Vector3(0, 0.5f, -0.7f);
            item.gameObject.SetActive(true);

            itemsOnCounter.Add(item);
        }

        pickedItems.Clear();

        itemsPicked = 0;
        ChangeState(AIState.CheckingOut);
    }
    void PerformCheckOut()
    {
        Debug.Log("Performing checkout.");

        // Place items at the checkout counter
        checkoutManager.PlaceItemsForCheckout();

        // AI randomly decides on the payment method
        string paymentMethod = Random.Range(0, 2) == 0 ? "Cash" : "Card";
        Debug.Log("AI chose payment method: " + paymentMethod);

        if (paymentMethod == "Cash")
        {
            // AI gives random cash: $10, $20, or $30
            int randomCash = Random.Range(1, 4) * 10;
            Debug.Log("AI gives $" + randomCash + " in cash.");

            // Get the total price of items in the cart
            float totalPrice = cart.totalCost;

            // Handle the cash payment (calculate change or additional payment)
            checkoutManager.HandleCashPayment(randomCash, totalPrice);
        }
        else
        {
            // Handle card payment
            checkoutManager.OpenCheckoutMenu("Card");
        }

        RemoveItemsFromCounter();
        // AI becomes idle after completing the checkout
        ChangeState(AIState.Idle);
    }

    void RemoveItemsFromCounter()
    {
        foreach (PickableItem item in itemsOnCounter)
        {
            // You can either deactivate the items
            item.gameObject.SetActive(false);

            // Or destroy the items completely if you no longer need them
            //Destroy(item.gameObject);
        }
        itemsOnCounter.Clear();
    }

    bool HasReachedCounter()
    {
        float distanceToCounter = Vector3.Distance(transform.position, counter.position);
        return distanceToCounter <= stopDistance;
    }
    IEnumerator WaitBeforePickingNextItem(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (itemsPicked < itemsToPick)
        {
            ChangeState(AIState.SearchingShelf);
        }
        else
        {
            currentTarget = counter;
            ChangeState(AIState.NavigatingToCounter);
        }
    }
}