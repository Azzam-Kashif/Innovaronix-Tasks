using UnityEngine;

public class AIController : MovementWithSpline
{
    public float obstacleDetectionRange = 5f; // The range of obstacle detection
    public float obstacleAvoidanceStrength = 5f; // The strength of obstacle avoidance
    public float sphereCastRadius = 0.5f; // The radius of the sphere used in SphereCast
    public float castHeightOffset = 1f; // Height offset for the sphere cast
    public float avoidanceDuration = 1f; // Reduced time for how long to avoid the obstacle before returning to spline
    public float returnSpeed = 5f; // Speed to return to the spline
    public LayerMask obstacleLayerMask; // Layer mask to ensure only obstacles are detected
    public LayerMask groundLayerMask;
    public float separationDistance = 1.5f;

    private Rigidbody _rb;
    private float groundCheckDistance = 1f;
    private float avoidanceTimer = 0f;
    private bool isAvoidingObstacle = false;
    private bool isReturningToSpline = false;
    private Vector3 avoidanceDirection; // Store the direction of avoidance

    protected override void Start()
    {
        base.Start(); // Call the base class's Start method
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            Debug.LogError("Rigidbody component not found.");
        }
        RankingManager rankingManager = FindObjectOfType<RankingManager>();
        if (rankingManager != null)
        {
            rankingManager.RegisterAI(this);
        }
    }
    protected override void FixedUpdate()
    {
        if (isReturningToSpline)
        {
            //ReturnToSpline(); // Smoothly return to the spline
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
        // Use the actual forward direction for raycasting based on velocity
        Vector3 forwardDirection = _rb.velocity.magnitude > 0.1f ? _rb.velocity.normalized : transform.forward;

        bool obstacleDetected = Physics.SphereCast(sphereCastOrigin, sphereCastRadius, forwardDirection, out hit, obstacleDetectionRange, obstacleLayerMask);

        if (obstacleDetected && hit.collider.CompareTag("Obstacle"))
        {
            isAvoidingObstacle = true;
            avoidanceTimer = 0f; // Reset the timer

            // Calculate avoidance direction
            avoidanceDirection = Vector3.Cross(hit.normal, Vector3.up).normalized;
        }
    }

    private void AvoidObstacle()
    {
        avoidanceTimer += Time.fixedDeltaTime;

        if (avoidanceTimer <= avoidanceDuration)
        {
            // Move away from the obstacle using calculated avoidance direction
            Vector3 targetPosition = transform.position + avoidanceDirection * obstacleAvoidanceStrength;
            targetPosition.y = _rb.position.y;

            // Apply a smooth force to avoid the obstacle
            _rb.velocity = Vector3.Lerp(_rb.velocity, (targetPosition - _rb.position) / Time.fixedDeltaTime, 0.5f);
        }
        else
        {
            // Start returning to the spline after avoidance
            isAvoidingObstacle = false;
            isReturningToSpline = true;
        }
    }
    private void ReturnToSpline()
    {
        // Smoothly return to spline direction
        _rb.velocity = Vector3.Lerp(_rb.velocity, transform.forward * forwardForce, returnSpeed * Time.fixedDeltaTime);

        // Check if we are close enough to spline movement to stop returning
        if (Vector3.Distance(_rb.velocity.normalized, transform.forward) < 0.1f)
        {
            isReturningToSpline = false;
            _rb.velocity = transform.forward * forwardForce; // Resume normal movement along the spline
        }
    }
    void MaintainSeparation()
    {
        // Find all other AI balls in the scene
        AIController[] allAIs = FindObjectsOfType<AIController>();

        foreach (AIController otherAI in allAIs)
        {
            // Skip self-check to avoid comparing to itself
            if (otherAI != this)
            {
                float distance = Vector3.Distance(transform.position, otherAI.transform.position);
                if (distance < separationDistance)
                {
                    // Calculate direction away from the other AI
                    Vector3 direction = (transform.position - otherAI.transform.position).normalized;
                    // Move this AI away from the other AI
                    transform.position += direction * (separationDistance - distance);
                }
            }
        }
    }
    private void CheckIfOffGround()
    {
        RaycastHit groundHit;
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, out groundHit, groundCheckDistance, groundLayerMask);

        if (!isGrounded)
        {
            // Let the Rigidbody handle the ball falling naturally with gravity
            _rb.useGravity = true; // Ensure gravity is applied
            _rb.isKinematic = false; // Allow physics to move the ball

            Debug.Log("[DEBUG] Ball has left the ground.");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EndPoint"))
        {
            EventManager.Instance.LevelFail();
        }
    }
}