using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float walkSpeed = 3f;
    [SerializeField] float runSpeed = 6f;
    [SerializeField] float rotationSpeed = 500f;
    [SerializeField] Joystick joystick;
    [SerializeField] Transform holdPosition;
    [SerializeField] LayerMask pickableLayer;
    [SerializeField] float pickUpRange = 2f;

    [SerializeField] float runThreshold = 0.8f;  // The threshold to switch to running

    private PickableItem pickedUpItem = null;

    Quaternion targetRotation;

    CameraController cameraController;
    Animator animator;
    Rigidbody rb;

    float currentMoveAmount = 0f;

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

            targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            rb.velocity = Vector3.zero;
            animator.SetFloat("moveAmount", 0f);
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
                DropItem();
            }
        }
    }

    private void TryPickUpItem()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, pickUpRange, pickableLayer))
        {
            PickableItem item = hit.collider.GetComponent<PickableItem>();
            if (item != null && !item.isPickedUp)
            {
                PickUpItem(item);
            }
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
            itemRb.isKinematic = true;  // Prevent the object from being affected by physics
            itemRb.useGravity = false;   // Disable gravity while the item is held
        }
        animator.SetLayerWeight(animator.GetLayerIndex("HoldingItemLayer"), 1f);
    }

    private void DropItem()
    {
        if (pickedUpItem != null)
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
