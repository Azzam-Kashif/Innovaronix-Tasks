using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public float forwardSpeed = 5f;
    public float dragSpeed = 0.1f;

    private float screenWidth;
    private float initialposition;
    private float targetposition;
    private float rotationSpeed = 300f;

    void Start()
    {
        screenWidth = Screen.width;
        targetposition = transform.position.x;
    }

    void Update()
    {
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime, Space.World);

        RotateBall();


        if (Input.GetMouseButtonDown(0))
        {
            initialposition = Input.mousePosition.x;
        }
        else if (Input.GetMouseButton(0))
        {
            float dragDistance = (Input.mousePosition.x - initialposition) / screenWidth;
            Vector3 newPosition = transform.position;
            newPosition.x += dragDistance * dragSpeed * screenWidth;

            transform.position = newPosition;

            initialposition = Input.mousePosition.x;
        }
    }
    void RotateBall()
    {
        float rotationAngle = forwardSpeed * rotationSpeed * Time.deltaTime;

        transform.Rotate(Vector3.right, rotationAngle);
    }
}
