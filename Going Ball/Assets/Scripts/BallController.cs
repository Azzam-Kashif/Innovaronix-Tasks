using UnityEngine;

public class BallController : MonoBehaviour
{
    public static BallController Instance { get; private set; }
    public float forwardForce = 10f;
    public float sidewaysForceMultiplier = 10f;
    public float maxSidewaysSpeed = 5f;
    public float dragSensitivity = 1f;

    private Rigidbody rb;
    private float initialPositionX;
    private float fallThreshold = -1;
    private Vector3 startPosition;
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

    void Update()
    {
        rb.AddForce(Vector3.forward * forwardForce, ForceMode.Acceleration);

        HandleMouseDrag();
        if (transform.position.y < fallThreshold)
        {
            EventManager.Instance.LevelFail();
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
