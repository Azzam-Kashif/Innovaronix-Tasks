using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float walkSpeed = 3f;
    [SerializeField] float runSpeed = 6f;
    [SerializeField] float rotationSpeed = 500f;
    [SerializeField] float pickUpRange = 2f;
    [SerializeField] float shelfRange = 2f;
    [SerializeField] float raySpacing = 0.1f;  // Spacing between rays
    [SerializeField] int numberOfRays = 3;  // Number of rays to cast
    [SerializeField] Joystick joystick;
    [SerializeField] Transform holdPosition;
    [SerializeField] Transform headTransform;
    [SerializeField] LayerMask pickableLayer;
    [SerializeField] LayerMask shelfLayer;
    [SerializeField] float runThreshold = 0.8f;

    private List<Transform> shelfSlots = new List<Transform>();  // List to hold shelf slots dynamically
    private List<bool> slotOccupied;  // Tracks whether each slot is occupied
    private PickableItem pickedUpItem = null;
    private CameraController cameraController;
    private Animator animator;
    private Rigidbody rb;
    private float currentMoveAmount = 0f;

    private void Awake()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        rb.freezeRotation = true;
        animator.applyRootMotion = false;

        // Dynamically find shelf slots by tag
        FindShelfSlots();

        // Initialize slot occupancy
        slotOccupied = new List<bool>(new bool[shelfSlots.Count]);
    }

    private void Update()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        float h = joystick.Horizontal;
        float v = joystick.Vertical;

        Vector2 input = new Vector2(Mathf.Clamp(h, -1f, 1f), Mathf.Clamp(v, -1f, 1f));
        float inputMagnitude = Mathf.Clamp01(input.magnitude);

        if (inputMagnitude > 0.1f)
        {
            float moveSpeed = (inputMagnitude > runThreshold) ? runSpeed : walkSpeed;

            DOTween.To(() => currentMoveAmount, x => currentMoveAmount = x, inputMagnitude, 0.09f).SetEase(Ease.OutQuad);
            animator.SetFloat("moveAmount", currentMoveAmount);

            Vector3 moveInput = new Vector3(h, 0, v).normalized;
            Vector3 moveDir = cameraController.PlanarRotation * moveInput;

            rb.velocity = moveDir * moveSpeed;

            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            rb.velocity = Vector3.zero;
            animator.SetFloat("moveAmount", 0f);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, cameraController.PlanarRotation.eulerAngles.y, 0), rotationSpeed * Time.deltaTime);
        }

        // Pick up or drop items
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (pickedUpItem == null)
            {
                TryPickUpItem();
            }
            else
            {
                if (IsNearShelf())
                {
                    PlaceItemInShelf();
                }
                else
                {
                    DropItem();
                }
            }
        }
    }

    private void FindShelfSlots()
    {
        // Find all shelf slots tagged as "ShelfSlot" in the scene
        GameObject[] slotObjects = GameObject.FindGameObjectsWithTag("ShelfSlot");
        foreach (GameObject slotObj in slotObjects)
        {
            shelfSlots.Add(slotObj.transform);
        }

        Debug.Log("Found " + shelfSlots.Count + " shelf slots.");
    }

    private void TryPickUpItem()
    {
        RaycastHit hit;
        bool itemFound = false;

        Vector3 rayOrigin = headTransform.position;
        Vector3 baseDirection = Camera.main.transform.forward;

        for (int i = -numberOfRays / 2; i <= numberOfRays / 2; i++)
        {
            Vector3 rayDirection = Quaternion.Euler(0, i * raySpacing, 0) * baseDirection;

            if (Physics.Raycast(rayOrigin, rayDirection, out hit, pickUpRange, pickableLayer))
            {
                PickableItem item = hit.collider.GetComponent<PickableItem>();
                if (item != null && !item.isPickedUp)
                {
                    PickUpItem(item);
                    itemFound = true;
                    break;
                }
            }
        }

        if (!itemFound)
        {
            Debug.Log("No items detected.");
        }
    }

    private void PickUpItem(PickableItem item)
    {
        pickedUpItem = item;
        item.isPickedUp = true;

        Physics.IgnoreCollision(item.GetComponent<Collider>(), GetComponent<Collider>(), true);

        item.transform.SetParent(holdPosition);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        Rigidbody itemRb = item.GetComponent<Rigidbody>();
        if (itemRb != null)
        {
            itemRb.isKinematic = true;
            itemRb.useGravity = false;
        }
        animator.SetLayerWeight(animator.GetLayerIndex("HoldingItemLayer"), 1f);
    }

    private void DropItem()
    {
        if (pickedUpItem != null)
        {
            if (IsNearShelf())
            {
                PlaceItemInShelf();
            }
            else
            {
                pickedUpItem.transform.SetParent(null);

                Rigidbody itemRb = pickedUpItem.GetComponent<Rigidbody>();
                if (itemRb != null)
                {
                    itemRb.isKinematic = false;
                    itemRb.useGravity = true;
                }

                Physics.IgnoreCollision(pickedUpItem.GetComponent<Collider>(), GetComponent<Collider>(), false);

                pickedUpItem.isPickedUp = false;
                pickedUpItem = null;
            }

            animator.SetLayerWeight(animator.GetLayerIndex("HoldingItemLayer"), 0f);
        }
    }

    private void PlaceItemInShelf()
    {
        RaycastHit hit;
        Vector3 rayOrigin = headTransform.position;
        Vector3 baseDirection = Camera.main.transform.forward;

        // Cast a ray to detect the shelf the player is pointing at
        if (Physics.Raycast(rayOrigin, baseDirection, out hit, shelfRange, shelfLayer))
        {
            Shelf shelf = hit.collider.GetComponent<Shelf>();
            if (shelf != null && shelf.HasAvailableSlot(out Transform availableSlot))
            {
                // Place the item in the available slot
                pickedUpItem.transform.position = availableSlot.position;
                pickedUpItem.transform.rotation = availableSlot.rotation;

                // Mark the slot as occupied in the shelf
                shelf.OccupySlot(availableSlot);

                // Reset picked up item
                pickedUpItem.transform.SetParent(null);
                pickedUpItem.isPickedUp = false;
                pickedUpItem = null;

                animator.SetLayerWeight(animator.GetLayerIndex("HoldingItemLayer"), 0f);

                Debug.Log("Item placed in shelf.");
            }
            else
            {
                Debug.Log("No available slot in the detected shelf.");
            }
        }
        else
        {
            Debug.Log("No shelf detected within range.");
        }
    }

    private bool IsNearShelf()
    {
        RaycastHit hit;
        Vector3 rayOrigin = headTransform.position;
        Vector3 baseDirection = Camera.main.transform.forward;

        // Cast a ray to detect a shelf within range
        return Physics.Raycast(rayOrigin, baseDirection, out hit, shelfRange, shelfLayer) && hit.collider.GetComponent<Shelf>() != null;
    }
    private void OnDrawGizmos()
    {
        // Ensure this only runs in play mode
        if (!Application.isPlaying) return;

        // Define the origin and direction of the ray
        Vector3 rayOrigin = headTransform.position;
        Vector3 baseDirection = Camera.main.transform.forward;

        // Draw rays for item detection (multiple rays)
        Gizmos.color = Color.green;  // Green for detecting pickable items
        for (int i = -numberOfRays / 2; i <= numberOfRays / 2; i++)
        {
            Vector3 rayDirection = Quaternion.Euler(0, i * raySpacing, 0) * baseDirection;
            Gizmos.DrawRay(rayOrigin, rayDirection * pickUpRange);  // Draw the ray for item detection
        }

        // Draw ray for shelf detection (single ray)
        Gizmos.color = Color.blue;  // Blue for detecting shelves
        Gizmos.DrawRay(rayOrigin, baseDirection * shelfRange);  // Draw the ray for shelf detection
    }
}
