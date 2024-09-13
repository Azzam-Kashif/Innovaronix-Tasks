using UnityEngine;

public class BallController : MonoBehaviour
{
    public static BallController Instance { get; private set; }
    public float forwardForce = 10f;
    public float sidewaysForceMultiplier = 10f;
    public float maxSidewaysSpeed = 5f;
    public float dragSensitivity = 1f;
    public float turnSpeed = 5f;

    private Rigidbody rb;
    private float initialPositionX;
    private float fallThreshold = -1;
    public Vector3 startPosition;
    private Vector3 currentDirection = Vector3.forward; // Current movement direction

    private Quaternion targetRotation;
    private bool isTurning = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        currentDirection = transform.forward; // Initialize movement direction
    }

    private void FixedUpdate()
    {
        // Adjust the forward force based on turning state
        Vector3 force = currentDirection * forwardForce;
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, force.z);
    }

    void Update()
    {
        HandleMouseDrag();

        // Check for falling below the threshold
        if (transform.position.y < fallThreshold)
        {
            EventManager.Instance.LevelFail();
        }

        // Handle smooth turning
        if (isTurning)
        {
            // Smoothly rotate towards the target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

            // Check if the rotation is close enough to the target to stop turning
            if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
            {
                isTurning = false;
                // Update the direction of movement after completing the turn
                currentDirection = transform.forward;
            }
        }
    }

    // Handle sideways movement based on mouse drag
    void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            initialPositionX = Input.mousePosition.x;
        }
        else if (Input.GetMouseButton(0))
        {
            float dragDistance = (Input.mousePosition.x - initialPositionX) / Screen.width;

            // Calculate sideways force based on drag
            float sidewaysForce = dragDistance * sidewaysForceMultiplier * dragSensitivity;

            Vector3 newVelocity = rb.velocity + Vector3.right * sidewaysForce;
            newVelocity.x = Mathf.Clamp(newVelocity.x, -maxSidewaysSpeed, maxSidewaysSpeed);
            rb.velocity = new Vector3(newVelocity.x, rb.velocity.y, newVelocity.z);

            initialPositionX = Input.mousePosition.x;
        }
    }

    // Method to start a turn towards a given direction
    public void StartTurn(Vector3 curveDirection)
    {
        Debug.Log("Starting turn with direction: " + curveDirection);
        targetRotation = Quaternion.LookRotation(curveDirection);
        isTurning = true;
        rb.velocity = Vector3.zero; // Stop the ball briefly to allow for smoother turns
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Spike") || transform.position.y < -10)
        {
            EventManager.Instance?.LevelFail();
        }
    }

    // Detect curves and level endpoints
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EndPoint"))
        {
            EventManager.Instance.LevelPass();
        }
        else if (other.CompareTag("CurvePoint"))
        {
            // Trigger a turn when reaching a curve
            Vector3 turnDirection = other.transform.forward.normalized; // Ensure direction is normalized
            StartTurn(turnDirection);
        }
    }

    // Respawn the ball to the starting position
    public void Respawn()
    {
        if (rb == null) return;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = startPosition;
        transform.rotation = Quaternion.identity;
        currentDirection = Vector3.forward; // Reset direction on respawn
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
