using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Rigidbody2D rb;

    [Header("Look Settings")]
    [SerializeField] private float rotationSpeed = 10f;
    
    private Vector2 moveInput;
    private Vector2 mousePosition;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    private void Update()
    {
        // Get movement input
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

        // Get mouse position for rotation
        mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    private void FixedUpdate()
    {
        // Move the player
        rb.linearVelocity = moveInput * moveSpeed;

        // Rotate to face mouse
        RotateTowardsMouse();
    }

    private void RotateTowardsMouse()
    {
        Vector2 lookDirection = mousePosition - (Vector2)transform.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90f;
        
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }

    public Vector2 GetLookDirection()
    {
        return transform.up; // In 2D top-down, the "up" vector is the forward direction
    }
}
