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
        PlacingItem
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


    void Start()
    {
        seeker = GetComponent<Seeker>();
        aiPath = GetComponent<AIPath>();
        animator = GetComponent<Animator>();

        shelves = FindObjectsOfType<Shelf>();

        ChangeState(AIState.SearchingShelf);

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
        }

        pickedItems.Clear(); 

        itemsPicked = 0; 
        ChangeState(AIState.Idle); 
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
