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

    [SerializeField] float walkSpeedThreshold = 0.5f;

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
            float moveSpeed = inputMagnitude < walkSpeedThreshold ? walkSpeed : runSpeed;

            DOTween.To(() => currentMoveAmount, x => currentMoveAmount = x, inputMagnitude, 0.1f).SetEase(Ease.OutQuad);
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
    }
}
