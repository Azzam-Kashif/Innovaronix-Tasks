using UnityEngine;

public class BallController : MonoBehaviour
{
    public float forwardForce = 10f;
    public float sidewaysForceMultiplier = 10f;
    public float maxSidewaysSpeed = 5f;
    public float dragSensitivity = 1f;

    private Rigidbody rb;
    private float initialPositionX;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        rb.AddForce(Vector3.forward * forwardForce, ForceMode.Acceleration);

        HandleMouseDrag();
    }

    void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            initialPositionX = Input.mousePosition.x;
        }
        else if (Input.GetMouseButton(0))
        {
            float dragDistance = (Input.mousePosition.x - initialPositionX) / Screen.width;

            float sidewaysForce = dragDistance * sidewaysForceMultiplier * dragSensitivity;

            Vector3 newVelocity = rb.velocity + Vector3.right * sidewaysForce;
            newVelocity.x = Mathf.Clamp(newVelocity.x, -maxSidewaysSpeed, maxSidewaysSpeed);
            rb.velocity = new Vector3(newVelocity.x, rb.velocity.y, rb.velocity.z);

            initialPositionX = Input.mousePosition.x;
        }
    }
}
