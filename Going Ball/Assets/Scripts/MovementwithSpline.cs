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
    public float obstacleDetectionDistance = 5f;  // For detecting obstacles
    public float dodgeStrength = 1f;  // Strength of dodging movements

    protected Rigidbody rb;
    protected float distancePercentage = 0f;
    protected float splineLength;
    protected float lateralOffset = 0f;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        splineLength = spline.CalculateLength();
    }

    protected virtual void FixedUpdate()
    {
        MoveAlongSpline();
    }

    protected virtual void Update()
    {
        // This will be overridden by AI or Player-specific input handling
    }


    // Core method for movement along the spline
    protected void MoveAlongSpline()
    {
        distancePercentage += forwardForce * Time.fixedDeltaTime / splineLength;
        distancePercentage = Mathf.Clamp01(distancePercentage);  // Clamp to prevent overflow

        Vector3 currentPosition = spline.EvaluatePosition(distancePercentage);
        Vector3 nextPosition = spline.EvaluatePosition(distancePercentage + 0.01f);
        Vector3 forwardDirection = (nextPosition - currentPosition).normalized;

        Vector3 lateralDirection = Vector3.Cross(forwardDirection, Vector3.up).normalized;
        Vector3 targetPosition = currentPosition + forwardDirection * forwardForce * Time.fixedDeltaTime;
        targetPosition += lateralDirection * lateralOffset;

        rb.velocity = (targetPosition - rb.position) / Time.fixedDeltaTime;

        Quaternion targetRotation = Quaternion.LookRotation(forwardDirection, Vector3.up);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime));
    }

    // Common method for dodging obstacles
    protected virtual void DetectAndDodgeObstacles()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, obstacleDetectionDistance))
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                // Random dodge direction left/right
                float dodgeDirection = Random.Range(0f, 1f) > 0.5f ? 1f : -1f;
                lateralOffset += dodgeDirection * dodgeStrength * Time.deltaTime;
                lateralOffset = Mathf.Clamp(lateralOffset, -maxLateralOffset, maxLateralOffset);
            }
        }
    }
    public virtual void Respawn()
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
