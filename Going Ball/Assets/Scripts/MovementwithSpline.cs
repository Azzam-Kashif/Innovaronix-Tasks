using UnityEngine;
using UnityEngine.Splines;

public class MovementWithSpline : MonoBehaviour
{
    public SplineContainer spline;
    public float forwardForce = 10f;
    public float sidewaysForceMultiplier = 10f;
    public float maxLateralOffset = 1f;
    public float dragSensitivity = 1f;
    public float turnSpeed = 5f;
    public float rotationSpeed = 200f;

    private Rigidbody rb;
    private float distancePercentage = 0f;
    private float splineLength;
    private float initialPositionX;
    private float lateralOffset = 0f;
    private float fallThreshold = -1;
    private bool isDragging = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        splineLength = spline.CalculateLength();
    }

    private void FixedUpdate()
    {
        MoveAlongSpline();
    }

    void Update()
    {
        HandleMouseDrag();

        if (transform.position.y < fallThreshold)
        {
            EventManager.Instance.LevelFail();
        }
    }

    private void MoveAlongSpline()
    {
        distancePercentage += forwardForce * Time.fixedDeltaTime / splineLength;

        if (distancePercentage > 1f)
        {
            distancePercentage = 0f;
        }

        Vector3 currentPosition = spline.EvaluatePosition(distancePercentage);
        Vector3 nextPosition = spline.EvaluatePosition(distancePercentage + 0.01f);
        Vector3 forwardDirection = (nextPosition - currentPosition).normalized;

        Vector3 lateralDirection = Vector3.Cross(forwardDirection, Vector3.up).normalized;
        Vector3 targetPosition = currentPosition + forwardDirection * forwardForce * Time.fixedDeltaTime;
        targetPosition += lateralDirection * lateralOffset;

        rb.velocity = (targetPosition - rb.position) / Time.fixedDeltaTime;
        Vector3 rollingAxis = Vector3.Cross(forwardDirection, Vector3.up).normalized;
        rb.AddTorque(rollingAxis * forwardForce * rotationSpeed * Time.fixedDeltaTime);

        Quaternion targetRotation = Quaternion.LookRotation(forwardDirection, Vector3.up);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime));
    }

    void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            initialPositionX = Input.mousePosition.x;
            isDragging = true;
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            float dragDistance = (Input.mousePosition.x - initialPositionX) / Screen.width;

            lateralOffset -= dragDistance * sidewaysForceMultiplier * dragSensitivity;
            lateralOffset = Mathf.Clamp(lateralOffset, -maxLateralOffset, maxLateralOffset);

            initialPositionX = Input.mousePosition.x;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    public void Respawn()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = spline.EvaluatePosition(0f);
        transform.rotation = Quaternion.identity;
        distancePercentage = 0f;
        lateralOffset = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EndPoint"))
        {
            EventManager.Instance.LevelPass();
        }
    }
}
