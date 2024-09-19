using UnityEngine;

public class AIController : MovementWithSpline
{
    public float obstacleDetectionRange = 5f; // The range of obstacle detection
    public float obstacleAvoidanceStrength = 5f; // The strength of obstacle avoidance
    public float sphereCastRadius = 0.5f; // The radius of the sphere used in SphereCast
    public float castHeightOffset = 1f; // Height offset for the sphere cast
    public float avoidanceDuration = 3f; // Time for how long to avoid the obstacle before returning to spline
    public float returnSpeed = 5f; // Speed to return to the spline
    public LayerMask obstacleLayerMask; // Layer mask to ensure only obstacles are detected
    public LayerMask groundLayerMask;

    private Rigidbody _rb;
    private float groundCheckDistance = 1f;
    private float avoidanceTimer = 0f;
    private bool isAvoidingObstacle = false;
    private bool isReturningToSpline = false;
    private Vector3 splineDirection; // The direction of spline movement
    private Vector3 fixedForwardDirection; // Fixed direction for raycasting (doesn't rotate)

    protected override void Start()
    {
        base.Start(); // Call the base class's Start method
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            Debug.LogError("Rigidbody component not found.");
        }

        // Set the initial spline direction and raycast direction
        splineDirection = transform.forward;
        fixedForwardDirection = splineDirection; // Keep a fixed forward direction for obstacle detection
    }

    protected override void FixedUpdate()
    {
        if (isReturningToSpline)
        {
            ReturnToSpline(); // Smoothly return to the spline
        }
        else if (isAvoidingObstacle)
        {
            AvoidObstacle();
        }
        else
        {
            // Continue moving along the spline if not avoiding an obstacle
            base.MoveAlongSpline();
            HandleObstacleDetection();
        }

        CheckIfOffGround();
    }

    private void HandleObstacleDetection()
    {
        Vector3 sphereCastOrigin = transform.position + Vector3.up * castHeightOffset;

        RaycastHit hit;
        // Use the fixed forward direction for raycasting (this won't rotate with the object)
        bool obstacleDetected = Physics.SphereCast(sphereCastOrigin, sphereCastRadius, fixedForwardDirection, out hit, obstacleDetectionRange, obstacleLayerMask);

        if (obstacleDetected && hit.collider.CompareTag("Obstacle"))
        {
            isAvoidingObstacle = true;
            avoidanceTimer = 0f; // Reset the timer
        }
    }

    private void AvoidObstacle()
    {
        avoidanceTimer += Time.fixedDeltaTime;

        // Calculate avoidance direction and move away from the obstacle
        Vector3 sphereCastOrigin = transform.position + Vector3.up * castHeightOffset;
        RaycastHit hit;
        if (Physics.SphereCast(sphereCastOrigin, sphereCastRadius, fixedForwardDirection, out hit, obstacleDetectionRange, obstacleLayerMask))
        {
            Vector3 avoidanceDirection = Vector3.Cross(hit.normal, Vector3.up).normalized;
            Vector3 targetPosition = transform.position + avoidanceDirection * obstacleAvoidanceStrength;
            targetPosition.y = _rb.position.y;

            // Move smoothly while avoiding the obstacle
            _rb.velocity = Vector3.Lerp(_rb.velocity, (targetPosition - _rb.position) / Time.fixedDeltaTime, 0.5f);
        }

        // After a certain time, start returning to the spline
        if (avoidanceTimer >= avoidanceDuration)
        {
            isAvoidingObstacle = false;
            isReturningToSpline = true;
        }
    }

    private void ReturnToSpline()
    {
        // Smoothly return to spline direction
        _rb.velocity = Vector3.Lerp(_rb.velocity, splineDirection * forwardForce, returnSpeed * Time.fixedDeltaTime);

        // Check if we are close enough to spline movement to stop returning
        if (Vector3.Distance(_rb.velocity.normalized, splineDirection) < 0.1f)
        {
            isReturningToSpline = false;
            _rb.velocity = splineDirection * forwardForce; // Resume normal movement along the spline
        }
    }

    private void CheckIfOffGround()
    {
        // Perform a raycast downwards to check if the ball is grounded
        RaycastHit groundHit;
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, out groundHit, groundCheckDistance, groundLayerMask);

        // If the ball is not grounded, destroy it
        if (!isGrounded)
        {
            Debug.Log("[DEBUG] Ball has left the ground. Destroying the ball.");
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 sphereCastOrigin = transform.position + Vector3.up * castHeightOffset;
        // Use the fixed forward direction for drawing the spherecast
        Gizmos.DrawLine(sphereCastOrigin, sphereCastOrigin + fixedForwardDirection * obstacleDetectionRange);
        Gizmos.DrawWireSphere(sphereCastOrigin + fixedForwardDirection * obstacleDetectionRange, sphereCastRadius);
    }
}
