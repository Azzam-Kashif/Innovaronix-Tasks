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
    private Vector3 currentDirection = Vector3.forward;
    
    private Quaternion targetRotation;
    private bool isTurning = false;

    private void Awake()
    {
        // Singleton pattern implementation
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
    }
    private void FixedUpdate()
    {
  
        // Set a consistent forward speed
        Vector3 velocity = rb.velocity;
        velocity.z = forwardForce;
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
    }
    void Update()
    {
       // rb.AddForce(Vector3.forward * forwardForce, ForceMode.Acceleration);

        HandleMouseDrag();
        if (transform.position.y < fallThreshold)
        {
            EventManager.Instance.LevelFail();
        }
        if (isTurning)
        {
            // Smoothly rotate towards the target direction
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

            // Stop turning when the rotation is nearly complete
            if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
            {
                isTurning = false;
            }
        }
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
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Spike") || transform.position.y < -10)
        {
            // Level fails if ball hits a spike or falls off
            EventManager.Instance?.LevelFail();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EndPoint"))
        {
            // Level passes if ball reaches the endpoint
            EventManager.Instance.LevelPass();
        }
    }
    public void Respawn()
    {
        if (rb == null) return;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = startPosition;
    }
    private void OnDestroy()
    {
        // Set the instance to null when destroyed to avoid stale references
        if (Instance == this)
        {
            Instance = null;
        }
    }

}
